using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.BAL.ServiceInterface;
using LeaveSolution.BAL.ServiceModels;
using LeaveSolution.DAL.Interfaces;
using AutoMapper;
using OfficeOpenXml;
using System.Drawing;


namespace LeaveSolution.BAL.ServiceClass
{
   public class LeaveHistoryService:ILeaveHistoryService
    {
        private readonly ILeaveHistoryRepository _ILeaveHistoryRepository;
        public LeaveHistoryService(ILeaveHistoryRepository leaveHistoryRepository)
        {
            _ILeaveHistoryRepository = leaveHistoryRepository;
        }
        public LeaveHistoryServiceViewModel GetLeaveHistoryDetails(string empid,string year)
        {
            var objServiceModel = new LeaveHistoryServiceViewModel
            {
                LeaveHistory = Mapper.Map<List<LeaveHistoryServiceViewModel>>(_ILeaveHistoryRepository.GetLeaveHistory(empid, year))
            };
            return objServiceModel;
        }
  
        public LeaveHistoryServiceViewModel GetEmployeeReport(string empId, string fromdate, string todate)
        {
            var objServiceModel = new LeaveHistoryServiceViewModel
            {
                LeaveHistory = Mapper.Map<List<LeaveHistoryServiceViewModel>>(_ILeaveHistoryRepository.GetEmployeeReport(empId,fromdate,todate))
            };
            return objServiceModel;
        }
        public void LeaveHistoryExcel(string empid)
        {
               _ILeaveHistoryRepository.LeaveHistoryExcel(empid);
        }

    }
}
