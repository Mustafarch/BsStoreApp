using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Repositories.EFCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class BookRepository : RepositoryBase<Book>, IBookRepository
    {
        public BookRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateOneBook(Book book) => Create(book);
        

        public void DeleteOneBook(Book book) => Delete(book);


        #region alttaki fonksiyonun eski hali Sayfalama yapılmadan önceki hali
        //public async Task<IEnumerable<Book>> GetAllBooksAsync(BookParameters bookParameters,
        //    bool trackChanges)
        //{
        //    return await FindAll(trackChanges).OrderBy(b => b.Id)
        //        .Skip((bookParameters.PageNumber-1)*bookParameters.PageSize)
        //        .Take(bookParameters.PageSize)
        //        .ToListAsync();
        //}
        #endregion
        public async Task<PagedList<Book>> GetAllBooksAsync(BookParameters bookParameters,
          bool trackChanges)
        {
            //var books = await FindAll(trackChanges)  // filtreleme yapmadan önceki kod hali
            var books = await FindAll(trackChanges)
                .FilterBooks(bookParameters.MinPrice, bookParameters.MaxPrice)
                .Search(bookParameters.SearchTerm)
                //.OrderBy(b => b.Id) //sıralamadan önce
                .Sort(bookParameters.OrderBy)
                .ToListAsync();

            return PagedList<Book>
                .ToPagedList(books, bookParameters.PageNumber, bookParameters.PageSize);
        }

        public async Task<List<Book>> GetAllBooksAsync(bool trackChanges)
        {
            return await FindAll(trackChanges)
                .OrderBy(b => b.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync(bool trackChanges)
        {
            return await _context
                .Books
                .Include(b => b.Category)   //Lazy Loading yaptığımız için Include ekledik.  //hem kitaba hemde category ye ekleme yapacak.
                .OrderBy(b => b.Id)
                .ToListAsync();
        }

        public async Task<Book> GetOneBookIdAsync(int id, bool trackChanges)
        {
            return await FindByCondition(b => b.Id.Equals(id), trackChanges).SingleOrDefaultAsync();
        }

        public void UpdateOneBook(Book book) => Update(book);
       
    }
}
