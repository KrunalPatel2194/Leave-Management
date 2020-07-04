using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using LeaveSolution.Models;
using LeaveSolution.BAL.ServiceInterface;
using LeaveSolution.BAL.ServiceModels;
using LeaveSolution.Core;
using AutoMapper;
using LeaveSolution.CustomFilter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace LeaveSolution.Controllers
{
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly ILeaveHistoryService _ILeaveHistoryService;
        public ReportController(ILeaveHistoryService leaveHistoryService, ILogger<ReportController> logger)
        {
            _ILeaveHistoryService = leaveHistoryService;
            _logger = logger;
        }
        public IActionResult EmployeeReport()
        {
            LeaveHistoryModelViewModel objModel = new LeaveHistoryModelViewModel();
            return View();
        }
        [HttpPost]
        public IActionResult EmployeeReport(LeaveHistoryModelViewModel obj)
        {
            try
            {
                var empid = "" ;
                if (obj.EmployeeID.Length != 8)
                {
                    empid = obj.EmployeeID.ToString().PadLeft(8, '0');
                }
                var fromdate = obj.FromDate;
                var todate = obj.ToDate;
                obj = Mapper.Map<LeaveHistoryModelViewModel>(_ILeaveHistoryService.GetEmployeeReport(empid, fromdate, todate));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return View(obj);
        }
    }
}