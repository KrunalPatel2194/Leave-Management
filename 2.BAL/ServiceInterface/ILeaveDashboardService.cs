using LeaveSolution.BAL.ServiceModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveSolution.BAL.ServiceInterface
{
    public interface ILeaveDashboardService
    {
        LeaveDashboardServiceViewModel GetLeaveDashboardDetails(string empid);
    }
}
