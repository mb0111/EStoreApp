using Microsoft.AspNetCore.Http;

namespace EStore.API.Service.Models.FileUpload
{
    public class FileUploadModel
    {
        public IFormFile File { get; set; }
    }
}
