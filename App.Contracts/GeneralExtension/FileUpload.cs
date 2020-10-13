using GOSLibraries.GOS_API_Response;
using Microsoft.AspNetCore.Http;

namespace Puchase_and_payables.Contracts.GeneralExtension
{
    public class FileUploadObj
    {
        public IFormFile File { get; set; }
    }

    public class FileUploadRespObj
    {
        public APIResponseStatus Status { get; set; }
    }

    public class FileDownloadRespObj
    {
        public APIResponseStatus Status { get; set; }
    }
}