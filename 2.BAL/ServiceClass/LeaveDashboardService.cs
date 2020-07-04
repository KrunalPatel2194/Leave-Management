using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.BAL.ServiceInterface;
using LeaveSolution.BAL.ServiceModels;
using LeaveSolution.DAL.Interfaces;
using AutoMapper;

namespace LeaveSolution.BAL.ServiceClass
{
    public class LeaveDashboardService:ILeaveDashboardService
    {
        private readonly ILeaveDashboardRepositary _ILeaveDashboardRepositary;
        public LeaveDashboardService(ILeaveDashboardRepositary leaveDashboardRepositary)
        {
            _ILeaveDashboardRepositary = leaveDashboardRepositary;
        }
        public LeaveDashboardServiceViewModel GetLeaveDashboardDetails(string empid)
        {
            var objServiceModel = new LeaveDashboardServiceViewModel
            {
                LeaveDashboard = Mapper.Map<List<LeaveDashboardServiceModel>>(_ILeaveDashboardRepositary.GetDashboardLeaveList(empid))
            };
            return objServiceModel;
        }
    }
}
