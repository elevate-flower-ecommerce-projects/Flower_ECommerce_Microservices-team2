namespace Cart_ServiceCart_Service.Entities;


public class CartItem : BaseEntity
{
    public long CartId { get; set; }

    public long ProductId { get; set; }

    public decimal TotalPrice { get; set; }

    public int Quantity { get; set; }

    public Cart Cart { get; set; } = null!;
}
