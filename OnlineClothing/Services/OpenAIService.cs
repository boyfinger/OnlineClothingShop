using OpenAI.Chat;
using DotNetEnv;
using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI;
using System.ClientModel;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OnlineClothing.Services
{
#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    public class OpenAIService : IOpenAIService
    {
        private readonly ChatClient _chatClient;
        private readonly OpenAIClient _openAIClient;
        private readonly OpenAIFileClient _fileClient;
        private readonly AssistantClient _assistantClient;
        public OpenAIService()
        {
            Env.Load();
            _chatClient = new(model: "gpt-4o", apiKey: Env.GetString("OPENAI_SECRET_KEY"));
            _openAIClient = new(Env.GetString("OPENAI_SECRET_KEY"));
            _fileClient = _openAIClient.GetOpenAIFileClient();
            _assistantClient = _openAIClient.GetAssistantClient();
        }

        public async Task<string> CheckDescriptionAsync(string description)
        {
            if (description == null)
            {
                return "No description";
            }

            string prompt = $"""
    Check if the following product description meets these rules:
    1. No inappropriate content (bloody, pornographic, violent, offensive, etc.).
    2. No misleading, exaggerated, or overly promotional content. Avoid descriptions that make unproven claims (e.g., “turns you into a celebrity” or “the best in the world”) or unrealistic promises.
    3. Description should focus on real product features (such as fabric, color, design) and maintain a professional, neutral tone.
    4. No unrelated or irrelevant information. Keep the description concise and factual.

    Description: "{description}"

    Respond with either:
    - "Valid" if it follows the rules.
    - "Invalid: [reason]" if it breaks the rules, specifying which rule(s) it violates.
    """;

            ChatCompletion result = await _chatClient.CompleteChatAsync(prompt);
            return result.Content[0].Text;
        }


        public async Task<string> CheckImage(IFormFile image, string description)
        {
            string prompt = $"""
Check the following product image and description:

1. Does the image visually match this product description: "{description}"?
   - The product's key features (such as color, fabric, design) should match the description.
   - The image should not contain any unrelated or misleading elements.
2. Does the image contain inappropriate content (e.g., bloody, pornographic, violent, offensive)?

Respond with:
- "Valid" if the image visually matches the description and is appropriate.
- "Invalid: [reason]" if the image does not match or contains inappropriate content.
""";

            if (string.IsNullOrWhiteSpace(description))
            {
                prompt = $"""
    Does the image contain inappropriate content (e.g., bloody, pornographic, violent, offensive)?

    Respond with:
    - "Valid" if the image is appropriate.
    - "Invalid: [reason]" if the image is inappropriate.
    """;
            }

            Assistant assistant = _assistantClient.CreateAssistant(
                "gpt-4o",
                new AssistantCreationOptions()
                {
                    Instructions = "When asked a question, attempt to answer very concisely. "
                    + "Prefer one-sentence answers whenever feasible."
                }
            );

            // Upload image from IFormFile Stream
            OpenAIFile uploadedFile;
            using (var stream = image.OpenReadStream())
            {
                uploadedFile = _fileClient.UploadFile(stream, image.FileName, FileUploadPurpose.Vision);
            }


            AssistantThread thread = _assistantClient.CreateThread(new ThreadCreationOptions()
            {
                InitialMessages =
                {
                    new ThreadInitializationMessage(
                        MessageRole.User,
                        [
                            prompt,
                            MessageContent.FromImageFileId(uploadedFile.Id),
                            //MessageContent.FromImageUri(linkToPictureOfOrange),
                        ]),
                }
            });
            CollectionResult<StreamingUpdate> streamingUpdates = _assistantClient.CreateRunStreaming(
                thread.Id,
                assistant.Id,
                new RunCreationOptions()
                {
                    //AdditionalInstructions = "When possible, try to sneak in puns if you're asked to compare things.",
                }
            );
            
            StringBuilder result = new StringBuilder();

            foreach (StreamingUpdate streamingUpdate in streamingUpdates)
            {
                if (streamingUpdate.UpdateKind == StreamingUpdateReason.RunCreated)
                {
                    Console.WriteLine($"----------------------- OPENAI VISION STARTED! -----------------------");
                }
                if (streamingUpdate is MessageContentUpdate contentUpdate)
                {
                    result.Append(contentUpdate.Text);
                }
            }
            return result.ToString();
        }
    }
#pragma warning enable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

}
