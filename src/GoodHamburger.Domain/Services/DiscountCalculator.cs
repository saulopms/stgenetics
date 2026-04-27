using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Services;

public static class DiscountCalculator
{
    public static decimal Calculate(IEnumerable<OrderItem> items)
    {
        var list = items.ToList();
        bool hasSandwich = list.Any(i => i.Type == MenuItemType.Sandwich);
        bool hasFries    = list.Any(i => i.Type == MenuItemType.Fries);
        bool hasSoda     = list.Any(i => i.Type == MenuItemType.Soda);

        return (hasSandwich, hasFries, hasSoda) switch
        {
            (true, true, true)   => 0.20m,
            (true, false, true)  => 0.15m,
            (true, true, false)  => 0.10m,
            _                    => 0.00m
        };
    }
}
