
using System.Text.Json;
using static System.Environment;
using Azure;
using Azure.AI.OpenAI;
using OpenAiAPIDemo.Functions;
using static OpenAiAPIDemo.Helpers.ChatComponentBase;


namespace OpenAiAPIDemo.Helpers
{
    public class OpenAIService
    {
        //private readonly HttpClient _httpClient;
        private readonly string? _apiKey;
        private readonly string? _apiUrl;
        private readonly string? _model;
        private readonly string? _version;
        //private readonly ILogger<OpenAIService> _logger;

        public OpenAIService(/*HttpClient httpClient,*/ IConfiguration configuration /*, ILogger<OpenAIService> logger*/)
        {
            //_httpClient = httpClient;
            _apiKey = configuration["AzureOpenAiSettings:ApiKey"];
            _apiUrl = configuration["AzureOpenAiSettings:Endpoint"];
            _model = configuration["AzureOpenAiSettings:Model"];
            _version =  configuration["AzureOpenAiSettings:ApiVersion"];
            //_logger = logger;
        }

        public async Task<string> GetChatCompletionAsync(string? prompt, string jsonResult)
        {
            Uri openAIUri = new(_apiUrl);

            // Instantiate OpenAIClient for Azure Open AI.
            OpenAIClient client = new(openAIUri, new AzureKeyCredential(_apiKey));

            ChatCompletionsOptions chatCompletionsOptions = new();
            chatCompletionsOptions.DeploymentName = _model;
            
            ChatChoice responseChoice;
            Response<ChatCompletions> responseWithoutStream;

            var matchedResult = LeavePolicyIndia.GetMatchedPolicyData(jsonResult, prompt);
            chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage($"{jsonResult}\n\nUser Query: {prompt}"));
            
            responseWithoutStream = await client.GetChatCompletionsAsync(chatCompletionsOptions);
            responseChoice = responseWithoutStream.Value.Choices[0];
            return responseChoice.Message.Content;
        }

        public async Task<string> GetCompletionByFunctionCallAsync(string? prompt)
        {
            Uri openAIUri = new(_apiUrl);

            // Instantiate OpenAIClient for Azure Open AI.
            OpenAIClient client = new(openAIUri, new AzureKeyCredential(_apiKey));
            ChatCompletionsOptions chatCompletionsOptions = new();
            chatCompletionsOptions.DeploymentName = _model;
            ChatChoice responseChoice;
            Response<ChatCompletions> responseWithoutStream;

            // Add function definitions
            FunctionDefinition getProductFunctionDefinition = ProductAgent.GetFunctionDefinition();
            FunctionDefinition getMostExpensiveProductDefinition = MostExpensiveProductAgent.GetFunctionDefinition();
            FunctionDefinition getTopProductByUnitPriceDefinition = TopProductsByUnitPrice.GetFunctionDefinition();
            FunctionDefinition getTopProductByUnitInStockDefinition = TopProductsByStock.GetFunctionDefinition();
            FunctionDefinition getLeavePolicyDetailsDefinition = LeavePolicyIndia.GetFunctionDefinition();

            chatCompletionsOptions.Functions.Add(getProductFunctionDefinition);
            chatCompletionsOptions.Functions.Add(getMostExpensiveProductDefinition);
            chatCompletionsOptions.Functions.Add(getTopProductByUnitPriceDefinition);
            chatCompletionsOptions.Functions.Add(getTopProductByUnitInStockDefinition);
            chatCompletionsOptions.Functions.Add(getLeavePolicyDetailsDefinition);
            
            chatCompletionsOptions.Messages.Add(
                new ChatRequestUserMessage(prompt)
            );

            responseWithoutStream = await client.GetChatCompletionsAsync(chatCompletionsOptions);
            responseChoice = responseWithoutStream.Value.Choices[0];

            while (responseChoice.FinishReason!.Value == CompletionsFinishReason.FunctionCall)
            {
                // Add message as a history.
                chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage(responseChoice.Message.ToString()));
                
                if (responseChoice.Message.FunctionCall.Name == ProductAgent.Name)
                {
                    string unvalidatedArguments = responseChoice.Message.FunctionCall.Arguments;
                    ProductInput input = JsonSerializer.Deserialize<ProductInput>(unvalidatedArguments,
                      new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })!;
                    var functionResultData = ProductAgent.GetProductDetails(input.ProductName);
                    var functionResponseMessage = new ChatRequestFunctionMessage(
                      ProductAgent.Name,
                        JsonSerializer.Serialize(
                            functionResultData,
                            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                      )
                    );
                    chatCompletionsOptions.Messages.Add(new ChatRequestSystemMessage("Don't waste time to look answer in your model, just transform content of role 'function' to plan human readable english"));
                    chatCompletionsOptions.Messages.Add(functionResponseMessage);
                }
                else if (responseChoice.Message.FunctionCall.Name == MostExpensiveProductAgent.Name)
                {

                    var functionResultData = MostExpensiveProductAgent.GetMostExpensiveProductDetails();
                    var functionResponseMessage = new ChatRequestFunctionMessage(
                      MostExpensiveProductAgent.Name,
                        JsonSerializer.Serialize(
                            functionResultData,
                            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                        )
                    );
                    chatCompletionsOptions.Messages.Add(new ChatRequestSystemMessage("Don't waste time to look answer in your model, just transform content of role 'function' to plan human readable english"));
                    chatCompletionsOptions.Messages.Add(functionResponseMessage);
                }
                else if (responseChoice.Message.FunctionCall.Name == TopProductsByUnitPrice.Name)
                {
                    var functionResultData = TopProductsByUnitPrice.GetTopProductDetailsByUnitPrice();
                    var functionResponseMessage = new ChatRequestFunctionMessage(
                      TopProductsByUnitPrice.Name,
                        JsonSerializer.Serialize(
                            functionResultData,
                            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                      )
                    );
                    chatCompletionsOptions.Messages.Add(new ChatRequestSystemMessage("Don't waste time to look answer in your model, just transform content of role 'function' to plan human readable english"));
                    chatCompletionsOptions.Messages.Add(functionResponseMessage);
                }
                else if (responseChoice.Message.FunctionCall.Name == TopProductsByStock.Name)
                {
                    var functionResultData = TopProductsByStock.GetTopProductDetailsByUnitInStock();
                    var functionResponseMessage = new ChatRequestFunctionMessage(
                      TopProductsByUnitPrice.Name,
                        JsonSerializer.Serialize(
                            functionResultData,
                            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                      )
                    );
                    chatCompletionsOptions.Messages.Add(new ChatRequestSystemMessage("Don't waste time to look answer in your model, just transform content of role 'function' to plan human readable english"));
                    chatCompletionsOptions.Messages.Add(functionResponseMessage);
                }
                else if (responseChoice.Message.FunctionCall.Name == LeavePolicyIndia.Name)
                {
                    var functionResultData = LeavePolicyIndia.GetLeavePolicyDetails(prompt);
                    var functionResponseMessage = new ChatRequestFunctionMessage(
                      LeavePolicyIndia.Name,
                        functionResultData
                    );
                    chatCompletionsOptions.Messages.Add(new ChatRequestSystemMessage("Don't waste time to look answer in your model, just transform content of role 'function' to plan human readable english"));
                    chatCompletionsOptions.Messages.Add(functionResponseMessage);
                }

                // Call LLM again to generate the response.

                responseWithoutStream = await client.GetChatCompletionsAsync(chatCompletionsOptions);
                responseChoice = responseWithoutStream.Value.Choices[0];
            }
            return responseChoice.Message.Content;

        }
    }

    

}
