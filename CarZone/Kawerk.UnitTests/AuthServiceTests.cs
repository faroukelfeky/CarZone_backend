namespace Kawerk.UnitTests;
using Kawerk.Application.Interfaces;
using Kawerk.Application.Services;
using Kawerk.Domain;
using Kawerk.Infastructure.Context;
using Kawerk.Infastructure.DTOs.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

public class AuthServiceTests
{
    private readonly DbBase _db;
    private readonly Mock<ITokenHandler> _tokenHandlerMock;
    private readonly Mock<ICurrentUserService> _currentUserService;
    private readonly Mock<IAuthorizationService> _authorizatioHandler;
    private readonly ICustomerService _customerService;
    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<DbBase>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new DbBase(options);
        _tokenHandlerMock = new Mock<ITokenHandler>();
        _currentUserService = new Mock<ICurrentUserService>();
        _authorizatioHandler = new Mock<IAuthorizationService>();
        _customerService = new CustomerService(
            _db,
            _tokenHandlerMock.Object,
            _currentUserService.Object,
            _authorizatioHandler.Object);
    }
    [Fact]
    public async Task SignUpTest()
    {
        // Arrange
        var dto = new CustomerCreationDTO
        {
            Name = "Test User",
            Username = "testuser",
            Email = "test@gmail.com",
            Password = "Strong_Pass123"
        };

        _tokenHandlerMock
            .Setup(t => t.CreateAccessToken(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync("access-token");

        _tokenHandlerMock
            .Setup(t => t.RefreshingToken(It.IsAny<Guid>()))
            .ReturnsAsync("refresh-token");

        // Act
        var result = await _customerService.CreateCustomer(dto);

        // Assert
        Assert.Equal(1, result.status);
        Assert.Equal("Customer created successfully", result.msg);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);

        Assert.Equal(1, await _db.Customers.CountAsync());
    }
    [Fact]
    public async Task CustomerAlreadyExists()
    {
        // Arrange
        _db.Customers.Add(new Customer
        {
            CustomerID = Guid.NewGuid(),
            Name = "Test User",
            Username = "testuser",
            Email = "test@mail.com",
            Password = "hashed",
            Role = "Customer"
        });
        await _db.SaveChangesAsync();

        var dto = new CustomerCreationDTO
        {
            Name = "Test User",
            Username = "testuser",
            Email = "test@mail.com",
            Password = "StrongPass123!"
        };

        // Act
        var result = await _customerService.CreateCustomer(dto);

        // Assert
        Assert.Equal(0, result.status);
        Assert.Equal("Customer already exists", result.msg);
    }

    [Fact]
    public async Task CreateCustomer_InvalidEmail_ReturnsFailure()
    {
        // Arrange
        var dto = new CustomerCreationDTO
        {
            Name = "Test User",
            Username = "testuser",
            Email = "invalid-email",
            Password = "StrongPass123!"
        };

        // Act
        var result = await _customerService.CreateCustomer(dto);

        // Assert
        Assert.Equal(0, result.status);
        Assert.Equal("Invalid Email", result.msg);
    }

}
