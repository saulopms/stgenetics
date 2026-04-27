using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Entities;

public sealed class OrderItem
{
    private OrderItem() { }

    public Guid Id { get; private set; }
    public int MenuItemId { get; private set; }
    public string MenuItemName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public MenuItemType Type { get; private set; }

    public static OrderItem Create(MenuItem menuItem) => new()
    {
        Id = Guid.NewGuid(),
        MenuItemId = menuItem.Id,
        MenuItemName = menuItem.Name,
        UnitPrice = menuItem.Price,
        Type = menuItem.Type
    };
}
