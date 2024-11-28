using AMLMonitor.Services.Interfaces;
using AMLMonitor.Models.Reports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMLMonitor.Services.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<MediaUsageReport>> GetMediaUsageReport(int branchId, DateTime startDate, DateTime endDate);
    }
}