using Azure.Core;
using KASHOP.BLL.Dto;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> AddToCart(string userId, CartItemRequest request)
        {
            var product = await _unitOfWork.ProductRepository.GetOneAsync(p => p.Id == request.ProductId);
            if (product is null)
                return Result<bool>.Fail("Product not Found");
            if (request.Count <= 0)
                return Result<bool>.Fail("Count Must be greater than 0");
            if (request.Count > product.Quantity)
                return Result<bool>.Fail("Not enough stock available");
            var existingCart = await _unitOfWork.CartRepository.GetOneAsync(c => c.UserId == userId && c.ProductID == request.ProductId);
            if (existingCart is not null)
            {
                existingCart.Count += request.Count;
                await _unitOfWork.ComplateAsync();
                return Result<bool>.Ok(true);
            }
            var newItem = new CartItem
            {
                UserId = userId,
                ProductID = request.ProductId,
                Count = request.Count,
            };
            await _unitOfWork.CartRepository.CreateAsync(newItem);
            await _unitOfWork.ComplateAsync();
            return Result<bool>.Ok(true);

        }

      
        public async Task<Result<List<CartItemResponse>>> GetCart(string userId)
        {
            var userCartItem = await _unitOfWork.CartRepository.GetAllAsync(c => c.UserId == userId,
                new string[] { nameof(CartItem.Product), $"{nameof(CartItem.Product)}.{nameof(Product.Translations)}" });
            var response = userCartItem.Adapt<List<CartItemResponse>>();
            return Result<List<CartItemResponse>>.Ok(response);

        }

        public async Task<Result<bool>> RemoveFromCart(string userId,int productId)
        {
            var existingItem = await _unitOfWork.CartRepository.GetOneAsync(c => c.UserId == userId && c.ProductID == productId);
            if (existingItem is null)
                return Result<bool>.Fail("Cart Item Not Found");
                _unitOfWork.CartRepository.Delete(existingItem);
            var affectedRows = await _unitOfWork.ComplateAsync();
            return affectedRows > 0 ? Result<bool>.Ok(true,"Remove from Cart Success") : Result<bool>.Fail("Remove from Cart Failed");
        }



        public async Task<Result<bool>> UpdateQuantity(string userId, CartItemRequest request)
        {
            if (request.Count <= 0)
                return Result<bool>.Fail("Count Must Be Greater than 0");

            var cartItem = await _unitOfWork.CartRepository.GetOneAsync(
                filter: c => c.UserId == userId && c.ProductID == request.ProductId,
                includes: new string[] { nameof(CartItem.Product) });

            if (cartItem.Product.Quantity < request.Count)
                return Result<bool>.Fail("Not enough stock available");

            cartItem.Count = request.Count;
            await _unitOfWork.ComplateAsync();
            return Result<bool>.Ok(true, "Update Success");


        }
        public async Task<Result<bool>> ClearCart(string userId)
        {
            var cartItem = await _unitOfWork.CartRepository.GetAllAsync(c => c.UserId == userId);
            if (!cartItem.Any())
                return Result<bool>.Fail("Cart Is already Empty");

            foreach(var item in cartItem)
            {
                 _unitOfWork.CartRepository.Delete(item);

            }
            await _unitOfWork.ComplateAsync();

            return Result<bool>.Ok(true, "Cart Cleared Successfully");
        }


    }
}
