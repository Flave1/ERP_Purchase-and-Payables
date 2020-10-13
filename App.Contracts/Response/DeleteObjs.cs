using GOSLibraries.GOS_API_Response; 
using System;
using System.Collections.Generic; 

namespace Puchase_and_payables.Contracts.Response
{
    public class DeleteItemReqObj
    {
        public int TargetId { get; set; }
    }
    public class MultiDeleteItemsReqObj
    {
        public List<DeleteItemReqObj> TargetIds { get; set; }
    }

    public class DeleteRespObj
    {
        public bool Deleted { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class DownloadFIleResp
    {
        public string FileName { get; set; }
        public byte[] FIle { get; set; }
        public string Extension { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
