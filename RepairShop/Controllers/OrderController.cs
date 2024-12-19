using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using RepairShop.BLL.interfaces;

namespace RepairShop.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderLogic _orderLogic;
    private readonly IUserLogic _userLogic;


    public OrderController(ILogger<OrderController> logger, IOrderLogic orderLogic, IUserLogic userLogic  )
    {
        _logger = logger;
        _orderLogic = orderLogic;
        _userLogic = userLogic;
        
    }
    
    [Authorize(Roles = "Technician")]
    [HttpPost("[action]")]
    public IActionResult AddOrder(string customerName, string itemName, string defect, string image)
    {
        
        if (string.IsNullOrEmpty(itemName)
            || string.IsNullOrEmpty(defect)
            || string.IsNullOrEmpty(image)
            || string.IsNullOrEmpty(customerName))
        {
            return BadRequest("Fields cannot be empty");
        }
        var userToFind = _userLogic.GetUserByName(customerName);
        var customerId = userToFind.UserId;
        try
        {
            _orderLogic.AddOrder(userToFind.UserId, itemName, defect, image);
            return Ok(new { message = "Order added successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [Authorize(Roles = "Admin,Technician")]
    [HttpGet("[action]")]
    public IActionResult GetAllOrders()
    {
        var orders = _orderLogic.GetAllOrders();
        return Ok(orders);
    }
    
    [Authorize(Roles = "Customer")]
    [HttpGet("[action]")]
    public IActionResult GetOrderByUserId([FromQuery] Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            return BadRequest("Customer ID cannot be empty.");
        }

        Console.WriteLine($"Received customerId: {customerId}");

        var order = _orderLogic.GetOrdersByUserId(customerId);
        if (order == null)
        {
            return NotFound("No orders found for this customer.");
        }

        return Ok(order);
    }

    
    [Authorize(Roles = "Technician")]
    [HttpPut("[action]")]
    public IActionResult ChangeOrderStatus(Guid orderId, int orderStatus)
    {
        if (orderStatus <= 0 || orderStatus >= 6)
        {
            return BadRequest("status must be between 1 and 5");
        }
        _orderLogic.ChangeOrderStatus(orderId, orderStatus);
        return Ok();
    }
    
    [Authorize(Roles = "Technician")]
    [HttpPut("[action]")]
    public IActionResult AddCommentToOrder(Guid orderId, string comment)
    {
        _orderLogic.AddCommentToOrder(orderId, comment);
        return Ok();
    }

    [Authorize(Roles = "Technician")]
    [HttpPut("[action]")]
    public IActionResult AddExpectedCompleteDateToOrder(Guid orderId, DateTime expectedCompleteDate)
    {
        if (expectedCompleteDate == null)
        {
            return BadRequest("must use format YYYY/MM/DD");
        }

        if (expectedCompleteDate < DateTime.UtcNow)
        {
            return BadRequest("Please select a date in the future");
        }
        _orderLogic.AddExpectedCompleteDateToOrder(orderId, expectedCompleteDate);
        return Ok();
    }   
    
    [Authorize(Roles = "Admin,Technician")]
    [HttpGet("[action]")]
    public IActionResult GetOrderByOrderNumber(int orderNumber)
    {
        if (orderNumber == null)
        {
            return BadRequest("please insert order number");
            
        }
        var orderToGet = _orderLogic.GetOrderByOrderNumber(orderNumber);
        return Ok(orderToGet);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("[action]")]
    public IActionResult GetAllOrdersInProgress()
    {
        var orderInProgress = _orderLogic.GetOrdersInProgress();
        return Ok(orderInProgress);
    }
    
    [Authorize(Roles = "Admin,Technician")]
    [HttpGet("[action]")]
    public IActionResult GetAllOrdersNotInProgress()
    {
        var orderNotInProgress = _orderLogic.GetOrdersNotInProgress();
        return Ok(orderNotInProgress);
    }
    
    [Authorize(Roles = "Technician")]
    [HttpGet("[action]")]
    public IActionResult GetOrderListForTechnician( Guid technicianId)
    {
        var ordersToFind = _orderLogic.GetOrdersInProgressByTechnicianId(technicianId);
        return Ok(ordersToFind);
    }
    
    [Authorize(Roles = "Admin,Technician")]
    [HttpGet("[action]")]
    public IActionResult GetOrdersNotInProgress()
    {
        var orders = _orderLogic.GetOrdersNotInProgress();
        if (orders == null)
        {
            return NotFound();
        }
        return Ok(orders);
    }
    
}