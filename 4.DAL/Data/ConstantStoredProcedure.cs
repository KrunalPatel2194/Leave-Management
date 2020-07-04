using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveSolution.DAL.Data
{
    class ConstantStoredProcedure
    {
        public static readonly string SPBCL_MAPPINGDATA = "BCL_MAPPINGDATA"; 
        public static readonly string SP_GETMOBILENO = "SP_GETMOBILENO";
        //public static readonly string SPBCL_GetLeaveCode = "BCL_GetLeaveCode";
        
        public static readonly string SP_SaveOTPGEN = "BCL_SAVEOTPGEN";
        public static readonly string SP_SAVEUSER = "BCL_SAVEUSERLOGIN";
        public static readonly string SP_AUTHENTICATEUSER = "BCL_AUTHENTICATIONUSER";
        public static readonly string SP_GETOTP = "BCL_GETOTP";
        public static readonly string SP_GetLeaveRequestMaster = "BCL_GetLeaveRequestMaster";//for Leave request
        public static readonly string SP_GetLeaveRequestMaster1 = "BCL_GetLeaveMaster";//for Leave request demo
        public static readonly string SP_GetLeaveRequestMasterDept = "BCL_GetLeaveRequestMasterDept";
        public static readonly string SP_GETAPPROVERDETAILS = "BCL_GETAPPROVERDETAILS";
        
            // public static readonly string SP_GetLeaveRequestMaster1 = "BCL_GetLeaveMaster";
        public static readonly string SP_BCL_SAVELEAVEREQUEST = "BCL_SAVELEAVEREQUEST";
        public static readonly string SP_BCL_UPDATELEAVEREQUEST = "BCL_UPDATELEAVEREQUEST";
        
        public static readonly string SP_BCL_SAVEDEPTLEAVEREQUEST = "BCL_SAVEDEPTLEAVEREQUEST";
        public static readonly string SP_BCL_GetLeaveForApproval = "BCL_GETLEAVEFORAPPROVAL";
        public static readonly string SP_BCL_SAVELEAVEAPPROVAL = "BCL_SAVELEAVEAPPROVAL";
        public static readonly string SP_BCL_HOLIDAYLIST = "BCL_HOLIDAYLIST";
        public static readonly string SP_BCLDB_sp_GetLeaveBalance = "BCLDB_sp_GetLeaveBalance";
        public static readonly string SP_BCL_GETLEAVECODE = "BCL_GETLEAVECODE";
        public static readonly string SP_BCL_GETEMPLOYEENAME = "BCL_GETEMPLOYEENAME";
        public static readonly string SP_BCL_GETEMPLOYEENAMEDEPT = "BCL_GETEMPLOYEENAMEDEPT";
        public static readonly string SP_BCL_GETLEAVE_APR_DASHBOARD = "BCL_GETLEAVE_APR_DASHBOARD";
        // public static readonly string SP_BCL_GETEMPLOYEENAME = "BCL_GETEMPLOYEENAME";SP_BCL_GETEMPLOYEENAMEDEPT BCL_GETEMPDETAILS
        public static readonly string SP_BCL_CheckLeaveValidation = "BCL_CheckLeaveValidation";

        
        public static readonly string SP_BCL_VALIDATEMIFAREID = "BCL_VALIDATEMIFAREID";
        public static readonly string SP_BCL_GETDOBFORKIOSK = "BCL_GETDOBFORKIOSK";
        public static readonly string SP_BCL_MENUMASTERDETAILS = "BCL_MENUMASTERDETAILS";
        // 
        //public static readonly string SP_BCLDB_sp_CheckEmployeeExist = "BCLDB_sp_CheckEmployeeexist";
        public static readonly string SP_BCL_GETLeaveQuotaDetails = "BCL_GETLeaveQuotaDetails";
        public static readonly string SP_BCL_GETLeaveHistoryDetails = "BCL_GETLeaveHistoryDetails";
        public static readonly string SP_BCL_DashboardDetails = "BCL_DashboardDetails";
        public static readonly string SP_BCL_GETEMPDETAILS = "BCL_GETEMPDETAILS";
        public static readonly string SP_BCL_GETLEAVEFORWITHDRAWAL = "BCL_GETLEAVEFORWITHDRAWAL";
        public static readonly string SP_BCL_SAVELEAVEWITHDRAWAL = "BCL_SAVELEAVEWITHDRAWAL";
        ///// public static readonly string SP_BCL_CheckLeaveValidation = "BCL_CheckLeaveValidation";
        public static readonly string SP_BCL_GETLEAVEID = "BCL_GETLEAVEID";
        public static readonly string SP_BCL_GETLEAVEDETAILSFORCHNAGE = "BCL_GETLEAVEDETAILSFORCHNAGE";
        public static readonly string SP_BCL_GETEMPDETAILS_Muliple = "BCL_GETEMPDETAILS_Muliple";
        public static readonly string SP_BCL_GETEMPDETAILS_E = "BCL_GETEMPDETAILS_E";

        //For Department Withdarw
        public static readonly string SP_BCL_GETLEAVEFORWITHDRAWAL_DEPT = "BCL_GETLEAVEFORWITHDRAWAL_DEPT";
        public static readonly string SP_BCL_SAVELEAVEWITHDRAWAL_DEPT = "BCL_SAVELEAVEWITHDRAWAL_DEPT";
        public static readonly string SP_BCL_GETLEAVEFORCHNAGE_DEPT = "BCL_GETLEAVEFORCHNAGE_DEPT";

        public static readonly string SP_BCL_DEPTID = "BCL_GETDepartmentID";
        public static readonly string SP_BCL_Delete_ConcurrentLogin = "BCL_Delete_ConcurrentLogin";
        public static readonly string SP_BCL_EMPLOYEEREPORT = "BCL_EMPLOYEEREPORT";
        public static readonly string SP_BCL_GETLEAVEHISTORYExcel = "BCL_GETLEAVEHISTORYExcel";
        public static readonly string SP_BCL_InsertApproverID = "BCL_InsertApproverID";

        
    }
    
}
