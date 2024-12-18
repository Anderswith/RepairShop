using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairShop.BLL.interfaces;

namespace RepairShop.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private ILogger<ChatController> _logger;
    private IChatLogic _chatLogic;
    
    
    public ChatController(IChatLogic chatLogic, ILogger<ChatController> logger)
    {
        _logger = logger;
        _chatLogic = chatLogic;
    }
    
    [Authorize(Roles = "Customer,Technician")]
    [HttpPost("[action]")]
    public IActionResult AddChatMessage(Guid senderId, Guid customerId, Guid technicianId, Guid orderId, string message)
    {
        if ( senderId == Guid.Empty ||customerId == Guid.Empty || technicianId == Guid.Empty || orderId == Guid.Empty)
        {
            return BadRequest("SenderId/UserId/technicianId/itemId is invalid");
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            return BadRequest("Message is empty");
        }
        _chatLogic.addChatMessage(senderId, customerId, technicianId, orderId, message);
        return Ok();
    }
    
    [Authorize(Roles = "Customer,Technician")]
    [HttpGet("[action]")]
    public IActionResult GetChatMessagesForOrder(Guid orderId)
    {
        if (orderId == Guid.Empty)
        {
            return BadRequest("OrderId is invalid");
        }
        var chatMessages = _chatLogic.getChatMessagesByOrderId(orderId);
        if (chatMessages == null)
        {
            return NotFound();
        }
        return Ok(chatMessages);
    }
}