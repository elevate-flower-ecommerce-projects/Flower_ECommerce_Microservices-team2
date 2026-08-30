namespace Cart_ServiceCart_Service.Entities;

public class Cart : BaseEntity
{
    public long UserId { get; set; }

    public decimal TotalPrice { get; set; }

    public bool IsCheckOut { get; set; } = false;

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
