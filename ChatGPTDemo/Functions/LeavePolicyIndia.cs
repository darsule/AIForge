using DocumentFormat.OpenXml.Packaging;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Wordprocessing;
using Azure.AI.OpenAI;
namespace OpenAiAPIDemo.Functions
{
    public class LeavePolicyIndia
    {
        static public string Name = "get_leave_policy_details_india";
        
        // Return the function metadata
        static public FunctionDefinition GetFunctionDefinition()
        {
            return new FunctionDefinition()
            {
                Name = Name,
                Description = "Get Globant India Pvt Ltd India leave policy details for various leave types like" +
                " Personal Time off(PTO/Paid Leaves), Leave Without Pay, Be kind to yourself, " +
                "Child hospitalization leave, Maternity and Adoption Leaves, Paternity, Bereavement Leave, " +
                "Public Holidays / Holiday Calendar 2024, Compensatory Time Off, Leave Encashment, Work from Home, " +
                "Unauthorized Leave, Leaves when working from Client Site, leave approver, guidelines for leave policy, " +
                "application of leave, points to keep in mind from the Microsoft word document",
            };
        }

        static public string GetLeavePolicyDetails(string? prompt)
        {
            string filePath = Path.Combine("wwwroot", "Leave Policy- Globant India.docx");
            string jsonResult = ConvertWordToJson(filePath);
            string matchedResult = GetMatchedPolicyData(jsonResult, prompt);

            return matchedResult;
        }

        public static string ExtractTextFromDocx(string filePath)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                var body = wordDoc?.MainDocumentPart?.Document.Body;
                if (body != null)
                {
                    return body.InnerText;
                }
                return string.Empty;
            }
        }
        public static string ConvertWordToJson(string filePath)
        {
            var wordData = new Dictionary<string, object>();
            string currentKey = null;
            string concatenatedParagraphs = "";

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                Body body = wordDoc.MainDocumentPart.Document.Body;

                foreach (var element in body.Elements())
                {
                    if (element is Paragraph)
                    {
                        string paragraphText = ((Paragraph)element).InnerText.Trim();
                        if (!string.IsNullOrEmpty(paragraphText))
                        {
                            if (paragraphText.Length < 40)
                            {
                                if (currentKey != null && !string.IsNullOrEmpty(concatenatedParagraphs))
                                {
                                    wordData[currentKey] = concatenatedParagraphs.Trim();
                                }
                                currentKey = paragraphText;
                                concatenatedParagraphs = "";
                            }
                            else
                            {
                                concatenatedParagraphs += paragraphText + " ";
                            }
                        }
                    }
                    else if (element is Table)
                    {
                        if (currentKey != null && !string.IsNullOrEmpty(concatenatedParagraphs))
                        {
                            wordData[currentKey] = concatenatedParagraphs.Trim();
                            currentKey = null;
                            concatenatedParagraphs = "";
                        }

                        string tableKey = $"Table_{wordData.Count + 1}";
                        var tableData = new Dictionary<string, string>();
                        int rowCount = 0;

                        foreach (TableRow row in element.Elements<TableRow>())
                        {
                            rowCount++;
                            var columnData = new List<string>();

                            foreach (TableCell cell in row.Elements<TableCell>())
                            {
                                columnData.Add(cell.InnerText.Trim());
                            }

                            tableData[$"Row_{rowCount}"] = string.Join(", ", columnData);
                        }

                        wordData[tableKey] = tableData;
                    }
                }

                // Handle the last set of concatenated paragraphs
                if (currentKey != null && !string.IsNullOrEmpty(concatenatedParagraphs))
                {
                    wordData[currentKey] = concatenatedParagraphs.Trim();
                }
            }

            return JsonConvert.SerializeObject(wordData, Formatting.Indented);
        }
        public static string GetMatchedPolicyData(string jsonString, string query)
        {

            var wordData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
            foreach (var key in wordData.Keys)
            {
                if (query.Contains(key, StringComparison.OrdinalIgnoreCase))
                {
                    return JsonConvert.SerializeObject(new Dictionary<string, object> { { key, wordData[key] } }, Formatting.Indented);
                }
            }
            return $"Key matching '{query}' not found.";
        }
    }

    public class WordDocumentData
    {
        public List<string> Paragraphs { get; set; } = new List<string>();
        public List<List<List<string>>> Tables { get; set; } = new List<List<List<string>>>();
    }
}
