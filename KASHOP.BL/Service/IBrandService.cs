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
    public interface IBrandService
    {
        Task<BrandResponse> CreateBrand(BrandRequest request);
        Task<List<BrandResponse>> GetBrands();
        Task<BrandResponse?> GetBrand(Expression<Func<Brand, bool>> filter);
        Task<bool> UpdateBrand(int id, BrandRequest request);
        Task<bool> DeleteBrand(int id);
    }
}
