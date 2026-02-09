using API.MiddleWares;
using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Domain.Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastucture.Extensions;
using Infrastucture.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using Persistence.Services;

namespace API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddDbContext<BinaLiteDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddMinioStorage(configuration);
        services.AddScoped<IFileStorageService, MinioFileStorageService>();

        services.AddScoped(typeof(IRepository<,>), typeof(GenericRepository<,>));

        services.AddDataProtection();


        services.AddIdentityCore<AppUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredLength = 6;
        })
        .AddRoles<IdentityRole>()                 
        .AddSignInManager()
        .AddEntityFrameworkStores<BinaLiteDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IPropertyAdService, PropertyAdService>();
        services.AddScoped<IPropertyAdRepository, PropertyAdRepository>();

        services.AddScoped<IPropertyMediaRepository, PropertyMediaRepository>();

        services.AddScoped<ICityService, CityService>();
        services.AddScoped<ICityRepository, CityRepository>();

        services.AddScoped<IDistrictService, DistrictService>();
        services.AddScoped<IDistrictRepository, DistrictRepository>();

        services.AddTransient<GlobalExceptionMiddleware>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(typeof(Application.Validations.PropertyAd.CreatePropertyAdRequestValidator).Assembly);

        services.AddOpenApi();

        return services;
    }
}
