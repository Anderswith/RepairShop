using RepairShop.BE;
using RepairShop.BLL.interfaces;
using RepairShop.DAL.Repositories.Interfaces;
using RepairShop.Helpers.interfaces;

namespace RepairShop.BLL;

public class TechnicianLogic : ITechnicianLogic
{
    private readonly ITechnicianRepository _TechnicianRepository;
    
    public TechnicianLogic(ITechnicianRepository TechnicianRepository)
    {
        _TechnicianRepository = TechnicianRepository;

    }

    public IEnumerable<User> GetTechnicians()
    {
       return _TechnicianRepository.GetTechnicians().ToList();
    }

    public User GetTechnicianByName(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentException("can't find technician.");
        }
        return _TechnicianRepository.GetTechnicianByName(username);
    }
}