using GraphQl.Extensions.Samples.AspNetCore.DAO;
using GraphQL.Types;

namespace GraphQl.Extensions.Samples.AspNetCore.Model
{
    public class ProductsQuery : ObjectGraphType
    {
        public ProductsQuery(IProductDao productDao)
        {
            Field<ListGraphType<ProductType>>("products",
                resolve: context =>
                    productDao.GetProducts());
        }
    }
}