using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveSolution.DAL.Data
{
   public class tbl_Leavewithdrawal
    {
        public string ErrorMsg { get; set; }
        public string Indicator { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeID { get; set; }
        // public string LeavelistId { get; set; }
        public int LeaveRequestId { get; set; }
        public int LeaveId { get; set; }
        public string LeaveCode { get; set; }
        public string LeaveCategory { get; set; }
        public DateTime FromDate { get; set; }
        public string FromDateString { get; set; }
        public string ToDateString { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime LeaveAppliedDate { get; set; }
        public string LeaveAppliedDateString { get; set; }
        public string LeaveShift { get; set; }
        public double AbsenceHour { get; set; }
        public string ApproverName { get; set; }
        public string ApproverId { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string DayType { get; set; }
        public double BalanceLeave { get; set; }
        public int ReturnValue { get; set; }
        public int Entitlement { get; set; }
        public double TotalLeaves { get; set; }
        public string MobileNo { get; set; }
        public IFormFile FileToUpload { get; set; }
        public string UploadFileName { get; set; }
    }
}
