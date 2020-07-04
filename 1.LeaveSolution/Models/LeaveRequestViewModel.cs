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

    public class AlternateApproverModel
    {
        public string ApproverMailID { get; set; }
        public string ApproverMobNo { get; set; }
        public string HODName { get; set; }
        public string HODMailID { get; set; }
        public string HODID { get; set; }
        public string ApproverName { get; set; }
        public string ApproverId { get; set; }
    }
    public class LeaveRequestViewModel
    {
        public string ApproverMailID { get; set; }
        public string ApproverMobNo { get; set; }
        public string HODName { get; set; }
        public string HODMailID { get; set; }
        public string HODID { get; set; }
        public string HODMobNo { get; set; }
        public string Category { get; set; }
        
        [Display(Name = "Working Days")]

        public string WorkingDays { get; set; }
        [Display(Name = "Non-Working Days")]
        public string NonWorkingDays { get; set; }
        [Display(Name = "Approved")]
        public string Approved { get; set; }
        [Display(Name = "SelfApproved")]
        public string SelfApproved { get; set; }
        [Display(Name = "Approval Pending")]
        public string ApprovalPending { get; set; }
        [Display(Name = "Public Holiday")]
        public string PublicHoliday { get; set; }


        [Display(Name = "Leave Type")]
        public string LeaveType { get; set; }
        [Display(Name = "Leave Category")]
        public string LeaveCategory { get; set; }
        public string LeaveRequestId { get; set; }
        [Required(ErrorMessage = "Please Select Leave Category")]
        public string LeavelistId { get; set; }
        public List<SelectListItem> Leavelist { get; set; }
        public int LeaveId { get; set; }
        public string LeaveCode { get; set; }
       [Required(ErrorMessage = "Please Select FromDate")]
        [Display(Name = "From Date")]
        public string FromDate { get; set; }
        [Display(Name = "To Date")]
       [Required(ErrorMessage = "Please Select ToDate")]
        public string ToDate { get; set; }
        //[Required(ErrorMessage = "Select any one option")]
        public string LeaveShift { get; set; }
        //  [Display(Name = "Attendance/Absence Hours", ResourceType = typeof(Resource))]
        [Display(Name = "Attendance/Absence Hours")]
     //   [Required(ErrorMessage = "Please Enter Absence Hour")]
        public double AbsenceCEHour { get; set; }
        [Display(Name = "Approver")]
        [Required(ErrorMessage = "Please Enter Approver Name")]
        public string ApproverName { get; set; }
        public string ApproverId { get; set; }
        [Display(Name = "Note")]
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
    public class LEAVEREQUESTDETAILS
    {
        public LeaveRequestDetails[] IT_ERROR_TEXT;
    }
    public class LeaveRequestDetails
    {
        public string TEXT { get; set; }

        public string IND { get; set; }
    }
}