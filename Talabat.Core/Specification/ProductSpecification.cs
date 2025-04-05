using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specification
{
    public class ProductSpecification : BaseSpecification<Product>
    {
        public ProductSpecification(string sort , int? brand , int? category, string? search) : base( p=>
            (! brand.HasValue ||  p.BrandId  == brand ) && (!category.HasValue || p.CategoryId == category) && (string.IsNullOrEmpty(search) || p.Name.Contains(search))

        )
        
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);
            if (!string.IsNullOrEmpty(sort)) 
            {
                switch (sort)
                {
                    case "OrderByAsc":
                        AddOrderByAsc(p => p.Price);
                        break;

                    case "OrderByDesc":
                        AddOrderByDesc(p => p.Price);
                        break;

                    default: AddOrderByAsc(p => p.Name);
                        break;
                }

            }
            
            else 
            {
                AddOrderByAsc(p => p.Name);
            }

           

        }

        public ProductSpecification(int id) : base(p => p.Id == id)
        {

            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);
            
        }

    }
}
