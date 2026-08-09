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

            var categories = await _categoryService.GetAllCategories();

            return Ok(new { _localizer["Success"].Value, categories });

        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CategoryRequest request)
        {
            await _categoryService.CreateCategory(request);

            return Ok();

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetCategoryById(id);

            if (category == null)
                return NotFound();

            return Ok(category);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var deleted = await _categoryService.DeleteCategory(id);
            if(!deleted) return BadRequest();
            return Ok();

        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateById(int id ,[FromBody]CategoryRequest request)
        {
            var updated= await _categoryService.UpdateCategory(id,request);
            if (!updated) return BadRequest();
            return Ok();
        }
        
    }
}
