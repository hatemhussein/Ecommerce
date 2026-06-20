using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Models
{
    public class Category : AuditableEntity
    {
        public int Id { get; set; }
        public List<CategoryTranslation> Translations { get; set; }
        public List<Product> Products { get; set; }
    }
}
