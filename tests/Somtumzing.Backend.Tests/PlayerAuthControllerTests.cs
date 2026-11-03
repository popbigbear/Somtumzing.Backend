using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Moq;
using Somtumzing.Backend.Api.Controllers;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;
using Somtumzing.Backend.Api.Services;
using Xunit;

namespace Somtumzing.Backend.Tests.Controllers;

public sealed class PlayerAuthControllerTests
{
    private readonly GameDbContext _database;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<AccessTokenService> _accessTokenServiceMock;
    private readonly Mock<IAuditLogWriter> _auditLogWriterMock;
    private readonly Mock<ILogger<PlayerAuthController>> _loggerMock;
    private readonly PlayerAuthController _controller;

    public PlayerAuthControllerTests()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(databaseName: $"PlayerAuthTestDb_{Guid.NewGuid():N}")
            .Options;
        _database = new GameDbContext(options);
        
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        
        _accessTokenServiceMock = new Mock<AccessTokenService>(new byte[32], "issuer", "audience");
        _auditLogWriterMock = new Mock<IAuditLogWriter>();
        _loggerMock = new Mock<ILogger<PlayerAuthController>>();

        _controller = new PlayerAuthController(
            _database,
            _userManagerMock.Object,
            _accessTokenServiceMock.Object,
            _auditLogWriterMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GuestLogin_ValidRequest_ReturnsOkWithTokenAndPlayer()
    {
        // Arrange
        var request = new PlayerAuthController.GuestLoginRequest("device-id-1234", "ios");
        
        _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Player"))
            .ReturnsAsync(IdentityResult.Success);

        _accessTokenServiceMock
            .Setup(ts => ts.CreatePlayerToken(It.IsAny<ApplicationUser>(), It.IsAny<DateTimeOffset>()))
            .Returns("mock-jwt-token");

        _auditLogWriterMock
            .Setup(al => al.WriteAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Guid?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.GuestLogin(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PlayerAuthController.GuestAuthResponse>(okResult.Value);
        
        Assert.Equal("mock-jwt-token", response.Token);
        Assert.NotNull(response.Player);
        Assert.NotEmpty(response.Player.Username);
    }

    [Fact]
    public async Task GuestLogin_CreationFails_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new PlayerAuthController.GuestLoginRequest("device-id-1234", "web");
        
        _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "DuplicateUserName" }));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _controller.GuestLogin(request, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateDisplayName_ValidName_ReturnsOkWithUpdatedPlayer()
    {
        // Arrange: เตรียมข้อมูลผู้เล่นเดิมใน In-Memory DB
        await using var database = CreateInMemoryDatabase();
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = "old-name",
            CreatedAtUtc = DateTimeOffset.UtcNow,
            IsSuspended = false
        };
        database.Users.Add(user);
        await database.SaveChangesAsync();

        var controller = CreateController(database);
        SetupControllerUser(controller, userId);

        var request = new PlayerAuthController.UpdateDisplayNameRequest("SomtumMaster");

        // Act: เรียกใช้งาน PATCH /me
        var result = await controller.UpdateDisplayName(request, CancellationToken.None);

        // Assert: ตรวจสอบผลลัพธ์เป็น 200 OK และชื่อเปลี่ยนเป็น "SomtumMaster"
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PlayerAuthController.PlayerResponse>(okResult.Value);

        Assert.Equal(userId, response.Id);
        Assert.Equal("SomtumMaster", response.Username);
    }

    [Fact]
    public async Task UpdateDisplayName_SuspendedPlayer_ReturnsForbidden()
    {
        // Arrange: ผู้เล่นถูกระงับบัญชี (IsSuspended = true)
        await using var database = CreateInMemoryDatabase();
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = "banned-player",
            CreatedAtUtc = DateTimeOffset.UtcNow,
            IsSuspended = true
        };
        database.Users.Add(user);
        await database.SaveChangesAsync();

        var controller = CreateController(database);
        SetupControllerUser(controller, userId);

        var request = new PlayerAuthController.UpdateDisplayNameRequest("NewName");

        // Act: เรียกใช้งาน PATCH /me
        var result = await controller.UpdateDisplayName(request, CancellationToken.None);

        // Assert: ตรวจสอบว่าได้ 403 Forbidden
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status403Forbidden, objectResult.StatusCode);
    }

    [Fact]
    public async Task UpdateDisplayName_NameTooShort_ReturnsValidationProblem()
    {
        // Arrange: ชื่อสั้นเกินไป (น้อยกว่า 2 ตัวอักษร)
        await using var database = CreateInMemoryDatabase();
        var userId = Guid.NewGuid();
        database.Users.Add(new ApplicationUser { Id = userId, UserName = "valid" });
        await database.SaveChangesAsync();

        var controller = CreateController(database);
        SetupControllerUser(controller, userId);

        var request = new PlayerAuthController.UpdateDisplayNameRequest("A");

        // Act: เรียกใช้งาน
        var result = await controller.UpdateDisplayName(request, CancellationToken.None);

        // Assert: ตรวจสอบว่าได้ Validation Problem (BadRequest / 400)
        Assert.IsType<ObjectResult>(result.Result);
        Assert.True(controller.ModelState.ContainsKey("DisplayName"));
    }

    private static GameDbContext CreateInMemoryDatabase()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(databaseName: $"AuthDb_{Guid.NewGuid():N}")
            .Options;
        return new GameDbContext(options);
    }

    private static PlayerAuthController CreateController(GameDbContext database)
    {
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        var userManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);
        
        userManager.Setup(um => um.UpdateAsync(It.IsAny<ApplicationUser>()))
                   .ReturnsAsync(IdentityResult.Success);

        var accessTokenService = new Mock<AccessTokenService>(new byte[32], "issuer", "audience");
        var auditLogWriter = new Mock<IAuditLogWriter>();
        var logger = new Mock<ILogger<PlayerAuthController>>();

        return new PlayerAuthController(
            database,
            userManager.Object,
            accessTokenService.Object,
            auditLogWriter.Object,
            logger.Object);
    }

    private static void SetupControllerUser(ControllerBase controller, Guid userId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString("D")),
            new Claim("sub", userId.ToString("D")),
            new Claim("role", "Player")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [Fact]
    public async Task GetProfile_ValidPlayer_ReturnsOkWithPlayerResponse()
    {
        // Arrange: เตรียมข้อมูลผู้เล่นปกติใน In-Memory DB
        await using var database = CreateInMemoryDatabase();
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = "somtum-gamer",
            CreatedAtUtc = DateTimeOffset.UtcNow,
            IsSuspended = false
        };
        database.Users.Add(user);
        await database.SaveChangesAsync();

        var controller = CreateController(database);
        SetupControllerUser(controller, userId);

        // Act: เรียกใช้งาน GET /me (GetProfile)
        var result = await controller.GetProfile(CancellationToken.None);

        // Assert: ตรวจสอบผลลัพธ์เป็น 200 OK และได้ข้อมูล PlayerResponse ครบถ้วน
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PlayerAuthController.PlayerResponse>(okResult.Value);

        Assert.Equal(userId, response.Id);
        Assert.Equal("somtum-gamer", response.Username);
        Assert.Equal(user.CreatedAtUtc, response.CreatedAtUtc);
    }

    [Fact]
    public async Task GetProfile_SuspendedPlayer_ReturnsForbidden()
    {
        // Arrange: ผู้เล่นถูกระงับบัญชี (IsSuspended = true)
        await using var database = CreateInMemoryDatabase();
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = "banned-gamer",
            CreatedAtUtc = DateTimeOffset.UtcNow,
            IsSuspended = true
        };
        database.Users.Add(user);
        await database.SaveChangesAsync();

        var controller = CreateController(database);
        SetupControllerUser(controller, userId);

        // Act: เรียกใช้งาน GET /me
        var result = await controller.GetProfile(CancellationToken.None);

        // Assert: ตรวจสอบว่าได้ 403 Forbidden
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status403Forbidden, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetProfile_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange: มี Claim แต่ไม่พบ User ในฐานข้อมูล
        await using var database = CreateInMemoryDatabase();
        var controller = CreateController(database);
        SetupControllerUser(controller, Guid.NewGuid());

        // Act: เรียกใช้งาน GET /me
        var result = await controller.GetProfile(CancellationToken.None);

        // Assert: ตรวจสอบว่าได้ 401 Unauthorized
        Assert.IsType<UnauthorizedResult>(result.Result);
    }
}