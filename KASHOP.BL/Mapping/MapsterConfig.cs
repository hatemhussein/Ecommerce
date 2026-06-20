using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BL.Mapping
{
    public static class MapsterConfig
    {
        public static void MapsterConfigRegister()
        {
            TypeAdapterConfig<Category, CategoryResponse>.NewConfig()
                .Map(dest => dest.category_Id, source => source.Id)
                .Map(dest => dest.User, source => source.CreatedBy.UserName)
                .Map(dest=> dest.Name, source => source.Translations.Where(
                    t => t.Language == CultureInfo.CurrentCulture.Name)
                .Select(t => t.Name).FirstOrDefault()
                    );

            TypeAdapterConfig<Product, ProductResponse>.NewConfig()
                .Map(dest => dest.UserCreated, source => source.CreatedBy.UserName)
                .Map(dest => dest.Name, source => source.Translations.Where(
                    t => t.Language == CultureInfo.CurrentCulture.Name)
                .Select(t => t.Name).FirstOrDefault()
                    )
                .Map(dest => dest.MainImage, source => $"https://localhost:7055/images/{source.MainImage}")
                .Map(dest => dest.BrandName, source => source.Brand == null ? null :
                    source.Brand.Translations.Where(
                        t => t.Language == CultureInfo.CurrentCulture.Name)
                    .Select(t => t.Name).FirstOrDefault())
                .Map(dest => dest.SubImages, source => source.Images.Select(i => $"https://localhost:7055/images/{i.ImagePath}"));

            TypeAdapterConfig<Brand, BrandResponse>.NewConfig()
                .Map(dest => dest.User, source => source.CreatedBy.UserName)
                .Map(dest => dest.Name, source => source.Translations.Where(
                    t => t.Language == CultureInfo.CurrentCulture.Name)
                .Select(t => t.Name).FirstOrDefault()
                    );

            TypeAdapterConfig<ProductUpdateRequest, Product>.NewConfig()
                .IgnoreNullValues(true);

            TypeAdapterConfig<Cart, CartResponse>.NewConfig()
                .Map(dest => dest.ProductName, source => source.Product.Translations.Where(
                    t => t.Language == CultureInfo.CurrentCulture.Name)
                .Select(t => t.Name).FirstOrDefault())
                .Map(dest => dest.Price, source => source.Product.Price)
                .Map(dest => dest.ProductImage, source => $"https://localhost:7055/images/{source.Product.MainImage}");

            TypeAdapterConfig<OrderItem, OrderItemResponse>.NewConfig()
                .Map(dest => dest.ProductName, source => source.Product.Translations.FirstOrDefault().Name);

        }
    }
}
