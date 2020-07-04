using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.BAL.ServiceModels;
using LeaveSolution.DAL.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveSolution.BAL.ServiceInterface
{
    public interface ILeaveQuotaService
    {
        LeaveQuotaServiceViewModel GetLeaveQuotaDetails(string objEmpID);
        //int SaveFromSAP(QuotaServiceModel model);
    }
}
