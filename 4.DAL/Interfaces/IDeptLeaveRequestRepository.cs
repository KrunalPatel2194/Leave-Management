using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Data;
using System.Data;
//using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveSolution.DAL.Interfaces
{
   public interface IDeptLeaveRequestRepository
    {
       int Saveuploadedleave (List<tbl_DeptLeaveRequest> objleave_request);
        List<tbl_DeptLeaveRequest> GetLeaveRequest(string PA,string PSA,string DeptMasterID);
        List<tbl_DeptLeaveRequest> GetAutoCompleteList(string search);
       tbl_DeptLeaveRequest GetEmpDetails(string EmpID);
        List<tblHolidayDept> GetHolidayList(string Empid);
        List<tbl_DeptLeaveRequest> GetBalanceForSingleEmp(string employeeid, string leaveid);
        Tuple<string, int> SaveLeaveRequest(tbl_DeptLeaveRequest objleave_request);
        int SaveConfirm(string AdminID,string userdata);
        string GetMappingData(string LeaveCode);
        string GetLeaveCode(string LeaveCode);
        Tuple<string, int> SaveFromSAP(List<tbl_LeaveQuotaSAP> tbl_USERs);
        int GetDEPTID(string spEmpDEPTID);
        List<tbl_DeptLeaveRequest> GetLeaveDetailsForChange(int objLeaverequestId);
    }
}
