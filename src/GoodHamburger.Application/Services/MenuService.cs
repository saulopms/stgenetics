using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Interfaces;

namespace GoodHamburger.Application.Services;

public sealed class MenuService(IMenuCatalog catalog) : IMenuService
{
    public IEnumerable<MenuItemResponse> GetMenu() =>
        catalog.GetAll().Select(m => new MenuItemResponse
        {
            Id    = m.Id,
            Name  = m.Name,
            Price = m.Price,
            Type  = m.Type.ToString()
        });
}
