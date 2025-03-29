using OpenAI.Assistants;
using OpenAI.Chat;

namespace OnlineClothing.Services
{
    public interface IOpenAIService
    {
        Task<string> CheckDescriptionAsync(string description);

        Task<string> CheckImage(IFormFile image, string description);
    }
}
