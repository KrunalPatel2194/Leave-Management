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
    public class LeaveQuotaModelViewModel
    {
        public List<LeaveQuotaModel> LeaveQuota { get; set; }
    }
    public class LeaveQuotaModel
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
        public string Quotatype { get; set; }
    }

    public class QUOTAOVERVIEWDETAILS
    {
        public QuotaOverviewDetails[] QUOTA_OVERVIEW;
    }
    public class QuotaOverviewDetails
    {
        private string KTEXT;

        private DateTime BEGDA;

        private DateTime ENDDA;

        private string ANZHL;

        private string KVERB;

        private string ANZHL_CLOSE;

        public string Ktext
        {
            get
            {
                return this.KTEXT;
            }
            set
            {
                this.KTEXT = value;
            }
        }

        public DateTime Begda
        {
            get
            {
                return this.BEGDA;
            }
            set
            {
                this.BEGDA = value;
            }
        }

        public DateTime Endda
        {
            get
            {
                return this.ENDDA;
            }
            set
            {
                this.ENDDA = value;
            }
        }

        public string Anzhl
        {
            get
            {
                return this.ANZHL;
            }
            set
            {
                this.ANZHL = value;
            }
        }

        public string Kverb
        {
            get
            {
                return this.KVERB;
            }
            set
            {
                this.KVERB = value;
            }
        }

        public string Anzhl_Close
        {
            get
            {
                return this.ANZHL_CLOSE;
            }
            set
            {
                this.ANZHL_CLOSE = value;
            }
        }
    }

}