using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.BAL.ServiceModels;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.Rendering;
namespace LeaveSolution.BAL.ServiceInterface
{

    public interface ILoginViewService
    {
        int ExtendSession(string employeeid);
        int SaveFromSAPQuota(QuotaServiceModel model); 
        LoginViewServiceModel LoginCredentials();
        long GenerateOtp(string employeeid, long mobileno);
        int SaveNewUser(LoginViewServiceModel model);
        int SaveApproverFromSAP(ApproverServiceModel modela);
        int SaveFromSAP(EmployeeServicemodel model);
        LoginViewServiceModel Authentication(LoginViewServiceModel userid);
        int OTPmatch(int otp,string employeeid,DateTime dob, string fromdata);
        long GetMobileno(string employeeid,string fromdata);
        int DOBMatchforKIOSK(string employeeid, DateTime dob);
        string ValidateMifareID(string id,string datafrom);
        void DeleteUser(string userid);
    }
}
