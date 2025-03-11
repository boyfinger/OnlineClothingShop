using OpenAI.Chat;

namespace OnlineClothing.Services
{
    public class OpenAIService : IOpenAIService
    {
        public ChatCompletion CheckDescription(string description)
        {
            ChatClient client = new(model: "gpt-4o", apiKey: "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");

            string prompt = $"""
        Check if the following product description meets these rules:
        1. No inappropriate content (bloody, pornographic, violent, etc.).
        2. No misleading or inaccurate information.

        Description: "{description}"

        Respond with either:
        - "Valid" if it follows the rules.
        - "Invalid: [reason]" if it breaks the rules.
        """;

            return client.CompleteChat(prompt);
        }
    }
}
