using RepairShop.BE;

namespace RepairShop.DAL.Repositories.Interfaces;

public interface IUserRepository
{
    void RegisterUser(User user);
    IEnumerable<User> GetUsers();
    User GetUserByUserName(string userName);
}