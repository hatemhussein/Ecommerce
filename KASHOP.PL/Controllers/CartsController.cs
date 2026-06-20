using KASHOP.BL.Service;
using KASHOP.DAL;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.Request;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CartsController(ICartService cartService, IStringLocalizer<SharedResources> localizer)
        {
            _cartService = cartService;
            _localizer = localizer;
        }

        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> AddToCart(AddToCartRequest request)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.AddToCart(request, userId);

            if (!result) return BadRequest();

            return Ok(new
            {
                message = _localizer["Success"].Value
            });
        }

        [HttpGet("")]
        public async Task<IActionResult> GetCart()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var items = await _cartService.GetCart(userId);

            return Ok(new { data = items });

        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveItem([FromRoute] int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var removedItem = await _cartService.RemoveItem(productId, userId);
            if (!removedItem) return BadRequest();
            return Ok(removedItem);
        }

        [HttpPatch("{productId}")]
        public async Task<IActionResult> UpdateQuantity([FromRoute] int productId, [FromBody] UpdateCartRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updated = await _cartService.UpdateQuantity(productId, request.Count, userId);
            if (!updated) return BadRequest();
            return Ok(updated);
        }
    }
}
