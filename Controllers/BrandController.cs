using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.api.Attributes;
using Talabat.Core.Entities;
using Talabat.Core.Repositories_Interfaces;
using Talabat.Core.Specification;

namespace Talabat.APIs.Controllers
{
  
    public class BrandController : BaseApiController
    {
        private readonly IGenericRepository<ProductBrand> _brandRepo;

        public BrandController(IGenericRepository<ProductBrand> brandRepo)
        {
            _brandRepo = brandRepo;
        }


        //[CacheAttributes(30)]
        [HttpGet]
        public async Task<ActionResult<ProductBrand>> GetBrand(string? sort, string? search)
        {
            var spec = new BrandSpecification(sort, search);
            var Brands = await _brandRepo.GetAllWithSpec(spec);
            return Ok(Brands);
        }


        //[CacheAttributes(30)]
        [HttpGet("{id}")]

        public async Task <ActionResult<ProductBrand>> GetBrandById(int id )
        {
            var brand =  await _brandRepo.GetAsync(id);
            if (brand is null)
            {
                return NotFound(new { Message = "Brand not Found" , StatusCode = 404}) ;

            }
            return Ok(brand);
        }

        [HttpPost]

        public async Task<ActionResult<ProductCategory>> AddBrand(ProductBrand model)
        {
            var brand = await _brandRepo.AddAsync(model);
            if (brand.Name == null) { return BadRequest(); }
            return Ok(brand);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductBrand>> DeleteBrand(int id)
        {

            await _brandRepo.DeleteAsync(id);
            return NoContent();

        }


    }
}
