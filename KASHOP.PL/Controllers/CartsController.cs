using Azure.Core;
using KASHOP.BLL.Dto;
using KASHOP.BLL.Services;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : BaseApiController
    {
        private readonly ICartService _cartService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CartsController(ICartService cartService, IStringLocalizer<SharedResources> localizer)
        {
            _cartService = cartService;
            _localizer = localizer;

        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(CartItemRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.AddToCart(CurrentUserId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _cartService.GetCart(CurrentUserId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id) {
            var result = await _cartService.RemoveFromCart(CurrentUserId, id);
            return result.Success ? Ok(result) : BadRequest(result);
          
    }
        [HttpPut]
        public async Task<IActionResult> UpdateQuantity(CartItemRequest request)
        {
            var result = await _cartService.UpdateQuantity(CurrentUserId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _cartService.ClearCart(CurrentUserId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
 

