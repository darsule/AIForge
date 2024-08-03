using Microsoft.CognitiveServices.Speech;

namespace OpenAiAPIDemo.Helpers
{
    public class ContinuousSpeechRecognization
    {
        public static async Task ContinuousRecognizeSpeechAsync()
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus2").
            var config = SpeechConfig.FromSubscription("eb4d969fd6ad4ff8b4c89fc2ba5bc08b", "westus2");

            // Creates a speech recognizer.
            using (var recognizer = new SpeechRecognizer(config))
            {
                // Subscribes to events.
                recognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.RecognizedSpeech)
                    {
                        Console.WriteLine($"We recognized: {e.Result.Text}");
                    }
                    else if (e.Result.Reason == ResultReason.NoMatch)
                    {
                        Console.WriteLine("NOMATCH: Speech could not be recognized.");
                    }
                };

                recognizer.SessionStarted += (s, e) =>
                {
                    Console.WriteLine("\nSession started event.");
                };

                recognizer.SessionStopped += (s, e) =>
                {
                    Console.WriteLine("\nSession stopped event.");
                    Console.WriteLine("\nStop recognition.");
                };

                recognizer.Canceled += (s, e) =>
                {
                    Console.WriteLine($"CANCELED: Reason={e.Reason}");

                    if (e.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                        Console.WriteLine("CANCELED: Did you update the subscription info?");
                    }
                };

                // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                await recognizer.StartContinuousRecognitionAsync();

                Console.WriteLine("Say something... (press Enter to stop)");
                Console.ReadLine();

                // Stops recognition.
                await recognizer.StopContinuousRecognitionAsync();
            }
        }
    }
}
