using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQl.Extensions.Samples.AspNetCore.Model;

namespace GraphQl.Extensions.Samples.AspNetCore.DAO
{
    public interface IProductDao
    {
        Task<List<Product>> GetProducts();
    }
}