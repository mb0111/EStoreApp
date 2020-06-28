using EStore.API.DTO.FileUpload;
using EStore.API.Service.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace EStore.API.Validation
{
    public class FileUploadAllowed : ValidationAttribute
    {
        private readonly string errorMemeber = "FileUpload";
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var fileUploadDTO = (FileUploadDTO)validationContext.ObjectInstance;
            string errorMessage = string.Empty;
            try
            {
                if (fileUploadDTO.File != null)
                {
                    string fileName = fileUploadDTO.File.FileName;
                    if (fileName.IsNullOrWhiteSpace() || fileName.GetFileExtension().IsNullOrWhiteSpace() || fileName.GetFileExtension().ToLowerInvariant().Contains("exe"))
                    {
                        errorMessage += $"Uploaded FileName is not valid. ";
                    }
                }
            }
            catch (Exception)
            {
                return new ValidationResult($"Uploaded FileName is not valid. ", new[] { errorMemeber });
            }
            if (!errorMessage.IsNullOrWhiteSpace())
            {
                return new ValidationResult($"{errorMessage}", new[] { errorMemeber });
            }
            return ValidationResult.Success;
        }
    }
}
