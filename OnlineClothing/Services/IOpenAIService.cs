using OpenAI.Assistants;
using OpenAI.Chat;

namespace OnlineClothing.Services
{
    public interface IOpenAIService
    {
        Task<string> CheckDescription(string description);

        Task<string> CheckImage(string image, string description);
    }
}
