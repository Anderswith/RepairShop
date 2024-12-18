using RepairShop.BE;
using RepairShop.DAL.Repositories.Interfaces;

namespace RepairShop.DAL.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly RepairShopContext _context;

    public IEnumerable<User> GetAdmins ()
    {
        const string adminRole = "admin";
        return _context.Users.Where(x=>x.Role == adminRole);
    }

    public User GetAdminByUsername(string username)
    {
        const string adminRole = "admin";
        return _context.Users.FirstOrDefault(a => a.Username.Equals(username, StringComparison.OrdinalIgnoreCase) 
                                                  && a.Role.Equals(adminRole, StringComparison.OrdinalIgnoreCase));
    }
}