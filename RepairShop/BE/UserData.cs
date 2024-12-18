using System.ComponentModel.DataAnnotations;

namespace RepairShop.BE;

public class UserData
{
    [Key]
    public Guid UserDataId { get; set; }
    public Guid UserId { get; set; }
    public int PhoneNumber { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}