using RepairShop.BE;

namespace RepairShop.DAL.Repositories.Interfaces;

public interface IChatRepository
{
    void AddChatMessage(Chat chat);
    IEnumerable<Chat> GetChatByOrderId( Guid orderId );
    
}