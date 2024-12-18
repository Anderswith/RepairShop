using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairShop.BE;
using RepairShop.BLL.interfaces;
using RepairShop.DAL.Repositories.Interfaces;

namespace RepairShop.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TechnicianController : ControllerBase
{
    private readonly ILogger<TechnicianController> _logger;
    private readonly ITechnicianLogic _technicianLogic;
    private readonly IOrderLogic _orderLogic;
    private readonly IUserLogic _userLogic;
    private readonly IUserDataLogic _userDataLogic;


    public TechnicianController(ILogger<TechnicianController> logger, ITechnicianLogic technicianLogic, IOrderLogic orderLogic, IUserLogic userLogic, IUserDataLogic userDataLogic)
    {
        _logger = logger;
        _technicianLogic = technicianLogic;
        _orderLogic = orderLogic;
        _userLogic = userLogic;
        _userDataLogic = userDataLogic;
    }
    
    [Authorize(Roles = "Technician")]
    [HttpGet("{username}")]
    public IActionResult GetLoggedInTechDetails(string username)
    {
        var tech =  _technicianLogic.GetTechnicianByName(username);
        if (tech == null)
        {
            return NotFound();
        }
        return Ok(tech);
    }
    
    
    [Authorize(Roles = "Technician")]
    [HttpPost("[action]")]
    public IActionResult AddTechnicianToOrder(Guid technicianId, Guid orderId)
    {
        _orderLogic.AddTechnicianToOrder(technicianId, orderId);
        return Ok();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("[action]")]
    public IActionResult CreateTechnician(string username, string password, string confirmPassword)
    {
        try
        {
            if (string.IsNullOrEmpty(username)
                || string.IsNullOrEmpty(password)
                || string.IsNullOrEmpty(confirmPassword))

            {
                return BadRequest("You must fill out all the fields");
            }

            if (!password.Equals(confirmPassword))
            {
                return BadRequest("Passwords do not match");
            }

            var existingUser = _userLogic.GetAllUsers().FirstOrDefault(u => u.Username == username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            var userId = Guid.NewGuid();
            var role = "Technician";
            _userLogic.RegisterUser(username, password, role, userId);


        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error registering user");
            
        }
        return Ok();
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("[action]")]
    public IActionResult GetAllTechnicians()
    {
        var technicians = _technicianLogic.GetTechnicians();
        return Ok(technicians);
    }

    
}