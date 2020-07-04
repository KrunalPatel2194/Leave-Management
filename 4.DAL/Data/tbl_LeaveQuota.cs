using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveSolution.DAL.Data
{
    public class tbl_LeaveQuota
    {
        public string LeaveCategory { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ApproverName { get; set; }
        public int BalanceLeave { get; set; }
        public int TotalAppliedLeave { get; set; }
        public string LeaveAppliedDate { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public int Quota { get; set; }
        public int TotalLeaves { get; set; }
    }
}
