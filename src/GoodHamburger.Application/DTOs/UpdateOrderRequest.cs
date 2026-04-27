using System.ComponentModel.DataAnnotations;

namespace GoodHamburger.Application.DTOs;

public sealed class UpdateOrderRequest
{
    /// <summary>Replacement list of menu item IDs for this order.</summary>
    [Required, MinLength(1, ErrorMessage = "An order must contain at least one item.")]
    public List<int> MenuItemIds { get; init; } = [];
}
