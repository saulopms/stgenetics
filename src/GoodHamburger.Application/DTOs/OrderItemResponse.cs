namespace GoodHamburger.Application.DTOs;

public sealed class OrderItemResponse
{
    public Guid Id { get; init; }
    public int MenuItemId { get; init; }
    public string MenuItemName { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public string Type { get; init; } = string.Empty;
}
