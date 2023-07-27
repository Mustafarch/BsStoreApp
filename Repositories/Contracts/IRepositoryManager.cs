using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IRepositoryManager
    {
        ICategoryRepository Category { get; } 
        IBookRepository Book { get; }
        Task SaveAsync(); //void Save(); şeklindeydi Task i yazınca void kullanamıyoruz ve silmemiz gerek  ve Save e Async ekleniyor.
    }
}
    