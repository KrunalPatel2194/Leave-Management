using System;
using System.DirectoryServices;
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
using Microsoft.Extensions.Configuration;

namespace LeaveSolution.Controllers
{
    public class ApproverLoginController : Controller
    {
        private readonly ILogger<ApproverLoginController> _logger;
        private readonly IMenuServices _imenuservice;
        private readonly ILeaveApprovalService _Ileaveapprovalservice;
        public readonly IConfiguration _rfcconfig;
        private readonly ILoginViewService _ILoginViewService;
        public ApproverLoginController(ILoginViewService loginViewService, IMenuServices menuServices, ILeaveApprovalService leaveApprovalService, ILogger<ApproverLoginController> logger, IConfiguration rfc)
        {
            _Ileaveapprovalservice = leaveApprovalService;
            _imenuservice = menuServices;
            _rfcconfig = rfc;
            _logger = logger;
            _ILoginViewService = loginViewService;
        }

        public IActionResult ApproverLogin()
        {
            try
            {
                HttpContext.Session.SetString(Constant.KISOK, "no");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult ApproverLogin(ApproverLogin loginViewModel)
        {
            if (!ModelState.IsValid) return View();
            try
            {
                var empid = "";
                loginViewModel.Password = AESEncrytDecry.DecryptStringAES(loginViewModel.Password);
                var result = AdLogin(loginViewModel.UserId, loginViewModel.Password, loginViewModel.MailID, loginViewModel.Mobileno, out empid, out string userName, out string firstname);
                if (!result)
                {
                    ViewBag.Messages = new[] { new AlertModel("danger", "Warning!", "Entered Credentials Did Not Match") };
                    return View();
                }
                else
                {
                    var EmpID = loginViewModel.UserId;
                    var employeeid = empid;
                    if (employeeid.Length != 8)
                    {
                        employeeid = employeeid.ToString().PadLeft(8, '0');
                    }
                    Tuple<string, int> Result = _Ileaveapprovalservice.InsertEmpId(employeeid);
                    if (Result.Item2 == 0)
                    {
                        TempData["loginendtime"] = Result.Item1;
                        return RedirectToAction("LoginTimeout", "Home");
                    }
                    HttpContext.Session.SetString(Constant.ApproverID, employeeid);
                    HttpContext.Session.SetString(Constant.AdminID, employeeid);
                    HttpContext.Session.SetString(Constant.SessionUserName, userName);
                    HttpContext.Session.SetString(Constant.SessionModulName, "Approver");
                    HttpContext.Session.SetString("FirstName", firstname);
                    ViewBag.UserName = HttpContext.Session.GetString(Constant.SessionUserName);
                    LeaveApprovalModelViewModel approverLogin = new LeaveApprovalModelViewModel();
                    string ModuleName = HttpContext.Session.GetString(Constant.SessionModulName);
                    var MenuItem = _imenuservice.GetMenu(ModuleName, employeeid);
                    MenuviewModel model = new MenuviewModel();
                    model.MenuModels = Mapper.Map<List<MenuModel>>(MenuItem);
                    HttpContext.Session.SetObjectAsJson<MenuviewModel>(Constant.Menu, model);
                    return RedirectToAction("ApproverDashboard", "LeaveApprover");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                ViewBag.Messages = new[] { new AlertModel("danger", "Warning!", "Entered Credentials Did Not Match") };
                return View();
            }
        }

        public IActionResult SignOut()
        {
            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString(Constant.ApproverID)))
                    _ILoginViewService.DeleteUser(HttpContext.Session.GetString(Constant.ApproverID));
                LoginViewModel objModel = new LoginViewModel();
                HttpContext.Session.SetString(Constant.SessionUserName, "");
                HttpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return RedirectToAction("ApproverLogin", "ApproverLogin");
        }

        public bool AdLogin(string usr, string pwd, string mail, long mobno, out string empid, out string UserName, out string firstname)
        {
            bool authenticated = false;
            string srvr = _rfcconfig.GetSection("LDAPString").GetSection("LDAP").Value;
            try
            {
                string domain = _rfcconfig.GetSection("LDAPString").GetSection("DOMAIN").Value;
                string strDomainAndUsername = domain + @"\" + usr;
                DirectoryEntry objDirEntry = new DirectoryEntry(srvr, strDomainAndUsername, pwd);
                DirectorySearcher search = new DirectorySearcher(objDirEntry);
                search.Filter = "(SAMAccountName=" + usr + ")";
                search.PropertiesToLoad.Add("EmployeeID");
                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("givenName");
                SearchResult srchResult = search.FindOne();
                if (null == srchResult)
                {
                    empid = "";
                    UserName = "";
                    firstname = "";
                    return false;
                }
                empid = (String)srchResult.Properties["EmployeeID"][0];
                UserName = srchResult.Properties["cn"][0].ToString();
                firstname = (String)srchResult.Properties["givenName"][0];
                if (null != srchResult)
                {
                    authenticated = true;
                    return authenticated;
                }
                return authenticated;
            }
            catch (DirectoryServicesCOMException ex)
            {
                _logger.LogError(ex, ex.Message);
                empid = "";
                UserName = "";
                firstname = "";
                return authenticated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                empid = "";
                UserName = ""; firstname = "";
                return authenticated;
            }
        }
    }
}