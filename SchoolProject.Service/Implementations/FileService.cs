using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Service.Implementations
{
    public class FileService : IFileService
    {
        #region Fields
        private readonly IWebHostEnvironment webHostEnvironment;
        #endregion

        #region Constructor
        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }
        #endregion

        #region Functions
        public async Task<string> UploadImage(string location, IFormFile file)
        {
            var path = webHostEnvironment.WebRootPath + "/" + location + "/";
            var extention = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString().Replace("-", string.Empty) + extention;
            if (file.Length > 0)
            {
                try
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (FileStream filestreem = File.Create(path + fileName))
                    {
                        await file.CopyToAsync(filestreem);
                        await filestreem.FlushAsync();
                        return $"/{location}/{fileName}";
                    }
                }
                catch (Exception)
                {
                    return "FailedToUploadImage";
                }
            }
            else
            {
                return "NoImage";
            }
        }
        #endregion
    }
}
