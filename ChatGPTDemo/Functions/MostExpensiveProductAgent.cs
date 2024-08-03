using Azure.AI.OpenAI;

namespace OpenAiAPIDemo.Functions
{
    public class MostExpensiveProductAgent
    {
        static public string Name = "get_most_expensive_product";
        static private List<Product> products = Product.LoadProducts();

        // Return the function metadata
        static public FunctionDefinition GetFunctionDefinition()
        {
            return new FunctionDefinition()
            {
                Name = Name,
                Description = "Get details of the most expensive product",
            };
        }

        static public string? GetMostExpensiveProductDetails()
        {
            var productDetails = products.OrderByDescending(p => p.UnitPrice).FirstOrDefault();
            if (productDetails == null)
            {
                return null;
            }
            return productDetails.ToString();
        }
    }
}
