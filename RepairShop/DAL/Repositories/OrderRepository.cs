using RepairShop.BE;
using RepairShop.DAL.Repositories.Interfaces;

namespace RepairShop.DAL.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly RepairShopContext _context;

    public OrderRepository(RepairShopContext context)
    {
        _context = context;
    }

    public void AddOrder(Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();
    }

    public IEnumerable<Order> GetOrdersByUserId(Guid customerId)
    {
        return _context.Orders.Where(c => c.CustomerId == customerId).ToList();
    }

  
    public IEnumerable<Order> GetAllOrders()
    {
        return _context.Orders.ToList();
    }
    

    public void ChangeOrderStatus(Guid orderId, int status)
    {
        var orderToFind = _context.Orders.FirstOrDefault(c => c.OrderId.Equals(orderId));
        if (orderToFind != null)
        {
            orderToFind.OrderStatus = status;
            _context.SaveChanges();
        }
        else
        {
            throw new Exception("order not found");
        }
    }

    public void AddTechnicianToOrder(Guid technicianId, Guid orderId)
    {
        var orderToFind = _context.Orders.FirstOrDefault(c => c.OrderId.ToString().ToLower() 
                                                              == orderId.ToString().ToLower());
        var technicianToFind = _context.Users.FirstOrDefault(c => c.UserId == technicianId);
        if (orderToFind != null)
        {
            orderToFind.TechnicianName = technicianToFind.Username;
            orderToFind.TechnicianId = technicianId;
            _context.SaveChanges();
        }
        else
        {
            throw new Exception("order not found");
        }
    }

    public void AddCommentToOrder(Guid orderId, string comment)
    {
        var orderToFind = _context.Orders.FirstOrDefault(c => c.OrderId.Equals(orderId));
        if (orderToFind != null)
        {
            orderToFind.Comment = comment;
            _context.SaveChanges();
        }
        else
        {
            throw new Exception("order not found");
        }
    }

    public void AddExpectedCompleteDate(Guid orderId, DateTime expectedCompleteDate)
    {
        var orderToFind = _context.Orders.FirstOrDefault(c => c.OrderId.Equals(orderId));
        if (orderToFind != null)
        {
            orderToFind.ExpectedCompleteDate = expectedCompleteDate;
            _context.SaveChanges();
        }
    }

    public Order GetOrderByOrderNumber(int orderNumber)
    {
        return _context.Orders.FirstOrDefault(c => c.OrderNumber == orderNumber);
    }

    public IEnumerable<Order> GetOrdersInProgress()
    {
        return _context.Orders.Where(x=>x.TechnicianId!=null).ToList();
    }

    public IEnumerable<Order> GetOrdersInProgressByTechnicianId(Guid technicianId)
    {
        return _context.Orders.Where(x => x.TechnicianId == technicianId).ToList();
    }

    public IEnumerable<Order> GetOrdersNotInProgress()
    {
        return _context.Orders.Where(x => x.TechnicianId == null).ToList();
    }
}