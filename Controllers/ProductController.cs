using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories_Interfaces;
using Talabat.Core.Specification;
using AutoMapper;
using Talabat.APIs.DTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Talabat.api.Attributes;

namespace Talabat.APIs.Controllers
{
    public class ProductController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IMapper _mapper;
      
       

        public ProductController(IGenericRepository<Product> ProductRepo, IMapper mapper)
        {
            _productRepo = ProductRepo;
            _mapper = mapper;
                     
        }

        [AllowAnonymous]
        [CacheAttributes(30)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProduct(string? sort, int? brand, int? category, string? search)
        {
            var Spec = new ProductSpecification(sort, brand, category, search);
            var products = await _productRepo.GetAllWithSpec(Spec);
            if (products == null)
            {
                return NotFound(new { Message = "not found", StatusCode = 404 });
            }

            return Ok(_mapper.Map<IEnumerable<Product> ,IEnumerable<ProductDto>>(products));
        }

        [CacheAttributes(30)]
        [AllowAnonymous] //  public no need for authorization 
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var Spec = new ProductSpecification(id);
            var product = await _productRepo.GetWithSpec(Spec); 

            if (product == null)
            {
                return NotFound(new { Message = "not found", StatusCode = 404 });
            }

            return Ok(_mapper.Map<Product, ProductDto>(product));
        }


        [HttpPost]
        public async Task<ActionResult<ProductDto>> AddProduct(ProductDto productDto)
        {
            if (productDto == null)
                return BadRequest();

            var product = new Product()
            {
                Name = productDto.Name,
                Price = productDto.Price,
                BrandId = productDto.BrandId,
                CategoryId = productDto.CategoryId,
                PictureUrl = productDto.PictureUrl,


            };

            await _productRepo.AddAsync(product);


            var createdProductDto = new ProductDto()
            {
                Name = product.Name,
                Price = product.Price,
                BrandId = product.BrandId,
                CategoryId = product.CategoryId,
                PictureUrl = product.PictureUrl,
            };
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, createdProductDto);


        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme) ]
        
        [HttpDelete("{id}")]
        
        public async Task<ActionResult<ProductDto>> DeleteProduct(int id)
        {
            
            await _productRepo.DeleteAsync(id);
            return NoContent();



        }




       

    }
}

