using AspNetCoreRateLimit;
using Entities.DataTransferObjects;
using Entities.Models;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Presentation.ActionFilters;
using Presentation.Controllers;
using Repositories.Contracts;
using Repositories.EFCore;
using Services;
using Services.Contracts;
using System.Text;

namespace WebApi.Extensions
{
    public static class ServicesExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
        }

        public static void ConfigureRepositoryManager(this IServiceCollection services) { 
        
        services.AddScoped<IRepositoryManager, RepositoryManager>();
        }

        public static void ConfigureServiceManager(this IServiceCollection services)
        {
            services.AddScoped<IServiceManager, ServiceManager>(); 
        }

        public static void ConfigureLoggerService(this IServiceCollection services) =>
            services.AddSingleton<ILoggerService, LoggerManager>();


        public static void ConfigureActionFilters(this IServiceCollection services)
        {
            services.AddScoped<ValidationFilterAttribute>(); //IoC kaydı oluşturuyor.
            services.AddSingleton<LogFilterAttribute>();
            services.AddScoped<ValidateMediaTypeAttribute>();
        }

        public static void ConfigureCors(this IServiceCollection services)
        {
          services.AddCors(options =>
          {
                     options.AddPolicy
                  ("CorsPolicy", builder =>
                  builder.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .WithExposedHeaders("X-Pagination"));
          });
        }

        public static void ConfigureDataShaper(this IServiceCollection services)
        {
            services.AddScoped<IDataShaper<BookDto>, DataShaper<BookDto>>();
        }

        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var systemTextJsonOutputFormatter = config
                .OutputFormatters
                .OfType<SystemTextJsonOutputFormatter>()?.FirstOrDefault();

                if (systemTextJsonOutputFormatter is not null)
                {
                    systemTextJsonOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.btkakademi.hateoas+json");

                    systemTextJsonOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.btkakademi.apiroot+json");
                }

                var xmlOutputFormatter = config
                .OutputFormatters
                .OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();
                
                if (xmlOutputFormatter is not null) 
                {
                    xmlOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.btkakademi.hateoas+xml");

                    xmlOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.btkakademi.apiroot+xml");
                }
            });
        }

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");

                opt.Conventions.Controller<BooksController>()   //bunları yaparak controllerda tanımlamamazı gerekmeyecek Yani ApiVersion ile controllerda belirtmedik.
                    .HasApiVersion(new ApiVersion(1, 0));

                opt.Conventions.Controller<BooksV2Controller>()          //bunları yaparak controllerda tanımlamamız gerekmeyecek. Yani ApiVersion ile controllerda belirtmedik.
                    .HasDeprecatedApiVersion(new ApiVersion(2, 0));
            });
        }

        public static void ConfigureResponseCaching(this IServiceCollection services) =>
            services.AddResponseCaching();
        //buralarda yaptığımız bir işlem Ioc Kaydıdır.

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services) =>
            services.AddHttpCacheHeaders(expirationOpt =>    //Normalde burayı boş bırakırsak defoult değerler geçerli olacak.
            {
                expirationOpt.MaxAge = 90;    // Defoult değer 60 tı bu kodu yazarak 90 e çıkarmış olduk.
                expirationOpt.CacheLocation = CacheLocation.Public;   //Defoult olarak public ti bu kodu yazarak private yapabilirdik.
            },
            validationOpt =>
            {
                validationOpt.MustRevalidate = false;
            });

         public static void ConfigureRateLimitingOptions(this IServiceCollection services)     //Hız sınırlama için.
        {
            var rateLimitRules = new List<RateLimitRule>()
            {
                new RateLimitRule()
                {
                    Endpoint = "*",  //hepsini kapsasın 
                    Limit = 60,       //dakikada üç istek sınırı olsun.
                    Period = "1m"    //1 dakikai içerisinde yalnızca 3 istek alınabilecek.
                }
            };

            services.Configure<IpRateLimitOptions>(opt =>     //Hız için tanımladığımız yukarıdaki kuralları bu kodu yazarak genel kurallar olarak tanımladık.
            {
                opt.GeneralRules = rateLimitRules;
            });

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<User, IdentityRole>(opts =>
            {
                opts.Password.RequireDigit = true;   //Parolada en az bir karekter bulunması gerektiğini belirtir.
                opts.Password.RequireLowercase = false; //küçük harf uyumu olmasın.
                opts.Password.RequireUppercase = false; //büyük harf uyumu olmasın
                opts.Password.RequireNonAlphanumeric = false;  //alfa numerik karakterler içermesin 
                opts.Password.RequiredLength = 6;  //Pasword 6 karekterli olmalı.

                opts.User.RequireUniqueEmail = true;  //aynı eposta ile bir defa kaydolunabilir.
            })
                .AddEntityFrameworkStores<RepositoryContext>()
                .AddDefaultTokenProviders();
        }

        public static void ConfigureJWT(this IServiceCollection services,
            IConfiguration configuration)//IConfiguration configuration bunu appsettings ile bağlantı kurmak için yaptık.
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["secretKey"];

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["validIssuer"],
                    ValidAudience = jwtSettings["validAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                }
            );
        }

        public static void ConfigureSwagger(this IServiceCollection services)    //versiyonlama için yapıldı birden fazla controller aynı anda çalışabilsin diye .
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "BTK Akademi",
                        Version = "v1",
                        Description = "BTK Akademi ASP.NET Core Web API",
                        TermsOfService = new Uri("https://www.btkakademi.gov.tr/"),
                        Contact = new OpenApiContact
                        {
                            Name = "Mustafa Küçük",
                            Email = "mustafa2kucuk3@gmail.com",
                            Url = new Uri("https://www.zafercomert.com")
                        }
                    });
                s.SwaggerDoc("v2", new OpenApiInfo { Title = "BTK Akademi", Version = "v2" });

                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Place to add JWT with Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement()   //Swagger in güvenliği sağlandı.
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer"
                        },
                        new List<string>()
                    }
                });
            });
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBookRepository, BookRepository>(); //BookRepository ifadesinin IBookRepository ifadesi için new leneceği anlamına geliyor bu kodun.
            services.AddScoped<ICategoryRepository, CategoryRepository>();
        }
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookManager>(); //BookRepository ifadesinin IBookRepository ifadesi için new leneceği anlamına geliyor bu kodun.
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<IAuthenticationService, AuthenticationManager>();
        }
    }
}
