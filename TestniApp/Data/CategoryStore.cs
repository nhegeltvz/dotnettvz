using Data.Data;
using Data.Model;
using Data.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Data
{
    public class CategoryStore
    {
        private readonly TicketDbContext _dbContext;

        public CategoryStore(TicketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateCategory(ICategory model)
        {
            var category = new Category();
            category.Name = model.Name;

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

        }

        public async Task<List<Category>> GetCategoriesAsync() => await _dbContext.Categories.ToListAsync();
        public async Task<Category?> FindCategoryAsync(int id) => await _dbContext.Categories.FirstOrDefaultAsync(category => category.Id == id);
        public async Task RemoveCategoryAsync(int id)
        {
            var category = await FindCategoryAsync(id);
            _dbContext.Remove(category);
            await _dbContext.SaveChangesAsync();
        }
    }
}
