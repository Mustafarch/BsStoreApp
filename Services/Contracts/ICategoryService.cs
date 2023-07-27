using Entities.DataTransferObjects;
using Entities.Models;

namespace Services.Contracts
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(bool trackChanges);
        Task<Category> GetOneCategoryByIdAsync(int id, bool trackChanges);
        //Task<BookDto> CreateOneCategoryAsync(Category category);
        //Task UpdateOneCategoryAsync(int id, Category category, bool trackChanges);
        //// void UpdateOneBook(int id, BookDtoForUpdate bookDto, bool trackChanges);
        //Task DeleteOneCategoryAsync(int id, bool trackChanges);
        ////void DeleteOneBook(int id, bool trackChanges);
        
    }
}
