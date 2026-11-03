using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // <-- เพิ่ม using นี้เข้ามา
using Moq;
using Somtumzing.Backend.Api.Controllers;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;
using Somtumzing.Backend.Api.Services;
using Xunit;

namespace Somtumzing.Backend.Tests.Controllers;

public sealed class AuthControllerExtendedTests
{
    private readonly GameDbContext _database;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly AccessTokenService _accessTokenService;
    private readonly Mock<IAuditLogWriter> _auditLogWriterMock;
    private readonly Mock<ILogger<PlayerAuthController>> _loggerMock;
    private readonly PlayerAuthController _controller;
    private readonly SystemController _systemController;

    public AuthControllerExtendedTests()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(databaseName: $"AuthExtDb_{Guid.NewGuid():N}")
            .Options;
        _database = new GameDbContext(options);
        
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        
        // สร้าง Instance จริงของ AccessTokenService ด้วย Test Key ขนาด 32 ไบต์
        var testSigningKey = new byte[32];
        Array.Fill(testSigningKey, (byte)1);
        _accessTokenService = new AccessTokenService(testSigningKey, "test-issuer", "test-audience");
        
        _auditLogWriterMock = new Mock<IAuditLogWriter>();
        _loggerMock = new Mock<ILogger<PlayerAuthController>>();

        _controller = new PlayerAuthController(
            _database,
            _userManagerMock.Object,
            _accessTokenService,
            _auditLogWriterMock.Object,
            _loggerMock.Object);

        _systemController = new SystemController();
    }

    [Fact]
    public void GetStatus_ReturnsOkWithStatus()
    {
        // Act
        var result = _systemController.GetStatus();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GuestLogin_ValidRequest_ReturnsOkWithTokenAndPlayer()
    {
        // Arrange
        var request = new PlayerAuthController.GuestLoginRequest("device-12345678", "web");
        
        _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Player"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.GuestLogin(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PlayerAuthController.GuestAuthResponse>(okResult.Value);
        Assert.False(string.IsNullOrWhiteSpace(response.Token));
        Assert.NotNull(response.Player);
    }
}