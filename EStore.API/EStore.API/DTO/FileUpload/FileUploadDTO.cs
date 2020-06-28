using EStore.API.Validation;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;

namespace EStore.API.DTO.FileUpload
{
    [DisplayName("FileUpload")]
    [FileUploadAllowed()]
    public class FileUploadDTO
    {
        public IFormFile File { get; set; }
    }
}
