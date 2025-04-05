using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specification
{
    public class CategorySpecification : BaseSpecification<ProductCategory>
    {

        public CategorySpecification(string? sort, string? search) : base(p => (string.IsNullOrEmpty(search) || p.Name.Contains(search)))
        {
            if (!string.IsNullOrWhiteSpace(sort)) 
            {
                if (sort.ToLower().Contains("name"))
                AddOrderByAsc(p => p.Name);
               
            }
           
        }
    }
}
