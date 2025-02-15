using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories_Interfaces;
using Talabat.Core.Specification;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
    public class CategoryController : BaseApiController
    {
        private readonly IGenericRepository<ProductCategory> _categoryRepo;

        public CategoryController(IGenericRepository<ProductCategory> CategoryRepo)
        {
            _categoryRepo = CategoryRepo;
        }

        [HttpGet]
        public async Task<ActionResult<ProductCategory>> GetCategory(string? sort, string? search)
        {
            var spec = new CategorySpecification(sort, search); 
            var Cateogries = await _categoryRepo.GetAllWithSpec(spec);
            return Ok(Cateogries);

        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCategory>> GetCategoryById(int id)
        {
            var category = await _categoryRepo.GetAsync(id); 

            if (category == null)
            {
              
                return NotFound(new { Message = "Category not found", StatusCode = 404 });
            }

            return Ok(category);
        }

        [HttpPost]

        public async Task<ActionResult<ProductCategory>> AddCategory(ProductCategory model)
        {
            var category = await _categoryRepo.AddAsync(model);
            if ( category.Name == null) { return BadRequest(); }
            return Ok(category);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductCategory>> DeleteCategory(int id)
        {
            
            await _categoryRepo.DeleteAsync(id);
            return NoContent();


            
        }
    }
}
