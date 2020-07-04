using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveSolution.Models
{
    public class LeaveDashboardModelViewModel
    {
        public List<LeaveDashboardModel> LeaveDashboard { get; set; }
    }
    public class LeaveDashboardModel
    {
        public string LeaveCategory { get; set; }
        public string ApprovedLeave { get; set; }
        public string PendingLeave { get; set; }
        public string Quota { get; set; }
        public string EndDate { get; set; }
    }
}
