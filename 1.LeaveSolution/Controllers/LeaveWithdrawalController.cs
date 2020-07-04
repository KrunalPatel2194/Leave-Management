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
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using LeaveSolution.CustomFilter;
using LeaveSolution.DAL.Interfaces;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using LeaveSolution.RESTAPI;
using System.Threading.Tasks;
using System.Globalization;

namespace LeaveSolution.Controllers
{
    public class LeaveWithdrawalController : Controller
    {
        private readonly IOptions<MySettingsModel> appSettings;
        private readonly ILogger<LeaveQuotaController> _logger;
        private readonly ILeaveRequestViewService _ILeaveRequestViewService;
        private readonly ILeaveWithdrawalService _iLeavewithdrawalService;
        private readonly ISMSUtility _SMSUtility;
        public readonly IConfiguration _rfcconfig;
        public LeaveWithdrawalController(IOptions<MySettingsModel> app, ILeaveWithdrawalService leaveWithdrawalService, ILeaveRequestViewService leaveRequestViewService, ISMSUtility SMSUtility, ILogger<LeaveQuotaController> logger, IConfiguration rfc)
        {
            appSettings = app;
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
            _iLeavewithdrawalService = leaveWithdrawalService;
            _ILeaveRequestViewService = leaveRequestViewService;
            _SMSUtility = SMSUtility;
            _logger = logger;
            _rfcconfig = rfc;
        }
        [Authentication]
        public IActionResult LeaveWithdrawal()
        {
            try
            {
                LeaveWithdrawalViewModel modeldata;
                LeaveWithdrawalViewModel objNewModel = new LeaveWithdrawalViewModel();
                string EmployeeId = HttpContext.Session.GetString(Constant.EmployeeID);
                string Approverid = Convert.ToString(EmployeeId);
                var EmployeeName = HttpContext.Session.GetString(Constant.SessionUserName);
                objNewModel.leavewithdrawal = new List<LeaveWithdrawalModel>();
                modeldata = Mapper.Map<LeaveWithdrawalViewModel>(_iLeavewithdrawalService.GetLeaveRequestForWithdrawal(EmployeeId));
                string Year = DateTime.Now.Year.ToString();
                string param = "LeaveHistory?pernr=" + EmployeeId + "&year=" + Year;
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
                        EMPLEAVEHISTORY QuotaOverviewDetails = JsonConvert.DeserializeObject<EMPLEAVEHISTORY>(readTask.Result);
                        if (QuotaOverviewDetails!= null && QuotaOverviewDetails.OUTPUT != null)
                        {
                            foreach (var item in QuotaOverviewDetails.OUTPUT)
                            {
                                LeaveWithdrawalModel objTempModel = new LeaveWithdrawalModel();
                                objTempModel.LeaveCode = Convert.ToString(item.LEAVE_CODE);
                                objTempModel.EmployeeID = EmployeeId;
                                objTempModel.LeaveAppliedDateString = "<span>" + item.APPLIED_DATE.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "</span>" + item.APPLIED_DATE.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Replace('-', '/'); ;
                                objTempModel.FromDateString = item.BEGDA.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Replace('-', '/'); ;
                                objTempModel.ToDateString = item.ENDDA.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Replace('-', '/'); ;
                                objTempModel.TotalLeaves = Convert.ToDouble(item.TOT_LEAVES);
                                objTempModel.Status = Convert.ToString(item.STATUS);
                                objTempModel.LeaveShift = Convert.ToString(item.LEAVE_TYPE);
                                objTempModel.Indicator = item.SAP_STATUS;
                                objTempModel.LeaveAppliedFrom = item.SUBMITTED_BY;
                                if (item.SAP_STATUS == "S")
                                {
                                    objTempModel.ErrorMsg = "SAP updated successfully";
                                }
                                else
                                {
                                    objTempModel.ErrorMsg = item.ERROR_MESSAGE;
                                }
                                objNewModel.leavewithdrawal.Add(objTempModel);
                            }
                        }
                    }
                }
                string PA = HttpContext.Session.GetString(Constant.PersonalArea);
                string PSA = HttpContext.Session.GetString(Constant.PersonalSubArea);
                string Category = HttpContext.Session.GetString(Constant.Category);
                var LeaveCategoryList = _ILeaveRequestViewService.GetLeaveRequest(PA, PSA, Category);
                LeaveCategoryList.RemoveAt(0);
                foreach (var subitem in objNewModel.leavewithdrawal)
                {
                    var urlNameExists = LeaveCategoryList.Any(s => s.Value.Split('~')[2] == subitem.LeaveCode);
                    if (urlNameExists)
                        subitem.LeaveCategory = LeaveCategoryList.FirstOrDefault(s => s.Value.Split('~')[2] == subitem.LeaveCode).Text;
                    else
                    {
                        subitem.LeaveCategory = subitem.LeaveCode;
                    }
                    LeaveWithdrawalModel model = modeldata.leavewithdrawal.FirstOrDefault(t =>
                    t.LeaveCode == subitem.LeaveCode &&
                                  t.LeaveType == subitem.LeaveType &&
                                  t.FromDateString == subitem.FromDateString &&
                                  t.ToDateString == subitem.FromDateString);
                    if (model != null)
                    {
                        subitem.ApproverName = model.ApproverName;
                        subitem.LeaveAppliedDate = model.LeaveAppliedDate;
                        modeldata.leavewithdrawal.Remove(model);
                    }
                }

                foreach (var subitem in modeldata.leavewithdrawal)
                {
                    if (subitem.LeaveShift == "F")
                    {
                        subitem.LeaveShift = "First Half";
                    }
                    else if (subitem.LeaveShift == "S")
                    {
                        subitem.LeaveShift = "Second Half";
                    }
                    else
                    {
                        subitem.LeaveShift = "Full Day";
                    }
                    string date = subitem.LeaveAppliedDateString.Replace('-', '/');
                    DateTime dtdate = DateTime.ParseExact(date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    subitem.LeaveAppliedDateString = "<span>" + dtdate.ToString("yyyyMMdd") + "</span>" + dtdate.ToString("dd/MM/yyyy");
                    subitem.LeaveAppliedFrom = "EMP";
                    subitem.FromDateString = subitem.FromDateString.Replace('-', '/');
                    subitem.ToDateString = subitem.ToDateString.Replace('-', '/');
                    objNewModel.leavewithdrawal.Add(subitem);
                }
                return View(objNewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return View();
            }
        }
        public bool DeleteLeaveFromSAP(LeaveWithdrawalModel model)
        {
            bool noerror = true;
            DateTime Begda = Convert.ToDateTime(model.FromDateString);
            model.FromDateString = Begda.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            DateTime Endda = Convert.ToDateTime(model.ToDateString);
            model.ToDateString = Endda.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            string param = "leaves?pernr_001=" + model.EmployeeID + "&subty_006=" + model.LeaveCode + "&begda_007=" + model.FromDateString + "&endda_008=" + model.ToDateString + "&full_first_sec=" + model.LeaveShift + "&simulation=" + "" + "&Deletion=" + "X";
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = _rfcconfig.GetSection("MySettings").GetSection("WebApiBaseUrl").Value + param;
                client.BaseAddress = new Uri(apiUrl);
                client.Timeout = TimeSpan.FromMinutes(5);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var responsecli = client.GetAsync(apiUrl);
                var data = responsecli.Result;
                if (data.IsSuccessStatusCode)
                {
                    var readTask = data.Content.ReadAsStringAsync();
                    LEAVEREQUESTDETAILS leaveRequestDetails = JsonConvert.DeserializeObject<LEAVEREQUESTDETAILS>(readTask.Result);
                    if (leaveRequestDetails != null && leaveRequestDetails.IT_ERROR_TEXT != null)
                    {
                        foreach (var detail in leaveRequestDetails.IT_ERROR_TEXT)
                        {
                            if (detail.IND == "E")
                            {
                                noerror = false;
                            }
                        }
                    }                    
                }
                else
                {
                    _logger.LogError("Gateway Timeout Error" + model.EmployeeID);                   
                }
            }
            return noerror;
        }
        [HttpPost]
        public IActionResult LeaveWithdrawal(LeaveWithdrawalModel model)
        {
            int result = 0;
            var msgText = "";
            bool returnres = false;
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.LeaveAppliedFrom == "T" || model.LeaveAppliedFrom == "E")
                    {
                        if (model.LeaveShift == "First Half")
                        {
                            model.LeaveShift = "F";
                        }
                        else if (model.LeaveShift == "First Half")
                        {
                            model.LeaveShift = "S";
                        }
                        else
                        {
                            model.LeaveShift = "";
                        }
                        bool retresult = DeleteLeaveFromSAP(model);
                        return Json(retresult);
                    }
                    string Approverid = (HttpContext.Session.GetString(Constant.EmployeeID));
                    var EmployeeName = HttpContext.Session.GetString(Constant.SessionUserName);
                    string EmployeeId = HttpContext.Session.GetString(Constant.EmployeeID);
                    string strmobileNo = HttpContext.Session.GetString(Constant.MobileNo);
                    string strmobileNoapp = HttpContext.Session.GetString(Constant.ApproverMobileNo);
                    if (model.Status == "Pending" || model.Status == "Approved")
                    {
                        result = _iLeavewithdrawalService.SaveLeaveWithdrawal(Mapper.Map<LeaveWithdrawalServiceModel>(model));
                        if (result == 1)
                        {
                            returnres = true;
                            #region For Message to Employee
                            if (model.Status == "Approved")
                            {
                                msgText =
                                     //"Your Subordinate wants to cancel/ withdraw below leave/attendance requests which were approved in the system." + Environment.NewLine +
                                     "You have submitted a request for cancellation / withdrawal of below mentioned leaves / attendance.Your request has been sent to your superior for further action." + Environment.NewLine +
                                     "Employee No:-" + HttpContext.Session.GetString(Constant.EmployeeID) + Environment.NewLine +
                                     "Request:-" + model.LeaveCategory + Environment.NewLine +
                                     "From Date:-" + model.FromDateString + Environment.NewLine +
                                     "To Date:-" + model.ToDateString + Environment.NewLine +
                                     "Days:-" + model.TotalLeaves + " days";
                            }
                            else if (model.Status == "Pending")
                            {
                                msgText =
                                   "You have submitted a request for cancellation/ withdrawal of below non- approved leaves / attendance which has been processed and submitted entry has been deleted." + Environment.NewLine +
                                   "Employee No:-" + HttpContext.Session.GetString(Constant.EmployeeID) + Environment.NewLine +
                                   "Request:-" + model.LeaveCategory + Environment.NewLine +
                                   "From Date:-" + model.FromDateString + Environment.NewLine +
                                   "To Date:-" + model.ToDateString + Environment.NewLine +
                                   "Days:-" + model.TotalLeaves + " days";

                            }
                            try
                            {
                                _SMSUtility.SendSMS(strmobileNo, msgText);
                            }
                            catch (Exception)
                            {
                            }



                            #endregion
                            #region For Message to Approver(Supervisor)
                            //var msgTextApp = "";
                            //if (model.Status == "Pending")
                            //{
                            //    msgTextApp =
                            //         "Your Subordinate has cancelled below non-Approved leave/attendance requests. This is for your information." + Environment.NewLine +
                            //         "Employee No:-" + HttpContext.Session.GetString(Constant.EmployeeID) + Environment.NewLine +
                            //         "Request:-" + model.LeaveCategory + Environment.NewLine +
                            //         "From Date:-" + model.FromDate + Environment.NewLine +
                            //         "To Date:-" + model.ToDate + Environment.NewLine +
                            //         "Days:-" + model.TotalLeaves + "days";
                            //}

                            //var smsapp = _SMSUtility.SendSMS(strmobileNoapp, msgTextApp);

                            //if (smsapp == true)
                            //{
                            //    strmobileNoapp = Convert.ToString(1);
                            //}
                            //else
                            //{
                            //    strmobileNoapp = Convert.ToString(0);
                            //}
                            #endregion
                        }
                    }
                    else
                    {
                        returnres = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return Json(returnres);
        }

        [HttpPost]
        public JsonResult LeaveWithdrawalPassData(LeaveWithdrawalModel model)
        {
            string ReturnUrl = Url.Action("CreateLeaveRequest", "Leave");
            try
            {
                if (model.Status.Trim() == "Pending" || model.Status.Trim() == "Approved")
                {
                    TempData["LeaveData"] = Convert.ToString(model.LeaveRequestId);
                    return Json(ReturnUrl);
                }
                else
                {
                    ViewBag.Messages = new[] { new AlertModel("danger", "warning!", "Data not found..") };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return Json(ReturnUrl);
        }

        public ActionResult DownloadAttachment(string filename)
        {
            try
            {
                string filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/FileUpload", filename);
                if (System.IO.File.Exists(filepath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
                }
                else
                {
                    return RedirectToAction("LeaveWithdrawal");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return RedirectToAction("LeaveWithdrawal");
            }
        }
    }
}