using Entities.DataTransferObjects;
using Entities.LinkModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IBookLinks //Hypermedia desteği için bu class ı ekledik.
    {
        LinkResponse TryGenerateLinks(IEnumerable<BookDto> booksDto,
            string fields, HttpContext httpContext);
    }
}
