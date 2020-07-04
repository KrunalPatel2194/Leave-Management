using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveSolution.DAL.Data
{
    public class tbl_LeaveHistory
    {
        public string ErrorMsg { get; set; }
        public string Indicator { get; set; }
        public string LeaveCategory { get; set; }
        public string LeaveAppliedDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public double TotalLeaves { get; set; }
        public string ApproverName { get; set; }
        public int TotalAppliedLeave { get; set; }
        public string Status { get; set; }
        public string LeaveType { get; set; }
        public string EmployeeID { get; set; }
        public string LeaveCode { get; set; }
    }
}

