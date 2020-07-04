using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveSolution.DAL.Data
{
    class ConstantsVariables
    {
        // user table
        public static readonly string P_EMPID = "P_EMPID";
        public static readonly string P_MifareID = "P_MifareID";
        public static readonly string P_USERID = "P_USERID";
        public static readonly string P_PASSWORD = "P_NewPassword";
        public static readonly string P_ISACTIVE = "P_ISACTIVE";
        public static readonly string P_ReturnValidation = "P_ReturnValidation";
        public static readonly string P_LeaveValidation = "P_ReturnValidation";

        public static readonly string P_STATUSUPDATE = "P_STATUSUPDATE";
        public static readonly string P_LEAVESHIFT = "P_LEAVESHIFT";
        public static readonly string P_TOTALLEAVE = "P_TOTALLEAVE";
        //otpgenerate table
        public static readonly string P_OTP = "P_OTP";
        public static readonly string P_OSTARTTIME = "P_OSTARTTIME";
        public static readonly string P_OENDTIME = "P_OENDTIME";
        //EMPMASTER TABLE
        public static readonly string P_Category = "P_Category";
        public static readonly string P_EMPNAME = "P_EMPNAME";
        public static readonly string P_DATEOFBIRTH = "P_DATEOFBIRTH";
        //public static readonly DateTime P_DATEOFBIRTHE = "P_DATEOFBIRTHE";
        public static readonly string P_PHONENO = "P_PHONENO";
        public static readonly string P_EMAILID = "P_EMAILID";

        //LeaveRequest Table
        public static readonly string P_CODE = "P_Code";
        public static readonly string P_DESCRIPTION = "P_DESCRIPTION";

        //  public static readonly string P_LeaveID = "P_LeaveID";
        public static readonly string P_LeaveDetails = "P_LeaveDetails";
        // Create Leave Request Page variable
        public static readonly string P_LEAVEREQUESTID = "P_LEAVEREQUESTID";
        public static readonly string P_LEAVEID = "P_LEAVEID";
        public static readonly string P_LEAVECODE = "P_LEAVECODE";
        public static readonly string P_LEAVECATEGORY = "P_LEAVECATEGORY";
        public static readonly string P_FROMDATE = "P_FROMDATE";
        public static readonly string P_TODATE = "P_TODATE";
        public static readonly string P_LEAVETYPE = "P_LEAVETYPE";

        public static readonly string P_ABSENCEHOUR = "P_ABSENCEHOUR";
        public static readonly string P_APPROVERNAME = "P_APPROVERNAME";
        public static readonly string P_APPROVERID = "P_APPROVERID";
        public static readonly string P_REMARKS = "P_REMARKS";
        public static readonly string P_STATUS = "P_STATUS";
        public static readonly string P_DAYTYPE = "P_DAYTYPE";
        public static readonly string P_BALANCELEAVE = "P_BALANCELEAVE";
        public static readonly string P_TOTALAPPLIEDLEAVE = "P_TOTALAPPLIEDLEAVE";
        public static readonly string P_LEAVEAPPLIEDDATE = "P_LEAVEAPPLIEDDATE";
        public static readonly string P_EMPLOYEEID = "P_EMPLOYEEID";
        public static readonly string P_EMPLOYEENAME = "P_EMPLOYEENAME";
        //public static readonly string P_STATUSUPDATE = "P_STATUSUPDATE";
        public static readonly string P_ENTITLEMENT = "P_ENTITLEMENT";
        public static readonly string P_EmpDetails = "P_EMPDETAILS";
        public static readonly string P_PA = "P_PA";
        public static readonly string P_PSA = "P_PSA";
        public static readonly string P_FILENAME = "P_FILENAME";
        public static readonly string P_CreatedBy = "P_CREATEDBY";
        public static readonly string P_DeptMasterID = "P_DeptMasterID";
        //  public static readonly string P_ReturnValidation = "P_LeaveValidation";

        //For Dashboard
        public static readonly string P_DashboardDetails = "P_DASHBOARDDETAILS";
        public static readonly string P_DashboardDetailsforPending = "P_DASHBOARDDETAILSFORPENDING";
        public static readonly string P_ADMINID = "P_ADMINID";

    }
}
