using RepairShop.BE;
using RepairShop.BLL.interfaces;
using RepairShop.DAL.Repositories.Interfaces;

namespace RepairShop.BLL;

public class ChatLogic: IChatLogic
{
    private readonly IChatRepository _chatRepository;

    public ChatLogic(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }
    public void addChatMessage(Guid senderId, Guid customerId, Guid technicianId, Guid orderId, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentNullException("no message provided");
        }

        if (Guid.Empty == customerId)
        {
            throw new ArgumentNullException("no user provided");
        }

        var chat = new Chat
        {
            ChatId = new Guid(),
            CustomerId = customerId,
            TechnicianId = technicianId,
            ChatText = message,
            ChatDate = DateTime.Now,
            OrderId = orderId,
            SenderId = senderId
        };
        _chatRepository.AddChatMessage(chat);

    }

    public IEnumerable<Chat> getChatMessagesByOrderId(Guid orderId)
    {
        if (Guid.Empty == orderId)
        {
            throw new ArgumentNullException("no order provided");
        }
        return _chatRepository.GetChatByOrderId(orderId);
    }
}