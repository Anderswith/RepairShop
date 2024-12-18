using System.ComponentModel.DataAnnotations;

namespace RepairShop.BE;

public class Order
{
    [Key]
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? TechnicianId { get; set; }
    public string? TechnicianName { get; set; }
    public string ItemName { get; set; }
    public string Defect { get; set; }
    public string? Comment { get; set; }
    public int OrderStatus { get; set; }
    public string Image { get; set; }
    public int OrderNumber { get; set; }
    public DateTime? ExpectedCompleteDate { get; set; }
    
}