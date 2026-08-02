using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Dto
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public List<CategoryResponseTranslation> Translations { get; set; }
    }
}
