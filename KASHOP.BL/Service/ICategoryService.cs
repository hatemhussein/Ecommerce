using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BL.Service
{
    public interface ICategoryService
    {
        Task<CategoryResponse> CreateCategory(CategoryRequest categoryRequest);
        Task<List<CategoryResponse>> GetCategories();
        Task<CategoryResponse?> GetCategory(Expression<Func<Category, bool>> filter);
        Task<bool> DeleteCategory(int id);
    }
}
