using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Validations
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        string[] _extensions = { ".png", ".jpg" };
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension))
                {
                    return new ValidationResult($"Allowed extensions are: {string.Join(", ", _extensions)} ");
                }
            }
            return ValidationResult.Success;

        }
    }
}
