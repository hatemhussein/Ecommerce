using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BL.Service
{
    public class CategoryService:ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryResponse> CreateCategory(CategoryRequest categoryRequest)
        {
            var category = categoryRequest.Adapt<Category>();
            await _categoryRepository.CreateAsync(category);

            return category.Adapt<CategoryResponse>();
        }

        public async Task<List<CategoryResponse>> GetCategories()
        {
            var categories = await _categoryRepository.GetAllAsync(
                cat => cat.Status == EntityStatus.Active,
                new string[] { nameof(Category.Translations),
          nameof(Category.CreatedBy) 
            });

            return categories.Adapt<List<CategoryResponse>>();
        }
        public async Task<CategoryResponse?> GetCategory(Expression<Func<Category, bool>> filter) 
        {
            var category = await _categoryRepository.GetOne(filter, new string[] {nameof(Category.Translations)});
            return category.Adapt<CategoryResponse>();
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetOne(c => c.Id == id);
            if (category == null) {
                return false;
            }
            return await _categoryRepository.DeleteAsync(category);
        }
    }
}
