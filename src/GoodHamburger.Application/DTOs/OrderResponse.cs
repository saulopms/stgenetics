namespace GoodHamburger.Application.DTOs;

public sealed class OrderResponse
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<OrderItemResponse> Items { get; init; } = [];
    public decimal Subtotal { get; init; }
    public decimal DiscountPercentage { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal Total { get; init; }
}
