using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Dto
{
    public class ProductQueryRequest : PaginationRequest
    {
        public int? CategoryId {  get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public string? SortBy { get; set; }
        public bool Descending { get; set; } = false;

    }
}
