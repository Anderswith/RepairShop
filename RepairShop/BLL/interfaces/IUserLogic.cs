using RepairShop.BE;

namespace RepairShop.BLL.interfaces;

public interface IUserLogic
{
    User RegisterUser(string username, string password, string role, Guid UserId);
    (User, string token) LoginUser(string username, string password);
    IEnumerable<User> GetAllUsers();
    User GetUserByName(string username);
    
}