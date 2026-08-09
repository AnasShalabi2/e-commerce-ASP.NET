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
        Task<List<CategoryResponse>?> GetAllCategories();
        Task<CategoryResponse> CreateCategory(CategoryRequest request);

        Task<CategoryResponse?> GetCategoryById(int id);

        Task<bool> DeleteCategory(int id);
        Task<bool> UpdateCategory(int id , CategoryRequest request);

    }
}
