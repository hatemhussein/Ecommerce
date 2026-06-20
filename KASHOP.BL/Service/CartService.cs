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
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<bool> AddToCart(AddToCartRequest request, string UserId)
        {
            var product = await _productRepository.GetOne(p => p.Id == request.ProductId);
            if (product == null) return false;
            var ExistingItem = await _cartRepository.GetOne(
                c => c.ProductId == request.ProductId && c.UserId == UserId);

            var currentCount = ExistingItem?.Count ?? 0;
            var newCount = currentCount + request.Count;
            if (newCount > product.Quantity) return false;

            if (ExistingItem != null)
            {
                ExistingItem.Count += request.Count;
                await _cartRepository.UpdateAsync(ExistingItem);
            }
            else
            {
                var CartItem = request.Adapt<Cart>();
                CartItem.UserId = UserId;
                await _cartRepository.CreateAsync(CartItem);
            }
            return true;
        }
        public async Task<List<CartResponse>> GetCart(string userId)
        {
            var items = await _cartRepository.GetAllAsync(
                filter: c => c.UserId == userId,
                new string[]
                {
                    nameof(Cart.Product),
                    $"{nameof(Cart.Product)}.{nameof(Product.Translations)}"
                }
                );

            return items.Adapt<List<CartResponse>>();
        }

        public async Task<bool> RemoveItem(int productId, string userId)
        {
            var item = await _cartRepository.GetOne(
                c => c.ProductId == productId && c.UserId == userId);

            if (item is null) return false;
            return await _cartRepository.DeleteAsync(item);
        }

        public async Task<bool> UpdateQuantity(int productId, int count, string userId)
        {
            var item = await _cartRepository.GetOne(c => c.ProductId == productId 
            && c.UserId == userId);
            if (item is null) return false;

            var product = await _productRepository.GetOne(p => p.Id ==  productId);
            if (count > product.Quantity) return false;

            item.Count = count;

            return await _cartRepository.UpdateAsync(item);
        }
        public async Task<bool> ClearCart(string userId)
        {
            var items = await _cartRepository.GetAllAsync(c => c.UserId == userId);
            if (!items.Any()) return false;

            return await _cartRepository.DeleteRangeAsync(items);
        }
    }
}
