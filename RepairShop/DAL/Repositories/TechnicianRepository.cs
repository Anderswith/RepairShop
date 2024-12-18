using RepairShop.BE;
using RepairShop.DAL.Repositories.Interfaces;

namespace RepairShop.DAL.Repositories;

public class TechnicianRepository : ITechnicianRepository
{
    private readonly RepairShopContext _context;

    public TechnicianRepository(RepairShopContext context)
    {
        _context = context;
    }


    public User GetTechnicianByName(string username)
    {
        return _context.Users.FirstOrDefault(u=>u.Username == username);
    }

    public IEnumerable<User> GetTechnicians()
    {
        const string TechnicianRole = "Technician";
        return _context.Users.Where(x=>x.Role ==  TechnicianRole).ToList();
    }
}