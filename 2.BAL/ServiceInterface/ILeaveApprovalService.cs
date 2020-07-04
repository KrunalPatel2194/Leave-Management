using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.BAL.ServiceModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveSolution.BAL.ServiceInterface
{
  public interface ILeaveApprovalService
    {
    LeaveApprovalServiceViewModel GetLeaveRequestForApproval(string ApproverId);
        LeaveApprovalServiceViewModel GetApprovalDetails(string ApproverID);

        LeaveApprovalServiceViewModel GetLeaveApprovalsForDashboard(string ApproverId);
        Tuple<string,int> SaveLeaveApproval(LeaveApprovalServiceViewModel model);
       // string GetMappingData(string leavecode);
       // int SaveFromSAP(QuotaServiceModel quotaServiceModel);
        LeaveApprovalServiceViewModel GetMulitpleEmployeeDetails(string strLeaveReqID);
        Tuple<string, int> InsertEmpId(string empid);
    }
}
