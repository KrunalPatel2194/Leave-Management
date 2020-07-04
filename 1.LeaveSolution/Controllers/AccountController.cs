using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LeaveSolution.Models;
using LeaveSolution.BAL.ServiceModels;
using LeaveSolution.DAL.Interfaces;
using LeaveSolution.DAL.Repository;
using LeaveSolution.BAL.ServiceInterface;
using LeaveSolution.Core;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using AutoMapper;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using LeaveSolution.CustomFilter;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using LeaveSolution.RESTAPI;
using System.Threading.Tasks;

namespace LeaveSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IOptions<MySettingsModel> appSettings;
        private readonly ILoginViewService _ILoginViewService;
        private readonly IMenuServices _ImenuServices;
        private readonly ISMSUtility _smsutility;
        private readonly ILogger<AccountController> _logger;
        public readonly IConfiguration _rfcconfig;
        public AccountController(IOptions<MySettingsModel> app, ILoginViewService loginViewService, IMenuServices menuServices, ISMSUtility sMS, IConfiguration rfc, ILogger<AccountController> logger)
        {
            appSettings = app;
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
            _ILoginViewService = loginViewService;
            _ImenuServices = menuServices;
            _smsutility = sMS;
            _rfcconfig = rfc;
            _logger = logger;
        }
        public IActionResult KIOSKHome(string datafr)
        {
            try
            {
                HttpContext.Session.SetString("MifareScan", datafr);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return View();
        }
        public IActionResult ValidateMifare(string Mifareid)
        {
            bool returnmsg = false;
            string msg = "";
            try
            {
                string datafrom = HttpContext.Session.GetString("MifareScan");
                string EmployeeID = _ILoginViewService.ValidateMifareID(Mifareid, datafrom);
                if (datafrom == "NewUser")
                {
                    if (EmployeeID == null || EmployeeID == "2")
                    {
                        EmployeeID = GetdataSAPMIFARECARDID(Mifareid);
                        if (EmployeeID != null && EmployeeID != "" && EmployeeID != "00000000")
                        {
                            returnmsg = true;
                            msg = EmployeeID;
                        }
                        else if (EmployeeID == "00000000")
                        {
                            msg = "InvalidMifare";
                        }
                    }
                    else if (EmployeeID == "1")
                    {
                        msg = "Exist";
                    }
                    else if (EmployeeID != null || EmployeeID != "2" || EmployeeID != "1")
                    {
                        msg = "Exist";
                    }
                    else
                    {
                        msg = null;
                    }
                }
                else if (datafrom == "Reset")
                {
                    if (EmployeeID != null && EmployeeID != "2")
                    {
                        returnmsg = true;
                        msg = EmployeeID;
                    }
                    else if (EmployeeID == "2")
                    {
                        msg = EmployeeID;
                    }
                    else
                    {
                        msg = "Exist";
                    }
                }
                else
                {
                    msg = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(new { result = false, msg = ex.Message });
            }
            return Json(new { result = returnmsg, msg = msg });
        }
        public IActionResult Login()
        {
            //Sendsms();
            try
            {                
                HttpContext.Session.Clear();
                HttpContext.Session.SetString(Constant.KISOK, "no");
                int flag = 0;
                flag = Convert.ToInt32(TempData["Flagforpwd"]);
                if (flag == 1)
                {
                    ViewBag.Messages = new[] { new AlertModel("success", "Success!", "New password generated successfully") };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return View();
        }
        public IActionResult LoginHome()
        {
            //Sendsms();
            try
            {
                HttpContext.Session.Clear();
                int flag = 0;
                flag = Convert.ToInt32(TempData["Flagforpwd"]);
                if (flag == 1)
                {
                    ViewBag.Messages = new[] { new AlertModel("success", "Success!", "New password generated successfully") };

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel loginViewModel, string btnLoginSubmit)
        {
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("EnterOTP");
            ModelState.Remove("NewPassword");
            if (!ModelState.IsValid)
            {
                if (btnLoginSubmit == "KIOSK")
                {
                    return View("LoginKIOSKHome");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                int flag = Convert.ToInt32(TempData["Flagforpwd"]);
                if (flag == 1)
                {
                    ViewBag.Messages = new[] { new AlertModel("success", "Success!", "Password changed successfully") };
                }
                try
                {
                    loginViewModel.Password = AESEncrytDecry.DecryptStringAES(loginViewModel.Password);
                    if (loginViewModel.UserId.Trim().Length != 8)
                    {
                        loginViewModel.UserId = loginViewModel.UserId.Trim().ToString().PadLeft(8, '0');
                    }
                    var result = Mapper.Map<LoginViewModel>(_ILoginViewService.Authentication(Mapper.Map<LoginViewServiceModel>(loginViewModel)));
                    if (result.ReturnsaveValue == 1)
                    {
                        ViewBag.Messages = new[] { new AlertModel("danger", "Warning!", "Incorrect UserID/Password. Your account will get locked out after 5 failure login attempts. If you've forgot password, please click on Forgot/Reset Password.") };
                        if (btnLoginSubmit == "KIOSK")
                        {
                            return View("LoginKIOSKHome");
                        }
                        else
                        {
                            return View();
                        }
                    }
                    else if (result.ReturnsaveValue == 2)
                    {
                        TempData["flag"] = 1;
                        TempData["EmployeeID"] = loginViewModel.UserId;
                        return RedirectToAction("GenerateOTP", "Account");
                    }
                    else if (result.ReturnsaveValue == 3)
                    {
                        TempData["loginstatus"] = "concurrentlogin";
                        TempData["loginendtime"] = result.ReturnValMessg;
                        return RedirectToAction("LoginTimeout", "Home");
                    }
                    else if (result.ReturnsaveValue == 4)
                    {
                        TempData["loginstatus"] = "loginattempfailed";
                        TempData["loginendtime"] = result.ReturnValMessg;
                        return RedirectToAction("LoginTimeout", "Home");
                    }
                    else
                    {
                        string userName = result.EmployeeName;
                        string EmailID = result.EmployeeMail;
                        string EmployeeId = Convert.ToString(result.EmployeeID.Trim());
                        if (EmployeeId.Length != 8)
                        {
                            EmployeeId = EmployeeId.ToString().PadLeft(8, '0');
                        }
                        string DOB = result.EmployeeDOB;
                        string Grade = result.Grade;
                        string Dept = result.Dept;
                        string mobileno = (result.MobileNO);
                        HttpContext.Session.SetString(Constant.SessionUserName, userName);
                        HttpContext.Session.SetString(Constant.Category, result.Category.ToString());
                        HttpContext.Session.SetString(Constant.EmployeeID, EmployeeId.Trim().ToString());
                        HttpContext.Session.SetString("FirstName", userName);
                        HttpContext.Session.SetString(Constant.PersonalArea, result.PersonalArea);
                        HttpContext.Session.SetString(Constant.PersonalSubArea, result.PersonalSubArea);
                        HttpContext.Session.SetString(Constant.MobileNo, mobileno.ToString());
                        ViewBag.UserName = HttpContext.Session.GetString(Constant.SessionUserName);
                        HttpContext.Session.SetString(Constant.SessionModulName, "Employee");
                        string ModuleName = HttpContext.Session.GetString(Constant.SessionModulName);
                        var EmployeeID = HttpContext.Session.GetString(Constant.EmployeeID);
                        GetdataSAP(EmployeeID);
                        var MenuItem = _ImenuServices.GetMenu(ModuleName, EmployeeId);
                        MenuviewModel model = new MenuviewModel();
                        model.MenuModels = Mapper.Map<List<MenuModel>>(MenuItem);
                        HttpContext.Session.SetObjectAsJson<MenuviewModel>(Constant.Menu, model);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    ViewBag.Messages = new[] { new AlertModel("danger", "Warning!", "Please try after some time") };
                    if (btnLoginSubmit == "KIOSK")
                    {
                        return View("LoginKIOSKHome");
                    }
                    else
                    {
                        return View();
                    }
                }
                return RedirectToAction("LeaveDashboard", "LeaveDashboard");
            }
        }
        public IActionResult SignOut()
        {
            string chkloginpage = "", approverlogin = "";
            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString(Constant.EmployeeID)))
                {
                    _ILoginViewService.DeleteUser(HttpContext.Session.GetString(Constant.EmployeeID));
                }
                LoginViewModel objModel = new LoginViewModel();
                chkloginpage = HttpContext.Session.GetString(Constant.KISOK);
                approverlogin = HttpContext.Session.GetString(Constant.SessionModulName);
                HttpContext.Session.SetString(Constant.SessionUserName, "");
                HttpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            if (chkloginpage == "yes")
            {
                return RedirectToAction("LoginKIOSKHome", "Account");
            }
            else
            {
                if (approverlogin == "Approver")
                {
                    return RedirectToAction("ApproverLogin", "ApproverLogin");
                }
                else if (approverlogin == "Employee")
                {
                    return RedirectToAction("Login", "Account");
                }
                return RedirectToAction("LoginHome", "Account");
            }
        }
        public IActionResult NewUser()
        {
            if (HttpContext.Session.GetString(Constant.ValidUser) == "valid")
            {
                return View();
            }
            else
            {
                return RedirectToAction("AccessDenied", "Home");
            }
        }
        public IActionResult LoginKIOSKHome()
        {
            try
            {
                HttpContext.Session.Clear();
                HttpContext.Session.SetString(Constant.KISOK, "yes");
                int flag = 0;
                flag = Convert.ToInt32(TempData["Flagforpwd"]);
                if (flag == 1)
                {
                    ViewBag.Messages = new[] { new AlertModel("success", "Success!", "New password generated successfully") };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return View();
        }

        public ActionResult ActionGetdataSAP(string Employeeid)
        {
            string res;
            try
            {
                res = GetdataSAP(Employeeid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                res = "NoEmployeeExist";
            }
            return Json(res);
        }

        public string GetdataSAP(string employeeid)
        {
            string returnresult = "";
            try
            {
                var EmployeeID1 = employeeid.Trim();// HttpContext.Session.GetString(Constant.EmployeeID);
                string param = "personal/" + EmployeeID1;

                //Zftm0008Response empDetails = await ApiClientFactory.Instance.GetEmpDetails(param);
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
                                if (string.IsNullOrEmpty(detail.PERS_AREA) || string.IsNullOrEmpty(detail.Name))
                                {
                                    return "NoEmployeeExist";
                                }
                                if (employeeid.Trim().Length != 8)
                                {
                                    employeeid = employeeid.Trim().ToString().PadLeft(8, '0');
                                }
                                DateTime DateOfBirth = Convert.ToDateTime(detail.Birthdate);
                                detail.Birthdate = DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                                detail.EmployeeID = employeeid.Trim();
                                if (_rfcconfig.GetSection("StaticValue").GetSection("EMobNo").Value != "")
                                {
                                    detail.Userid = _rfcconfig.GetSection("StaticValue").GetSection("EMobNo").Value;
                                }

                                var returndata1 = _ILoginViewService.SaveFromSAP(Mapper.Map<EmployeeServicemodel>(empDetails));
                                if (returndata1 == 1)
                                {
                                    returnresult = "Valid";
                                }
                                else
                                {
                                    returnresult = "NotValid";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                returnresult = "NoEmployeeExist";
            }
            return returnresult;
        }
        [HttpPost]
        public IActionResult NewUser(LoginViewModel model)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("Password");
            ModelState.Remove("EnterOTP");
            if (!ModelState.IsValid) return View();
            model.EmployeeID = HttpContext.Session.GetString(Constant.EmployeeID);
            string eid = model.EmployeeID;
            model.UserId = model.EmployeeID.Trim();
            if (model.UserId.Length != 8)
            {
                model.UserId = model.UserId.ToString().PadLeft(8, '0');
                // model.UserId = model.EmployeeID.Trim();
            }
            model.Password = model.ConfirmPassword;

            try
            {
                if (model.EmployeeID != null)
                {
                    int Savenewuser = _ILoginViewService.SaveNewUser(Mapper.Map<LoginViewServiceModel>(model));
                    _ILoginViewService.DeleteUser(eid);
                    if (Savenewuser == 1)
                    {
                        TempData["Flagforpwd"] = 1;
                        //Getting data from SAP
                        GetdataSAP(model.EmployeeID.Trim());
                        //////End of getting data from sap
                        ///
                        string chkloginpage = "";
                        chkloginpage = HttpContext.Session.GetString(Constant.KISOK);
                        if (chkloginpage == "no")
                        {
                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {
                            return RedirectToAction("LoginKIOSKHome", "Account");
                        }
                    }
                    else if (Savenewuser == 2)
                    {
                        ViewBag.Messages = new[] { new AlertModel("danger", "Alert!", "New password should not be same as  previous three password") };
                        //return View(model);
                    }

                    else
                    {
                        ViewBag.Messages = new[] { new AlertModel("danger", "Warning!", "Error during generate password !!Please Try again") };
                        //return View(model);
                    }
                }
                else
                {
                    ViewBag.Messages = new[] { new AlertModel("danger", "Warning!", "Error during generate password !!Please Try again") };
                    //return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                ViewBag.Messages = new[] { new AlertModel("danger", "Warning!", "Error during generate password !!Please Try again") };
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult OtpMatch(string enteredotp, string empid, string dob, string datafr)
        {
            int chkdata = 0;
            int chk = 0;
            if (empid.Trim().Length != 8)
            {
                empid = empid.Trim().ToString().PadLeft(8, '0');
            }
            HttpContext.Session.SetString(Constant.EmployeeID, empid);
            try
            {
                string fromdata = "";
                string tempdt = Convert.ToString(HttpContext.Session.GetString(Constant.DataFrom));
                if (tempdt != null && tempdt != "")
                {
                    fromdata = tempdt;
                }
                else
                {
                    fromdata = datafr;
                }
                DateTime dt;
                DateTime.TryParseExact(dob, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
                string otndecrypt = AESEncrytDecry.DecryptStringAES(enteredotp.ToString());
                HttpContext.Session.SetString(Constant.ValidUser, "invalid");
                chk = _ILoginViewService.OTPmatch(Convert.ToInt32(otndecrypt), empid, dt, fromdata);
                if (chk == 0)
                {
                    //new user
                    HttpContext.Session.SetString(Constant.ValidUser, "valid");
                    chkdata = 1;
                }
                else if (chk == 2)
                {
                    //generate otp
                    chkdata = 2;
                    TempData["flagfromotpMatch"] = 1;
                }
                else if (chk == 1)
                {
                    chkdata = 0;
                }
                else if (chk == 4)
                {
                    chkdata = 4;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                ViewBag.Messages = new[] { new AlertModel("danger", "Warning!", "Entered Credentials Did Not Match") };
            }
            return Json(chkdata);
        }
        public IActionResult ValidateDOBForKIOSK(string empid, string dob)
        {
            int chk = 0;
            try
            {
                if (empid.Trim().Length != 8)
                {
                    empid = empid.Trim().ToString().PadLeft(8, '0');
                }
                HttpContext.Session.SetString(Constant.EmployeeID, empid);
                DateTime dateTime = new DateTime();
                dateTime = Convert.ToDateTime(DateTime.ParseExact(dob, "dd-MM-yyyy", CultureInfo.InvariantCulture));
                var returndata = _ILoginViewService.DOBMatchforKIOSK(empid, dateTime);
                if (returndata == 1)
                {
                    HttpContext.Session.SetString(Constant.ValidUser, "valid");
                    chk = 1;
                }
                else
                {
                    HttpContext.Session.SetString(Constant.ValidUser, "invalid");
                    chk = 0;
                }
            }
            catch (Exception ex)
            {
                chk = 0;
                HttpContext.Session.SetString(Constant.ValidUser, "invalid");
                _logger.LogError(ex, ex.Message);
                ViewBag.Messages = new[] { new AlertModel("danger", "Warning!", "Entered Date Of Birth Did Not Match") };

            }
            return Json(chk);
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult GenerateOTPLayer(string datafrom)
        {
            HttpContext.Session.SetString(Constant.DataFrom, datafrom);
            return RedirectToAction("GenerateOTP", "Account");
        }

        public IActionResult GenerateOTP(LoginViewModel model)
        {
            try
            {
                int flagfromlogin = Convert.ToInt32(TempData["flag"]);
                int flagfromotpmatch = Convert.ToInt32(TempData["flagfromotpMatch"]);
                string employeeid = Convert.ToString(TempData["EmployeeID"]);
                if (flagfromlogin == 1)
                {
                    model.EmployeeID = employeeid.ToString();
                    ViewBag.Messages = new[] { new AlertModel("danger", "Warning!", "Your Password is expired!!!! Please Generate New Pasword") };
                    TempData["flag"] = 0;
                    return View(model);
                }
                if (flagfromotpmatch == 1)
                {
                    model.EmployeeID = HttpContext.Session.GetString(Constant.EmployeeID);
                    ViewBag.Messages = new[] { new AlertModel("danger", "Warning!", "Your OTP is expired!!!! Please Generate New OTP") };
                    TempData["flagfromotpMatch"] = 0;
                    return View(model);
                }
                else
                {
                    flagfromlogin = 0;
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return View();
            }
        }
        public string GetdataSAPMIFARECARDID(string employeeMifareCard)
        {
            string returnresult = "";
            try
            {
                string param = "empId/" + employeeMifareCard;
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
                        EmployeId PersonNumber = JsonConvert.DeserializeObject<EmployeId>(readTask.Result);
                        if (PersonNumber != null && !string.IsNullOrEmpty(PersonNumber.PERSONNAL_NUMBER))
                        {
                            returnresult = PersonNumber.PERSONNAL_NUMBER;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                returnresult = null;
            }
            return returnresult;
        }
        // 
        public IActionResult GenerateOTPForReset(string datafrom)
        {
            HttpContext.Session.SetString(Constant.DataFrom, datafrom);
            bool data = true;
            return Json(data);
        }
        [HttpPost]
        public JsonResult GetMobileno(string employeeid, string datafrom)
        {
            try
            {
                string fromdata = "";
                string tempdt = Convert.ToString(HttpContext.Session.GetString(Constant.DataFrom));
                if (tempdt != null && tempdt != "")
                {
                    fromdata = tempdt;
                }
                else
                {
                    fromdata = datafrom;
                }

                ////Getting data from SAP
                string result = GetdataSAP(employeeid);
                if (result == "NoEmployeeExist")
                {
                    return Json(2);
                }
                else if (result == "NotValid")
                {
                    return Json(3);
                }
                else
                {
                    if (employeeid.Trim().Length != 8)
                    {
                        employeeid = employeeid.Trim().ToString().PadLeft(8, '0');
                    }
                    ////End of getting data from sap
                    long mobileno = _ILoginViewService.GetMobileno(employeeid, fromdata);
                    if (mobileno == 2)
                    {
                        return Json(4);
                    }
                    else if (mobileno != 2 && mobileno != 0)
                    {
                        TempData["EmpMoNumber"] = mobileno.ToString();
                        return Json(mobileno);
                    }
                    else
                    {
                        return Json(0);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(0);
            }
        }
        [HttpPost]
        public JsonResult GenerateOTPbyid(string employeeid)
        {
            var employeeid1 = employeeid;
            try
            {
                if (employeeid.Trim().Length != 8)
                {
                    employeeid = employeeid.Trim().ToString().PadLeft(8, '0');
                }
                HttpContext.Session.SetString(Constant.EmployeeID, employeeid.ToString());
                long mobno = Convert.ToInt64(TempData["EmpMoNumber"]);
                long otpsmssent = _ILoginViewService.GenerateOtp(employeeid, mobno);
                if (otpsmssent != 0)
                {
                    return Json(otpsmssent);
                }
                else
                {
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                ViewBag.Messages = new[] { new AlertModel("danger", "Warning!", ex.Message) };
                return Json(string.Empty);
            }
        }

        public ActionResult ExtendSession()
        {
            try
            {
                if (HttpContext.Session.GetString(Constant.SessionUserName) != null)
                {
                    _ILoginViewService.ExtendSession(HttpContext.Session.GetString(Constant.EmployeeID));
                    return Json(string.Empty);
                }
                else
                {
                    return RedirectToAction("SignOut", "Account");
                }
            }
            catch (Exception)
            {
                return Json(string.Empty);
            }
        }
    }
}