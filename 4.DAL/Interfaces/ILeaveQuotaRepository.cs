using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Data;
using System.Data;

namespace LeaveSolution.DAL.Interfaces
{
    public interface ILeaveQuotaRepository
    {
        List<tbl_LeaveQuota> GetLeaveQuota(string objEmpID);
    }
}
