using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveSolution.DAL.Data
{
    public class tbl_LeaveDashboard
    {
        public string LeaveCategory { get; set; }
        public string ApprovedLeave { get; set; }
        public double PendingLeave { get; set; }
        public double Quota { get; set; }
        public string EndDate { get; set; }
    }
}
