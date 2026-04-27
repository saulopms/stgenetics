using GoodHamburger.Application.DTOs;

namespace GoodHamburger.Application.Interfaces;

public interface IMenuService
{
    IEnumerable<MenuItemResponse> GetMenu();
}
