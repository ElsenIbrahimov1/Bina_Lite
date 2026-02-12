using API.MiddleWares;
using API.Options;
using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.Options;
using Domain.Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastucture.Extensions;
using Infrastucture.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
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
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "BinaLite API",
                Version = "v1"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter: Bearer {your JWT token}"
            });

            c.AddSecurityRequirement(document => new()
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        });


        // ✅ DbContext
        services.AddDbContext<BinaLiteDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // ✅ MinIO + File storage
        services.AddMinioStorage(configuration);
        services.AddScoped<IFileStorageService, MinioFileStorageService>();

        // ✅ Generic repository
        services.AddScoped(typeof(IRepository<,>), typeof(GenericRepository<,>));

        // ✅ Needed for Identity token providers
        services.AddDataProtection();

        // ✅ Identity
        services.AddIdentityCore<AppUser>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredLength = 6;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<BinaLiteDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        // ✅ JWT options
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        // ✅ AuthN/AuthZ
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.ConfigureOptions<ConfigureJwtBearerOptions>();
        services.AddAuthorization();

        // ✅ Refresh Token DI
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        // ✅ Auth / JWT generator
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();

        // ✅ Your existing services
        services.AddScoped<IPropertyAdService, PropertyAdService>();
        services.AddScoped<IPropertyAdRepository, PropertyAdRepository>();
        services.AddScoped<IPropertyMediaRepository, PropertyMediaRepository>();

        services.AddScoped<ICityService, CityService>();
        services.AddScoped<ICityRepository, CityRepository>();

        services.AddScoped<IDistrictService, DistrictService>();
        services.AddScoped<IDistrictRepository, DistrictRepository>();

        // ✅ Middleware
        services.AddTransient<GlobalExceptionMiddleware>();

        // ✅ AutoMapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // ✅ FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(typeof(Application.Validations.PropertyAd.CreatePropertyAdRequestValidator).Assembly);

        

        return services;
    }
}
