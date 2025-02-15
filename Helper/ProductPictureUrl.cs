using AutoMapper;
using Talabat.APIs.DTOS;
using Talabat.Core.Entities;

namespace Talabat.APIs.Helper
{
    public class ProductPictureUrl : IValueResolver<Product, ProductDto, string>
    {
        private readonly IConfiguration _configuration;

        public ProductPictureUrl(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(Product source, ProductDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
               return $"{_configuration["APIBaseUrl"]}/{source.PictureUrl}";
            
            return string.Empty ;
        }
    }
}
