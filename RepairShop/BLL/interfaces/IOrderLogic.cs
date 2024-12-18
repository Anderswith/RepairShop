using RepairShop.BE;

namespace RepairShop.BLL.interfaces;

public interface IOrderLogic
{
    void AddOrder( Guid customerId, string itemName,
        string defect, string image);
    void ChangeOrderStatus(Guid orderId, int orderStatus);
    IEnumerable<Order> GetOrdersByUserId(Guid customerId);
    IEnumerable<Order> GetAllOrders();
    void AddTechnicianToOrder(Guid technicianId, Guid orderId);
    void AddCommentToOrder(Guid orderId, string comment);
    void AddExpectedCompleteDateToOrder(Guid orderId, DateTime expectedCompleteDate);
    Order GetOrderByOrderNumber(int orderNumber);
    IEnumerable<Order> GetOrdersInProgress();
    IEnumerable<Order> GetOrdersInProgressByTechnicianId(Guid technicianId);
    IEnumerable<Order> GetOrdersNotInProgress();

}