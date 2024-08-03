using Azure.AI.OpenAI;
using System.Text.Json;

namespace OpenAiAPIDemo.Functions
{
    public class ProductAgent
    {
        static public string Name = "get_product_details";
        static private List<Product> products = Product.LoadProducts();

        // Return the function metadata
        static public FunctionDefinition GetFunctionDefinition()
        {
            return new FunctionDefinition()
            {
                Name = Name,
                Description = "Get product details by product name",
                Parameters = BinaryData.FromObjectAsJson(
                new
                {
                    Type = "object",
                    Properties = new
                    {
                        ProductName = new
                        {
                            Type = "string",
                            Description = "The product name, e.g. Pavlova",
                        }
                    },
                    Required = new[] { "productName" },
                },
                new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
            };
        }

        static public string? GetProductDetails(string product)
        {
            var productDetails = products.Where(p => p.ProductName == product).FirstOrDefault();
            if (productDetails == null)
            {
                return null;
            }
            return productDetails.ToString();
        }
    }

    // Argument for the function
    public class ProductInput
    {
        public string ProductName { get; set; } = string.Empty;
    }
}
