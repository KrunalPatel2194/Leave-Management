using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LeaveSolution.Models;
using LeaveSolution.BAL.ServiceModels;
using LeaveSolution.BAL.ServiceInterface;
using LeaveSolution.Core;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using AutoMapper;
using LeaveSolution.DAL.Interfaces;
using LeaveSolution.DAL.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using LeaveSolution.CustomFilter;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LeaveSolution.RESTAPI;

namespace LeaveSolution.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IOptions<MySettingsModel> appSettings;
        public readonly IConfiguration _rfcconfig;
        private readonly ILogger<AttendanceController> _logger;
        public AttendanceController(IOptions<MySettingsModel> app, IConfiguration rfc, ILogger<AttendanceController> logger)
        {
            appSettings = app;
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
            _rfcconfig = rfc;
            _logger = logger;
        }
        public SelectList GetYears(int? iSelectedYear)
        {
            List<SelectListItem> ddlYears = new List<SelectListItem>();
            try
            {
                int CurrentYear = DateTime.Now.Year;

                for (int i = 2010; i <= CurrentYear; i++)
                {
                    ddlYears.Add(new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return new SelectList(ddlYears, "Value", "Text", iSelectedYear);
        }

        [Authentication]
        public async Task<IActionResult> Attendance(AttendanceModel Atm)
        {
            try
            {
                string EmployeeID = HttpContext.Session.GetString(Constant.EmployeeID);
                DateTime now = DateTime.Now;
                if (Atm.Month == null)
                {
                    Atm.Month = now.Month.ToString();
                    Atm.Year = now.Year.ToString();
                }
                string Month = Atm.Month;
                string Year = Atm.Year;
                string param = "attendance?pernr=" + EmployeeID + "&month=" + Month + "&year=" + Year;
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
                        EMPATTENDANCE AttendanceDetails = JsonConvert.DeserializeObject<EMPATTENDANCE>(readTask.Result);
                        Atm.ListOfAttendance = new List<AttendanceModel>();
                        if (AttendanceDetails != null && AttendanceDetails.OUTPUT != null)
                        {
                            foreach (var item in AttendanceDetails.OUTPUT)
                            {
                                AttendanceModel model = new AttendanceModel();
                                DateTime d = DateTime.ParseExact(item.Ldate, "yyyyMMdd", CultureInfo.InvariantCulture);
                                model.Date = d.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                                model.Day = item.Day_Type;
                                model.DayName = d.ToString("dddd");
                                model.Firsthalf = item.Shift;
                                string swipein = item.Swipe_In.ToString("HH:mm");
                                string swipeout = item.Swipe_Out.ToString("HH:mm");
                                if (swipein != "00:00")
                                {
                                    model.Intime = item.Swipe_In.ToLongTimeString();
                                }
                                if (swipeout != "00:00")
                                {
                                    model.Outtime = item.Swipe_Out.ToLongTimeString();
                                }
                                if (item.Swipe_In.ToLongTimeString() == null || item.Swipe_Out.ToLongTimeString() == null || (swipein == "00:00") || (swipeout == "00:00"))
                                {
                                    model.AttendanceRegularization = "Regularize the Attendance";
                                }
                                else
                                {
                                    model.AttendanceRegularization = "No";
                                }
                                Atm.ListOfAttendance.Add(model);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return View(Atm);
        }
    }
}