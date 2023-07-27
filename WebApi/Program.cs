using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using Presentation.ActionFilters;
using Repositories.EFCore;
using Services;
using Services.Contracts;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
    config.CacheProfiles.Add("5mins", new CacheProfile() { Duration = 300 });  //Ön belleðe almak için.
})
    .AddXmlDataContractSerializerFormatters()
    .AddCustomCsvFormatter()
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly)
    .AddNewtonsoftJson(opt => 
    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

//builder.Services.AddScoped<ValidationFilterAttribute>(); //IoC kaydý oluþturuyor. //WebApi/Extensions/ServiceExtensions a taþýdýk.

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
}
);
    

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();   //builder.Services.AddSwaggerGen(); //versiyonlamadan önce.

builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureLoggerService();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureActionFilters(); ////builder.Services.AddScoped<ValidationFilterAttribute>(); //IoC kaydý oluþturuyor. //WebApi/Extensions/ServiceExtensions a taþýdýk. Üstten taþýndý ve buraya baðlantý için bu kod yazýldý.
builder.Services.ConfigureCors();  //Sayfalama kaydý.
builder.Services.ConfigureDataShaper();
builder.Services.AddCustomMediaTypes();
builder.Services.AddScoped<IBookLinks, BookLinks>();
builder.Services.ConfigureVersioning(); //Versiyonlama için
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();  // önbelleðe alma için yapýldý.
builder.Services.AddMemoryCache();   //Hýz sýnýrlama için 
builder.Services.ConfigureRateLimitingOptions(); //Hýz sýnýrlama için.
builder.Services.AddHttpContextAccessor(); //Hýz sýnýrlama için.
builder.Services.ConfigureIdentity(); // Kullanýcý adý ve þifrenin niteliklerini belirttik. //ServiceExtensions ta belirttik.
builder.Services.ConfigureJWT(builder.Configuration);/*builder.Services.AddAuthentication();*/ //«eski hali buydu ServiceExtensionsa ConfigureJWT eklendiði için onu aldýk. // Kullanýcý adý ve þifreyi kullanabilmek için ekledik.

builder.Services.RegisterRepositories();
builder.Services.RegisterServices();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerService>();
app.ConfigureExceptionHandler(logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "BTK Akademi v1");
        s.SwaggerEndpoint("/swagger/v2/swagger.json", "BTK Akademi v2");
        
    });  // app.UseSwaggerUI(); versiyonlamadan önce.
}

if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseIpRateLimiting(); //Hýz sýnýrlama için.
app.UseCors("CorsPolicy"); //Sayfalama izinleri.
app.UseResponseCaching();
app.UseHttpCacheHeaders(); //Önbelleðe alma iþlemi için yapýldý

app.UseAuthentication(); //Kullanýcý adý ve þifreyli giriþ yapýlýr sonra aþaðýdaki yetkilendirme yapýlýr.
app.UseAuthorization();  //yetkilendirme. //oturum açýldýktan sonra yetkilendirme yapýlýr.

app.MapControllers();

app.Run();
