using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IServiceManager _services;

        public CategoriesController(IServiceManager services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            return Ok(await _services
                .CategoryService
                .GetAllCategoriesAsync(false));
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneCategoryByIdAsync([FromRoute] int id)
        {
            return Ok(await _services
                .CategoryService
                .GetOneCategoryByIdAsync(id, false));
        }

        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        //[HttpPost(Name = "CreateOneCategory")]
        //public async Task<IActionResult> CreateOneCategoryAsync([FromBody] Category _category)
        //{
        //    var category = await _services.CategoryService.CreateOneCategoryAsync(_category);
        //    return StatusCode(201,);
        //}

    }
}
