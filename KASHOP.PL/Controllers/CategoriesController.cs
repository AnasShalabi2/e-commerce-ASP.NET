using KASHOP.BLL.Services;
using KASHOP.DAL.Data;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using KASHOP.PL.Resources;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ICategoryService _categoryService;

        public CategoriesController(IStringLocalizer<SharedResources> localizer, ICategoryService categoryService) {
            _localizer = localizer;
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var result = await _categoryService.GetAllCategories();

            return result.Success ? Ok(result) : BadRequest(result);

        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CategoryRequest request)
        {
            var result = await _categoryService.CreateCategory(request);


            return result.Success ? Ok(result) : BadRequest(result);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoryService.GetCategoryById(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var result = await _categoryService.DeleteCategory(id);

            return result.Success ? Ok(result) : NotFound(result);

        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateById(int id ,[FromBody]CategoryRequest request)
        {
            var result = await _categoryService.UpdateCategory(id,request);
            return result.Success ? Ok(result) : NotFound(result);

        }

    }
}
