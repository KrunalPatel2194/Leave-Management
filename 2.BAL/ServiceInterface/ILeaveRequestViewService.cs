using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.BAL.ServiceModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveSolution.BAL.ServiceInterface
{
    public interface ILeaveRequestViewService
    {
        int SaveApproverFromSAP(ApproverServiceModel modela);
        List<SelectListItem> GetLeaveRequest(string PA, string PSA, string Category);
        string[] GetApproverEmail(string EmpID);
        List<SelectListItem> GetLeaveRequestList(string PA, string PSA, string Category);
        Tuple<string, int> SaveLeaveRequest(LeaveRequestViewtServiceModel model);
        List<HolidayServiceModel> GetHolidayList(string Empid);
        double GetBalanceForSingleEmp(string employeeid, string leaveid);
        List<string> GetAutoCompleteList(string search);
        List<LeaveRequestViewtServiceModel> GetLeaveDetailsForChange(int objleaverequestId);
        int SaveFromSAP(QuotaServiceModel model);
        string GetMappingData(string Leavecode);
        string GetLeaveCode(string LeaveID);
     //   List<LeaveRequestViewtServiceModel> GetEmpLeaveDetails();
    }
}
