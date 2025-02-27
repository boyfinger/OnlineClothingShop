namespace OnlineClothing.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly string folderName = "images";
        private readonly IWebHostEnvironment _hostEnvironment;

        public FileUploadService(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public async Task<string> UploadImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid image file.");
            }

            string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, folderName);
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadDir, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/" + folderName + "/" + uniqueFileName;
        }
    }
}
