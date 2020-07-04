using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.BAL.ServiceInterface;
using LeaveSolution.BAL.ServiceModels;
using LeaveSolution.DAL.Interfaces;
using AutoMapper;

namespace LeaveSolution.BAL.ServiceClass
{
    public class LeaveQuotaService : ILeaveQuotaService
    {
        private readonly ILeaveQuotaRepository _ILeaveQuotaRepository;
        public LeaveQuotaService(ILeaveQuotaRepository leaveQuotaRepository)
        {
            _ILeaveQuotaRepository = leaveQuotaRepository;
        }

        public LeaveQuotaServiceViewModel GetLeaveQuotaDetails(string objEmpID)
        {

            var objServiceModel = new LeaveQuotaServiceViewModel
            {
                LeaveQuota = Mapper.Map<List<LeaveQuotaServiceModel>>(_ILeaveQuotaRepository.GetLeaveQuota(objEmpID))
            };
            return objServiceModel;
        }
    }
}
