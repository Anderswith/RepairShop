using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairShop.BLL.interfaces;

namespace RepairShop.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserLogic _userLogic;
    private readonly IUserDataLogic _userDataLogic;

    public UserController(ILogger<UserController> logger, IUserLogic userLogic, IUserDataLogic userDataLogic)
    {
        _logger = logger;
        _userLogic = userLogic;
        _userDataLogic = userDataLogic;
    }

    
    
    [HttpPost("[action]")]
    public IActionResult LoginUser(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return BadRequest("Username and password are required.");
        }

        try
        {
            var (user, token) = _userLogic.LoginUser(username, password);
            if (user != null && token != null)
            {
                var userResponse = new
                {
                    userId = user.UserId,
                    username = user.Username,
                    token = token
                };
                return Ok(userResponse);
            }
            else
            {
                return BadRequest("Invalid username or password");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user");
            return StatusCode(500, "Error logging in user");
        }
    }
    
    [Authorize(Roles = "Admin,Technician")]
    [HttpGet("[action]")]
    public IActionResult GetUserByUsername(string username)
    {
        var user =  _userLogic.GetUserByName(username);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
    
    [Authorize(Roles = "Customer")]
    [HttpPut("[action]")]
    public IActionResult UpdateUserData(Guid userId, string email, int phoneNumber, string firstName, string lastName)
    {
        // Input validation
        if (userId == Guid.Empty)
        {
            return BadRequest("User ID is invalid.");
        }

        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            return BadRequest("Please enter a valid email.");
        }

        if (phoneNumber.ToString().Length != 8)
        {
            return BadRequest("Invalid phone number.");
        }

        try
        {
            _userDataLogic.UpdateUserData(userId, email, phoneNumber, firstName, lastName);
            return Ok("User data updated successfully.");
        }
        catch (Exception ex)
        {
            // Log the error (not shown here)
            return StatusCode(500, "An error occurred: " + ex.Message);
        }
    }
    [Authorize(Roles = "Customer,Admin,Technician")]
    [HttpGet]
    [Route("[action]/{userId}")]
    public IActionResult GetUserData(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest("User ID is invalid.");
        }
        var userData = _userDataLogic.GetUserData(userId);
        return Ok(userData);
    }
    
    [Authorize(Roles = "Technician")]
    [HttpPost("[action]")]
    public IActionResult CreateCustomer(string username, string password,
        string confirmPassword, string email, int phoneNumber, string firstName, string lastName)
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
            if (string.IsNullOrEmpty(email)
                || phoneNumber <= 0
                || string.IsNullOrEmpty(firstName)
                || string.IsNullOrEmpty(lastName))
            {
                return BadRequest("all fields must be filled");
            }
            if (!email.Contains("@") || !email.Contains("."))
            {
                return BadRequest("please enter a valid email");
            }

            var existingUser = _userLogic.GetAllUsers().FirstOrDefault(u => u.Username == username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }
            var userId = Guid.NewGuid();
            var role = "Customer";
            _userLogic.RegisterUser(username, password, role, userId);
            _userDataLogic.CreateUserData(userId, email, phoneNumber, firstName, lastName);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return StatusCode(500, "Error registering user");
        }
    }



}