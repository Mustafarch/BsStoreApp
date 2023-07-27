using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Entities.RequestFeatures;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BookManager : IBookService
    {
        private readonly ICategoryService _categoryService;
        private readonly IRepositoryManager _manager;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        private readonly IBookLinks _bookLinks;

        public BookManager(IRepositoryManager manager, ILoggerService logger, IMapper mapper, IBookLinks bookLinks, ICategoryService categoryService)
        {
            _manager = manager;
            _logger = logger;
            _mapper = mapper;
            _bookLinks = bookLinks;
            _categoryService = categoryService;
        }

        public async Task<BookDto> CreateOneBookAsync(BookDtoForInsertion bookDto)
        {
            //var category = await _manager.Category
            //    .GetOneCategoryByIdAsync(bookDto.CategoryId, false);

            //if (category is null)
            //{
            //    throw new CategoryNotFoundException(bookDto.CategoryId);
            //}
            var category = await _categoryService.GetOneCategoryByIdAsync(bookDto.CategoryId, false); // kodu yazdığımız için üsteki koda gerek kalmadı.

            var entity = _mapper.Map<Book>(bookDto);

            _manager.Book.CreateOneBook(entity);
            await _manager.SaveAsync();
            return _mapper.Map<BookDto>(entity);
        }

       // public void DeleteOneBook(int id, bool trackChanges)
        public async Task DeleteOneBookAsync(int id, bool trackChanges)
        {
            var entity = await GetOneBookByIdAndCheckExists(id, trackChanges);
            _manager.Book.DeleteOneBook(entity);
            _manager.SaveAsync();
        }

        #region altakinin eski hali
        //public async Task<(IEnumerable<BookDto> books, MetaData metaData)> GetAllBooksAsync(BookParameters bookParameters,
        //   bool trackChanges)
        //{
        //    if (!bookParameters.ValidPriceRange)
        //    {
        //        throw new PriceOutofRangeBadRequestException();
        //    }

        //    var booksWithMetaData = await _manager.Book.GetAllBooksAsync(bookParameters, trackChanges);
        //    var booksDto = _mapper.Map<IEnumerable<BookDto>>(booksWithMetaData);

        //    return (booksDto, booksWithMetaData.MetaData);
            #endregion
            public async Task<(LinkResponse linkResponse, MetaData metaData)>
            GetAllBooksAsync(LinkParameters linkParameters,
            bool trackChanges)
        {
            if (!linkParameters.BookParameters.ValidPriceRange)
            {
                throw new PriceOutofRangeBadRequestException();
            }

            var booksWithMetaData = await _manager.Book.GetAllBooksAsync(linkParameters.BookParameters, trackChanges);
            var booksDto = _mapper.Map<IEnumerable<BookDto>>(booksWithMetaData);

            var links = _bookLinks.TryGenerateLinks(booksDto,
                linkParameters.BookParameters.Fields,
                linkParameters.HttpContext);

            return (linkResponse: links, metaData: booksWithMetaData.MetaData); ;
        }
        #region yukarıdaki fonksiyonun eski hali sayfalama yapmadan önceki hali
        //public async  Task<IEnumerable<BookDto>> GetAllBooksAsync(BookParameters bookParameters,
        //    bool trackChanges)
        //{
        //    var books = await _manager.Book.GetAllBooksAsync(bookParameters, trackChanges);
        //    return _mapper.Map<IEnumerable<BookDto>>(books);
        //}
        #endregion

        public async Task<List<Book>> GetAllBooksAsync(bool trackChanges)
        {
           var books = await _manager.Book.GetAllBooksAsync(trackChanges);
            return books;
        }

        public async Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync(bool trackChanges)
        {
            return await _manager.Book.GetAllBooksWithDetailsAsync(trackChanges);
        }

        public async Task<BookDto> GetOneBookByIdAsync(int id, bool trackChanges)
        {
            var book = await GetOneBookByIdAndCheckExists(id, trackChanges);
            #region eski hali
            //var book = await _manager.Book.GetOneBookIdAsync(id, trackChanges);  //Üstteki kodu yazdığımız için buna gerek kalmadı zaten fonksiyonumuz kitabı kontrol ediyor.
            //if(book is null)
            //     throw new BookNotFoundException(id);
            #endregion
            return _mapper.Map<BookDto>(book);
        }

        public async Task UpdateOneBookAsync(int id, BookDtoForUpdate bookDto, bool trackChanges)
        //public void UpdateOneBook(int id, BookDtoForUpdate bookDto, bool trackChanges)
        {
            var entity = await GetOneBookByIdAndCheckExists(id, trackChanges);
            #region eski hali
            // ^^^^  //if (entity is null)                    //yukarıdakini yaptığımız için bu işleme gerek kalmadı.
            // throw new BookNotFoundException(id);
            #endregion

            #region Mapping in gereksiz kıldığı işlemler
            //Mapping
            //entity.Title = book.Title;
            //entity.Price = book.Price;
            #endregion
            entity = _mapper.Map<Book>(bookDto); //yukardaki işlemi mapping sayesinde tek kod da çözdük.

            _manager.Book.Update(entity);
            await _manager.SaveAsync();
        }

        private async Task<Book> GetOneBookByIdAndCheckExists(int id, bool trackChanges)
        {
            // var entity = _manager.Book.GetOneBookId(id, trackChanges);
            var entity = await _manager.Book.GetOneBookIdAsync(id, trackChanges);
            if (entity is null)
                throw new BookNotFoundException(id);

            return entity;
        }
    }
}
