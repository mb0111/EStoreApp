using EStore.API.Service.Contracts;
using Microsoft.AspNetCore.StaticFiles;

namespace EStore.API.Service.Services
{
    public class MimeMappingService : IMimeMappingService
    {
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;

        public MimeMappingService(FileExtensionContentTypeProvider contentTypeProvider)
        {
            _contentTypeProvider = contentTypeProvider;
        }

        /// <summary>
        /// Based on filename return content type
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Content Type for the filename</returns>
        public string Map(string fileName)
        {
            if (!_contentTypeProvider.TryGetContentType(fileName, out string contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
