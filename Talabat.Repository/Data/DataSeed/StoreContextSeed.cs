using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Repository.Data.DataSeed
{
    public static class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext dbContext)
        {
            // Seed Brands
            if (!dbContext.Set<ProductBrand>().Any())
            {
                var brandData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);
              
                if (brands != null && brands.Any())
                {
                    foreach (var brand in brands)
                    {
                        dbContext.Set<ProductBrand>().Add(brand);
                    }
                    await dbContext.SaveChangesAsync();
                }
            }

            // Seed Categories
            if (!dbContext.Set<ProductCategory>().Any())
            {
                var categoryData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/categories.json");
                var categories = JsonSerializer.Deserialize<List<ProductCategory>>(categoryData);

                if (categories != null && categories.Any())
                {
                    foreach (var category in categories)
                    {
                        dbContext.Set<ProductCategory>().Add(category);
                    }
                    await dbContext.SaveChangesAsync();
                }
            }

            // Seed Products
            if (!dbContext.Set<Product>().Any())
            {
                var productData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/product.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productData);

                if (products != null && products.Any())
                {
                    foreach (var product in products)
                    {
                        dbContext.Set<Product>().Add(product);
                    }
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}