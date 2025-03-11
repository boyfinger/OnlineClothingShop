using OpenAI.Chat;
using DotNetEnv;
namespace OnlineClothing.Services
{
    public class OpenAIService : IOpenAIService
    {
        public ChatCompletion CheckDescription(string description)
        {
            Env.Load();
            ChatClient client = new(model: "gpt-4o", apiKey: Env.GetString("OPENAI_SECRET_KEY"));

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
