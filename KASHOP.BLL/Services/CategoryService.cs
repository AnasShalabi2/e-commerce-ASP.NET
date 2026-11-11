using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CategoryResponse>> CreateCategory(CategoryRequest request)
        {
            try
            {
                var category = request.Adapt<Category>();
                await _unitOfWork.CategoryRepository.CreateAsync(category);
                await _unitOfWork.ComplateAsync();
                return new Result<CategoryResponse>
                {
                    Success = true,
                    Message = "Success",
                };
            }
            catch (Exception ex)
            {
                return new Result<CategoryResponse>
                {
                    Success = true,
                    Message = ex.InnerException.Message
                };
            }
        }

        public async Task<Result<bool>> DeleteCategory(int id)
        {
          
                var category = await _unitOfWork.CategoryRepository.GetOneAsync(c => c.Id == id);
                if (category == null)
                {

                    return new Result<bool>
                    {
                        Success = false,
                        Message = "Categort Not Found"
                    };
                }
                   _unitOfWork.CategoryRepository.Delete(category);
                var affectedRows = await _unitOfWork.ComplateAsync();
                return new Result<bool>
                {
                    Success = affectedRows > 0 ,
                    Message = affectedRows > 0 ? "Success" : "Failed to delete Category"
                };
             
            }
           



        public async Task<Result<List<CategoryResponse>>> GetAllCategories()
        {
            try
            {
                var categories = await _unitOfWork.CategoryRepository.GetAllAsync(
                   includes : new string[] { nameof(Category.Translations), "CreatedBy" });
                return new Result<List<CategoryResponse>>
                {
                    Success = true,
                    Message = "Success",
                    Data = categories.Adapt<List<CategoryResponse>>()
                };

            }
            catch (Exception ex)
            {
                return new Result<List<CategoryResponse>>
                {
                    Success = false,
                    Message = ex.InnerException.Message
                };

            }
        }

        public async Task<Result<CategoryResponse>>? GetCategoryById(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetOneAsync(
                    c => c.Id == id,
                    new string[] { nameof(Category.Translations) });

                if (category == null)
                {
                    return new Result<CategoryResponse>
                    {
                        Success = false,
                        Message = "Category Not Found"
                    };
                }
                return new Result<CategoryResponse>
                {
                    Success = true,
                    Message = "Success",
                    Data = category.Adapt<CategoryResponse>()
                };
            }

            catch (Exception ex)
            {
                return new Result<CategoryResponse>
                {
                    Success = false,
                    Message = ex.InnerException.Message
                };

            }
        }

        public async Task<Result<bool>> UpdateCategory(int id, CategoryRequest request)
        {
            
                var category = await _unitOfWork.CategoryRepository.GetOneAsync(c => c.Id == id, new string[] { nameof(Category.Translations) });
                if (category == null)
                {
                    return new Result<bool>
                    {
                        Success = false,
                        Message = "Category Not Found"
                    };
                }
                category.Translations = request.Translations.Adapt<List<CategoryTranslation>>();
                _unitOfWork.CategoryRepository.Update(category);
                var affectedRows = await _unitOfWork.ComplateAsync();
                return new Result<bool>
                {
                    Success = affectedRows>0,
                    Message = affectedRows>0 ? "Success" : "Failed to Update",
                };


            }
          

            }
        }
    

        








