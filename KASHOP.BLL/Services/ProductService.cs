using KASHOP.BLL.Dto;
using KASHOP.BLL.Extenstions;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace KASHOP.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IFileService _fileService;

        public ProductService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public IUnitOfWork UnitOfWork { get; }

        public async Task<Result<ProductResponse>> CreateProduct(ProductRequest request)
        {
            {
                if (request.MainImage is null)
                {
                    return Result<ProductResponse>.Fail("Main image is required");
                }
                var uploadResult = await _fileService.UploadAsync(request.MainImage);
                if (!uploadResult.Success)
                {
                    return Result<ProductResponse>.Fail(uploadResult.Message);
                }
                 
                var product = request.Adapt<Product>();

                product.MainImage = uploadResult.Data.Url;
                product.MainImagePublicId = uploadResult.Data.PublicId;

                await _unitOfWork.ProductRepository.CreateAsync(product);
                await _unitOfWork.ComplateAsync();

                return Result<ProductResponse>.Ok(product.Adapt<ProductResponse>());

            }

        }


        public async Task<Result<PaginationResponse<ProductResponse>>> GetAllProducts(ProductQueryRequest request)
        {

            var query = _unitOfWork.ProductRepository.getQueryable(
               includes: new string[] { nameof(Product.Translations), nameof(Product.Category) });

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(p => p.Translations.Any(t => t.Name.Contains(request.Search)));

            if (request.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == request.CategoryId);
            if(request.MinPrice.HasValue)
                query= query.Where(p => p.Price >= request.MinPrice);
            if(request.MaxPrice.HasValue)
                query= query.Where(p => p.Price <= request.MaxPrice);

            var language = CultureInfo.CurrentCulture.Name;

            var sortBy = request.SortBy?.ToLower();
            if (sortBy == "price")
            {
                query = request.Descending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
            }
            else if (sortBy == "rate")
            {
                query = request.Descending ? query.OrderByDescending(p => p.Rate) : query.OrderBy(p => p.Rate);
            }else if(sortBy == "name")
            {
                query = request.Descending ? query.OrderByDescending(p => p.Translations.Where(t => t.Name == language).Select(t => t.Name).FirstOrDefault()) :
                query.OrderBy(p => p.Translations.Where(t => t.Name == language).Select(t => t.Name).FirstOrDefault());
            }else
            {
                query = request.Descending ? query.OrderByDescending(p =>p.Id) : query.OrderBy(p => p.Id);
            }

            var result = await query.ToPaginationAsync(request.Page, request.Limit);

            return Result<PaginationResponse<ProductResponse>>.Ok(result.Adapt<PaginationResponse<ProductResponse>>());

        }

        public async Task<Result<ProductResponse>> GetProduct(int id)
        {
          
                var product = await _unitOfWork.ProductRepository.GetOneAsync(
                       c => c.Id == id,
                        new string[] { nameof(Product.Translations), nameof(Product.Category) });

                if (product is null) { 
                    return Result<ProductResponse>.Fail("Product Not Found");
                 }
                    return Result<ProductResponse>.Ok(product.Adapt<ProductResponse>());


            }
        
        public async Task<Result<bool>> DeleteProduct(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetOneAsync(p => p.Id == id);
            if (product is null)
            {
                return Result<bool>.Fail("Produnt not Found");
            }
            var deleteImageResult = await _fileService.Delete(product.MainImagePublicId);
            if (!deleteImageResult.Success)
            {
                return Result<bool>.Fail(deleteImageResult.Message);
            }
            _unitOfWork.ProductRepository.Delete(product);
            var affectedRows = await _unitOfWork.ComplateAsync();
            return affectedRows > 0 ? Result<bool>.Ok(true) : Result<bool>.Fail("Failed to Delete Product");
        }
    }
}


