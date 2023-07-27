using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CategoryManager : ICategoryService
    {
        private readonly IRepositoryManager _manager;       //bu ve alttaki işleme Constructor İnjection denir araştır.  //Bu yöntem, nesnelerin diğer nesnelerle olan ilişkilerini sağlamak için kullanılır.

        public CategoryManager(IRepositoryManager manager)   //bu ve üstteki işleme Constructor İnjection denir araştır.  //Bu yöntem, nesnelerin diğer nesnelerle olan ilişkilerini sağlamak için kullanılır.
        {
            _manager = manager;
        }

        //public Task<Category> CreateOneCategoryAsync(Category category)
        //{
        //    _manager.Category.CreateOneCategory();
        //    await _manager.SaveAsync();
        //    return entity;
        //}

        //public Task DeleteOneCategoryAsync(int id, bool trackChanges)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(bool trackChanges)
        {
            return await _manager
                .Category
                .GetAllCategoriesAsync(trackChanges);
        }

        public async Task<Category> GetOneCategoryByIdAsync(int id, bool trackChanges)
        {
            var category = await _manager
                .Category
                .GetOneCategoryByIdAsync(id, trackChanges);

            if (category is null)
            {
                throw new CategoryNotFoundException(id);
            }
            return category;
        }

        //public Task UpdateOneCategoryAsync(int id, Category category, bool trackChanges)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
