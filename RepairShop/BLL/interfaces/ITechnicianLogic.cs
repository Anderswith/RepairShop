using RepairShop.BE;

namespace RepairShop.BLL.interfaces;

public interface ITechnicianLogic
{

    IEnumerable<User> GetTechnicians();
    User GetTechnicianByName(string username);
}