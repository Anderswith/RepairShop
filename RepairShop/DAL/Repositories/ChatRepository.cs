using RepairShop.BE;
using RepairShop.DAL.Repositories.Interfaces;

namespace RepairShop.DAL.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly RepairShopContext _context;

    public ChatRepository(RepairShopContext context)
    {
        _context = context;
    }

    public void AddChatMessage(Chat chat)
    {
        _context.Chats.Add(chat);
        _context.SaveChanges();
    }

    public IEnumerable<Chat> GetChatByOrderId(Guid orderId)
    {
        var chatMessages = _context.Chats
            .Join(
                _context.Users, 
                cm => cm.SenderId,   // Join on SenderId from Chats
                u => u.UserId,        // Join on UserId from Users
                (cm, u) => new Chat   // Create a new Chat object with data from both tables
                {
                    ChatId = cm.ChatId,
                    TechnicianId = cm.TechnicianId,
                    CustomerId = cm.CustomerId,
                    ChatText = cm.ChatText,
                    ChatDate = cm.ChatDate,
                    OrderId = cm.OrderId,
                    SenderId = cm.SenderId,
                    SenderUsername = u.Username // Assign the username from the Users table
                })
            .Where(cm => cm.OrderId == orderId) 
            .OrderBy(cm => cm.ChatDate)         
            .ToList();

        return chatMessages;
    }
}