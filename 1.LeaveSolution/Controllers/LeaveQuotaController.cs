using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LeaveSolution.Models;
using LeaveSolution.BAL.ServiceModels;
using LeaveSolution.BAL.ServiceInterface;
using LeaveSolution.Core;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using AutoMapper;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using LeaveSolution.DAL.Data;
using LeaveSolution.DAL.Interfaces;
using LeaveSolution.CustomFilter;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using LeaveSolution.RESTAPI;
using Microsoft.Extensions.Options;

namespace LeaveSolution.Controllers
{
    public class LeaveQuotaController : Controller
    {
        private readonly IOptions<MySettingsModel> appSettings;
        private readonly ILogger<LeaveQuotaController> _logger;
        private readonly ILeaveQuotaService _ILeaveQuotaService;
        public readonly IConfiguration _rfcconfig;
        public LeaveQuotaController(IOptions<MySettingsModel> app, IConfiguration rfc, ILeaveQuotaService leaveQuotaService, ILogger<LeaveQuotaController> logger)
        {
            appSettings = app;
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
            _rfcconfig = rfc;
            _ILeaveQuotaService = leaveQuotaService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authentication]
        public IActionResult LeaveQuota()
        {
            try
            {
                LeaveQuotaModelViewModel modeldata = new LeaveQuotaModelViewModel();
                var EmployeeId = HttpContext.Session.GetString(Constant.EmployeeID);
                var EmployeeName = HttpContext.Session.GetString(Constant.SessionUserName);
                modeldata.LeaveQuota = new List<LeaveQuotaModel>();
                modeldata.LeaveQuota = AddSAPQuota(EmployeeId);
                return View(modeldata);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                ViewBag.Messages = new[] { new AlertModel("danger", "Warning!", "Entered Credentials Did Not Match") };
                return View();
            }
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
                        if(QuotaOverviewDetails != null && QuotaOverviewDetails.QUOTA_OVERVIEW != null)
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