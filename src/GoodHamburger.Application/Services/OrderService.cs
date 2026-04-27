using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Domain.Services;

namespace GoodHamburger.Application.Services;

public sealed class OrderService(IOrderRepository repository, IMenuCatalog catalog) : IOrderService
{
    public async Task<IEnumerable<OrderResponse>> GetAllAsync()
    {
        var orders = await repository.GetAllAsync();
        return orders.Select(MapToResponse);
    }

    public async Task<OrderResponse> GetByIdAsync(Guid id)
    {
        var order = await repository.GetByIdAsync(id)
                    ?? throw new NotFoundException($"Order '{id}' not found.");
        return MapToResponse(order);
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request)
    {
        var items = BuildOrderItems(request.MenuItemIds);
        var discount = DiscountCalculator.Calculate(items);
        var order = Order.Create(items, discount);
        await repository.AddAsync(order);
        return MapToResponse(order);
    }

    public async Task<OrderResponse> UpdateAsync(Guid id, UpdateOrderRequest request)
    {
        var order = await repository.GetByIdAsync(id)
                    ?? throw new NotFoundException($"Order '{id}' not found.");

        var items = BuildOrderItems(request.MenuItemIds);
        var discount = DiscountCalculator.Calculate(items);
        order.Update(items, discount);
        await repository.UpdateAsync(order);
        return MapToResponse(order);
    }

    public async Task DeleteAsync(Guid id)
    {
        var exists = await repository.GetByIdAsync(id);
        if (exists is null)
            throw new NotFoundException($"Order '{id}' not found.");
        await repository.DeleteAsync(id);
    }

    // ── private helpers ──────────────────────────────────────────────────────

    private List<OrderItem> BuildOrderItems(List<int> menuItemIds)
    {
        if (menuItemIds.Count == 0)
            throw new DomainValidationException("An order must contain at least one item.");

        var duplicates = menuItemIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (duplicates.Count > 0)
            throw new DomainValidationException(
                $"Duplicate menu item(s) in order: {string.Join(", ", duplicates)}. Each item can appear only once.");

        var items = menuItemIds.Select(id =>
        {
            var menuItem = catalog.GetById(id)
                           ?? throw new DomainValidationException($"Menu item with id '{id}' does not exist.");
            return OrderItem.Create(menuItem);
        }).ToList();

        ValidateItemTypeConstraints(items);
        return items;
    }

    private static void ValidateItemTypeConstraints(List<OrderItem> items)
    {
        using var enumerator = items
            .GroupBy(i => i.Type)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .GetEnumerator();

        if (enumerator.MoveNext())
            throw new DomainValidationException(
                $"An order can contain only one item of type '{enumerator.Current}'. " +
                "Please select a single sandwich, a single fries, and a single soda.");
    }

    private static OrderResponse MapToResponse(Order order) => new()
    {
        Id                 = order.Id,
        CreatedAt          = order.CreatedAt,
        UpdatedAt          = order.UpdatedAt,
        Items              = order.Items.Select(i => new OrderItemResponse
        {
            Id           = i.Id,
            MenuItemId   = i.MenuItemId,
            MenuItemName = i.MenuItemName,
            UnitPrice    = i.UnitPrice,
            Type         = i.Type.ToString()
        }).ToList(),
        Subtotal           = order.Subtotal,
        DiscountPercentage = order.DiscountPercentage,
        DiscountAmount     = order.DiscountAmount,
        Total              = order.Total
    };
}
