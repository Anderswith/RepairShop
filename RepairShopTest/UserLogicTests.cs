using Moq;
using NUnit.Framework;
using RepairShop.BE;
using RepairShop.BLL;
using RepairShop.BLL.interfaces;
using RepairShop.DAL.Repositories.Interfaces;
using RepairShop.Helpers.interfaces;
using System;

namespace RepairShop.Tests
{
    [TestFixture]
    public class UserLogicTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IPasswordEncrypter> _mockPasswordEncrypter;
        private Mock<IJwtToken> _mockJwtToken;
        private UserLogic _userLogic;

        [SetUp]
        public void SetUp()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPasswordEncrypter = new Mock<IPasswordEncrypter>();
            _mockJwtToken = new Mock<IJwtToken>();
            _userLogic = new UserLogic(_mockUserRepository.Object, _mockPasswordEncrypter.Object, _mockJwtToken.Object);
        }

        [Test]
        public void RegisterUserShouldReturnValidUser()
        {
            // Arrange
            var username = "testuser";
            var password = "password123";
            var email = "test@example.com";
            var phoneNumber = 12345678;
            var firstName = "John";
            var lastName = "Doe";

            var (hash, salt) = ("hashedPassword", "salt");
            _mockPasswordEncrypter.Setup(p => p.EncryptPassword(It.IsAny<string>())).Returns((hash, salt));
            _mockUserRepository.Setup(u => u.GetUserByUserName(It.IsAny<string>())).Returns((User)null); // No existing user

            // Act
            var result = _userLogic.RegisterUser(username, password, email, phoneNumber, firstName, lastName);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(username, result.Username);
            Assert.AreEqual(email, result.Email);
            _mockUserRepository.Verify(u => u.RegisterUser(It.IsAny<User>()), Times.Once);
        }

        [Test]
        public void RegisterUserExceptionWhenUsernameNullOrEmpty()
        {
            // Arrange
            var username = "";
            var password = "password123";
            var email = "test@example.com";
            var phoneNumber = 12345678;
            var firstName = "John";
            var lastName = "Doe";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _userLogic.RegisterUser(username, password, email, phoneNumber, firstName, lastName));
            Assert.AreEqual("Username cannot be null or empty.", ex.Message);
        }

        [Test]
        public void RegisterUserExceptionWhenUsernameTaken()
        {
            // Arrange
            var username = "testuser";
            var password = "password123";
            var email = "test@example.com";
            var phoneNumber = 12345678;
            var firstName = "John";
            var lastName = "Doe";

            var existingUser = new User { Username = username };
            _mockUserRepository.Setup(u => u.GetUserByUserName(It.IsAny<string>())).Returns(existingUser);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _userLogic.RegisterUser(username, password, email, phoneNumber, firstName, lastName));
            Assert.AreEqual("Username is already taken.", ex.Message);
        }

        [Test]
        public void LoginUserReturnValidToken()
        {
            // Arrange
            var username = "testuser";
            var password = "password123";
            var user = new User { Username = username, Hash = "hashedPassword", Salt = "salt" };

            _mockUserRepository.Setup(u => u.GetUserByUserName(username)).Returns(user);
            _mockPasswordEncrypter.Setup(p => p.EncryptPasswordWithUsersSalt(password, "salt")).Returns("hashedPassword");
            _mockJwtToken.Setup(j => j.GenerateJwtToken(username, "User")).Returns("token");

            // Act
            var result = _userLogic.LoginUser(username, password);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(username, result.Item1.Username);
            Assert.AreEqual("token", result.Item2);
        }

        [Test]
        public void LoginUserExceptionWithWrongPassword()
        {
            // Arrange
            var username = "testuser";
            var password = "wrongpassword";
            var user = new User { Username = username, Hash = "hashedPassword", Salt = "salt" };

            _mockUserRepository.Setup(u => u.GetUserByUserName(username)).Returns(user);
            _mockPasswordEncrypter.Setup(p => p.EncryptPasswordWithUsersSalt(password, "salt")).Returns("wrongHash");

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _userLogic.LoginUser(username, password));
            Assert.AreEqual("Invalid username or password.", ex.Message);
        }

        [Test]
        public void GetNameByUsernameShouldReturnCorrectName()
        {
            // Arrange
            var username = "testuser";
            var user = new User { Username = username };

            _mockUserRepository.Setup(u => u.GetUserByUserName(username)).Returns(user);

            // Act
            var result = _userLogic.GetUserByName(username);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(username, result.Username);
        }

        [Test]
        public void GetUserByNameReturnNullWhenUserDoesntExist()
        {
            // Arrange
            var username = "nonexistentuser";
            _mockUserRepository.Setup(u => u.GetUserByUserName(username)).Returns((User)null);

            // Act
            var result = _userLogic.GetUserByName(username);

            // Assert
            Assert.IsNull(result);
        }
    }
}