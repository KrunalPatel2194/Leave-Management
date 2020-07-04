using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.BAL.ServiceModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveSolution.BAL.ServiceInterface
{
   public interface IDeptLeaveRequestViewService
    {
        Tuple<string, int> SaveLeaveRequest(DeptLeaveRequestViewtServiceModel model);

     int SaveUploadedLeave(List<DeptLeaveRequestViewtServiceModel> objModel);
      //  List<DeptLeaveRequestViewtServiceModel> SaveUploadedLeave(List<DeptLeaveRequestErrorModel> objModel);
        List<SelectListItem> GetLeaveRequest(string PA,string PSA,string DeptMasterID);
        List<SelectListItem> GetAutoCompleteList(string search);
        List<HolidayServiceModel> GetHolidayList(string EmpID);
        DeptLeaveRequestViewtServiceModel GetEmpDetails(string EmpID);
        double GetBalanceForSingleEmp(string employeeid, string leaveid);
        int SaveConfirm(string AdminID,string userdata);
        string GetLeaveCode(string LeaveID);
        int SaveFromSAP(QuotaServiceModel model);
        string GetMappingData(string Leavecode);
        int GetDEPTID(string spempdeptid);   
        List<DeptLeaveRequestViewtServiceModel> GetLeaveDetailsForChange(int objleaverequestId);
    }
}
