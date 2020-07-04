using System;
using System.Collections.Generic;
using System.Text;
//using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LeaveSolution.DAL.Data
{
    public class tbl_DeptLeaveRequest
    {
        public string ErrorMsg { get; set; }
        public string Indicator { get; set; }
        public string AdminID { get; set; }
        // public List<SelectListItem> Leavelist { get; set; }
        public string LeavelistId { get; set; }
        public int LeaveId { get; set; }
        public string LeaveCode { get; set; }
        public string LeaveCategory { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string LeaveShift { get; set; }
        public double AbsenceCEHour { get; set; }
        public string ApproverName { get; set; }
        public string ApproverId { get; set; }
        public string Remarks { get; set; }
        public double BalanceLeave { get; set; }
        public double TotalAppliedLeave { get; set; }
        public string LeaveAppliedDate { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public double Entitlement { get; set; }
        public string HolidayDate { get; set; }
        public string HolidayName { get; set; }
        public string Description { get; set; }
        public string Grade { get; set; }
        public string PA { get; set; }
        public string PSA { get; set; }
        public int Allowed { get; set; }
        public string File { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public string UploadFileName { get; set; }
        public string CreatedBy { get; set; }
        public string LeaveRequestId { get; set; }
        public string ERRORMESSAGE { get; set; }
        public string TEXTMESSAGE { get; set; }
        public string empPhoneNo { get; set; }
    }
    public class tblHolidayDept
    {
        public string HolidayDate { get; set; }
        public string HolidayName { get; set; }
        public string Text { get; set; }
        public string Text1 { get; set; }
    }
}
