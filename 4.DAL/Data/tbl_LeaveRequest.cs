using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.Internal;
//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;


namespace LeaveSolution.DAL.Data
{
    public class tbl_LeaveRequest
    {
        public string LeaveType { get; set; }
        public string LeaveRequestID { get; set; }
        public string LeaveCategory { get; set; }
        public string LeavelistId { get; set; }
        public string Quotatype { get; set; }

        public int LeaveId { get; set; }
        public string LeaveCode { get; set; }
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
        public string StatusUpdate { get; set; }
        public int Entitlement { get; set; }
        public string Description { get; set; }
        public string Grade { get; set; }
        public string PA { get; set; }
        public string PSA { get; set; }
        public IFormFile FileToUpload { get; set; }
        public string UploadFileName { get; set; }
        public string CreatedBy { get; set; }
        public string ModifyBy { get; set; }
    }
    public class tblHoliday
    {
        public string HolidayDate { get; set; }
        public string HolidayName { get; set; }
        public string Text { get; set; }
        public string Text1 { get; set; }
    }
}


