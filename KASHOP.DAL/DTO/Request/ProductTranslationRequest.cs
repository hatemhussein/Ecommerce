using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.DTO.Request
{
    public class ProductTranslationRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
    }
}
