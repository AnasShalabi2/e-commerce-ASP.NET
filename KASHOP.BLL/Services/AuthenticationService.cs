using KASHOP.BLL.Common;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration config)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _config = config;
        }
        public async Task<bool> ConfirmEmail(ConfirmEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return false;
            request.Token = Uri.UnescapeDataString(request.Token);
            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (!result.Succeeded) return false;
            return true;

        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return new LoginResponse()
                {
                    Message = "Invalid Message"

                };

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return new LoginResponse()
                {
                    Message = "email is not confirmed"

                };


            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
                return new LoginResponse()
                {
                    Message = "Invalid Message"
                };
            return new LoginResponse()
            {
                Message = "Success",
                AccessToken = await GenerateJwt(user)

            };
        }

        private async Task<string> GenerateJwt(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userClaims = new List<Claim>() {
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim (ClaimTypes.Name, user.UserName),
                new Claim (ClaimTypes.Email, user.Email),
                new Claim (ClaimTypes.Role, string.Join(",", roles))

            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["ApiSettings:SecurityKey"]!));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var token = new JwtSecurityToken(
                issuer: _config["ApiSettings:issuer"],
                audience: _config["ApiSettings:audience"],
                claims: userClaims,
                expires: DateTime.UtcNow.AddDays(20),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var user = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return new RegisterResponse()
                {
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = Uri.EscapeDataString(token);
            var emailUrl = $"https://localhost:7172/api/Account/ConfirmEmail?token={token}&userid={user.Id}";

            await _emailSender.SendEmailAsync(request.Email, "confirm email", "<div> <h2>Welcome</h2>"
                + $"<a href='{emailUrl}'>confirm </a>" + "</div>");

            return new RegisterResponse()
            {
                Message = "Success"
            };

        }


    }
}

