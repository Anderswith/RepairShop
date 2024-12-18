using RepairShop.BE;

namespace RepairShop.DAL.Repositories.Interfaces;

public interface ITechnicianRepository
{

    User GetTechnicianByName(string username);
    IEnumerable<User> GetTechnicians();
}