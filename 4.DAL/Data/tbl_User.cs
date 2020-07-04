using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveSolution.DAL.Data
{
     public class tbl_User
    {
     public string MifareID { get; set; }
        public string EmployeeID { get; set; }
        public string PersonalArea { get; set; }
        public string CostCenter { get; set; }
        public string Category { get; set; }
        public string CardID { get; set; }
        public string PersonalSubArea { get; set; }

        public string UserID { get; set; }
        public string Password { get; set; }
        public string ApproverId { get; set; }
        public string ApproverName { get; set; }
        public string EmployeeName { get; set; }
        public string EmpEmail { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int ReturnValue { get; set; }
        public int Ispwdexpired { get; set; }
        public string Grade { get; set; }
        public string Dept { get; set; }
        public string MobileNo { get; set; }
        public string ApproverMobileNo { get; set; }
        public string ReturnValMessg { get; set; }
    }
}
