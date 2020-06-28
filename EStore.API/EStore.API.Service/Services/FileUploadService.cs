using AutoMapper;
using EStore.API.Service.Contracts;
using EStore.API.Service.Helpers;
using EStore.API.Service.Models.FileUpload;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EStore.API.Service.Services
{
    public class FileUploadService : IFileUploadService
    {
        #region Private Property

        private readonly IMimeMappingService _mimeMappingService;
        private readonly IMapper _mapper;
        private readonly ILogger<FileUploadService> _logger;

        #endregion

        #region Constructor

        public FileUploadService(IMimeMappingService mimeMappingService,
                                 IMapper mapper,
                                 ILogger<FileUploadService> logger)
        {
            this._mimeMappingService = mimeMappingService ?? throw new ArgumentNullException(nameof(mimeMappingService));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Service Implementation

        public async Task<Output<FileModel>> ReadFileAsync(FileUploadModel fileUploadModel)
        {
            if (fileUploadModel.File == null)
            {
                _logger.LogInformation($"No file was uploaded");
                return new Output<FileModel>()
                {
                    Status = EnResultStatus.Error,
                    Message = $"No file was uploaded"
                };
            }
            try
            {
                using var ms = new MemoryStream();
                await fileUploadModel.File.OpenReadStream().CopyToAsync(ms);
                byte[] fileBytes = ms.ToArray();
                return new Output<FileModel>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"File was read with success",
                    Result = new FileModel()
                    {
                        Name = fileUploadModel.File.FileName,
                        Data = fileBytes
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading File bytes for File name {fileUploadModel.File.FileName}. Error: {ex.Message}");
                return new Output<FileModel>()
                {
                    Status = EnResultStatus.Error,
                    Message = $"Error reading File bytes for File name {fileUploadModel.File.FileName}"
                };
            }
        }

        public async Task<Output<FormFile>> CreateFormFileAsync(string fileName, byte[] data)
        {
            if (fileName.IsNullOrWhiteSpace())
            {
                _logger.LogInformation($"FileName could not be of string null or empty");
                return new Output<FormFile>()
                {
                    Status = EnResultStatus.Error,
                    Message = $"FormFile could not be created"
                };
            }
            if (data.IsNullOrEmpty())
            {
                _logger.LogInformation($"File data could not be null or empty");
                return new Output<FormFile>()
                {
                    Status = EnResultStatus.Error,
                    Message = $"FormFile could not be created"
                };
            }
            try
            {
                using var ms = new MemoryStream();
                await ms.WriteAsync(data, 0, data.Length);
                var file = new FormFile(ms, 0, ms.Length, null, fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = _mimeMappingService.Map(fileName)
                };
                return new Output<FormFile>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"File was read with success",
                    Result = file
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating FormFile for {fileName}. Error: {ex.Message}");
                return new Output<FormFile>()
                {
                    Status = EnResultStatus.Error,
                    Message = $"Error creating FormFile for {fileName}"
                };
            }
        }

        #endregion
    }
}
