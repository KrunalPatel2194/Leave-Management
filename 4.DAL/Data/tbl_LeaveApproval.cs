using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
//using Microsoft.AspNetCore.Mvc.Rendering;
namespace LeaveSolution.DAL.Data
{
    public class tbl_LeaveApproval
    {
        public string OldStatus { get; set; }
        public string HODID { get; set; }
        public string ErrorMsg { get; set; }
        public string Indicator { get; set; }
        public string HODName { get; set; }
        public bool checkbox { get; set; }
        public string FileName { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeID { get; set; }
        public int LeaveRequestId { get; set; }
        public string ApproverMailID { get; set; }
        public string ApproverMobNo { get; set; }
        public string HODMailID { get; set; }
        public string HODMobNo { get; set; }
        public string LeaveCategory { get; set; }
        public DateTime LeaveAppliedDate { get; set; }
        public string LeaveAppliedDateString { get; set; }
        public string Status { get; set; }
        public double TotalLeaves { get; set; }
        public int LeaveId { get; set; }
        public string ApprovedDate { get; set; }
        public string FromData { get; set; }
        public string LeaveCode { get; set; }
        public DateTime FromDate { get; set; }
        public string FromDateString { get; set; }
        public string ToDateString { get; set; }
        public DateTime ToDate { get; set; }
        public string LeaveShift { get; set; }
        public int AbsenceHour { get; set; }
        public string ApproverName { get; set; }
        public string ApproverId { get; set; }
        public string Remarks { get; set; }
        public string SelectEmployee { get; set; }
        public string DayType { get; set; }
        public double BalanceLeave { get; set; }
        public int ReturnValue { get; set; }
        public int Entitlement { get; set; }
        public string DeptID { get; set; }
        public string empmobileno { get; set; }
    }
}
