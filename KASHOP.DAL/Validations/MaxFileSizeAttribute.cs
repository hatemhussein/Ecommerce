using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Validations
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxSizeInMB;
        public MaxFileSizeAttribute(int maxSizeInMB)
        {
            _maxSizeInMB = maxSizeInMB;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var sizeInMB = file.Length / (1024 * 1024);
                if(sizeInMB > _maxSizeInMB) {
                    return new ValidationResult($"Max size allowed is {_maxSizeInMB}MB");
                }
            }
            return ValidationResult.Success;
        }
    }
}
