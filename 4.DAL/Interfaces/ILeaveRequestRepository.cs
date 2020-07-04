using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Data;
using System.Data;

namespace LeaveSolution.DAL.Interfaces
{
    public interface ILeaveRequestRepository
    {

        Tuple<string, int> SaveApproverFromSAP(List<tbl_LeaveApproval> tbl_USERa);
        Tuple<string, int> SaveFromSAP(List<tbl_LeaveQuotaSAP> tbl_USERs);
        List<tbl_LeaveRequest> GetLeaveRequest(string PA, string PSA, string Category);
        string[] GetApproverEmail(string EmpID);
        Tuple<string, int> SaveLeaveRequest(tbl_LeaveRequest objleave_request);
        Tuple<string, int> UpdateLeaveRequest(tbl_LeaveRequest objleave_request);
        List<tblHoliday> GetHolidayList(string Empid);
        double GetBalanceForSingleEmp(string employeeid, string leaveid);
        List<string> GetAutoCompleteList(string search);
        List<tbl_LeaveRequest> GetLeaveDetailsForChange(int objLeaverequestId);
        //int CheckLeaveApplied(string employeeid, string leaveid, string PA, string PSA);
        //int GetLeaveID(string LeaveCategory);
        string GetLeaveCode(string LeaveCode);
        string GetMappingData(string LeaveCode);
 
        
    }
}
