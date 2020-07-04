using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Data;
using System.Data;
//using Microsoft.AspNetCore.Mvc
namespace LeaveSolution.DAL.Interfaces
{
    public interface IGenerateOTPRepositary
    {
        // DataTable GetMobileno();
         List<tbl_OTPG> Getsingledata(string id,string fromdata);
        int ExtendSession(string employeeid);
        string ValidateMifareID(string id,string datafrom);
        Tuple<string, int> SaveFromSAPQuota(List<tbl_LeaveQuotaSAP> tbl_USERs); 
        Tuple<string, int> SaveFromSAP(List<tbl_Employee> tbl_USERs);
        Tuple<string, int> SaveApproverFromSAP(List<tbl_LeaveApproval> tbl_USERa); 
        tbl_OTPG Getotp(string id, string fromdata);
        tbl_OTPG GetDOBForKIOSK(string id);
        Tuple<string,int> SaveOTP(List<tbl_OTPG> tbl_OTPGs);
        Tuple<string, int> SaveNewUser(List<tbl_User> tbl_USERs);
        //List<tbl_USER> Authenticate(int userid);
       List<tbl_User> Authenticate(List<tbl_User> tbl_USERs);
        //   bool Authenticate(tbl_USER tbl_USERsA);
        void DeleteUser(string userid);
        
    }
}
