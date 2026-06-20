using KASHOP.BL.Extensions;
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
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileService _fileService;

        public ProductService(IProductRepository productRepository, IFileService fileService)
        {
            _productRepository = productRepository;
            _fileService = fileService;
        }
        public async Task CreateProduct(ProductRequest request)
        {
            var product = request.Adapt<Product>();
            if(request.MainImage != null)
            {
                var imagePath = await _fileService.UploadAsync(request.MainImage);
                product.MainImage = imagePath;
            }
            if (request.SubImages != null)
            {
                foreach(var image in  request.SubImages)
                {
                    var imagePath = await _fileService.UploadAsync(image);
                    product.Images.Add(new ProductImage()
                    {
                        ImagePath = imagePath
                    });
                }
            }
            await _productRepository.CreateAsync(product);
        }

        public async Task<PaginationResponse<ProductResponse>> GetAllProductsAsync(ProductFilterRequest request)
        {
            var query = _productRepository.GetQueryable(
                p => p.Status == EntityStatus.Active,
                new string[]
                {
                    nameof(Product.Translations),
                    nameof(Product.CreatedBy),
                    nameof(Product.Brand),
                    $"{nameof(Product.Brand)}.{nameof(Brand.Translations)}",
                    nameof(Product.Images)
                });

            if(request.Search != null)
            {
                query = query.Where(p => p.Translations.Any(t => t.Name.Contains(request.Search)));
            }
            if (request.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == request.CategoryId);
            if (request.MaxPrice != null)
                query = query.Where(p => p.Price <= request.MaxPrice);
            if (request.MinPrice != null)
                query = query.Where(p => p.Price >= request.MinPrice);
            if (request.MinRate != null)
                query = query.Where(p => p.Rate >= request.MinRate);

            var paginated = await query.ToPaginationAsync(request.Page, request.Limit);

            return new PaginationResponse<ProductResponse>
            {
                Data = paginated.Data.Adapt<List<ProductResponse>>(),
                Limit = paginated.Limit,
                Page = paginated.Page,
                TotalCount = paginated.TotalCount
            };

        }

        public async Task<ProductResponse?> GetProduct(Expression<Func<Product, bool>> filter)
        {
            var product = await _productRepository.GetOne(filter, new string[] { nameof(Product.Translations),
            nameof(Product.CreatedBy),
            nameof(Product.Brand),
            $"{nameof(Product.Brand)}.{nameof(Brand.Translations)}"});
            if (product == null) return null;
            return product.Adapt<ProductResponse>();
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _productRepository.GetOne(p => p.Id == id,
                new string[] {nameof(Product.Images)});
            if (product == null)
            {
                return false;
            }
            _fileService.Delete(product.MainImage);

            foreach (var image in product.Images)
            {
                _fileService.Delete(image.ImagePath);
            }
            return await _productRepository.DeleteAsync(product);
        }

        public async Task<bool> UpdateProduct(int  id, ProductUpdateRequest request)
        {
            var product = await _productRepository.GetOne(p => p.Id == id,
                new string[] {nameof(Product.Translations), nameof(Product.Images)});
            if (product == null) return false;

            request.Adapt(product);

            if(request.Translations != null)
            {
                foreach (var translation in request.Translations)
                {
                    var existing = product.Translations.FirstOrDefault(t => t.Language ==  translation.Language);
                    if (existing != null)
                    {
                        if(translation.Name != null)
                        {
                            existing.Name = translation.Name;
                        }
                        if(translation.Description != null) {
                            existing.Description = translation.Description;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            var oldImage = product.MainImage;
            if(request.MainImage != null)
            {
                _fileService.Delete(oldImage);
                product.MainImage = await _fileService.UploadAsync(request.MainImage);
            }
            else
            {
                product.MainImage = oldImage;
            }

            if(request.SubImages != null)
            {
                foreach(var image in product.Images)
                {
                    _fileService.Delete(image.ImagePath);
                }
                product.Images.Clear();
                foreach(var image in request.SubImages)
                {
                    var imagePath = await _fileService.UploadAsync(image);
                    product.Images.Add(new ProductImage()
                    {
                        ImagePath = imagePath
                    });
                }
            }

            if(request.NewImages != null)
            {
                foreach(var image in request.NewImages)
                {
                    var imagePath = await _fileService.UploadAsync(image);
                    product.Images.Add(new ProductImage()
                    {
                        ImagePath = imagePath
                    });
                }
            }

            return await _productRepository.UpdateAsync(product);
        }

        public async Task<bool> ToggleStatus(int id)
        {
            var product = await _productRepository.GetOne(p=> p.Id == id);

            if (product is null) return false;

            product.Status = product.Status == EntityStatus.Inactive ? EntityStatus.Active : EntityStatus.Inactive;

            return await _productRepository.UpdateAsync(product);
        }

    }
}
