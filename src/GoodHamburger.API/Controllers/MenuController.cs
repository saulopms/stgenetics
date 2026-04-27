using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

/// <summary>Returns the available menu items.</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class MenuController(IMenuService menuService) : ControllerBase
{
    /// <summary>Retrieves the full menu catalog.</summary>
    /// <returns>List of all available menu items with prices.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MenuItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetMenu() => Ok(menuService.GetMenu());
}
