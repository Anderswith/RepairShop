using RepairShop.BE;
using RepairShop.DAL.Repositories.Interfaces;

namespace RepairShop.DAL.Repositories;

public class UserDataRepository : IUserDataRepository
{
    private readonly RepairShopContext _context;

    public UserDataRepository(RepairShopContext context)
    {
        _context = context;
    }
    public UserData GetUserData(Guid userId)
    {
        return _context.UserData.FirstOrDefault(u => u.UserId == userId);
    }

    public void CreateUserData(UserData userData)
    {
        _context.UserData.Add(userData);
        _context.SaveChanges();
    }

    public void UpdateUserData(UserData userData)
    {
        _context.UserData.Update(userData);
        _context.SaveChanges();
    }
    
}