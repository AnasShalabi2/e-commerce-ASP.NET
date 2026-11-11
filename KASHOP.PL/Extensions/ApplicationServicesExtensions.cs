using KASHOP.BLL.Common;
using KASHOP.BLL.Services;
using KASHOP.DAL.Repository;
using KASHOP.PL.Utils;
using Stripe;
using System.Text.Json.Serialization;

namespace KASHOP.PL.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection addApplicationServices(this IServiceCollection Services, IConfiguration Configuration)
    {
            Services.AddOpenApi();
            Services.AddScoped<ICategoryService, CategoryService>();
            Services.AddScoped<IAuthenticationService, AuthenticationService>();
            Services.AddScoped<ISeedData, RoleSeedData>();
            Services.AddScoped<ISeedData, UserSeedData>();
            Services.AddTransient<IEmailSender, EmailSender>();
            Services.AddScoped<IProductService, BLL.Services.ProductService>();
            Services.AddScoped<IFileService, BLL.Services.FileService>();
            Services.AddExceptionHandler<GlobalExceptionHandler>();
            Services.AddProblemDetails();
            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            Services.AddScoped<ICartService, CartService>();
            Services.AddScoped<IOrderService, OrderService>();
            Services.AddScoped<ICheckoutService, BLL.Services.CheckoutService>();
            Services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = Configuration["Stripe:SecretKey"];
            Services.AddControllers().AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            return Services;
        }

    }
}
