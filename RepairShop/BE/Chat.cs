using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairShop.BE;

public class Chat
{
    [Key]
    public Guid ChatId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid TechnicianId { get; set; }
    public string ChatText { get; set; }
    public DateTime ChatDate { get; set; }
    public Guid OrderId { get; set; }
    public Guid SenderId { get; set; }
    
    //not mapped for ellers tror denat der er en SenderUsername column i databasen.
    [NotMapped] 
    public string SenderUsername { get; set; }
}