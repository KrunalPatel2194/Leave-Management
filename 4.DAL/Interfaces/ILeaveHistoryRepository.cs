using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Data;
using System.Data;

namespace LeaveSolution.DAL.Interfaces
{
    public interface ILeaveHistoryRepository
    {
        List<tbl_LeaveHistory> GetLeaveHistory(string empid,string year);
        List<tbl_LeaveHistory> GetEmployeeReport(string empId, string fromdate, string todate);
        void LeaveHistoryExcel(string empid);
    }
}
