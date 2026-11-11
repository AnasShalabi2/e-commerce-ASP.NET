using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Dto
{
    public class PaginationResponse<A>
    {
        public List<A> Data { get; set; }
        public int TotalCount {  get; set; }
        public int Page {  get; set; }
        public int Limit { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalCount / Limit);

    }
}
