using System.ComponentModel.DataAnnotations;

namespace GoodHamburger.Application.DTOs;

public sealed class CreateOrderRequest
{
    /// <summary>List of menu item IDs to include in the order.</summary>
    [Required, MinLength(1, ErrorMessage = "An order must contain at least one item.")]
    public List<int> MenuItemIds { get; init; } = [];
}
