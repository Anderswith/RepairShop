using RepairShop.BE;
using RepairShop.DAL.Repositories.Interfaces;

namespace RepairShop.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly RepairShopContext _context;

    public UserRepository(RepairShopContext context)
    {
        _context = context;
    }
    public void RegisterUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public IEnumerable<User> GetUsers()
    {
        return _context.Users.ToList();
    }

    public User GetUserByUserName(string userName)
    {
        return _context.Users.FirstOrDefault(u=>u.Username == userName);
    }
}