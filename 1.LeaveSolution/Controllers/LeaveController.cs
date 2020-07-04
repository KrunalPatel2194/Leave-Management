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
using System.Web;
using Microsoft.Extensions.Options;
using LeaveSolution.RESTAPI;

namespace LeaveSolution.Controllers
{
    public class LeaveController : Controller
    {
        private readonly IOptions<MySettingsModel> appSettings;
        private readonly ILeaveRequestViewService _ILeaveRequestViewService;
        private readonly ISMSUtility _SMSUtility;
        private readonly IDeptLeaveRequestViewService _IDeptLeaveRequestViewService;
        public readonly IConfiguration _rfcconfig;
        private readonly ILogger<LeaveController> _logger;
        public LeaveController(IOptions<MySettingsModel> app, IDeptLeaveRequestViewService DeptleaveRequestViewService, IConfiguration rfc, ILeaveRequestViewService leaveRequestViewService, ISMSUtility smsutility, ILogger<LeaveController> logger)
        {
            appSettings = app;
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
            _rfcconfig = rfc;
            _IDeptLeaveRequestViewService = DeptleaveRequestViewService;
            _ILeaveRequestViewService = leaveRequestViewService;
            _SMSUtility = smsutility;
            _logger = logger;
        }
        public IActionResult Index()
        {
            //    ViewData["Message"] = _Localizer["Leave Category"];
            return View();
        }

        /// <summary>
        /// For Leave Bind in Dropdown
        /// </summary>
        /// <returns></returns>
        public JsonResult GetLeaveRequestList()
        {
            try
            {
                LeaveRequestViewModel obj = new LeaveRequestViewModel();
                obj.PA = HttpContext.Session.GetString(Constant.PersonalArea);
                obj.PSA = HttpContext.Session.GetString(Constant.PersonalSubArea);
                var leaveList = _ILeaveRequestViewService.GetLeaveRequest(obj.PA, obj.PSA, obj.Category);
                return Json(leaveList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(string.Empty);
            }
        }
        /// <summary>
        /// Get Holiday 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetHolidayList()
        {
            try
            {
                LeaveRequestViewModel objModel = new LeaveRequestViewModel();
                objModel.EmployeeID = HttpContext.Session.GetString(Constant.EmployeeID);
                var holidaylist = _ILeaveRequestViewService.GetHolidayList(objModel.EmployeeID);
                return Json(holidaylist);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(string.Empty);
            }
        }
        public JsonResult GetApproverList(string search)
        {
            try
            {
                string EmployeeID = HttpContext.Session.GetString(Constant.EmployeeID);
                List<AlternateApproverModel> autocompempid = GetApprover(EmployeeID, "A");
                return Json(new { result = true, data = autocompempid });
                // JsonResult result = Json(new { result = true, data = autocompempid })
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(new { result = false, msg = "No approver found." });
            }
        }
        public List<AlternateApproverModel> GetApprover(string EmployeeID, string Approver)
        {
            List<AlternateApproverModel> Getdata = new List<AlternateApproverModel>();
            try
            {
                string param = "hod?pernr_no=" + EmployeeID + "&approver=" + Approver;
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
                        Zftm0006Response hodDetails = JsonConvert.DeserializeObject<Zftm0006Response>(readTask.Result);
                        if (hodDetails != null && hodDetails.DETAILS != null)
                        {
                            foreach (var hoddata in hodDetails.DETAILS)
                            {
                                var res = new AlternateApproverModel();
                                res.ApproverId = hoddata.PERNR_SUP;
                                res.ApproverName = hoddata.CNAME_SUP;
                                res.ApproverMobNo = hoddata.CELL_SUP;
                                res.ApproverMailID = hoddata.EMAIL_SUP;
                                res.HODID = hoddata.PERNR_HOD;
                                res.HODName = hoddata.CNAME_HOD;
                                res.HODMailID = hoddata.EMAILID_HOD;
                                Getdata.Add(res);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Getdata;
        }
        public bool GetApprover(LeaveRequestViewModel obj)
        {
            bool returndata = false;
            string EmployeeID1 = HttpContext.Session.GetString(Constant.EmployeeID);
            try
            {
                string param = "hod?pernr_no=" + EmployeeID1 + "&approver=" + "S";
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
                        var responses = JsonConvert.DeserializeObject<Zftm0006Response>(readTask.Result);
                        var hodDetails = responses;
                        if (hodDetails.DETAILS != null)
                        {
                            foreach (var detail in hodDetails.DETAILS)
                            {
                                detail.EmployeeID = EmployeeID1;
                                if (_rfcconfig.GetSection("StaticValue").GetSection("EMobNo").Value != "")
                                {
                                    detail.CELL_SUP = _rfcconfig.GetSection("StaticValue").GetSection("EMobNo").Value;
                                    detail.EMAIL_SUP = _rfcconfig.GetSection("StaticValue").GetSection("EmailApr").Value;
                                    detail.EMAILID_HOD = _rfcconfig.GetSection("StaticValue").GetSection("EMailHOD").Value;
                                    detail.PERNR_SUP = _rfcconfig.GetSection("StaticValue").GetSection("AprID").Value;
                                    detail.CNAME_SUP = _rfcconfig.GetSection("StaticValue").GetSection("AprName").Value;
                                    obj.ApproverId = _rfcconfig.GetSection("StaticValue").GetSection("AprID").Value;
                                    obj.ApproverName = _rfcconfig.GetSection("StaticValue").GetSection("AprName").Value;
                                }
                                else
                                {
                                    obj.ApproverId = detail.PERNR_SUP;
                                    obj.ApproverName = detail.CNAME_SUP;
                                }
                                if (obj.ApproverId.Length != 8)
                                {
                                    obj.ApproverId = obj.ApproverId.ToString().PadLeft(8, '0');
                                }
                                if (detail.PERNR_SUP.Length != 8)
                                {
                                    detail.PERNR_SUP = detail.PERNR_SUP.ToString().PadLeft(8, '0');
                                }
                                var datareturn = _ILeaveRequestViewService.SaveApproverFromSAP(Mapper.Map<ApproverServiceModel>(hodDetails));
                                returndata = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                returndata = false;
                throw;
            }
            return returndata;
        }
        public JsonResult SaveAlternateApprover(LeaveRequestViewModel model)
        {
            try
            {
                string EmpID = HttpContext.Session.GetString(Constant.EmployeeID);
                Zftm0006Response z = new Zftm0006Response();
                z.DETAILS = new ZFTM0006_TAB[6];
                z.DETAILS[0] = new ZFTM0006_TAB();
                if (_rfcconfig.GetSection("StaticValue").GetSection("EMobNo").Value != "")
                {
                    z.DETAILS[0].CELL_SUP = _rfcconfig.GetSection("StaticValue").GetSection("EMobNo").Value;
                    z.DETAILS[0].EMAIL_SUP = _rfcconfig.GetSection("StaticValue").GetSection("EmailApr").Value;
                    z.DETAILS[0].EMAILID_HOD = _rfcconfig.GetSection("StaticValue").GetSection("EMailHOD").Value;
                    z.DETAILS[0].PERNR_SUP = _rfcconfig.GetSection("StaticValue").GetSection("AprID").Value;
                    z.DETAILS[0].CNAME_SUP = _rfcconfig.GetSection("StaticValue").GetSection("AprName").Value;
                }
                else
                {
                    z.DETAILS[0].PERNR_SUP = model.ApproverId.Trim();
                    z.DETAILS[0].CNAME_SUP = model.ApproverName.Trim();
                    z.DETAILS[0].CELL_SUP = model.ApproverMobNo.Trim();
                    z.DETAILS[0].EMAIL_SUP = model.ApproverMailID.Trim();
                    z.DETAILS[0].EMAILID_HOD = model.HODMailID.Trim();
                }
                z.DETAILS[0].PERNR_HOD = model.HODID;
                z.DETAILS[0].CNAME_HOD = model.HODName;
                z.DETAILS[0].EmployeeID = EmpID;
                z.DETAILS[0].LeaveRequestID = model.LeaveRequestId;
                var datareturn = _ILeaveRequestViewService.SaveApproverFromSAP(Mapper.Map<ApproverServiceModel>(z));
                var msg = "saved";
                return Json(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(string.Empty);
            }
        }

        [Authentication]
        public IActionResult CreateLeaveRequest()
        {
            LeaveRequestViewModel obj = new LeaveRequestViewModel();
            try
            {
                string firsttimelogin = HttpContext.Session.GetString("FirstTimeLogin");
                if (firsttimelogin == null)
                {
                    GetApprover(obj);
                    HttpContext.Session.SetString("FirstTimeLogin", "yes");
                }
                else
                {
                    string EmpID = HttpContext.Session.GetString(Constant.EmployeeID);
                    var returnresult = Mapper.Map<DeptLeaveRequestViewModel>(_IDeptLeaveRequestViewService.GetEmpDetails(EmpID));
                    obj.ApproverId = returnresult.ApproverId.Split('_')[0];
                    obj.ApproverName = returnresult.ApproverId.Split('_')[1];
                }
                List<SelectListItem> leaveList = new List<SelectListItem>();
                ViewBag.IsChangeLeave = "No";
                string editleaveid = Convert.ToString(TempData["LeaveData"]);
                if (editleaveid != "")
                {
                    obj = Mapper.Map<List<LeaveRequestViewModel>>(_ILeaveRequestViewService.GetLeaveDetailsForChange(Convert.ToInt32(editleaveid))).FirstOrDefault();
                    ViewBag.IsChangeLeave = "Yes";
                }
                obj.PA = HttpContext.Session.GetString(Constant.PersonalArea);
                obj.PSA = HttpContext.Session.GetString(Constant.PersonalSubArea);
                obj.Category = HttpContext.Session.GetString(Constant.Category);
                var LeaveCategoryList = _ILeaveRequestViewService.GetLeaveRequest(obj.PA, obj.PSA, obj.Category);
                obj.Leavelist = LeaveCategoryList;
                if (editleaveid != "")
                {
                    obj.Leavelist.Find(x => x.Text == obj.LeaveCategory).Selected = true;
                }
                if (obj.ApproverId.Length != 8)
                {
                    obj.ApproverId = obj.ApproverId.ToString().PadLeft(8, '0');
                }
                obj.ApproverName = obj.ApproverId + '_' + obj.ApproverName;
                obj.EmployeeName = HttpContext.Session.GetString(Constant.SessionUserName);
                return View((obj));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return View((obj));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateLeaveRequest(LeaveRequestViewModel objModel)
        {
            string msg = "";
            try
            {
                objModel.EmployeeName = HttpContext.Session.GetString(Constant.SessionUserName);
                objModel.PA = HttpContext.Session.GetString(Constant.PersonalArea);
                objModel.PSA = HttpContext.Session.GetString(Constant.PersonalSubArea);
                objModel.Category = HttpContext.Session.GetString(Constant.Category);
                if (objModel.ApproverName != null || objModel.ApproverName != string.Empty || objModel.ApproverName != "")
                {
                    string[] arrApproverName = objModel.ApproverName.Split('_');
                    int objArrCount = arrApproverName.Length;
                    if (objArrCount > 1)
                    {
                        var objApproverId = arrApproverName[0].ToString();
                        var objApproverName = arrApproverName[1].ToString();
                        objModel.ApproverId = objApproverId;
                        objModel.ApproverName = objApproverName;
                    }
                }
                if (objModel.LeaveShift == null)
                {
                    objModel.LeaveShift = "";
                }
                if (objModel.Remarks == null)
                {
                    objModel.Remarks = "";
                }
                else
                {
                    objModel.Remarks = HttpUtility.HtmlEncode(objModel.Remarks);
                }
                if (objModel.FileToUpload == null)
                {
                    objModel.UploadFileName = null;
                }
                else
                {
                    var Filesize = 2 * 1024 * 1024; // Size Limit 2 MB.
                    if (objModel.FileToUpload == null || objModel.FileToUpload.Length == 0)
                    {
                        msg = "danger" + "~" + "Please select file Or file can not be empty";
                        TempData["Messages"] = msg;
                        return RedirectToAction("LeaveDashboard", "LeaveDashboard");
                    }
                    else if (objModel.FileToUpload.Length > Filesize)
                    {
                        msg = "danger" + "~" + "File size should not more than 2 MB";
                        TempData["Messages"] = msg;
                        return RedirectToAction("LeaveDashboard", "LeaveDashboard");
                    }
                    string dateformate = DateTime.Now.ToString("ddMMyyyyhhmmss");
                    string FilePath = objModel.FileToUpload.FileName;
                    if (Path.GetExtension(FilePath).ToLower() != ".pdf")
                    {
                        msg = "danger" + "~" + "Please upload only PDF File";
                        TempData["Messages"] = msg;
                        return RedirectToAction("LeaveDashboard", "LeaveDashboard");
                    }
                    string FileName = Path.GetFileNameWithoutExtension(FilePath);//Without Extension                    
                    var objFile = HttpContext.Session.GetString(Constant.EmployeeID) + '_' + FileName + '_' + dateformate + ".pdf";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/FileUpload", objFile);
                    HttpContext.Session.SetString(Constant.FileName, FilePath);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        objModel.FileToUpload.CopyToAsync(stream);
                        objModel.UploadFileName = objFile;
                    }
                }
                objModel.EmployeeID = HttpContext.Session.GetString(Constant.EmployeeID);
                string startDateInText = objModel.FromDate;
                string endDateInText = objModel.ToDate;
                System.DateTime startDate = DateTime.ParseExact(startDateInText, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture);
                System.DateTime endDate = DateTime.ParseExact(endDateInText, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture);
                var diffdate = (endDate - startDate).TotalDays;
                var totalleave = diffdate + 1;
                double objToalLeave = Convert.ToDouble(totalleave);
                if (objModel.LeaveShift == "F" || objModel.LeaveShift == "S")
                {
                    objToalLeave = (totalleave) - 0.5;
                }
                objModel.TotalAppliedLeave = objToalLeave;
                objModel.CreatedBy = HttpContext.Session.GetString(Constant.SessionUserName);
                Tuple<string, int> Result = _ILeaveRequestViewService.SaveLeaveRequest(Mapper.Map<LeaveRequestViewtServiceModel>(objModel));
                if (Result.Item2 == 1 || Result.Item2 == 2)
                {
                    if (Result.Item2 == 1)
                    {
                        msg = "success" + "~" + Result.Item1;
                        #region Message   to Employee
                        string strmobileNo = HttpContext.Session.GetString(Constant.MobileNo);
                        var msgText =
                             "You submitted below leaves/attendance " + Environment.NewLine +
                             "Employee No:-" + HttpContext.Session.GetString(Constant.EmployeeID) + Environment.NewLine +
                             "Request:-" + objModel.LeaveCategory + Environment.NewLine +
                             "From Date:-" + objModel.FromDate + Environment.NewLine +
                             "To Date:-" + objModel.ToDate + Environment.NewLine +
                             "Days:-" + objModel.TotalAppliedLeave + " days";
                        try
                        {
                            _SMSUtility.SendSMS(strmobileNo, msgText);
                        }
                        catch (Exception)
                        {
                        }

                        #endregion
                    }
                    else
                    {
                        msg = "danger" + "~" + Result.Item1;
                    }
                }
                else
                {
                    _logger.LogError("Error", Result.Item1);
                    msg = "danger" + "~" + "Error occured while processing leave request..";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                msg = "danger" + "~" + "Error occured while processing leave request...............";
            }
            TempData["Messages"] = msg;
            return RedirectToAction("LeaveDashboard", "LeaveDashboard");
        }
        [HttpPost]
        public IActionResult UpdateLeaveRequest(string Approver, string LeaverequestID)
        {
            string msg = "";
            try
            {
                LeaveRequestViewModel objModel = new LeaveRequestViewModel();
                string[] arrApproverName = Approver.Split('_');
                int objArrCount = arrApproverName.Length;
                if (objArrCount > 1)
                {
                    var objApproverId = arrApproverName[0].ToString();
                    var objApproverName = arrApproverName[1].ToString();
                    objModel.ApproverId = objApproverId;
                    objModel.ApproverName = objApproverName;
                }
                objModel.LeaveRequestId = LeaverequestID;
                objModel.EmployeeID = HttpContext.Session.GetString(Constant.EmployeeID);
                Tuple<string, int> Result = _ILeaveRequestViewService.SaveLeaveRequest(Mapper.Map<LeaveRequestViewtServiceModel>(objModel));
                if (Result.Item2 == 1)
                {
                    msg = "success" + "~" + Result.Item1;
                }
                else
                {
                    msg = "danger" + "~" + Result.Item1;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                msg = "danger" + "~" + "Error occured while processing leave request...............";
            }
            TempData["Messages"] = msg;
            //return RedirectToAction("LeaveDashboard", "LeaveDashboard");
            return Json(new { msg = msg });
        }

        public IActionResult ValidationFromSAP(string LeaveID, string FromDate, string ToDate, string LeaveShift)
        {
            string valid = "";
            bool flag = true;
            try
            {
                string esbfromdate = FromDate.Trim().Split("/")[1] + "/" + FromDate.Trim().Split("/")[0] + "/" + FromDate.Trim().Split("/")[2];
                string esbtodate = ToDate.Trim().Split("/")[1] + "/" + ToDate.Trim().Split("/")[0] + "/" + ToDate.Trim().Split("/")[2];
                string LeaveCode = _ILeaveRequestViewService.GetLeaveCode(LeaveID);
                var EmployeeID = HttpContext.Session.GetString(Constant.EmployeeID);
                string param = "leaves?pernr_001=" + EmployeeID + "&subty_006=" + LeaveCode + "&begda_007="
                    + esbfromdate + "&endda_008=" + esbtodate + "&full_first_sec=" + LeaveShift + "&simulation=" + "X" + "&Deletion=" + "";
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
                                    valid = detail.TEXT;
                                    flag = false;
                                }
                            }
                            if (flag == true)
                            {
                                valid = "Data is Valid";
                            }
                        }
                    }
                    else
                    {
                        _logger.LogError("Gateway Timeout Error");
                        valid = "Please try after some time";
                        flag = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                flag = false;
                valid = ex.Message;
            }
            return Json(new { valid = valid, flag = flag });
        }
    }
}

