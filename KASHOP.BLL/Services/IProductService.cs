using KASHOP.BLL.Dto;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public interface IProductService
    {
        Task<Result<ProductResponse>> CreateProduct(ProductRequest request);
        Task<Result<PaginationResponse<ProductResponse>>> GetAllProducts(ProductQueryRequest request);
        Task<Result<ProductResponse>> GetProduct(int id);
        Task<Result<bool>> DeleteProduct(int id);

    }
}
