namespace OnlineClothing.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadImageAsync(IFormFile? file);
    }
}
