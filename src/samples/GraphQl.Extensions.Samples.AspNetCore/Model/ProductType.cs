using GraphQL.Types;

namespace GraphQl.Extensions.Samples.AspNetCore.Model
{
    public class ProductType : ObjectGraphType<Product>
    {
        public ProductType()
        {
            Field(x => x.Id).Description(nameof(Product.Id));
            Field(x => x.Category).Description(nameof(Product.Category));
            Field(x => x.Code).Description(nameof(Product.Code));
            Field(x => x.Description).Description(nameof(Product.Description));
            Field(x => x.Name).Description(nameof(Product.Name));
        }
    }
}