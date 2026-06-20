using KASHOP.BL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.Models;
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
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public OrdersController(IOrderService orderService, IStringLocalizer<SharedResources> localizer)
        {
            _orderService = orderService;
            _localizer = localizer;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _orderService.GetUserOrders(userId);
            return Ok(new
            {
                data = orders
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _orderService.GetUserOrder(userId, id);
            return Ok(new
            {
                data = order
            });
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetAllOrders([FromQuery] OrderStatusEnum orderStatusEnum = OrderStatusEnum.Pending)
        {
            var orders = await _orderService.GetAllOrders(orderStatusEnum);
            return Ok(new
            {
                data = orders
            });
        }

        [HttpPatch("admin/{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeOrderStatusRequest request)
        {
            var result = await _orderService.ChangeOrderStatus(id, request);
            if (!result) return BadRequest();

            return Ok(result);
        }
    }
}
