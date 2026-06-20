using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.DTO.Request
{
    public class BrandRequest
    {
        public List<BrandTranslationRequest> Translations { get; set; }
    }
}
