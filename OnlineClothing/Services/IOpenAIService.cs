using OpenAI.Chat;

namespace OnlineClothing.Services
{
    public interface IOpenAIService
    {
        ChatCompletion CheckDescription(string description);
    }
}
