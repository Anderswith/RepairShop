using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RepairShop.BE;
using RepairShop.BLL;
using RepairShop.DAL.Repositories.Interfaces;
using RepairShop.Helpers;

namespace RepairShop.Tests
{
    [TestFixture]
    public class OrderLogicTests
    {
        private Mock<IOrderRepository> _orderRepositoryMock;
        private OrderLogic _orderLogic;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<EmailHelper> _emailHelperMock;

        [SetUp]
        public void SetUp()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _orderLogic = new OrderLogic(_orderRepositoryMock.Object, _emailHelperMock.Object, _userRepositoryMock.Object);
        }

        [Test]
        public void AddOrderExceptionWhenUserIdEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                _orderLogic.AddOrder(Guid.Empty, "itemName", "defect", "image"));
        }

        [Test]
        public void AddOrderExceptionWhenItemNameEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                _orderLogic.AddOrder(Guid.NewGuid(), "", "defect", "image"));
        }

        [Test]
        public void AddOrderExceptionWhenDefectEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
                _orderLogic.AddOrder(Guid.NewGuid(), "itemName", "", "image"));
        }

        [Test]
        public void AddOrderShouldAddOrderToRepository()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orders = new List<Order>();
            _orderRepositoryMock.Setup(repo => repo.GetAllOrders())
                .Returns(orders);

            // Act
            _orderLogic.AddOrder(userId, "itemName", "defect", "image");

            // Assert
            _orderRepositoryMock.Verify(repo => repo.AddOrder(It.IsAny<Order>()), Times.Once);
        }

        [Test]
        public void ChangeOrderStatusExceptionInvalidStatus()
        {
            Assert.Throws<ArgumentException>(() => _orderLogic.ChangeOrderStatus(Guid.NewGuid(), 0));
            Assert.Throws<ArgumentException>(() => _orderLogic.ChangeOrderStatus(Guid.NewGuid(), 6));
        }
        
        [TestCase(1, true, TestName = "ChangeOrderStatus_ValidLowerBoundary_1")]
        [TestCase(5, true, TestName = "ChangeOrderStatus_ValidUpperBoundary_5")]
        [TestCase(0, false, TestName = "ChangeOrderStatus_InvalidLowerBoundary_0")]
        [TestCase(6, false, TestName = "ChangeOrderStatus_InvalidUpperBoundary_6")]
        public void ChangeOrderBoundryTestOnlyValidNumbersPass(int orderStatus, bool isValid)
        {
            // Arrange
            var itemId = Guid.NewGuid();

            if (isValid)
            {
                // Act & Assert: Ensure no exceptions are thrown
                Assert.DoesNotThrow(() => _orderLogic.ChangeOrderStatus(itemId, orderStatus));
                _orderRepositoryMock.Verify(r => r.ChangeOrderStatus(itemId, orderStatus), Times.Once);
            }
            else
            {
                // Act & Assert: Ensure exception is thrown with correct message
                var ex = Assert.Throws<ArgumentException>(() => _orderLogic.ChangeOrderStatus(itemId, orderStatus));
                Assert.AreEqual("Order status must be between 1 and 5", ex.Message);
                _orderRepositoryMock.Verify(r => r.ChangeOrderStatus(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
            }
        }

        [Test]
        public void ChangeOrderStatusWithValidStatus()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            const int validStatus = 3;

            // Act
            _orderLogic.ChangeOrderStatus(orderId, validStatus);

            // Assert
            _orderRepositoryMock.Verify(repo => repo.ChangeOrderStatus(orderId, validStatus), Times.Once);
        }
        
        

        [Test]
        public void GetOrderByUserIdReturnOrderWhenValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orders = new List<Order>
            {
                new Order { OrderId = Guid.NewGuid(), UserId = userId },
                new Order { OrderId = Guid.NewGuid(), UserId = userId }
            };

            _orderRepositoryMock.Setup(repo => repo.GetOrdersByUserId(userId))
                .Returns(orders);

            // Act
            var result = _orderLogic.GetOrdersByUserId(userId);

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetAllOrdersReturnAllOrders()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { OrderId = Guid.NewGuid() },
                new Order { OrderId = Guid.NewGuid() }
            };

            _orderRepositoryMock.Setup(repo => repo.GetAllOrders())
                .Returns(orders);

            // Act
            var result = _orderLogic.GetAllOrders();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void AddTechnicianToOrdersShouldAdd()
        {
            // Arrange
            var technicianId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            // Act
            _orderLogic.AddTechnicianToOrder(technicianId, orderId);

            // Assert
            _orderRepositoryMock.Verify(repo => repo.AddTechnicianToOrder(technicianId, orderId), Times.Once);
        }

        [Test]
        public void AddCommentToOrderShouldAdd()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            const string comment = "This is a comment";

            // Act
            _orderLogic.AddCommentToOrder(orderId, comment);

            // Assert
            _orderRepositoryMock.Verify(repo => repo.AddCommentToOrder(orderId, comment), Times.Once);
        }

        [Test]
        public void AddExpectedDateShouldAdd()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var expectedDate = DateTime.UtcNow;

            // Act
            _orderLogic.AddExpectedCompleteDateToOrder(orderId, expectedDate);

            // Assert
            _orderRepositoryMock.Verify(repo => repo.AddExpectedCompleteDate(orderId, expectedDate), Times.Once);
        }
        [Test]
        public void ExpectedDateCantBeInPastException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var pastDate = DateTime.UtcNow.AddDays(-1);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _orderLogic.AddExpectedCompleteDateToOrder(orderId, pastDate));
            Assert.AreEqual("Expected date must be in the future", ex.Message);
            _orderRepositoryMock.Verify(r => r.AddExpectedCompleteDate(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never);
        }
    }
}
