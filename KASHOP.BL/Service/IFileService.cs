using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BL.Service
{
    public interface IFileService
    {
        Task<string?> UploadAsync(IFormFile file);
        void Delete(string fileName);
    }
}
