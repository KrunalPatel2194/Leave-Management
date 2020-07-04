using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.BAL.ServiceModels;

namespace LeaveSolution.BAL.ServiceInterface
{
    public interface ILeaveWithdrawalService
    {        
        LeaveWithdrawalServiceViewModel GetLeaveRequestForWithdrawal(string EmployeeId);
        int SaveLeaveWithdrawal(LeaveWithdrawalServiceModel model);
        //int SaveFromSAP(QuotaServiceModel quotaServiceModel);
        //string GetMappingData(string Leavecode);
    }
}