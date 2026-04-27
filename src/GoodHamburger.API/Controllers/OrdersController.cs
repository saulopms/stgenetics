using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

/// <summary>CRUD operations for Good Hamburger orders.</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class OrdersController(IOrderService orderService) : ControllerBase
{
    /// <summary>Returns all orders.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll() => Ok(await orderService.GetAllAsync());

    /// <summary>Returns a single order by its identifier.</summary>
    /// <param name="id">Order GUID.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(Guid id) => Ok(await orderService.GetByIdAsync(id));

    /// <summary>Creates a new order.</summary>
    /// <remarks>
    /// Each order may contain at most one sandwich, one fries and one soda.
    ///
    /// Discount rules applied automatically:
    /// - Sandwich + Fries + Soda → 20 %
    /// - Sandwich + Soda → 15 %
    /// - Sandwich + Fries → 10 %
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var order = await orderService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>Replaces the items of an existing order.</summary>
    /// <param name="id">Order GUID.</param>
    /// <param name="request">New item list.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderRequest request) =>
        Ok(await orderService.UpdateAsync(id, request));

    /// <summary>Deletes an order.</summary>
    /// <param name="id">Order GUID.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await orderService.DeleteAsync(id);
        return NoContent();
    }
}
