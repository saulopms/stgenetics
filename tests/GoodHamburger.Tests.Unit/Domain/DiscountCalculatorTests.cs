using FluentAssertions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Services;

namespace GoodHamburger.Tests.Unit.Domain;

public sealed class DiscountCalculatorTests
{
    private static MenuItem Sandwich => new() { Id = 1, Name = "X Burger", Price = 5.00m, Type = MenuItemType.Sandwich };
    private static MenuItem Fries    => new() { Id = 4, Name = "Fries",    Price = 2.00m, Type = MenuItemType.Fries };
    private static MenuItem Soda     => new() { Id = 5, Name = "Soda",     Price = 2.50m, Type = MenuItemType.Soda };

    [Fact]
    public void NoItems_Returns_ZeroDiscount()
    {
        var result = DiscountCalculator.Calculate([]);
        result.Should().Be(0m);
    }

    [Fact]
    public void SandwichOnly_Returns_ZeroDiscount()
    {
        var result = DiscountCalculator.Calculate([OrderItem.Create(Sandwich)]);
        result.Should().Be(0m);
    }

    [Fact]
    public void SandwichAndFries_Returns_TenPercent()
    {
        var result = DiscountCalculator.Calculate(
            [OrderItem.Create(Sandwich), OrderItem.Create(Fries)]);
        result.Should().Be(0.10m);
    }

    [Fact]
    public void SandwichAndSoda_Returns_FifteenPercent()
    {
        var result = DiscountCalculator.Calculate(
            [OrderItem.Create(Sandwich), OrderItem.Create(Soda)]);
        result.Should().Be(0.15m);
    }

    [Fact]
    public void SandwichFriesAndSoda_Returns_TwentyPercent()
    {
        var result = DiscountCalculator.Calculate(
            [OrderItem.Create(Sandwich), OrderItem.Create(Fries), OrderItem.Create(Soda)]);
        result.Should().Be(0.20m);
    }

    [Fact]
    public void FriesAndSodaWithoutSandwich_Returns_ZeroDiscount()
    {
        var result = DiscountCalculator.Calculate(
            [OrderItem.Create(Fries), OrderItem.Create(Soda)]);
        result.Should().Be(0m);
    }

    [Fact]
    public void Order_TotalIsCorrect_WithTwentyPercentDiscount()
    {
        // X Burger 5.00 + Fries 2.00 + Soda 2.50 = 9.50  →  -20% = 1.90  →  total = 7.60
        var items = new List<OrderItem>
        {
            OrderItem.Create(Sandwich),
            OrderItem.Create(Fries),
            OrderItem.Create(Soda)
        };
        var discount = DiscountCalculator.Calculate(items);
        var order    = Order.Create(items, discount);

        order.Subtotal.Should().Be(9.50m);
        order.DiscountPercentage.Should().Be(0.20m);
        order.DiscountAmount.Should().Be(1.90m);
        order.Total.Should().Be(7.60m);
    }

    [Fact]
    public void Order_TotalIsCorrect_WithFifteenPercentDiscount()
    {
        // X Egg 4.50 + Soda 2.50 = 7.00  →  -15% = 1.05  →  total = 5.95
        var egg  = new MenuItem { Id = 2, Name = "X Egg", Price = 4.50m, Type = MenuItemType.Sandwich };
        var items = new List<OrderItem> { OrderItem.Create(egg), OrderItem.Create(Soda) };
        var discount = DiscountCalculator.Calculate(items);
        var order    = Order.Create(items, discount);

        order.Subtotal.Should().Be(7.00m);
        order.DiscountPercentage.Should().Be(0.15m);
        order.DiscountAmount.Should().Be(1.05m);
        order.Total.Should().Be(5.95m);
    }
}
