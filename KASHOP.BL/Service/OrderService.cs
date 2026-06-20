using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BL.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderResponse>> GetUserOrders(string userId)
        {
            var orders = await _orderRepository.GetAllAsync(o => o.UserId == userId,
                includes: new[]
                {
                    nameof(Order.OrderItems),
                    $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}",
                    $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}.{nameof(Product.Translations)}"
                });
            return orders.Adapt<List<OrderResponse>>();
        }

        public async Task<OrderDetailsResponse?> GetUserOrder(string userId, int orderId)
        {
            var order = await _orderRepository.GetOne(
                filter: o => o.UserId == userId && o.Id == orderId,
                includes: new[]
                {
                    nameof(Order.OrderItems),
                    $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}",
                    $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}.{nameof(Product.Translations)}"
                });

            if (order is null) return null;
            return order.Adapt<OrderDetailsResponse>();
        }

        public async Task<List<OrderResponse>> GetAllOrders(OrderStatusEnum orderStatusEnum)
        {
            var orders = await _orderRepository.GetAllAsync(
                filter: o => o.OrderStatus == orderStatusEnum);

            return orders.Adapt(new List<OrderResponse>());
        }

        public async Task<bool> CancelOrder(string userId, int orderId)
        {
            var order = await _orderRepository.GetOne(
                filter: o => o.Id == orderId && o.UserId == userId);

            if (order is null) return false;

            if (order.OrderStatus != OrderStatusEnum.Pending)
            {
                return false;
            }

            order.OrderStatus = OrderStatusEnum.Cancelled; 
            
            return await _orderRepository.UpdateAsync(order);

        }

        public async Task<bool> ChangeOrderStatus(int orderId, ChangeOrderStatusRequest request)
        {
            var order = await _orderRepository.GetOne(o => o.Id == orderId);

            if (request.OrderStatusEnum == OrderStatusEnum.Cancelled ||
                request.OrderStatusEnum == OrderStatusEnum.Delivered) return false;

            if ((int)request.OrderStatusEnum != (int)order.OrderStatus + 1) { return false; }

            order.OrderStatus = request.OrderStatusEnum;
            return await _orderRepository.UpdateAsync(order);
        }

    }
}
