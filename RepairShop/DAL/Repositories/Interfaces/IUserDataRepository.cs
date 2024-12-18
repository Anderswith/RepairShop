using RepairShop.BE;

namespace RepairShop.DAL.Repositories.Interfaces;

public interface IUserDataRepository
{
    UserData GetUserData(Guid userId);         
    void CreateUserData(UserData userData);        
    void UpdateUserData(UserData userData);
}