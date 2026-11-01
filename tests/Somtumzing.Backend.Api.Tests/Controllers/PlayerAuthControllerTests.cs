using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Somtumzing.Backend.Api.Controllers;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;
using Somtumzing.Backend.Api.Services;
using Xunit;

public sealed class PlayerAuthControllerTests
{
    private readonly Mock<GameDbContext> _databaseMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<AccessTokenService> _accessTokenServiceMock;
    private readonly Mock<IAuditLogWriter> _auditLogWriterMock;
    private readonly Mock<ILogger<PlayerAuthController>> _loggerMock;
    private readonly PlayerAuthController _controller;

    public PlayerAuthControllerTests()
    {
        _databaseMock = new Mock<GameDbContext>(new Microsoft.EntityFrameworkCore.DbContextOptions<GameDbContext>());
        
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        
        _accessTokenServiceMock = new Mock<AccessTokenService>(new byte[32], "issuer", "audience");
        _auditLogWriterMock = new Mock<IAuditLogWriter>();
        _loggerMock = new Mock<ILogger<PlayerAuthController>>();

        _controller = new PlayerAuthController(
            _databaseMock.Object,
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
}