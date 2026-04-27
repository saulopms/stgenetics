using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Entities;

public sealed class MenuItem
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public MenuItemType Type { get; init; }
}
