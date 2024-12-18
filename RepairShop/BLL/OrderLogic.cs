using Microsoft.IdentityModel.Tokens;
using RepairShop.BE;
using RepairShop.BLL.interfaces;
using RepairShop.DAL.Repositories.Interfaces;
using RepairShop.Helpers;

namespace RepairShop.BLL;

public class OrderLogic : IOrderLogic
{
    private readonly IOrderRepository _orderRepository;
    private readonly EmailHelper _emailHelper;
    private readonly IUserRepository _userRepository;
    private readonly IUserDataRepository _userDataRepository;

    public OrderLogic(IOrderRepository orderRepository, EmailHelper emailHelper, IUserRepository userRepository, IUserDataRepository userDataRepository)
    {
        _orderRepository = orderRepository;
        _emailHelper = emailHelper;
        _userRepository = userRepository;
        _userDataRepository = userDataRepository;
    }

    public void AddOrder(Guid customerId, string itemName, string defect,
        string image)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty");
        }

        if (string.IsNullOrEmpty(itemName))
        {
            throw new ArgumentException("Item name cannot be empty");
        }

        if (string.IsNullOrEmpty(defect))
        {
            throw new ArgumentException("Must define what is wrong with item");
        }

        var baseOrderNumber = 1288400000;
        var orders = _orderRepository.GetAllOrders().Count();
        var order = new Order()
        {
            OrderId = Guid.NewGuid(),
            CustomerId = customerId,
            TechnicianId = null,
            ItemName = itemName,
            Defect = defect,
            Comment = null,
            OrderStatus = 1,
            Image = image,
            OrderNumber = baseOrderNumber + orders,
            ExpectedCompleteDate = null,
        };
        _orderRepository.AddOrder(order);
    }

  

    public void ChangeOrderStatus(Guid orderId, int orderstatus)
    {
        if (orderstatus <= 0 || orderstatus >= 6)
        {
            throw new ArgumentException("Order status must be between 1 and 5");
        }
        var orderToFind = _orderRepository.GetAllOrders().FirstOrDefault(o => o.OrderId == orderId);
        if (orderToFind == null)
        {
            throw new ArgumentException("Order not found");
        }
        var userToFind = _userRepository.GetUsers().FirstOrDefault(u => u.UserId == orderToFind.CustomerId);
        if (userToFind == null)
        {
            throw new ArgumentException("User not found");
        }

        if (orderstatus == 5)
        {
            
            
            var userDataToFind = _userDataRepository.GetUserData(userToFind.UserId);
            if (userDataToFind == null)
            {
                throw new ArgumentException("User data not found");
            }
            _emailHelper.SendOrderCompleteEmail(userDataToFind.Email, userDataToFind.FirstName, userDataToFind.LastName,
                orderToFind.ItemName);
            
        }
        _orderRepository.ChangeOrderStatus(orderId, orderstatus);
    }
    

    public IEnumerable<Order> GetOrdersByUserId(Guid customerId)
    {
        return _orderRepository.GetOrdersByUserId(customerId);
    }



    public IEnumerable<Order> GetAllOrders()
    {
        return _orderRepository.GetAllOrders();
    }

    public void AddTechnicianToOrder(Guid technicianId, Guid orderId)
    {
        _orderRepository.AddTechnicianToOrder(technicianId, orderId);
        var newOrderStatus = 2;
        _orderRepository.ChangeOrderStatus(orderId, newOrderStatus);
        
    }

    public void AddCommentToOrder(Guid orderId, string comment)
    {
        _orderRepository.AddCommentToOrder(orderId, comment);
    }

    public void AddExpectedCompleteDateToOrder(Guid orderId, DateTime expectedCompleteDate)
    {
        if (expectedCompleteDate < DateTime.UtcNow)
        {
            throw new ArgumentException("Expected date must be in the future");
        }
        _orderRepository.AddExpectedCompleteDate(orderId, expectedCompleteDate);
    }

    public Order GetOrderByOrderNumber(int orderNumber)
    {
        if (orderNumber == null)
        {
            throw new ArgumentException("Order number cannot be null");
        }
        var orderToGet = _orderRepository.GetOrderByOrderNumber(orderNumber);
        return orderToGet;
    }

    public IEnumerable<Order> GetOrdersInProgress()
    {
        return _orderRepository.GetOrdersInProgress();
    }

    public IEnumerable<Order> GetOrdersInProgressByTechnicianId(Guid technicianId)
    {
        return _orderRepository.GetOrdersInProgressByTechnicianId(technicianId);
    }

    public IEnumerable<Order> GetOrdersNotInProgress()
    {
        return _orderRepository.GetOrdersNotInProgress();
    }
}