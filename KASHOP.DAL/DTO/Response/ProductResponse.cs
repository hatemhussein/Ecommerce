using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.DTO.Response
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string UserCreated { get; set; }
        public string Name { get; set; }
        public string MainImage { get; set; }
        public List<string> SubImages { get; set; }
        public string BrandName { get; set; }
    }
}
