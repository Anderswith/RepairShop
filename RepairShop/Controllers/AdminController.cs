using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairShop.BLL.interfaces;
using RepairShop.DAL.Repositories.Interfaces;

namespace RepairShop.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IUserLogic _userLogic;

    public AdminController( IUserLogic userLogic)
    {
        _userLogic = userLogic;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("[action]")]
    public IActionResult CreateAdmin(string username, string password, string confirmPassword)
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
            var role = "Admin";
            _userLogic.RegisterUser(username, password, role, userId);


        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error registering admin");
            
        }
        return Ok();
    }
}
 