using Inventory.Core.Entities;
using Inventory.Core.Interface.Repositories;
using Inventory.Services.Auth;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Inventory.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<User>> _userRepoMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userRepoMock = new Mock<IRepository<User>>();
            _configMock = new Mock<IConfiguration>();

            _configMock.Setup(c => c["Jwt:Key"]).Returns("TestKeyWithAtLeast32CharactersForHmacSha256Algorithm");
            _configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

            _unitOfWorkMock.Setup(u => u.Repository<User>()).Returns(_userRepoMock.Object);
            _authService = new AuthService(_unitOfWorkMock.Object, _configMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_UniqueUsername_CreatesUser()
        {
            _userRepoMock.Setup(r => r.GetByCodeAsync("newuser")).ReturnsAsync((User?)null);
            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _authService.RegisterAsync("newuser", "Password123!", "User");

            Assert.NotNull(result);
            Assert.Equal("newuser", result.Username);
            Assert.Equal("User", result.Role);
            Assert.NotNull(result.PasswordHash);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_DuplicateUsername_ThrowsInvalidOperationException()
        {
            var existingUser = new User { Id = 1, Username = "existinguser", PasswordHash = "hash", Role = "User" };
            _userRepoMock.Setup(r => r.GetByCodeAsync("existinguser")).ReturnsAsync(existingUser);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _authService.RegisterAsync("existinguser", "Password123!", "User"));
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");
            var user = new User { Id = 1, Username = "testuser", PasswordHash = hashedPassword, Role = "User" };
            _userRepoMock.Setup(r => r.GetByCodeAsync("testuser")).ReturnsAsync(user);

            var token = await _authService.LoginAsync("testuser", "Password123!");

            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ReturnsNull()
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
            var user = new User { Id = 1, Username = "testuser", PasswordHash = hashedPassword, Role = "User" };
            _userRepoMock.Setup(r => r.GetByCodeAsync("testuser")).ReturnsAsync(user);

            var token = await _authService.LoginAsync("testuser", "WrongPassword");

            Assert.Null(token);
        }

        [Fact]
        public async Task LoginAsync_NonExistentUser_ReturnsNull()
        {
            _userRepoMock.Setup(r => r.GetByCodeAsync("nonexistent")).ReturnsAsync((User?)null);

            var token = await _authService.LoginAsync("nonexistent", "Password123!");

            Assert.Null(token);
        }
    }
}
