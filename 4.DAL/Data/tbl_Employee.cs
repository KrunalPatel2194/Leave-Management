using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveSolution.DAL.Data
{
   public class tbl_Employee
    {
        public string PersonalArea { get; set; }
        public string PersonalSubArea { get; set; }
        public string ApproverId { get; set; }
        public string ApproverName { get; set; }
        public string Grade { get; set; }
        public string Dept { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string CostCenter { get; set; }
        public string EmpSubGroup { get; set; }
        public string EmpGroup { get; set; }

        public string CardID { get; set; }
        public string NewPassword { get; set; }
        public string MobileNo { get; set; }
        public string ConfirmPassword { get; set; }
        public string ReturnUrl { get; set; }
        public string UserName { get; set; }
        public int EnterOTP { get; set; }
        public string EmployeeID { get; set; }
        // public int EnterOTP { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeMail { get; set; }
        public string EmployeeDOB { get; set; }
        //public  string GetConnectionString()
        //{
        //    return Startup.ConnectionString;

        //}

        public bool ViewAssessmentClicked { get; set; }
        public int otpflag { get; set; }
        public string EmpEmail { get; set; }
        public string DateOfBirth { get; set; }
        public int ReturnsaveValue { get; set; }
        public int IsPwdExpired { get; set; }
    }
}
