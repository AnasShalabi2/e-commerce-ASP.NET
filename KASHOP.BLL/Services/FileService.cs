using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using KASHOP.DAL.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public class FileService : IFileService
    {
        private readonly string[] _allowedExtensions = {".jpg", ".png", ".webp", ".jpeg", ".svg" };
        private const long _maxFileSize = 5 * 1024 * 1024;
        private readonly Cloudinary _cloudinary;


        public FileService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["CloudinarySettings:CloudName"],
                configuration["CloudinarySettings:ApiKey"],
                configuration["CloudinarySettings:ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<Result<FileUploadResult>> UploadAsync(IFormFile file)
        {
            if (file is null || file.Length == 0)
            {
                return Result<FileUploadResult>.Fail("No File Was Provided");
            }
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                return  Result<FileUploadResult>.Fail($"File Type {extension} is Not Allowed");
            }
            if (file.Length > _maxFileSize)
            {
                return Result<FileUploadResult>.Fail("File Size exceeds 5 MB Limit");
            }
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
               {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "KA14"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    return Result<FileUploadResult>.Fail(uploadResult.Error.Message);
                }

                return Result<FileUploadResult>.Ok(new FileUploadResult
                {
                    Url = uploadResult.SecureUri.ToString(),
                    PublicId = uploadResult.PublicId
                });

            }


        }

        public async Task<Result<bool>> Delete(string publicId)
        {
            var delettionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(delettionParams);
            if(result.Result != "ok")
            {
                return Result<bool>.Fail("Faield to delete image from Cloudinary");
            }

            return Result<bool>.Ok(true);
               
        }
    }
}
        
    

    

    

