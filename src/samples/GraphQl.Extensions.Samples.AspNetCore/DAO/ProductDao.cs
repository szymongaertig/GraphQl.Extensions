using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQl.Extensions.Samples.AspNetCore.DAO;

namespace GraphQl.Extensions.Samples.AspNetCore.Model
{
    public class ProductDao : IProductDao
    {
        public Task<List<Product>> GetProducts()
        {
            var result = new List<Product>();

            for (var i = 0; i < 100; i++)
            {
                result.Add(new Product
                {
                    Id = i + 1,
                    Category = Guid.NewGuid().ToString(),
                    Code = Guid.NewGuid().ToString(),
                    Description = $"Description for product: {i + 1}",
                    Name = $"Name for product: {i + 1}"
                });
            }

            return Task.FromResult(result);
        }
    }
}