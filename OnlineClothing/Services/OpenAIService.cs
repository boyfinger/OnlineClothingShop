using OpenAI.Chat;
using DotNetEnv;
using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI;
using System.ClientModel;
using System.Text;

namespace OnlineClothing.Services
{
#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    public class OpenAIService
    {
        private readonly ChatClient _chatClient;
        private readonly OpenAIClient _openAIClient;
        private readonly OpenAIFileClient _fileClient;
        private readonly AssistantClient _assistantClient;
        public OpenAIService()
        {
            Env.Load();
            _chatClient = new(model: "gpt-4o", apiKey: Env.GetString("OPENAI_API_KEY"));
            _openAIClient = new(Env.GetString("OPENAI_API_KEY"));
            _fileClient = _openAIClient.GetOpenAIFileClient();
            _assistantClient = _openAIClient.GetAssistantClient();
        }

        public async Task<string> CheckDescriptionAsync(string description)
        {
            string prompt = $"""
            Check if the following product description meets these rules:
            1. No inappropriate content (bloody, pornographic, violent, etc.).
            2. No misleading or inaccurate information.

            Description: "{description}"

            Respond with either:
            - "Valid" if it follows the rules.
            - "Invalid: [reason]" if it breaks the rules.
            """;

            ChatCompletion result = await _chatClient.CompleteChatAsync(prompt);
            return result.Content[0].Text;
        }

        public async Task<string> CheckImage(string image, string description)
        {
            string prompt = $"""
            Check the following product image and description:

            1. Does the image match this product description: "{description}"?
            2. Does the image contain inappropriate content (bloody, pornographic, violent, etc.)?

            Respond with:
            - "Valid" if the image matches and is appropriate.
            - "Invalid: [reason]" if the image does not match or is inappropriate.
            """;

            Assistant assistant = _assistantClient.CreateAssistant(
                "gpt-4o",
                new AssistantCreationOptions()
                {
                    Instructions = "When asked a question, attempt to answer very concisely. "
                    + "Prefer one-sentence answers whenever feasible."
                }
            );

            // Image 
            OpenAIFile imageFile = _fileClient.UploadFile(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Assets", image),
                FileUploadPurpose.Vision
            );


            AssistantThread thread = _assistantClient.CreateThread(new ThreadCreationOptions()
            {
                InitialMessages =
                {
                    new ThreadInitializationMessage(
                        MessageRole.User,
                        [
                            prompt,
                            MessageContent.FromImageFileId(imageFile.Id),
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
}
