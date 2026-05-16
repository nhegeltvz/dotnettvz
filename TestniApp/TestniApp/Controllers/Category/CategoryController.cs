using Data;
using Data.Model.DTO;
using Microsoft.AspNetCore.Mvc;

using CategoryEntity = Data.Model.Category;

namespace TestniApp.Controllers.Category
{
    [Route("category")]
    public class CategoryController : Controller
    {

        private readonly CategoryStore _categoryStore;
        public CategoryController(CategoryStore categoryStore)
        {
            _categoryStore = categoryStore;
        }

        [HttpGet("list")]
        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryFormDto form)
        {
            if (!ModelState.IsValid)
                return View("List", form);

            var category = new CategoryEntity { Name = form.Name };

            await _categoryStore.CreateCategory(category);

            return RedirectToAction("List");
        }

        [HttpGet("getcategories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryStore.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpDelete("deletecategory")]
        public async Task<IActionResult> DeleteCategory([FromQuery] int id)
        {
            await _categoryStore.RemoveCategoryAsync(id);
            return Ok();
        }
    }
}
