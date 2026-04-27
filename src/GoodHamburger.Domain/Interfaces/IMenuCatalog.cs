using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Domain.Interfaces;

public interface IMenuCatalog
{
    IEnumerable<MenuItem> GetAll();
    MenuItem? GetById(int id);
}
