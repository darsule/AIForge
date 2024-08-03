

using Azure.AI.OpenAI;
using Azure;

namespace OpenAiAPIDemo.dump_code
{
    public class dump_code
    {
        //public async Task<string?> GetCompletionAsync(string prompt)
        //{
        //    try
        //    {
        //        var request = new HttpRequestMessage
        //        {
        //            Method = HttpMethod.Post,
        //            RequestUri = new Uri(_apiUrl),
        //            Headers =
        //            {
        //                {
        //                    "Authorization", $"Bearer {_apiKey}"
        //                }
        //            },

        //            Content = JsonContent.Create(new
        //            {
        //                model = "gpt-3.5-turbo",
        //                prompt = prompt,
        //                max_tokens = 150
        //            })
        //        };

        //        var response = await _httpClient.SendAsync(request);

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            _logger.LogError($"OpenAI API request failed with status code {response.StatusCode}");
        //            var errorContent = await response.Content.ReadAsStringAsync();
        //            _logger.LogError($"OpenAI API error content: {errorContent}");
        //            return "Error: Unable to get response from OpenAI API";
        //        }

        //        var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>();

        //        if (result != null)
        //        {
        //            return result.Choices?[0].text;
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Exception occurred while calling OpenAI API: {ex.Message}");
        //        return "Error: Unable to get response from OpenAI API";
        //    }
        //}

        //public async Task<string> GetCompletionAsync(string? prompt)
        //{
        //    try
        //    {
        //        // Adding authorization header
        //        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        //        // Creating request payload
        //        var request = new
        //        {
        //            model = "gpt-3.5-turbo",
        //            messages = new[]
        //            {
        //                // Message to ChatGPT (prompt)
        //                //new { role = "system", content = "Convert string to integer to get salary of the employee from the provided string."},

        //                // User input value
        //                new { role = "user", content = prompt }
        //            }
        //        };
        //        var jsonRequest = JsonSerializer.Serialize(request);
        //        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        //        // Sending request to OpenAI API
        //        HttpResponseMessage response = await _httpClient.PostAsync(_apiUrl, content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            // Parsing and extracting response content
        //            string responseContent = await response.Content.ReadAsStringAsync();
        //            using JsonDocument document = JsonDocument.Parse(responseContent);
        //            var root = document.RootElement;
        //            string answer = root.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        //            return answer;
        //        }
        //        else
        //        {
        //            _logger.LogError($"OpenAI API request failed with status code {response.StatusCode}");
        //            var errorContent = await response.Content.ReadAsStringAsync();
        //            JObject jsonObject = JObject.Parse(errorContent);
        //            string message = jsonObject["error"]?["message"]?.ToString();
        //            _logger.LogError($"OpenAI API error content: {errorContent}");
        //            return message;
        //        }
        //    }
        //    catch (HttpRequestException hre)
        //    {
        //        // Handle HTTP request exceptions
        //        Console.WriteLine("HTTP Request Exception: " + hre.Message);
        //        return "An error occurred while making the HTTP request.";
        //    }
        //    catch (JsonException je)
        //    {
        //        // Handle JSON parsing exceptions
        //        Console.WriteLine("JSON Exception: " + je.Message);
        //        return "An error occurred while parsing the JSON response.";
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Error: Unable to get response from OpenAI API";
        //        // Handle other general exceptions
        //        Console.WriteLine("General Exception: " + ex);
        //        return "An unexpected error occurred.";
        //    }
        //}

        //public async Task<string> GetChatCompletionAsync(string? prompt)
        //{
        //    string? endpoint = GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
        //    string? key = GetEnvironmentVariable("AZURE_OPENAI_API_KEY");

        //    OpenAIClient client1 = new OpenAIClient
        //    (
        //      new Uri(endpoint),
        //      new AzureKeyCredential(key)
        //    );

        //    Response<ChatCompletions> responseWithoutStream = await client1.GetChatCompletionsAsync
        //    (

        //        new ChatCompletionsOptions()
        //        {
        //            DeploymentName = "OpenAIService-demo",
        //            Messages =
        //            {
        //                new ChatRequestSystemMessage ("You are an AI assistant that helps people find information based on there queries."),
        //                new ChatRequestUserMessage(prompt)
        //            },
        //            Temperature = (float)0.5,
        //            MaxTokens = 800,
        //            NucleusSamplingFactor = (float)0.95,
        //            FrequencyPenalty = 0,
        //            PresencePenalty = 0,
        //        }
        //    );

        //    ChatCompletions response = responseWithoutStream.Value;
        //    return response.Choices[0].Message.Content;
        //}
    }
}
