using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Data;

namespace LeaveSolution.DAL.Interfaces
{
    public interface ILeaveApprovalRepositary
    {
      //  Tuple<string, int> SaveFromSAP(List<tbl_LeaveQuotaSAP> tbl_USERs);
        List<tbl_LeaveApproval> GetApprovalDetails(string ApproverId);
        Tuple<string, int> SaveLeaveApprovalScheduler(List<tbl_LeaveApproval> tbl_LeaveApprovals );
        List<tbl_LeaveApproval> GetLeaveApprovals(string ApproverId);
        List<tbl_LeaveApproval> GetLeaveApprovalsForDashboard(string ApproverId); 
        Tuple<string, int> SaveAprrovedLeaveRequest(List<tbl_LeaveApproval> tblLeaveApprovals);
       // string GetMappingData(string LeaveCode);
        List<tbl_LeaveApproval> GetMulitpleEmployeeDetails(string strLeaveReqID);
        List<tbl_LeaveApproval> GetEmpLeaveDetails(string SchedulerType);
        Tuple<string, int> InsertEmpId(string empid);
    }
}
