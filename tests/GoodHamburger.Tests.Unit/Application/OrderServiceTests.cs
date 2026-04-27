using FluentAssertions;
using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Infrastructure.Data;
using GoodHamburger.Infrastructure.Repositories;

namespace GoodHamburger.Tests.Unit.Application;

public sealed class OrderServiceTests
{
    // Real catalog & in-memory repo — no mocks needed for this layer
    private static readonly IMenuCatalog Catalog = new MenuCatalog();

    private static OrderService CreateService(IOrderRepository? repo = null) =>
        new(repo ?? new InMemoryOrderRepository(), Catalog);

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_WithSandwichFriesSoda_AppliesTwentyPercentDiscount()
    {
        var svc    = CreateService();
        var result = await svc.CreateAsync(new CreateOrderRequest { MenuItemIds = [1, 4, 5] });

        result.DiscountPercentage.Should().Be(0.20m);
        result.Total.Should().Be(result.Subtotal * 0.80m);
    }

    [Fact]
    public async Task Create_WithSandwichAndSoda_AppliesFifteenPercentDiscount()
    {
        var svc    = CreateService();
        var result = await svc.CreateAsync(new CreateOrderRequest { MenuItemIds = [1, 5] });

        result.DiscountPercentage.Should().Be(0.15m);
    }

    [Fact]
    public async Task Create_WithSandwichAndFries_AppliesTenPercentDiscount()
    {
        var svc    = CreateService();
        var result = await svc.CreateAsync(new CreateOrderRequest { MenuItemIds = [1, 4] });

        result.DiscountPercentage.Should().Be(0.10m);
    }

    [Fact]
    public async Task Create_WithSandwichOnly_AppliesZeroDiscount()
    {
        var svc    = CreateService();
        var result = await svc.CreateAsync(new CreateOrderRequest { MenuItemIds = [1] });

        result.DiscountPercentage.Should().Be(0m);
        result.Total.Should().Be(result.Subtotal);
    }

    [Fact]
    public async Task Create_WithDuplicateItem_ThrowsValidationException()
    {
        var svc = CreateService();
        var act = () => svc.CreateAsync(new CreateOrderRequest { MenuItemIds = [1, 1] });

        await act.Should().ThrowAsync<DomainValidationException>()
            .WithMessage("*Duplicate*");
    }

    [Fact]
    public async Task Create_WithTwoSandwiches_ThrowsValidationException()
    {
        // IDs 1 (X Burger) and 2 (X Egg) — different IDs but both Sandwich type
        var svc = CreateService();
        var act = () => svc.CreateAsync(new CreateOrderRequest { MenuItemIds = [1, 2] });

        await act.Should().ThrowAsync<DomainValidationException>()
            .WithMessage("*Sandwich*");
    }

    [Fact]
    public async Task Create_WithInvalidMenuItemId_ThrowsValidationException()
    {
        var svc = CreateService();
        var act = () => svc.CreateAsync(new CreateOrderRequest { MenuItemIds = [99] });

        await act.Should().ThrowAsync<DomainValidationException>()
            .WithMessage("*does not exist*");
    }

    [Fact]
    public async Task Create_WithEmptyList_ThrowsValidationException()
    {
        var svc = CreateService();
        var act = () => svc.CreateAsync(new CreateOrderRequest { MenuItemIds = [] });

        await act.Should().ThrowAsync<DomainValidationException>();
    }

    // ── GetById ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_ExistingOrder_ReturnsOrder()
    {
        var svc     = CreateService();
        var created = await svc.CreateAsync(new CreateOrderRequest { MenuItemIds = [1] });

        var found = await svc.GetByIdAsync(created.Id);

        found.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetById_NonExistingOrder_ThrowsNotFoundException()
    {
        var svc = CreateService();
        var act = () => svc.GetByIdAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Update_ChangingItems_RecalculatesDiscount()
    {
        var svc     = CreateService();
        var created = await svc.CreateAsync(new CreateOrderRequest { MenuItemIds = [1] });

        var updated = await svc.UpdateAsync(created.Id,
            new UpdateOrderRequest { MenuItemIds = [1, 4, 5] });

        updated.DiscountPercentage.Should().Be(0.20m);
    }

    [Fact]
    public async Task Update_NonExistingOrder_ThrowsNotFoundException()
    {
        var svc = CreateService();
        var act = () => svc.UpdateAsync(Guid.NewGuid(),
            new UpdateOrderRequest { MenuItemIds = [1] });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_ExistingOrder_RemovesIt()
    {
        var svc     = CreateService();
        var created = await svc.CreateAsync(new CreateOrderRequest { MenuItemIds = [1] });

        await svc.DeleteAsync(created.Id);

        var all = await svc.GetAllAsync();
        all.Should().NotContain(o => o.Id == created.Id);
    }

    [Fact]
    public async Task Delete_NonExistingOrder_ThrowsNotFoundException()
    {
        var svc = CreateService();
        var act = () => svc.DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
