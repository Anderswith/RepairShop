using System.ComponentModel.DataAnnotations;

namespace RepairShop.BE;

public class User
{
    [Key]
    public Guid UserId { get; set; }
    public string Username { get; set; }
    
    public string Role { get; set; }
    public string Hash { get; set; }
    public string Salt { get; set; }
    
}