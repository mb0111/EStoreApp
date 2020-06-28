using EStore.API.Service.Helpers;
using EStore.API.Service.Models.FileUpload;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EStore.API.Service.Contracts
{
    public interface IFileUploadService
    {
        Task<Output<FileModel>> ReadFileAsync(FileUploadModel fileUploadModel);

        Task<Output<FormFile>> CreateFormFileAsync(string fileName, byte[] data);
    }
}
