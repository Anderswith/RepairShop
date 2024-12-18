using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RepairShop.BE;
using RepairShop.BLL;
using RepairShop.DAL.Repositories.Interfaces;
using RepairShop.Helpers.interfaces;

namespace RepairShop.Tests
{
    [TestFixture]
    public class TechnicianLogicTests
    {
        private Mock<ITechnicianRepository> _technicianRepositoryMock;
        private Mock<IPasswordEncrypter> _passwordEncrypterMock;
        private Mock<IJwtToken> _jwtTokenMock;
        private TechnicianLogic _technicianLogic;

        [SetUp]
        public void SetUp()
        {
            _technicianRepositoryMock = new Mock<ITechnicianRepository>();
            _passwordEncrypterMock = new Mock<IPasswordEncrypter>();
            _jwtTokenMock = new Mock<IJwtToken>();

            _technicianLogic = new TechnicianLogic(
                _technicianRepositoryMock.Object,
                _passwordEncrypterMock.Object,
                _jwtTokenMock.Object
            );
        }

        [Test]
        public void RegisterUserExceptionUsernameOrPasswordNullOrEmpty()
        {
            Assert.Throws<ArgumentException>(() => _technicianLogic.RegisterTechnician("", "password"));
            Assert.Throws<ArgumentException>(() => _technicianLogic.RegisterTechnician("username", ""));
        }

        [Test]
        public void RegisterTechnicianExceptionIfAlreadyExists()
        {
            // Arrange
            _technicianRepositoryMock
                .Setup(repo => repo.GetTechnicianByName(It.IsAny<string>()))
                .Returns(new Technician());

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _technicianLogic.RegisterTechnician("existingTech", "password"));
        }

        [Test]
        public void RegisterTechnicialShouldReturnCorrectTechnician()
        {
            // Arrange
            _technicianRepositoryMock
                .Setup(repo => repo.GetTechnicianByName(It.IsAny<string>()))
                .Returns((Technician)null);

            _passwordEncrypterMock
                .Setup(enc => enc.EncryptPassword(It.IsAny<string>()))
                .Returns(("hashedPassword", "salt"));

            // Act
            var result = _technicianLogic.RegisterTechnician("newTech", "password");

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("newTech", result.Username);
            _technicianRepositoryMock.Verify(repo => repo.RegisterTechnician(It.IsAny<Technician>()), Times.Once);
        }

        [Test]
        public void LoginTechnicianExceptionIfUsernameOrPasswordIsNull()
        {
            Assert.Throws<ArgumentException>(() => _technicianLogic.LoginTechnician("", "password"));
            Assert.Throws<ArgumentException>(() => _technicianLogic.LoginTechnician("username", ""));
        }

        [Test]
        public void LoginTechnicianExceptionIfTechnicianDoesntExist()
        {
            // Arrange
            _technicianRepositoryMock
                .Setup(repo => repo.GetTechnicianByName(It.IsAny<string>()))
                .Returns((Technician)null);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _technicianLogic.LoginTechnician("nonExistentTech", "password"));
        }

        [Test]
        public void LoginTechnicianExceptionIncorrectPassword()
        {
            // Arrange
            var technician = new Technician
            {
                Username = "existingTech",
                Salt = "salt",
                Hash = "correctHash"
            };

            _technicianRepositoryMock
                .Setup(repo => repo.GetTechnicianByName(It.IsAny<string>()))
                .Returns(technician);

            _passwordEncrypterMock
                .Setup(enc => enc.EncryptPasswordWithUsersSalt(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("wrongHash");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _technicianLogic.LoginTechnician("existingTech", "wrongPassword"));
        }

        [Test]
        public void LoginTechnicianReturnsTokenAndTechnician()
        {
            // Arrange
            var technician = new Technician
            {
                Username = "existingTech",
                Salt = "salt",
                Hash = "correctHash"
            };

            _technicianRepositoryMock
                .Setup(repo => repo.GetTechnicianByName(It.IsAny<string>()))
                .Returns(technician);

            _passwordEncrypterMock
                .Setup(enc => enc.EncryptPasswordWithUsersSalt(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("correctHash");

            _jwtTokenMock
                .Setup(token => token.GenerateJwtToken(It.IsAny<string>(), "Technician"))
                .Returns("generatedToken");

            // Act
            var (resultTechnician, token) = _technicianLogic.LoginTechnician("existingTech", "password");

            // Assert
            Assert.NotNull(resultTechnician);
            Assert.AreEqual("existingTech", resultTechnician.Username);
            Assert.AreEqual("generatedToken", token);
        }

        [Test]
        public void GetTechniciansShouldReturnListOfTechnicians()
        {
            // Arrange
            var technicians = new List<Technician>
            {
                new Technician { Username = "Tech1" },
                new Technician { Username = "Tech2" }
            };

            _technicianRepositoryMock
                .Setup(repo => repo.GetTechnicians())
                .Returns(technicians);

            // Act
            var result = _technicianLogic.GetTechnicians();

            // Assert
            Assert.AreEqual(2, result.Count());
        }
    }
}
