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
    config.CacheProfiles.Add("5mins", new CacheProfile() { Duration = 300 });  //�n belle�e almak i�in.
})
    .AddXmlDataContractSerializerFormatters()
    .AddCustomCsvFormatter()
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly)
    .AddNewtonsoftJson(opt => 
    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

//builder.Services.AddScoped<ValidationFilterAttribute>(); //IoC kayd� olu�turuyor. //WebApi/Extensions/ServiceExtensions a ta��d�k.

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
}
);
    

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();   //builder.Services.AddSwaggerGen(); //versiyonlamadan �nce.

builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureLoggerService();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureActionFilters(); ////builder.Services.AddScoped<ValidationFilterAttribute>(); //IoC kayd� olu�turuyor. //WebApi/Extensions/ServiceExtensions a ta��d�k. �stten ta��nd� ve buraya ba�lant� i�in bu kod yaz�ld�.
builder.Services.ConfigureCors();  //Sayfalama kayd�.
builder.Services.ConfigureDataShaper();
builder.Services.AddCustomMediaTypes();
builder.Services.AddScoped<IBookLinks, BookLinks>();
builder.Services.ConfigureVersioning(); //Versiyonlama i�in
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();  // �nbelle�e alma i�in yap�ld�.
builder.Services.AddMemoryCache();   //H�z s�n�rlama i�in 
builder.Services.ConfigureRateLimitingOptions(); //H�z s�n�rlama i�in.
builder.Services.AddHttpContextAccessor(); //H�z s�n�rlama i�in.
builder.Services.ConfigureIdentity(); // Kullan�c� ad� ve �ifrenin niteliklerini belirttik. //ServiceExtensions ta belirttik.
builder.Services.ConfigureJWT(builder.Configuration);/*builder.Services.AddAuthentication();*/ //�eski hali buydu ServiceExtensionsa ConfigureJWT eklendi�i i�in onu ald�k. // Kullan�c� ad� ve �ifreyi kullanabilmek i�in ekledik.

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
        
    });  // app.UseSwaggerUI(); versiyonlamadan �nce.
}

if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseIpRateLimiting(); //H�z s�n�rlama i�in.
app.UseCors("CorsPolicy"); //Sayfalama izinleri.
app.UseResponseCaching();
app.UseHttpCacheHeaders(); //�nbelle�e alma i�lemi i�in yap�ld�

app.UseAuthentication(); //Kullan�c� ad� ve �ifreyli giri� yap�l�r sonra a�a��daki yetkilendirme yap�l�r.
app.UseAuthorization();  //yetkilendirme. //oturum a��ld�ktan sonra yetkilendirme yap�l�r.

app.MapControllers();

app.Run();
