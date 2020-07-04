using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace LeaveSolution.BAL.ServiceModels
{
    public class LeaveQuotaServiceViewModel
    {
        public List<LeaveQuotaServiceModel> LeaveQuota { get; set; }
    }
    public class LeaveQuotaServiceModel
    {

        [Display(Name = "Leave Category")]
        public string LeaveCategory { get; set; }
        [Display(Name = "From Date")]
        public string FromDate { get; set; }
        [Display(Name = "To Date")]
        public string ToDate { get; set; }
        [Display(Name = "Approver")]
        [Required(ErrorMessage = "Please Enter Approver Name")]
        public string ApproverName { get; set; }
        public double BalanceLeave { get; set; }
        public double TotalAppliedLeave { get; set; }
        public string LeaveAppliedDate { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public double Quota { get; set; }
        public double TotalLeaves { get; set; }
    }
}