using Microsoft.CognitiveServices.Speech;

namespace OpenAiAPIDemo.Helpers
{
    public class TextToSpeechService
    {
        public static async Task SynthesisToSpeakerAsync(string response)
        {
            // To support Chinese Characters on Windows platform
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Console.InputEncoding = System.Text.Encoding.Unicode;
                Console.OutputEncoding = System.Text.Encoding.Unicode;
            }

            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var endpoint = new Uri("https://westus2.api.cognitive.microsoft.com/");
            var config = SpeechConfig.FromSubscription("eb4d969fd6ad4ff8b4c89fc2ba5bc08b", "westus2");

            // Set the voice name, refer to https://aka.ms/speech/voices/neural for full list.
            config.SpeechSynthesisVoiceName = "en-US-AvaMultilingualNeural";


            // Creates a speech synthesizer using the default speaker as audio output.
            using (var synthesizer = new SpeechSynthesizer(config))
            {               
                using (var result = await synthesizer.SpeakTextAsync(response))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        Console.WriteLine($"Speech synthesized to speaker for text [{response}]");
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }
                    }
                }
            }
        }
    }
}
