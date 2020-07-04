using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.BAL.ServiceModels;

namespace LeaveSolution.BAL.ServiceInterface
{
    public interface ILeaveHistoryService
    {
        LeaveHistoryServiceViewModel GetLeaveHistoryDetails(string empId,string year);
    //    byte[] ExportLeaveHistory(string id);
        LeaveHistoryServiceViewModel GetEmployeeReport(string empId,string fromdate,string todate);
        void LeaveHistoryExcel(string empId);

    }
}



