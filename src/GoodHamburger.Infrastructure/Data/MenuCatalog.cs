using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Interfaces;

namespace GoodHamburger.Infrastructure.Data;

public sealed class MenuCatalog : IMenuCatalog
{
    private static readonly IReadOnlyList<MenuItem> _items =
    [
        new MenuItem { Id = 1, Name = "X Burger", Price = 5.00m, Type = MenuItemType.Sandwich },
        new MenuItem { Id = 2, Name = "X Egg",    Price = 4.50m, Type = MenuItemType.Sandwich },
        new MenuItem { Id = 3, Name = "X Bacon",  Price = 7.00m, Type = MenuItemType.Sandwich },
        new MenuItem { Id = 4, Name = "French Fries",  Price = 2.00m, Type = MenuItemType.Fries   },
        new MenuItem { Id = 5, Name = "Soda",     Price = 2.50m, Type = MenuItemType.Soda    }
    ];

    private static readonly IReadOnlyDictionary<int, MenuItem> _index =
        _items.ToDictionary(m => m.Id);

    public IEnumerable<MenuItem> GetAll() => _items;

    public MenuItem? GetById(int id) =>
        _index.TryGetValue(id, out var item) ? item : null;
}
