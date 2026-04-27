namespace GoodHamburger.Domain.Entities;

public sealed class Order
{
    private Order() { }

    private List<OrderItem> _items = [];

    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    public decimal Subtotal => _items.Sum(i => i.UnitPrice);
    public decimal DiscountPercentage { get; private set; }
    public decimal DiscountAmount => Math.Round(Subtotal * DiscountPercentage, 2);
    public decimal Total => Subtotal - DiscountAmount;

    public static Order Create(IEnumerable<OrderItem> items, decimal discountPercentage)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            DiscountPercentage = discountPercentage
        };
        order._items.AddRange(items);
        return order;
    }

    public void Update(IEnumerable<OrderItem> newItems, decimal discountPercentage)
    {
        _items = newItems.ToList();
        DiscountPercentage = discountPercentage;
        UpdatedAt = DateTime.UtcNow;
    }
}
