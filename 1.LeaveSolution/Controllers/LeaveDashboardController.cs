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
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using LeaveSolution.RESTAPI;
using Microsoft.Extensions.Options;

namespace LeaveSolution.Controllers
{
    public class LeaveDashboardController : Controller
    {
        private readonly IOptions<MySettingsModel> appSettings;
        private readonly ILeaveDashboardService _ILeaveDashboardService;
        private readonly ILogger<LeaveDashboardController> _logger;
        private readonly ILeaveHistoryService _ILeaveHistoryService;
        public readonly IConfiguration _rfcconfig;
        public LeaveDashboardController(IOptions<MySettingsModel> app, ILeaveDashboardService leaveDashboardService, ILeaveHistoryService leaveHistoryService, ILogger<LeaveDashboardController> logger, IConfiguration rfc)
        {
            appSettings = app;
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
            _ILeaveDashboardService = leaveDashboardService;
            _ILeaveHistoryService = leaveHistoryService;
            _logger = logger;
            _rfcconfig = rfc;
        }
        [Authentication]
        public IActionResult LeaveDashboard()
        {
            LeaveQuotaModelViewModel modeldata = new LeaveQuotaModelViewModel();
            try
            {
                string msg = Convert.ToString(TempData["Messages"]);
                if (msg != "")
                {
                    ViewData["Messages"] = new[] { new AlertModel(msg.Split("~")[0], "", msg.Split("~")[1]) };
                }
                var EmployeeId = HttpContext.Session.GetString(Constant.EmployeeID);
                modeldata.LeaveQuota = new List<LeaveQuotaModel>();
                modeldata.LeaveQuota =  AddSAPQuota(EmployeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return View(modeldata);
        }
        public IActionResult LeaveHistoryData()
        {
            LeaveHistoryModelViewModel objModel = new LeaveHistoryModelViewModel();
            try
            {
                var objEmployeeId = HttpContext.Session.GetString(Constant.EmployeeID);
                string Approverid = Convert.ToString(objEmployeeId);
                string Year = DateTime.Now.Year.ToString();
                var EmployeeName = HttpContext.Session.GetString(Constant.SessionUserName);
                objModel = Mapper.Map<LeaveHistoryModelViewModel>(_ILeaveHistoryService.GetLeaveHistoryDetails(objEmployeeId, Year));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return Json(new { result = objModel.LeaveHistory.OrderByDescending(x => x.LeaveAppliedDate) });
        }
        public List<LeaveQuotaModel> AddSAPQuota(string EmployeeID)
        {
            List<LeaveQuotaModel> objlist = new List<LeaveQuotaModel>();
            try
            {
                var firstDay = new DateTime(DateTime.Now.Year, 1, 1);
                var lastDay = new DateTime(DateTime.Now.Year, 12, 31);
                string Begda = firstDay.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture); //"01/01/2019"; // 
                string Endda = lastDay.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture); // //"31/12/2019"; // 
                string param = "quota?pernr_no=" + EmployeeID + "&begda=" + Begda + "&endda=" + Endda;
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = _rfcconfig.GetSection("MySettings").GetSection("WebApiBaseUrl").Value + param;
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var response = client.GetAsync(apiUrl);
                    var data = response.Result;
                    if (data.IsSuccessStatusCode)
                    {
                        var readTask = data.Content.ReadAsStringAsync();
                        QUOTAOVERVIEWDETAILS QuotaOverviewDetails = JsonConvert.DeserializeObject<QUOTAOVERVIEWDETAILS>(readTask.Result);
                        if (QuotaOverviewDetails != null && QuotaOverviewDetails.QUOTA_OVERVIEW != null)
                        {
                            foreach (var item in QuotaOverviewDetails.QUOTA_OVERVIEW)
                            {
                                LeaveQuotaModel model = new LeaveQuotaModel();
                                model.LeaveCategory = item.Ktext;
                                model.Quota = Convert.ToDouble(item.Anzhl);
                                model.BalanceLeave = Convert.ToDouble(item.Anzhl_Close);
                                model.FromDate = item.Begda.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                                model.ToDate = item.Endda.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                                model.TotalLeaves = Convert.ToDouble(item.Kverb);
                                objlist.Add(model);
                            }
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return objlist;
        }
    }
}
