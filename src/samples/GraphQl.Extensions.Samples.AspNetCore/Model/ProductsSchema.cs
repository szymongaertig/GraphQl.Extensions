using GraphQL;
using GraphQL.Types;

namespace GraphQl.Extensions.Samples.AspNetCore.Model
{
    public class ProductsSchema : Schema
    {
        public ProductsSchema(IDependencyResolver resolver)
            : base(resolver)
        {
            Query = resolver.Resolve<ProductsQuery>();
        }
    }
}