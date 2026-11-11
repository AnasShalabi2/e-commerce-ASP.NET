using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Models
{
    public class Product : AuditableEntity
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public decimal DisCount { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }

        public string MainImage { get; set; }
        public string MainImagePublicId { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<ProductTranslation> Translations { get; set; }


    }
}
