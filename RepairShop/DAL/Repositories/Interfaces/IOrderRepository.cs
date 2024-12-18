using RepairShop.BE;

namespace RepairShop.DAL.Repositories.Interfaces;

public interface IOrderRepository
{
    void AddOrder(Order order);
    IEnumerable<Order> GetOrdersByUserId(Guid customerId);
    IEnumerable<Order> GetAllOrders();
    void ChangeOrderStatus(Guid orderId, int status);
    void AddTechnicianToOrder(Guid technicianId, Guid orderId);
    void AddCommentToOrder(Guid orderId, string comment);
    void AddExpectedCompleteDate(Guid orderId, DateTime expectedCompleteDate);
    Order GetOrderByOrderNumber(int orderNumber);
    IEnumerable<Order> GetOrdersInProgress();
    IEnumerable<Order> GetOrdersNotInProgress();
    IEnumerable<Order> GetOrdersInProgressByTechnicianId(Guid technicianId);

}