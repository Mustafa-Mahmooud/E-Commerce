using AutoMapper;
using Talabat.APIs.DTOS;
using Talabat.Core.Entities;

namespace Talabat.APIs.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ForMember(d=> d.Brand ,s => s.MapFrom(o => o.Brand) );
            CreateMap<Product, ProductDto>().ForMember(d => d.Category, s => s.MapFrom(o => o.Category));
            CreateMap<Product, ProductDto>().ForMember(d => d.PictureUrl, o => o.MapFrom<ProductPictureUrl>());

        }
    }
}
