using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveSolution.BAL.ServiceModels
{
    public class LeaveDashboardServiceViewModel
    {
        public List<LeaveDashboardServiceModel> LeaveDashboard { get; set; }
    }
    public class LeaveDashboardServiceModel
    {
        public string LeaveCategory { get; set; }
        public string ApprovedLeave { get; set; }
        public double PendingLeave { get; set; }
        public double Quota { get; set; }
        public string EndDate { get; set; }
    }
}
