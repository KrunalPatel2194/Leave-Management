using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Data;

namespace LeaveSolution.DAL.Interfaces
{
    public interface ILeaveDashboardRepositary
    {
        List<tbl_LeaveDashboard> GetDashboardLeaveList(string empid);
    }
}
