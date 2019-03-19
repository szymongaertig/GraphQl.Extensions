using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQl.Extensions.Samples.AspNetCore.DAO;

namespace GraphQl.Extensions.Samples.AspNetCore.Model
{
    public class ProductDao : IProductDao
    {
        private static List<Product> _products;
        public Task<List<Product>> GetProducts()
        {
            if (_products == null)
            {
                _products = new List<Product>();

                for (var i = 0; i < 500000; i++)
                {
                    _products.Add(new Product
                    {
                        Id = i + 1,
                        Category = Guid.NewGuid().ToString(),
                        Code = Guid.NewGuid().ToString(),
                        Description = $"Description for product: {i + 1}",
                        Name = $"Name for product: {i + 1}"
                    });
                }
            }
            return Task.FromResult(_products);
        }
    }
}