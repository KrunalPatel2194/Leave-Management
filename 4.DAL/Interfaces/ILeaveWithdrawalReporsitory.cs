using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Data;


namespace LeaveSolution.DAL.Interfaces
{
   public interface ILeaveWithdrawalRepository
    {
        //string GetMappingData(string LeaveCode);
        //Tuple<string, int> SaveFromSAP(List<tbl_LeaveQuotaSAP> tbl_USERs);
        List<tbl_Leavewithdrawal> GetLeavesforWithdrawal(string EmployeeId);
        Tuple<string, int> SaveWithdrawalLeaveRequest(tbl_Leavewithdrawal tblLeaveWithdrawal);
    }
}
