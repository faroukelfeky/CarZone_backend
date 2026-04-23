using Kawerk.Application.Interfaces;
using Kawerk.Application.Services;
using Kawerk.Domain;
using Kawerk.Infastructure.Context;
using Kawerk.Infastructure.DTOs.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kawerk.UnitTests
{
    public class CustomerServiceTests
    {
        private readonly DbBase _db;
        private readonly Mock<ITokenHandler> _tokenHandlerMock;
        private readonly Mock<ICurrentUserService> _currentUserService;
        private readonly Mock<IAuthorizationService> _authorizatioHandler;
        private readonly ICustomerService _customerService;
        public CustomerServiceTests()
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
        public async Task UpdateTest()
        {
            // Setup user
            var customerID = Guid.NewGuid();
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Name, "Test User"),
                    new Claim(JwtRegisteredClaimNames.Email, "test@mail.com"),
                    new Claim(JwtRegisteredClaimNames.Sub, customerID.ToString()),
                    new Claim(ClaimTypes.Role, "Customer")
                }, "TestAuth")
            );

            // Add customer to in-memory DB
            _db.Customers.Add(new Customer
            {
                CustomerID = customerID,
                Name = "Test User",
                Username = "testuser",
                Email = "test@mail.com",
                Password = "hashed",
                Role = "Customer"
            });
            await _db.SaveChangesAsync();

            // Mock current user
            _currentUserService.Setup(x => x.User).Returns(user);

            // Mock authorization to always succeed
            _authorizatioHandler.Setup(x => x.AuthorizeAsync(
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());

            // Arrange DTO
            var dto = new CustomerUpdateDTO
            {
                Username = "Updated User",
                Address = "123 Updated St",
                Phone = "123-456-7890",
                Country = "Updated Country"
            };

            // Act
            var result = await _customerService.UpdateCustomer(customerID, dto);

            // Assert
            Assert.Equal(2, result.status);
            Assert.Equal("Updated Successfully", result.msg);
            var updatedCustomer = await _db.Customers.FindAsync(customerID);
            Assert.Equal("Updated User", updatedCustomer!.Username);
        }
        [Fact]
        public async Task UnauthorizedUpdate()
        {
            // Setup user
            var customerID = Guid.NewGuid();
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Name, "Test User"),
                    new Claim(JwtRegisteredClaimNames.Email, "test@mail.com"),
                    new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),//not the same ID as the user
                    new Claim(ClaimTypes.Role, "Customer")
                }, "TestAuth")
            );

            // Add customer to in-memory DB
            _db.Customers.Add(new Customer
            {
                CustomerID = customerID,
                Name = "Test User",
                Username = "testuser",
                Email = "test@mail.com",
                Password = "hashed",
                Role = "Customer"
            });
            await _db.SaveChangesAsync();

            // Mock current user
            _currentUserService.Setup(x => x.User).Returns(user);

            // Mock authorization to always succeed
            _authorizatioHandler.Setup(x => x.AuthorizeAsync(
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()))
                .ReturnsAsync(() =>
                {
                    if(user.FindFirst(JwtRegisteredClaimNames.Sub)!.Value != customerID.ToString())
                    {
                        return AuthorizationResult.Failed();
                    }
                    return AuthorizationResult.Success();
                });

            // Arrange DTO
            var dto = new CustomerUpdateDTO
            {
                Username = "Unauthorized user",
            };

            // Act
            var result = await _customerService.UpdateCustomer(customerID, dto);

            // Assert
            Assert.Equal(1, result.status);
            Assert.Equal("Unauthorized to update this customer.", result.msg);
        }
        [Fact]
        public async Task AlreadyUsedUsername()
        {
            //First User
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
            // Setup user
            var customerID = Guid.NewGuid();
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Name, "Test User"),
                    new Claim(JwtRegisteredClaimNames.Email, "test@mail.com"),
                    new Claim(JwtRegisteredClaimNames.Sub, customerID.ToString()),
                    new Claim(ClaimTypes.Role, "Customer")
                }, "TestAuth")
            );

            // Mock current user
            _currentUserService.Setup(x => x.User).Returns(user);

            // Mock authorization to always succeed
            _authorizatioHandler.Setup(x => x.AuthorizeAsync(
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()))
                .ReturnsAsync(() =>
                {
                    if (user.FindFirst(JwtRegisteredClaimNames.Sub)!.Value != customerID.ToString())
                    {
                        return AuthorizationResult.Failed();
                    }
                    return AuthorizationResult.Success();
                }); ;

            // Arrange DTO
            _db.Customers.Add(new Customer
            {
                CustomerID = customerID,
                Name = "Test2 User",
                Username = "test2user",
                Email = "test2@mail.com",
                Password = "hashed",
                Role = "Customer"
            });
            await _db.SaveChangesAsync();
            var dto = new CustomerUpdateDTO
            {
                Username = "testuser",
            };

            // Act
            var result = await _customerService.UpdateCustomer(customerID, dto);

            // Assert
            Assert.Equal(0, result.status);
            Assert.Equal("Username is already used", result.msg);
        }
    }
}
