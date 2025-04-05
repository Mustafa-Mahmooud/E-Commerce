using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specification
{
    public class BrandSpecification : BaseSpecification<ProductBrand> 
    {
        public BrandSpecification(string? sort,string? search):base(b => (string.IsNullOrEmpty(search) || b.Name.Contains(search)))
        {
            if (! string.IsNullOrEmpty(sort))
                if (sort.ToLower().Contains("name"))
                AddOrderByAsc(b => b.Name);

            else AddOrderByAsc (b=>b.Id);
               
           
           
            
        }

    }
}
