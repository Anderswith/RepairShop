using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RepairShop.BE;
using RepairShop.BLL;
using RepairShop.DAL.Repositories.Interfaces;

namespace RepairShop.Tests
{
    [TestFixture]
    public class ChatLogicTests
    {
        private Mock<IChatRepository> _chatRepositoryMock;
        private ChatLogic _chatLogic;

        [SetUp]
        public void SetUp()
        {
            _chatRepositoryMock = new Mock<IChatRepository>();
            _chatLogic = new ChatLogic(_chatRepositoryMock.Object);
        }

        [Test]
        public void AddChatMessageExceptionWhenMessageIsEmpty()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var technicianId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _chatLogic.addChatMessage(userId, technicianId, orderId, null));
            Assert.AreEqual("no message provided", exception.ParamName);
        }

        [Test]
        public void AddChatMessageExceptionWhenUserIdIsEmpty()
        {
            // Arrange
            var technicianId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            const string message = "Hello, this is a message";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _chatLogic.addChatMessage(Guid.Empty, technicianId, orderId, message));
            Assert.AreEqual("no user provided", exception.ParamName);
        }

        [Test]
        public void AddChatMessageShouldCallRepositoryWhenValidInput()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var technicianId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            const string message = "Hello, this is a message";

            // Act
            _chatLogic.addChatMessage(userId, technicianId, orderId, message);

            // Assert
            _chatRepositoryMock.Verify(repo => repo.AddChatMessage(It.Is<Chat>(chat =>
                chat.UserId == userId &&
                chat.TechnicianId == technicianId &&
                chat.OrderId == orderId &&
                chat.ChatText == message &&
                chat.ChatDate.Date == DateTime.Now.Date
            )), Times.Once);
        }

        [Test]
        public void GetChatMessagesByOrderIdExceptionWhenOrderIdIsEmpty()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _chatLogic.getChatMessagesByOrderId(Guid.Empty));
            Assert.AreEqual("no order provided", exception.ParamName);
        }

        [Test]
        public void GetChatMessagesByOrderIdShouldReturnChatsWhenOrderIdIsValid()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var chats = new List<Chat>
            {
                new Chat { ChatId = Guid.NewGuid(), OrderId = orderId, ChatText = "Message 1" },
                new Chat { ChatId = Guid.NewGuid(), OrderId = orderId, ChatText = "Message 2" }
            };

            _chatRepositoryMock.Setup(repo => repo.GetChatByOrderId(orderId))
                .Returns(chats);

            // Act
            var result = _chatLogic.getChatMessagesByOrderId(orderId).ToList(); // Convert to List

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Message 1", result[0].ChatText);
            Assert.AreEqual("Message 2", result[1].ChatText);
        }

    }
}
