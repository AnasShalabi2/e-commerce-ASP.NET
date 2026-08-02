using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryResponse> CreateCategory(CategoryRequest request)
        {
            var category = request.Adapt<Category>();
            await _categoryRepository.CreateAsync(category);
            return category.Adapt<CategoryResponse>();
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var categoty = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (categoty == null) return false;
            return await _categoryRepository.DeleteAsync(categoty);
        }



        public async Task<List<CategoryResponse>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllAsync(
                new string[] { nameof(Category.Translations) });

            return categories.Adapt<List<CategoryResponse>>();
        }

        public async Task<CategoryResponse> GetCategory(Expression<Func<Category, bool>> filter)
        {
            var category = await _categoryRepository.GetOneAsync(filter, new string[] { nameof(Category.Translations) });
            return category.Adapt<CategoryResponse>();
        }

        public async Task<bool> UpdateCategory(int id, CategoryRequest request)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id, new string[] { nameof(Category.Translations) });
            if (category == null) return false;

            category.Translations = request.Translations.Adapt<List<CategoryTranslation>>();

            return await _categoryRepository.UpdateAsync(category);
        }
    }
}

