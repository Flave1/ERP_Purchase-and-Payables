using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Puchase_and_payables.Contracts.Response.Report;
using Puchase_and_payables.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Puchase_and_payables.Handlers.Report
{
    public class ProjectStatusQuery : IRequest<ProjectStatusStatsresp>
    {
        public class ProjectStatusQueryHandler : IRequestHandler<ProjectStatusQuery, ProjectStatusStatsresp>
        {
            private readonly DataContext _dataContext;
            public ProjectStatusQueryHandler(DataContext dataContext)
            {
                _dataContext = dataContext;
            }
            public async Task<ProjectStatusStatsresp> Handle(ProjectStatusQuery request, CancellationToken cancellationToken)
            {
                var response = new ProjectStatusStatsresp { Status = new APIResponseStatus { Message = new APIResponseMessage(), }, Datasets  = new List<ProjectStatusStats>() };
                var lpos = await _dataContext.purch_plpo.Where(q => q.JobStatus != (int)JobProgressStatus.Executed_Successfully && q.DateCompleted.Value.Year != DateTime.UtcNow.Year).ToListAsync();
                try
                {
                  
                    List<int> dataValue = new List<int>();
                    var Labels = new[] { "Completed", "Cancelled", "In Progress", "Not Started"} ;
                    var datas = new List<ProjectStatusStats>();
                    
                    dataValue.Add(lpos.Count(q => q.JobStatus == (int)JobProgressStatus.Executed_Successfully));
                    dataValue.Add(lpos.Count(q => q.JobStatus == (int)JobProgressStatus.In_Progress));
                    dataValue.Add(lpos.Count(q => q.JobStatus == (int)JobProgressStatus.Cancelled));
                    dataValue.Add(lpos.Count(q => q.JobStatus == (int)JobProgressStatus.Not_Started));

                    var dataset = new ProjectStatusStats
                    {
                        Label = "Job Progress Status",
                        BorderColor = "",
                        Data = dataValue.ToArray(),
                        Fill = false,
                    };
                    datas.Add(dataset);
                    response.Datasets = datas;
                    response.Labels = Labels.ToArray();

                    return await Task.Run(() =>  response);
                }
                catch (Exception ex)
                { 
                    throw ex;
                }
            }
        }
    }
}
