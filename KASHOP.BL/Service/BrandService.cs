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
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;

        public BrandService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<BrandResponse> CreateBrand(BrandRequest request)
        {
            var brand = request.Adapt<Brand>();
            await _brandRepository.CreateAsync(brand);

            return brand.Adapt<BrandResponse>();
        }

        public async Task<List<BrandResponse>> GetBrands()
        {
            var brands = await _brandRepository.GetAllAsync(
                b => b.Status == EntityStatus.Active,
                new string[]
                {
                    nameof(Brand.Translations),
                    nameof(Brand.CreatedBy)
                });

            return brands.Adapt<List<BrandResponse>>();
        }

        public async Task<BrandResponse?> GetBrand(Expression<Func<Brand, bool>> filter)
        {
            var brand = await _brandRepository.GetOne(filter,
                new string[]
                {
                    nameof(Brand.Translations),
                    nameof(Brand.CreatedBy)
                });
            if (brand == null) return null;

            return brand.Adapt<BrandResponse>();
        }

        public async Task<bool> UpdateBrand(int id, BrandRequest request)
        {
            var brand = await _brandRepository.GetOne(b => b.Id == id,
                new string[] { nameof(Brand.Translations) });
            if (brand == null) return false;

            foreach (var translation in request.Translations)
            {
                var existing = brand.Translations.FirstOrDefault(t => t.Language == translation.Language);
                if (existing != null)
                {
                    existing.Name = translation.Name;
                }
                else
                {
                    return false;
                }
            }

            return await _brandRepository.UpdateAsync(brand);
        }

        public async Task<bool> DeleteBrand(int id)
        {
            var brand = await _brandRepository.GetOne(b => b.Id == id,
                new string[] { nameof(Brand.Products) });
            if (brand == null)
            {
                return false;
            }
            if (brand.Products != null && brand.Products.Any())
            {
                return false;
            }
            return await _brandRepository.DeleteAsync(brand);
        }
    }
}
