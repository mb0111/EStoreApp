using AutoMapper;
using EStore.API.DTO.FileUpload;
using EStore.API.Service.Models.FileUpload;

namespace EStore.API.Profiles
{
    public class FileUploadProfile : Profile
    {
        public FileUploadProfile()
        {
            CreateMap<FileUploadDTO, FileUploadModel>()
                .ReverseMap();
        }
    }
}
