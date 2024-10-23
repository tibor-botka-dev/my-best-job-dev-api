using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoFramework;
using MyBestJob.API.Factories;
using MyBestJob.API.Middleware;
using MyBestJob.BLL.Exceptions;
using MyBestJob.BLL.Services;
using MyBestJob.DAL.Constants;
using MyBestJob.DAL.Database;
using MyBestJob.DAL.Database.Models;
using Serilog;
using Serilog.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyBestJob.API.Extensions;

public static partial class Extensions
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        // Factories
        builder.Services.AddScoped<IUserClaimsPrincipalFactory<User>,
            UserClaimsPrincipalFactory<User, Role>>();
        builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

        // Services
        builder.Services.AddScoped<IInitService, InitService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<IExternalAuthenticationService, ExternalAuthenticationService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<ISettingService, SettingService>();
        builder.Services.AddScoped<IHttpService, HttpService>();
    }

    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        var configuration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.File(
                path: Path.Combine("Logs", "my-best-job.txt"),
                fileSizeLimitBytes: 10485760,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: 15,
                rollingInterval: RollingInterval.Day)
            .ReadFrom.Configuration(builder.Configuration);

        Log.Logger = configuration.CreateLogger();

        Log.Information("Logging configured.");
    }

    public static void AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "My Best Job Api",
                Description = "Manage ASP.NET Core Web API calls"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid JWT Bearer token.",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    public static void AddMongoDb(this WebApplicationBuilder builder)
    {
        var mongoSetting = builder.Configuration.GetSection(nameof(MongoSetting)).Get<MongoSetting>()
            ?? throw new MissingSettingException(nameof(MongoSetting));

        builder.Services.AddIdentity<User, Role>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedAccount = true;
            options.SignIn.RequireConfirmedEmail = true;
            options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
        })
        .AddMongoDbStores<User, Role, Guid>(mongoSetting.ConnectionString, mongoSetting.Database)
        .AddDefaultTokenProviders();

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        builder.Services.AddTransient<IMongoDbConnection>(options =>
            MongoDbConnection.FromConnectionString(mongoSetting.ConnectionString));
        builder.Services.AddTransient<MyBestJobDbContext>();
    }

    public static void AddJwtBearer(this WebApplicationBuilder builder)
    {
        var jwtSetting = builder.Configuration.GetSection(nameof(JwtSetting)).Get<JwtSetting>()
            ?? throw new MissingSettingException(nameof(JwtSetting));

        var googleSetting = builder.Configuration.GetSection(nameof(GoogleSetting)).Get<GoogleSetting>()
            ?? throw new MissingSettingException(nameof(GoogleSetting));

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddGoogle(options =>
            {
                options.ClientId = googleSetting.ClientId;
                options.ClientSecret = googleSetting.ClientSecret;
            })
            .AddCookie(options => options.ForwardDefaultSelector = httpContext =>
                    httpContext.Request.Path.StartsWithSegments("/api")
                        ? JwtBearerDefaults.AuthenticationScheme
                        : default)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new()
                {
                    ValidateIssuerSigningKey = jwtSetting.ValidateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSetting.IssuerSigningKey)),
                    ValidateIssuer = jwtSetting.ValidateIssuer,
                    ValidIssuer = jwtSetting.ValidIssuer,
                    ValidateAudience = jwtSetting.ValidateAudience,
                    ValidAudience = jwtSetting.ValidAudience,
                    RequireExpirationTime = jwtSetting.RequireExpirationTime,
                    ValidateLifetime = jwtSetting.ValidateLifetime,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.Headers["token-expired"] = "true";
                        }

                        return Task.CompletedTask;
                    }
                };
            });
    }

    public static void AddMappingProfiles(this WebApplicationBuilder builder)
    {
        var config = new MapperConfiguration(config => config.AddMaps(AppDomain.CurrentDomain.GetAssemblies()));

        var mapper = config.CreateMapper();
        builder.Services.AddSingleton(mapper);
    }

    public static void AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options => options.AddPolicy(Constants.Policy, builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("token-expired")));
    }

    public static void ConfigureAppSettings(this WebApplicationBuilder builder)
    {
        ConfigureAndValidateAppSettings<AdminSetting>(builder);
        ConfigureAndValidateAppSettings<MongoSetting>(builder);
        ConfigureAndValidateAppSettings<MailSetting>(builder);
        ConfigureAndValidateAppSettings<JwtSetting>(builder);
        ConfigureAndValidateAppSettings<RouteSetting>(builder);
        ConfigureAndValidateAppSettings<GoogleSetting>(builder);
    }

    private static IServiceCollection ConfigureAndValidateAppSettings<T>(WebApplicationBuilder builder)
        where T : class, new()
    {
        var name = typeof(T).Name;
        var result = builder.Services.Configure<T>(builder.Configuration.GetSection(name));

        using var scope = builder.Services.BuildServiceProvider().CreateScope();
        var settings = scope.ServiceProvider.GetRequiredService<IOptions<T>>();

        var context = new ValidationContext(settings.Value);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(settings.Value, context, results, true);

        var configErrors = results
            .Select(x => x.ErrorMessage)
            .ToList();

        return configErrors.Count == 0
            ? result
            : throw new ValidationException($"{configErrors.Count} konfigurációs hiba található a(z) {name}.cs fájlban: {string.Join("\n", configErrors)}");
    }

    public static void ConfigureMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<LocalizationMiddleware>();
    }

    public static void ConfigureLocalization(this WebApplication app)
    {
        var supportedCultures = DataSeeder.Languages.GetSupportedCultures();
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(Constants.DefaultLanguages.DefaultLanguage),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });
    }
}
