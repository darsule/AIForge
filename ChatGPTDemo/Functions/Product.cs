using CsvHelper;
using System.Formats.Asn1;
using System.Globalization;

namespace OpenAiAPIDemo.Functions
{
    public class Product
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int UnitsInStock { get; set; }
        public float UnitPrice { get; set; }

        // Load products from a csv file named products.csv in the wwwroot folder
        public static List<Product> LoadProducts()
        {
            var products = new List<Product>();
            using (var reader = new StreamReader(Path.Combine("wwwroot", "products.csv")))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    products = csv.GetRecords<Product>().ToList();
                }
            }
            return products;
        }

        public override string ToString()
        {
            return $"Product ID: {ProductId}, Product Name: {ProductName}, Units In Stock: {UnitsInStock}, Unit Price: {UnitPrice}";
        }
    }
}
