using System.ComponentModel.DataAnnotations.Schema;
using Talabat.Core.Entities;

namespace Talabat.APIs.DTOS
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }
        public string? Brand { get; set; }    
        public int BrandId { get; set; }
        public string? Category { get; set; }
        public int CategoryId { get; set; }
    }
}
