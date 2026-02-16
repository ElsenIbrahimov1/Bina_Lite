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
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Persistence.Context;
using Persistence.Repositories;
using Persistence.Services;
using System.Text;

namespace API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        // ✅ Swagger + JWT Authorize button (your OpenApiSecuritySchemeReference approach)
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

        // ✅ Options
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<SeedOptions>(configuration.GetSection(SeedOptions.SectionName));

        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        // ✅ AuthN (configure inline => avoids "signature key was not found")
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        // ✅ Policies (AddAuthorization ONLY ONCE)
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Domain.Constants.Policies.ManageCities,
                p => p.RequireRole(Domain.Constants.RoleNames.Admin));

            options.AddPolicy(Domain.Constants.Policies.ManageProperties,
                p => p.RequireAuthenticatedUser());
        });

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