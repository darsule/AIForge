namespace OpenAiAPIDemo.Models
{
    public class OpenAIResponse
    {
        public Choice[]? Choices { get; set; }
    }

    public class Choice
    {
        public string? text { get; set; }
    }
}
