using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.DTO.Response
{
    public class LoginResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public string? AccessToken { get; set; }
    }
}
