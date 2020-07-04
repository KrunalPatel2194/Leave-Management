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
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using OfficeOpenXml;
using Newtonsoft.Json;
using LeaveSolution.CustomFilter;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using LeaveSolution.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using LeaveSolution.RESTAPI;

namespace LeaveSolution.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IOptions<MySettingsModel> appSettings;
        private readonly ILogger<DepartmentController> _logger;
        private readonly IDeptLeaveRequestViewService _IDeptLeaveRequestViewService;
        private readonly ISMSUtility _SMSUtility;
        public readonly IConfiguration _rfcconfig;
        private readonly ILoginViewService _ILoginViewService;
        private readonly ILeaveRequestViewService _ILeaveRequestViewService;

        public DepartmentController(IOptions<MySettingsModel> app, IDeptLeaveRequestViewService DeptleaveRequestViewService, IConfiguration rfc, ILogger<DepartmentController> logger, ILoginViewService loginViewService, ILeaveRequestViewService leaveRequestViewService, ISMSUtility smsutility)
        {
            appSettings = app;
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
            _rfcconfig = rfc;
            _IDeptLeaveRequestViewService = DeptleaveRequestViewService;
            _logger = logger;
            _SMSUtility = smsutility;
            _ILoginViewService = loginViewService;
            _ILeaveRequestViewService = leaveRequestViewService;
        }
        [Authentication]
        public IActionResult DepartmentLeaveRequest()
        {
            string msg = Convert.ToString(TempData["Messages"]);
            if (msg != "")
            {
                ViewData["Messages"] = new[] { new AlertModel(msg.Split("~")[0], "", msg.Split("~")[1]) };
            }
            DeptLeaveRequestViewModel obj = new DeptLeaveRequestViewModel();
            List<SelectListItem> leaveList = new List<SelectListItem>();
            return View(obj);
        }
        [HttpPost]
        public IActionResult DepartmentLeaveRequest(DeptLeaveRequestViewModel objModel, string EmployeeName1, string Shift, string empphoneNo1)
        {
            string msg = "";
            try
            {
                if (Shift == null)
                {
                    Shift = "";
                }
                objModel.EmployeeName = EmployeeName1;
                objModel.empphoneNo = empphoneNo1;
                objModel.PA = HttpContext.Session.GetString(Constant.PersonalArea);
                objModel.PSA = HttpContext.Session.GetString(Constant.PersonalSubArea);
                if (objModel.EmployeeID.Length != 8)
                {
                    objModel.EmployeeID = objModel.EmployeeID.ToString().PadLeft(8, '0');
                }
                if (ModelState.IsValid)
                {
                    if (objModel.FileToUpload == null || objModel.FileToUpload.Length == 0)
                    {
                        objModel.UploadFileName = null;
                    }
                    else
                    {
                        string dateformate = DateTime.Now.ToString("ddMMyyyyhhmmss");
                        string FilePath = objModel.FileToUpload.FileName;
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
                    string startDateInText = objModel.FromDate;
                    string endDateInText = objModel.ToDate;
                    if (objModel.Remarks == null)
                    {
                        objModel.Remarks = "";
                    }
                    System.DateTime startDate = DateTime.ParseExact(startDateInText, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture);
                    System.DateTime endDate = DateTime.ParseExact(endDateInText, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture);
                    var diffdate = (endDate - startDate).TotalDays;
                    var totalleave = diffdate + 1;
                    double objToalLeave = Convert.ToDouble(totalleave);
                    if (objModel.LeaveShift == "F" || objModel.LeaveShift == "S")
                    {
                        objToalLeave = (totalleave) - 0.5;
                    }
                    objModel.TotalAppliedLeave = Convert.ToDouble(objToalLeave);

                    objModel.BalanceLeave = 0;
                    objModel.CreatedBy = HttpContext.Session.GetString(Constant.AdminID);
                    objModel.AdminID = HttpContext.Session.GetString(Constant.AdminID);
                    objModel.LeaveShift = Shift;
                    Tuple<string, int> Result = _IDeptLeaveRequestViewService.SaveLeaveRequest(Mapper.Map<DeptLeaveRequestViewtServiceModel>(objModel));
                    if (Result.Item2 == 1)
                    {
                        #region Message fro Employee
                        string strmobileNo = objModel.empphoneNo;
                        var msgText =
                             "You submitted below leaves" + Environment.NewLine +
                             "Employee No:-" + objModel.EmployeeID + Environment.NewLine +
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

                        #region for APprovermessage
                        //For Approver
                        ////string strapprovermobileNo = HttpContext.Session.GetString(Constant.ApproverMobileNo);
                        ////var msgTextapprover =
                        ////     "Below leaves / attendance requests have been submitted for your action. Please review and approve, reject accordingly." + Environment.NewLine +
                        ////     "Employee No:-" + HttpContext.Session.GetString(Constant.EmployeeID) + Environment.NewLine +
                        ////     "Request:-" + objModel.LeaveCategory + Environment.NewLine +
                        ////     "From Date:-" + objModel.FromDate + Environment.NewLine +
                        ////     "To Date:-" + objModel.ToDate + Environment.NewLine +
                        ////     "Days:-" + objModel.TotalAppliedLeave + "days";
                        ////var sms1 = _SMSUtility.SendSMS(strapprovermobileNo, msgTextapprover);
                        ////if (sms1 == true)
                        ////{
                        ////    strapprovermobileNo = Convert.ToString(1);
                        ////}
                        ////else
                        ////{
                        ////    strapprovermobileNo = Convert.ToString(0);
                        ////}
                        #endregion
                        if (Result.Item2 == 1)
                        {
                            msg = "success" + "~" + Result.Item1;
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                msg = "danger" + "~" + "Error occured while processing leave request...............";
            }
            TempData["Messages"] = msg;

            return RedirectToAction("DepartmentLeaveRequest");

        }

        public JsonResult GetLeaveRequestList()
        {
            try
            {
                var adminid = HttpContext.Session.GetString(Constant.AdminID);
                var PArea = HttpContext.Session.GetString(Constant.PersonalArea);
                var PSubArea = HttpContext.Session.GetString(Constant.PersonalSubArea);
                var leaveList = _IDeptLeaveRequestViewService.GetLeaveRequest(PArea, PSubArea, Convert.ToString(adminid));
                return Json(leaveList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                ViewBag.Messages = new[] { new AlertModel("danger", "Error!", ex.Message) };
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
            }
            catch (Exception ex)
            {
                return Json(string.Empty);
            }
            return null;
        }

        public bool GetdataSAP(DeptLeaveRequestViewModel objmodel)
        {
            bool returnresult = false;
            try
            {
                var EmployeeID = objmodel.EmployeeID.Trim();// HttpContext.Session.GetString(Constant.EmployeeID);

                string param = "personal/" + EmployeeID;
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
                        Zftm0008Response empDetails = JsonConvert.DeserializeObject<Zftm0008Response>(readTask.Result);
                        if (empDetails != null && empDetails.Details != null)
                        {
                            foreach (var detail in empDetails.Details)
                            {
                                if (!string.IsNullOrEmpty(detail.Name))
                                {
                                    if (objmodel.EmployeeID.Length != 8)
                                    {
                                        objmodel.EmployeeID = objmodel.EmployeeID.ToString().PadLeft(8, '0');
                                    }
                                    if (_rfcconfig.GetSection("StaticValue").GetSection("EMobNo").Value != "")
                                    {
                                        detail.Userid = _rfcconfig.GetSection("StaticValue").GetSection("EMobNo").Value;
                                    }

                                    DateTime DateOfBirth = Convert.ToDateTime(detail.Birthdate);
                                    detail.Birthdate = DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    detail.EmployeeID = objmodel.EmployeeID;
                                    var returndata1 = _ILoginViewService.SaveFromSAP(Mapper.Map<EmployeeServicemodel>(empDetails));
                                    returnresult = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                returnresult = false;
            }
            return returnresult;
        }

        public bool GetApproverAsync(DeptLeaveRequestViewModel objmodel)
        {
            bool returndata = false;
            try
            {
                string EmployeeID = objmodel.EmployeeID;
                string Approver = "S";
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
                            foreach (var detail in hodDetails.DETAILS)
                            {
                                detail.EmployeeID = EmployeeID;
                                objmodel.ApproverId = detail.PERNR_SUP;
                                objmodel.ApproverName = detail.CNAME_SUP;
                                if (_rfcconfig.GetSection("StaticValue").GetSection("EMobNo").Value != "")
                                {
                                    detail.CELL_SUP = _rfcconfig.GetSection("StaticValue").GetSection("EMobNo").Value;
                                    detail.EMAIL_SUP = _rfcconfig.GetSection("StaticValue").GetSection("EmailApr").Value;
                                    detail.EMAILID_HOD = _rfcconfig.GetSection("StaticValue").GetSection("EMailHOD").Value;
                                    objmodel.ApproverId = _rfcconfig.GetSection("StaticValue").GetSection("AprID").Value;
                                    objmodel.ApproverName = _rfcconfig.GetSection("StaticValue").GetSection("AprName").Value;
                                }
                                else
                                {
                                    objmodel.ApproverId = detail.PERNR_SUP;
                                    objmodel.ApproverName = detail.CNAME_SUP;
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

        [HttpGet]
        public JsonResult GetEmployeeDetails(string EmpID)
        {
            bool returnresult = false;
            DeptLeaveRequestViewModel objmodel = new DeptLeaveRequestViewModel();
            if (EmpID.Length != 8)
            {
                EmpID = EmpID.ToString().PadLeft(8, '0');
            }
            objmodel.EmployeeID = EmpID;
            try
            {
                objmodel = Mapper.Map<DeptLeaveRequestViewModel>(_IDeptLeaveRequestViewService.GetEmpDetails(EmpID));
                objmodel.EmployeeID = EmpID;
                if (objmodel.EmployeeName == null)
                {
                    GetdataSAP(objmodel);
                }
                if (String.IsNullOrEmpty(objmodel.ApproverId) || objmodel.ApproverId.Trim() == "_")
                {
                    bool approverresult = GetApproverAsync(objmodel);
                    if (approverresult == true)
                    {
                        string approver = objmodel.ApproverId + "_" + objmodel.ApproverName;
                        objmodel.ApproverId = approver;
                    }
                    else
                    {
                        returnresult = true;
                    }
                }
                HttpContext.Session.SetString(Constant.PersonalArea, objmodel.PA);
                HttpContext.Session.SetString(Constant.PersonalSubArea, objmodel.PSA);
                returnresult = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(new { data = objmodel, result = false });
            }
            return Json(new { data = objmodel, result = returnresult });
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            List<DisaplyError> objlist = new List<DisaplyError>();
            bool returndata = false;
            try
            {
                DeptLeaveRequestViewModel model = new DeptLeaveRequestViewModel();
                var Filesize = 2 * 1024 * 1024; // Size Limit 2 MB.
                if (file == null || file.Length == 0)
                {
                    return Json(new { returndata = false, msg = "Please select file Or file can not be empty" });
                }
                if (file.Length > Filesize)
                {
                    return Json(new { returndata = false, msg = "File size should not more than 2 MB" });
                }
                string fname = file.FileName;
                var supportedTypes = new[] { "xls", "xlsx" };
                var fileExt = System.IO.Path.GetExtension(fname).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    return Json(new { returndata = false, msg = "Please upload only Excel File" });
                }
                string[] arrfilename = fname.Split('.');
                string filename = arrfilename[0] + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + arrfilename[1];

                var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot" + "/Documents/",
                            filename);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                List<DeptLeaveRequestViewModel> LeaveRequests = new List<DeptLeaveRequestViewModel>();
                FileInfo files = new FileInfo(Path.Combine(path));
                using (ExcelPackage package = new ExcelPackage(files))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;
                    var ValidStartDate = new DateTime(DateTime.Today.Year, 1, 1);
                    var ValidEndDate = new DateTime(DateTime.Today.Year, 12, 31);
                    for (int row = 2; row <= rowCount; row++)
                    {
                        DeptLeaveRequestViewModel mod = new DeptLeaveRequestViewModel();
                        string FromDateString = worksheet.Cells[row, 3].Value.ToString();
                        DateTime FromDate = DateTime.ParseExact(FromDateString, @"dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        string ToDateString = worksheet.Cells[row, 4].Value.ToString();
                        DateTime ToDate = DateTime.ParseExact(ToDateString, @"dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        bool chkleavecode = _rfcconfig.GetSection("DepartmentValidation").GetSection("LCode").Value.Split(',').Contains(worksheet.Cells[row, 2].Value.ToString());
                        if (chkleavecode == true)
                        {
                            if (ValidStartDate <= FromDate && FromDate <= ValidEndDate && ValidStartDate <= ToDate && ToDate <= ValidEndDate)
                            {
                                mod.EmployeeID = worksheet.Cells[row, 1].Value.ToString();
                                if (mod.EmployeeID.Length != 8)
                                {
                                    mod.EmployeeID = mod.EmployeeID.ToString().PadLeft(8, '0');
                                }
                                mod.LeaveCode = worksheet.Cells[row, 2].Value.ToString();
                                mod.FromDate = FromDateString;
                                mod.ToDate = ToDateString;
                                mod.LeaveShift = Convert.ToString(worksheet.Cells[row, 5].Value);
                                mod.Remarks = Convert.ToString(worksheet.Cells[row, 6].Value);
                                var diffdate = (ToDate - FromDate).TotalDays;
                                var totalleave = diffdate + 1;
                                double objToalLeave = Convert.ToDouble(totalleave);
                                bool leaveind = false;
                                if (mod.LeaveShift.ToLower() == "full day")
                                {
                                    leaveind = true;
                                    mod.LeaveShift = "";
                                }
                                else if (mod.LeaveShift.ToLower() == "first half" || mod.LeaveShift.ToLower() == "second half")
                                {
                                    if (mod.LeaveShift.ToLower() == "first half")
                                    {
                                        mod.LeaveShift = "F";
                                    }
                                    if (mod.LeaveShift.ToLower() == "second half")
                                    {
                                        mod.LeaveShift = "S";
                                    }
                                    if (objToalLeave > 1)
                                    {
                                        DisaplyError objerror = new DisaplyError();
                                        objerror.LeaveCode = worksheet.Cells[row, 2].Value.ToString();
                                        objerror.FromDate = FromDateString;
                                        objerror.ToDate = ToDateString;
                                        objerror.indicator = "E";
                                        objerror.errormsg = "First/Second Half availabe for only 1 day";
                                        objerror.EmployeeID = worksheet.Cells[row, 1].Value.ToString();
                                        objlist.Add(objerror);
                                    }
                                    else
                                    {
                                        leaveind = true;
                                        objToalLeave = (totalleave) - 0.5;
                                    }
                                }
                                else
                                {
                                    DisaplyError objerror = new DisaplyError();
                                    objerror.LeaveCode = worksheet.Cells[row, 2].Value.ToString();
                                    objerror.FromDate = FromDateString;
                                    objerror.ToDate = ToDateString;
                                    objerror.indicator = "E";
                                    objerror.errormsg = "Leave Indicator mismatch";
                                    objerror.EmployeeID = worksheet.Cells[row, 1].Value.ToString();
                                    objlist.Add(objerror);
                                }
                                mod.TotalAppliedLeave = Convert.ToDouble(objToalLeave);
                                mod.CreatedBy = mod.EmployeeID;
                                mod.AdminID = HttpContext.Session.GetString(Constant.AdminID);
                                if (leaveind == true)
                                {
                                    LeaveRequests.Add(mod);
                                }
                            }
                            else
                            {
                                return Json(new { returndata = false, msg = "From date Or Todate should be current year!" });
                            }
                        }
                        else
                        {
                            DisaplyError objerror = new DisaplyError();
                            objerror.LeaveCode = worksheet.Cells[row, 2].Value.ToString();
                            objerror.FromDate = FromDateString;
                            objerror.ToDate = ToDateString;
                            objerror.indicator = "E";
                            objerror.errormsg = "LeaveCode is not valid";
                            objerror.EmployeeID = worksheet.Cells[row, 1].Value.ToString();
                            objlist.Add(objerror);
                        }
                    }
                    List<string> empId = new List<string>();
                    DeptLeaveRequestErrorModel model2 = new DeptLeaveRequestErrorModel();
                    dynamic indicator = "";
                    dynamic errormsg = "";
                    foreach (var l in LeaveRequests)
                    {
                        bool error = false;
                        string esbfromdate = l.FromDate.Split("/")[1] + "/" + l.FromDate.Split("/")[0] + "/" + l.FromDate.Split("/")[2];
                        string esbtodate = l.ToDate.Split("/")[1] + "/" + l.ToDate.Split("/")[0] + "/" + l.ToDate.Split("/")[2];
                        string param = "leaves?pernr_001=" + l.EmployeeID + "&subty_006=" + l.LeaveCode + "&begda_007=" + esbfromdate + "&endda_008=" + esbtodate + "&full_first_sec=" + l.LeaveShift + "&simulation=" + "X" + "&Deletion=" + "";
                        using (HttpClient client = new HttpClient())
                        {
                            string apiUrl = _rfcconfig.GetSection("MySettings").GetSection("WebApiBaseUrl").Value + param;
                            client.BaseAddress = new Uri(apiUrl);
                            client.Timeout = TimeSpan.FromMinutes(5);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                            var responseclient = client.GetAsync(apiUrl);
                            var data = responseclient.Result;
                            if (data.IsSuccessStatusCode)
                            {
                                var readTask = data.Content.ReadAsStringAsync();
                                DisaplyError objerror = new DisaplyError();
                                LEAVEREQUESTDETAILS leaveRequestDetails = JsonConvert.DeserializeObject<LEAVEREQUESTDETAILS>(readTask.Result);
                                if (leaveRequestDetails != null && leaveRequestDetails.IT_ERROR_TEXT != null)
                                {
                                    foreach (var detail in leaveRequestDetails.IT_ERROR_TEXT)
                                    {
                                        if (detail.IND == "E")
                                        {
                                            error = true;
                                            errormsg = detail.TEXT;
                                            indicator = detail.IND;
                                        }
                                    }
                                    if (error == false)
                                    {
                                        l.Indicator = "S";
                                        l.ErrorMsg = "";
                                        objerror.indicator = "S";
                                        objerror.errormsg = "";
                                    }
                                    else
                                    {
                                        l.Indicator = "E";
                                        l.ErrorMsg = errormsg;
                                        objerror.errormsg = errormsg;
                                        objerror.indicator = "E";
                                    }
                                    objerror.LeaveCode = l.LeaveCode;
                                    objerror.EmployeeID = l.EmployeeID;
                                    objerror.FromDate = l.FromDate;
                                    objerror.ToDate = l.ToDate;
                                    objlist.Add(objerror);
                                }
                            }
                            else
                            {
                                _logger.LogError("Gateway timeout Error : EID - " + l.EmployeeID);
                            }
                        }
                    }
                    foreach (var lst in objlist)
                    {
                        if (lst.indicator != "E")
                        {
                            DeptLeaveRequestViewModel objmodel = new DeptLeaveRequestViewModel();
                            objmodel = Mapper.Map<DeptLeaveRequestViewModel>(_IDeptLeaveRequestViewService.GetEmpDetails(lst.EmployeeID));
                            if (objmodel.EmployeeName == null)
                            {
                                objmodel.EmployeeID = lst.EmployeeID;
                                GetdataSAP(objmodel);
                            }
                            if ((String.IsNullOrEmpty(objmodel.ApproverId) || objmodel.ApproverId.Trim() == "_"))
                            {
                                GetApproverAsync(objmodel);
                            }
                        }
                    }
                    for (int i = LeaveRequests.Count - 1; i >= 0; i--)
                    {
                        if (LeaveRequests[i].Indicator == "E")
                        {
                            LeaveRequests.RemoveAt(i);
                        }
                    }
                    if (LeaveRequests.Count > 0)
                    {
                        _IDeptLeaveRequestViewService.SaveConfirm(HttpContext.Session.GetString(Constant.AdminID), "No");
                        var result1 = _IDeptLeaveRequestViewService.SaveUploadedLeave(Mapper.Map<List<DeptLeaveRequestViewtServiceModel>>(LeaveRequests));
                        if (result1 != -99)
                        {
                            returndata = true;
                        }
                    }
                    else
                    {
                        returndata = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(new { returndata = false, msg = "Error occured while processing your request!" });
            }
            return Json(new { ErrorList = objlist.Where(x => x.indicator == "E"), returndata = returndata });
        }
        public IActionResult UploadSaveConfirm(string userdata)
        {
            bool result = false;
            string msg = "";
            try
            {
                DeptLeaveRequestViewModel obj = new DeptLeaveRequestViewModel();
                var AdminID = HttpContext.Session.GetString(Constant.AdminID);
                var m = _IDeptLeaveRequestViewService.SaveConfirm(AdminID, userdata);
                if (m == 0)
                {
                    result = false;
                    msg = "Error occured while processing your request.";
                }
                else
                {
                    result = true;
                    msg = "Leave Request saved successfully & sent for further approver.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = false;
                msg = "Error occured while processing your request.";
            }
            return Json(new { result = result, msg = msg });
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
            };
        }

        public async Task<IActionResult> ValidationFromSAP(string LeaveID, string FromDate, string ToDate, string LeaveShift, string employeeid)
        {
            try
            {
                bool flag = true;
                string LeaveCode = _IDeptLeaveRequestViewService.GetLeaveCode(LeaveID);
                string valid = "";
                var EmployeeID = employeeid;
                string esbfromdate = FromDate.Trim().Split("/")[1] + "/" + FromDate.Trim().Split("/")[0] + "/" + FromDate.Trim().Split("/")[2];
                string esbtodate = ToDate.Trim().Split("/")[1] + "/" + ToDate.Trim().Split("/")[0] + "/" + ToDate.Trim().Split("/")[2];
                string param = "leaves?pernr_001=" + EmployeeID + "&subty_006=" + LeaveCode + "&begda_007=" + esbfromdate + "&endda_008=" + esbtodate + "&full_first_sec=" + LeaveShift + "&simulation=" + "X" + "&Deletion=" + "";
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
                                    flag = false;
                                    valid = detail.TEXT;
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
                return Json(new { valid = valid, flag = flag });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(new { valid = "Error occured", flag = false });
            }
        }
    }

    public class DisaplyError
    {
        public string indicator { get; set; }
        public string errormsg { get; set; }
        public string LeaveCode { get; set; }
        public string EmployeeID { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}