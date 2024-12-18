using RepairShop.BE;

namespace RepairShop.BLL.interfaces;

public interface IUserDataLogic
{
    void CreateUserData(Guid userId, string email, int phoneNumber, string firstName, string lastName);
    UserData GetUserData(Guid userId);
    void UpdateUserData(Guid loggedInUserId, string email, int phoneNumber, string firstName, string lastName);
}