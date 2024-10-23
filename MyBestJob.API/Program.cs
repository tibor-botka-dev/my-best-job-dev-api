using MongoDB.Driver;
using MyBestJob.API.Extensions;
using MyBestJob.API.Localizations;
using MyBestJob.BLL.Exceptions;
using MyBestJob.DAL.Constants;
using MyBestJob.DAL.Database.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

try
{
    builder.ConfigureLogging();
    builder.AddSwagger();
    builder.ConfigureAppSettings();
    builder.AddMongoDb();
    builder.AddJwtBearer();

    builder.Services.AddRouting(x => x.LowercaseUrls = true);
    builder.Services.AddLocalization(x => x.ResourcesPath = "Localizations");
    builder.Services.AddDistributedMemoryCache();

    builder.Services.AddControllers()
        .AddNewtonsoftJson(x =>
        {
            x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            x.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            x.SerializerSettings.Converters.Add(new StringEnumConverter());
        })
        .AddJsonOptions(x => x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
        .AddDataAnnotationsLocalization(x => x.DataAnnotationLocalizerProvider = (type, factory)
            => factory.Create(typeof(Locales)));

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddHttpClient(nameof(HttpClient));

    builder.AddMappingProfiles();
    builder.AddCors();
    builder.AddServices();

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(LogLevel.Trace);
    builder.Host.UseSerilog();

    var app = builder.Build();

    app.ConfigureLocalization();

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.MapControllers();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
    }

    app.UseCors(Constants.Policy);
    app.UseAuthentication();
    app.UseAuthorization();

    app.ConfigureMiddlewares();

    app.Run();
}
catch (MongoException mongoException)
{
    var mongoSetting = builder.Configuration.GetSection(nameof(MongoSetting)).Get<MongoSetting>()
            ?? throw new MissingSettingException(nameof(MongoSetting));

    Log.Error(new Exception($"Mongo DB exception -> ConnectionString = " +
        $"{mongoSetting.ConnectionString}, Database = {mongoSetting.Database}", mongoException), string.Empty);
    throw;
}
catch (Exception ex)
{
    Log.Error(ex, string.Empty);
    throw;
}
