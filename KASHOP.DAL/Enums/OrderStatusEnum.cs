using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Enums
{
    public enum OrderStatusEnum
    {
        Pending = 1,
        Confirmed =2,
        Preparing =3,
        Deliverd =4,
        Cancelled =5
    }
}
