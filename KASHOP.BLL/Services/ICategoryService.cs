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
    public interface ICategoryService
    {
        Task<Result<List<CategoryResponse>>> GetAllCategories();
        Task<Result<CategoryResponse>> CreateCategory(CategoryRequest request);

        Task<Result<CategoryResponse>>? GetCategoryById(int id);

        Task<Result<bool>> DeleteCategory(int id);
        Task<Result<bool>> UpdateCategory(int id , CategoryRequest request);

    }
}
