using KASHOP.DAL.Validations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.DTO.Request
{
    public class ProductRequest
    {
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        [AllowedExtensions]
        [MaxFileSize(2)]
        public IFormFile? MainImage { get; set; }
        public List<IFormFile>? SubImages { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public int? BrandId { get; set; }
        public List<ProductTranslationRequest > Translations { get; set; }
    }
}
