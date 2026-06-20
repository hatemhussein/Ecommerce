using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BL.Service
{
    public interface ICartService
    {
        Task<bool> AddToCart(AddToCartRequest request, string userId);
        Task<List<CartResponse>> GetCart(string userId);
        Task<bool> UpdateQuantity(int productId, int count, string userId);
        Task<bool> RemoveItem(int productId, string userId);
        Task<bool> ClearCart(string userId);
    }
}
