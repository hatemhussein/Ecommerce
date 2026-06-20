using KASHOP.DAL.Data;
using KASHOP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Repository
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<List<Product>> DecreaseQuantityAsync(List<OrderItem> orderItems);
    }
}
