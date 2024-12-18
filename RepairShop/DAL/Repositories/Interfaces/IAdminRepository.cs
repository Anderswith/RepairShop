using RepairShop.BE;

namespace RepairShop.DAL.Repositories.Interfaces;

public interface IAdminRepository
{
    IEnumerable<User> GetAdmins();
    User GetAdminByUsername(string username);
}