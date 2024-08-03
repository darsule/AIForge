using Microsoft.AspNetCore.Components;
using System.Data;
using OpenAiAPIDemo.Functions;
using Microsoft.AspNetCore.Components.Forms;
using Markdig;
namespace OpenAiAPIDemo.Helpers
{
    public class ChatComponentBase : ComponentBase
    {
        [Inject]
        public OpenAIService OpenAIService { get; set; }
        private readonly FileUpload fileUpload = new FileUpload();
        public List<Message> Messages { get; set; } = new List<Message>();
        public string? UserInput { get; set; }
        public static string? FileContent { get; set; } = string.Empty;
        public string? Prompt { get; set; } = string.Empty;

        public void ClearChat()
        {
            Messages.Clear();
        }
        public async void UploadFiles(InputFileChangeEventArgs e)
        {
            long maxFileSize = 1024 * 1024 * 5;
            List<string> errors = new();

            try
            {
                string tempPath = Path.GetTempPath();
                string userName = Environment.UserName.Split(".").First();
                string targetPath = Path.Combine(tempPath, userName);
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                string targetFileFullPath = Path.Combine(targetPath, e.File.Name);
                await using FileStream fs = new(targetFileFullPath, FileMode.Create);
                await e.File.OpenReadStream(maxFileSize).CopyToAsync(fs);
                fs.Close();

                if (File.Exists(targetFileFullPath)) 
                {
                    FileContent = LeavePolicyIndia.ExtractTextFromDocx(targetFileFullPath);                   
                }

            }
            catch (Exception ex)
            {
                errors.Add($"File: {e.File.Name} Error: {ex.Message}");
            }
            
        }
        public async void SendMessage()
        {   
            if (!string.IsNullOrWhiteSpace(UserInput))
            {
                Prompt = UserInput;
                UserInput = null;
                string? botResponse = string.Empty;

                Messages.Add(new Message { Role = "user", Content = Prompt });
                Messages.Add(new Message { Role = "Assistant", Content = "Loading..." });

                if (!string.IsNullOrEmpty(FileContent))
                {
                    botResponse = await OpenAIService.GetChatCompletionAsync(Prompt, FileContent);
                }
                else
                {
                    botResponse = await OpenAIService.GetCompletionByFunctionCallAsync(Prompt);
                }
                
                // Simulate a bot response
                if (botResponse != null)
                {
                    string formattedResponse = botResponse.Trim();
                    var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                    string html = Markdown.ToHtml(formattedResponse, pipeline);
                    Messages.RemoveAt(Messages.Count - 1);
                    Messages.Add(new Message { Role = "Assistant", Content = html });
                    StateHasChanged();
                    //await TextToSpeechService.SynthesisToSpeakerAsync(botResponse);
                }
                

            }
        }

        public class Message
        {
            public string? Role { get; set; }
            public string? Content { get; set; }
        }
    }
}




