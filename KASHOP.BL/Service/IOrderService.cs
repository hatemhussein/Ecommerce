using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BL.Service
{
    public interface IOrderService
    {
        Task<List<OrderResponse>> GetUserOrders(string userId);
        Task<OrderDetailsResponse?> GetUserOrder(string userId, int orderId);
        Task<List<OrderResponse>> GetAllOrders(OrderStatusEnum orderStatusEnum);
        Task<bool> CancelOrder(string userId, int orderId);
        Task<bool> ChangeOrderStatus(int orderId, ChangeOrderStatusRequest request);
    }
}
