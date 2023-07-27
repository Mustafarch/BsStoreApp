using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiVersion("1.0")] //Conventions tanımını ServiceExtensions ta yaptığımız için burdan kaldırabiliriz.
    [ApiExplorerSettings(GroupName = "v1")]
    [ServiceFilter(typeof(LogFilterAttribute))]  // buraya tanımladığımız için tüm proje loglanacak.
    [ApiController]
    [Route("api/books")]
    //[ResponseCache(CacheProfileName = "5mins")] //ServiceExtensions ta Configure edip program.cs te build edip sonra app yaptıktan sonra bu işleme gerek kalmadı ve defould olarak 60 saniye olarak uyguladı.
    //[Route("api/{v:apiversion}/books")]
    /*[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 80)]*/ // Önbelleğe alma işlemi. Bu sadece bu controllerda geçerli diğer controllerlarda ServiceExtensions ta tanımlanan değerler geçerli.
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _manager;

        public BooksController(IServiceManager manager)
        {
                _manager = manager;
        }

        [Authorize]  // ====[Authorize(Roles = "User, Editor, Admin")]  aynı şey
        [HttpHead]
        [HttpGet(Name = "GetAllBooksAsync")]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))] //Hateos, Hypermedia desteği için yazıldı.
        /*[ResponseCache(Duration = 60)]*/    //60 saniye boyunca önbelleğe alabiliriz. //Metoda yakın olan ifade geçerli olacaktır. burdaki 60 saniye değilde 5 dk olması için burdakini silmemiz gerek ki yukarıdaki geçerli olsun 
        public async Task<IActionResult> GetAllBooksAsync([FromQuery]BookParameters bookParameters)
        {
            var linkParameters = new LinkParameters()
            {
                BookParameters = bookParameters,
                HttpContext = HttpContext
            };
                var result  = await _manager.BookService
                                  .GetAllBooksAsync(linkParameters,false);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.metaData));
                return result.linkResponse.HasLinks ?

                Ok(result.linkResponse.LinkedEntities) :
                Ok(result.linkResponse.ShapedEntities);
        }
        #region yukarıdaki kodun sayfalamadan önceki hali
        //public async Task<IActionResult> GetAllBooksAsync([FromQuery] BookParameters bookParameters)
        //{
        //    var books = await _manager.BookService.GetAllBooksAsync(bookParameters, false);
        //    return Ok(books);
        //}
        #endregion

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBookAsync([FromRoute(Name = "id")] int id)
        {
                var book = await _manager.BookService.GetOneBookByIdAsync(id, false);
                return Ok(book);
        }

        [Authorize]
        [HttpGet("details")]
        public async Task<IActionResult> GetAllBooksWithDetailsAsync()
        {
            return Ok(await _manager.BookService.GetAllBooksWithDetailsAsync(false));
        }


        [Authorize(Roles = "Editör, Admin")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost(Name = "CreateOneBookAsync")]
        public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto)
        {
                #region
                //if (bookDto is null)
                //{
                //    BadRequest();
                //}
                //if (!ModelState.IsValid)                         // [ServiceFilter(typeof(ValidationFilterAttribute))] bunu yazdığımız için bu kodların görevini görüyor ve 
                //{                                                  bunlara ihtiyacımız kalmıyor.
                //    return UnprocessableEntity(ModelState);      //Presantation katmanında ActionFilters klasörü altında ValidationFilterAttribute class ında tanımlıyoruz.
                //}                                                 Program.cs tede bunu tanıtmamız gerekiyor => builder.Services.AddScoped<ValidationFilterAttribute>(); //IoC kaydı oluşturuyor. ve
                //ve tanıtıyoruz.
                #endregion
                var book = await _manager.BookService.CreateOneBookAsync(bookDto);
                return StatusCode(201, book);
        }

        [Authorize(Roles = "Editör, Admin")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] int id,
            [FromBody] BookDtoForUpdate bookDto)
        {
            #region Filtreleme
            //    if (bookDto is null)
            //    {
            //        BadRequest(); //400
            //    }
            //if (!ModelState.IsValid) //Yazdığımız şartların kontrol edilmesini sağlar.
            //{
            //    return UnprocessableEntity(ModelState); //422 hatası geçersiz bir nesne var ise
            //}
            //                     ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
            //    [ServiceFilter(typeof(ValidationFilterAttribute))] bunu yazdığımız için bu kodların görevini görüyor ve 
            //  bunlara ihtiyacımız kalmıyor.
            //  //Presantation katmanında ActionFilters klasörü altında ValidationFilterAttribute class ında tanımlıyoruz.
            //  Program.cs tede bunu tanıtmamız gerekiyor => builder.Services.AddScoped<ValidationFilterAttribute>(); //IoC kaydı oluşturuyor. ve
            //  ve tanıtıyoruz.
            #endregion
                await _manager.BookService.UpdateOneBookAsync(id, bookDto, false);
                return NoContent(); //204
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOneBookAsync([FromRoute(Name = "id")] int id)
        { 
                var entity =await _manager.BookService.GetOneBookByIdAsync(id, false);
                return NoContent();
        }
        [Authorize]
        [HttpOptions]
        public IActionResult GetBooksOptions()
        {
            Response.Headers.Add("Allow", "GET, PUT, POST, PATCH, DELETE, HEAD, OPTIONS");
                                //Key      // ---------------Value--------------------//
            return Ok();
        }
    }
}
