using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace LeaveSolution.Models
{
    //public class DeptLeaveRequestAllModel
    //{
    //    public DeptLeaveRequestErrorModel DeptLeaveRequestErrorModel { get; set; }
    //    public DeptLeaveRequestViewModel DeptLeaveRequestViewModel { get; set; }


    //}
    public class DeptLeaveRequestErrorModel
    {
        public List<DeptLeaveRequestViewModel> DeptLeaveErrorList { get; set; }
    }
    public class DeptLeaveRequestViewModel
    {
        [Display(Name = "Working Days")]
        public string WorkingDays { get; set; }
        [Display(Name = "Non-Working Days")]
        public string NonWorkingDays { get; set; }
        [Display(Name = "Approved")]
        public string Approved { get; set; }
        [Display(Name = "Approval Pending")]
        public string ApprovalPending { get; set; }
        [Display(Name = "Public Holiday")]
        public string PublicHoliday { get; set; }
        [Display(Name = "Leave Type")]
        public string LeaveType { get; set; }
        [Display(Name = "Leave Category")]
        public string LeaveCategory { get; set; }
        public string CreatedBy { get; set; }
        //[Required(ErrorMessage = "Please Select Leave Category")]
        public string LeavelistId { get; set; }
        public List<SelectListItem> Leavelist { get; set; }
        public int LeaveId { get; set; }
        public string LeaveCode { get; set; }
       //[Required(ErrorMessage = "Please Select From Date")]
        [Display(Name = "From Date")]
        public string FromDate { get; set; }
        [Display(Name = "To Date")]
        //[Required(ErrorMessage = "Please Select To Date")]
        public string ToDate { get; set; }
        //[Required(ErrorMessage = "Select any one option")]
        public string LeaveShift { get; set; }
        [Display(Name = "Attendance/Absence Hours")]
        //[Required(ErrorMessage = "Please Enter Absence Hour")]
        public double AbsenceCEHour { get; set; }
        [Display(Name = "Approver")]
       //[Required(ErrorMessage = "Please Enter Approver Name")]
        public string ApproverName { get; set; }
        public string ApproverId { get; set; }
        //[Required(ErrorMessage = "Please Enter Remarks")]
        [Display(Name = "Note")]
        public string Remarks { get; set; }
        public double BalanceLeave { get; set; }
        public double TotalAppliedLeave { get; set; }
        public string LeaveAppliedDate { get; set; }
        [Display(Name = "Employee ID")]
        //[Required(ErrorMessage = "Please select Employee ID")]
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public double Entitlement { get; set; }
        public string AdminID { get; set; }
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
        public IFormFile FileToUpload { get; set; }
        public string UploadFileName { get; set; }
        public string LeaveRequestId { get; set; }
        public string HiddenEmpName { get; set; }
        public string DeptMasterID { get; set; }
        public string ErrorMsg { get; set; }
        public string Indicator { get; set; }
        public string empphoneNo { get; set; }
    }
}
