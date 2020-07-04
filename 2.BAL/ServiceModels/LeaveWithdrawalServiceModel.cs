using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveSolution.BAL.ServiceModels
{

    public class LeaveWithdrawalServiceViewModel
    {
        public List<LeaveWithdrawalServiceModel> leavewithdrawal { get; set; }
        public int SelectedLeaveType { get; set; }
    }
    public class LeaveWithdrawalServiceModel
    {
        public string ErrorMsg { get; set; }
        public string Indicator { get; set; }
        public bool checkbox { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeID { get; set; }
        public int LeaveRequestId { get; set; }

        public List<SelectListItem> Leavelist { get; set; }
        public string LeavelistId { get; set; }
        public string LeaveCategory { get; set; }
        public DateTime LeaveAppliedDate { get; set; }
        public string LeaveAppliedDateString { get; set; }
        public string Status { get; set; }
        public double TotalLeaves { get; set; }
        public int LeaveId { get; set; }
        public string LeaveCode { get; set; }
        public DateTime? FromDate { get; set; }
        public string FromDateString { get; set; }
        public string ToDateString { get; set; }
        public DateTime? ToDate { get; set; }
        public string LeaveShift { get; set; }
        public double AbsenceHour { get; set; }
        public string ApproverName { get; set; }
        public string ApproverId { get; set; }
        public string Remarks { get; set; }
        public string SelectEmployee { get; set; }
        public string DayType { get; set; }
        public string MobileNo { get; set; }
        public double BalanceLeave { get; set; }
        public int ReturnValue { get; set; }
        public int Entitlement { get; set; }
        public IFormFile FileToUpload { get; set; }
        public string UploadFileName { get; set; }
    }
}
