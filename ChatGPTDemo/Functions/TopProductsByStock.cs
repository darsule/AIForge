using Azure.AI.OpenAI;

namespace OpenAiAPIDemo.Functions
{
    public class TopProductsByStock
    {
        static public string Name = "get_top_products_by_unitsinstock";
        static private List<Product> products = Product.LoadProducts();

        // Return the function metadata
        static public FunctionDefinition GetFunctionDefinition()
        {
            return new FunctionDefinition()
            {
                Name = Name,
                Description = "Get top 1,2,3,4,5,.... products details by filtering the units in stock in descending order",
            };
        }

        static public List<Product> GetTopProductDetailsByUnitInStock()
        {
            var productDetails = products
            .OrderByDescending(p => p.UnitsInStock)
            .ToList();

            if (productDetails == null)
            {
                return null;
            }

            return productDetails.ToList();
        }
    }
}
