namespace KASHOP.PL.Extensions
{
    public static  class ServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddControllers();
            Services.AddOpenApi();
            Services.AddOpenApi();
            Services.AddDatabaseServices(Configuration);
            Services.AddLocalizationServices();
            Services.AddIdentityServices();
            Services.AddJwtAuthenticationServices(Configuration);
            Services.addApplicationServices(Configuration);
            return Services;
        }
    }
}
