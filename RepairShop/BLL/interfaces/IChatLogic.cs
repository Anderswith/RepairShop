using RepairShop.BE;

namespace RepairShop.BLL.interfaces;

public interface IChatLogic
{
    void addChatMessage(Guid senderId, Guid customerId, Guid technicianId, Guid OrderId, string message);
    IEnumerable<Chat> getChatMessagesByOrderId(Guid OrderId);
}