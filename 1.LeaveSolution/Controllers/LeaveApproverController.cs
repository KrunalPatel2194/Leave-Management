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
using LeaveSolution.DAL.Interfaces;
using LeaveSolution.DAL.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using LeaveSolution.CustomFilter;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Globalization;
using LeaveSolution.RESTAPI;

namespace LeaveSolution.Controllers
{
   // [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class LeaveApproverController : Controller
    {
        private readonly ILogger<LeaveApproverController> _logger;
        private readonly ILeaveApprovalService _iLeaveapprovalService;
        private readonly ILeaveRequestViewService _ILeaveRequestViewService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ISMSUtility _SMSUtility;
        SMSData smsd = new SMSData();
        public readonly IConfiguration _rfcconfig;
        public LeaveApproverController(ILeaveRequestViewService leaveRequestViewService, IConfiguration rfc, ILeaveApprovalService leaveApprovalService, ISMSUtility SMSUtility, IHostingEnvironment hostingEnvironment, ILogger<LeaveApproverController> logger)
        {
            _ILeaveRequestViewService = leaveRequestViewService;
            _rfcconfig = rfc;
            _iLeaveapprovalService = leaveApprovalService;
            _SMSUtility = SMSUtility;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }
        public JsonResult GetApproverList(string employeeID, string leaverequestid)
        {
            try
            {
                List<AlternateApproverModel> autocompempid = GetApprover(employeeID, "A");
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

        [Authentication]
        public IActionResult LeaveApproval()
        {
            try
            {
                LeaveApprovalModelViewModel modeldata;
                string EmployeeId = HttpContext.Session.GetString(Constant.ApproverID);
                string Approverid = Convert.ToString(EmployeeId);
                var EmployeeName = HttpContext.Session.GetString(Constant.SessionUserName);
                modeldata = Mapper.Map<LeaveApprovalModelViewModel>(_iLeaveapprovalService.GetLeaveRequestForApproval(Approverid));
                return View(modeldata);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                ViewBag.Messages = new[] { new AlertModel("danger", "Warning!", ex.Message) };
                return Json(string.Empty);
            }
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult LeaveApproval(string LeaveRequestID, string status)
        {
            string returnmsg = "";
            try
            {
                LeaveRequestID = LeaveRequestID.Replace(",,", "");
                LeaveApprovalModelViewModel model = new LeaveApprovalModelViewModel();
                model = Mapper.Map<LeaveApprovalModelViewModel>(_iLeaveapprovalService.GetMulitpleEmployeeDetails(LeaveRequestID));
                LeaveApprovalModelViewModel model2 = new LeaveApprovalModelViewModel();
                model2 = model;
                returnmsg = SaveDataToSAP(model, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                ViewBag.Messages = new[] { new AlertModel("danger", "Warning!", ex.Message) };
            }
            return Json(new { valid = returnmsg.Split('~')[0], msg = returnmsg.Split('~')[1] });
        }

        [Authentication]
        public IActionResult ApproverDashboard()
        {
            LeaveApprovalModelViewModel modeldata;
            var EmployeeId = HttpContext.Session.GetString(Constant.EmployeeID);
            string Approverid = HttpContext.Session.GetString(Constant.AdminID);//Convert.ToString(EmployeeId);
            var EmployeeName = HttpContext.Session.GetString(Constant.SessionUserName);
            modeldata = Mapper.Map<LeaveApprovalModelViewModel>(_iLeaveapprovalService.GetLeaveApprovalsForDashboard(Approverid));
            List<LeaveApprovalModelViewModel> l = new List<LeaveApprovalModelViewModel>();
            modeldata.leavePendings = new List<LeaveApprovalModel>();
            modeldata.leaveApprovedforview = new List<LeaveApprovalModel>();
            for (int i = 0; i < modeldata.leaveApprovals.Count; i++)
            {
                if (modeldata.leaveApprovals[i].Status == "Pending" || modeldata.leaveApprovals[i].Status == "Pending Withdrawal")
                {
                    modeldata.leavePendings.Add(modeldata.leaveApprovals[i]);
                }
                else if (modeldata.leaveApprovals[i].Status == "Approved" || modeldata.leaveApprovals[i].Status == "Withdrawal Rejected")
                {
                    DateTime date = Convert.ToDateTime(modeldata.leaveApprovals[i].ApprovedDate);
                    modeldata.leaveApprovals[i].ApprovedDate = date.ToString("dd/MM/yyyy");
                    modeldata.leaveApprovedforview.Add(modeldata.leaveApprovals[i]);
                }

            }
            return View(modeldata);
        }

        public string SaveDataToSAP(LeaveApprovalModelViewModel modellist, string status)
        {
            bool valid = false;
            string msg = "";
            try
            {
                string Mobileno = "";
                string MobilenoR = "";
                string Approverid = (HttpContext.Session.GetString(Constant.ApproverID));
                dynamic indicator = "";
                dynamic errormsg = "";
                if (status == "Approved")
                {
                    try
                    {
                        foreach (var model in modellist.leaveApprovals)
                        {
                            bool error = false;
                            string deletion = "";
                            if (model.OldStatus == "Pending Withdrawal")
                            {
                                status = "Withdrawal";
                                deletion = "X";
                            }
                            else
                            {
                                deletion = "";
                            }
                            model.FromDateString = model.FromDateString.Replace('-', '/');
                            model.ToDateString = model.ToDateString.Replace('-', '/');
                            string esbfromdate = model.FromDateString.Split("/")[1] + "/" + model.FromDateString.Split("/")[0] + "/" + model.FromDateString.Split("/")[2];
                            string esbtodate = model.ToDateString.Split("/")[1] + "/" + model.ToDateString.Split("/")[0] + "/" + model.ToDateString.Split("/")[2];

                            string param = "leaves?pernr_001=" + model.EmployeeID + "&subty_006="
                                + model.LeaveCode + "&begda_007=" + esbfromdate + "&endda_008=" + esbtodate + "&full_first_sec=" + model.LeaveShift + "&simulation=" + "" + "&Deletion=" + deletion;
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
                                                error = true;
                                                errormsg = detail.TEXT;
                                                indicator = detail.IND;
                                            }
                                        }
                                        if (error == false)
                                        {
                                            errormsg = "Successfully updated in SAP";
                                            indicator = "S";
                                        }
                                    }
                                }
                                else
                                {
                                    _logger.LogError("Gateway Timeout Error" + model.EmployeeID);
                                }
                            }
                            model.ErrorMsg = errormsg;
                            model.Indicator = indicator;
                            model.ApproverId = Approverid;
                            model.Status = error == true ? "Error" : status;
                        }
                        Tuple<string, int> result = _iLeaveapprovalService.SaveLeaveApproval(Mapper.Map<LeaveApprovalServiceViewModel>(modellist));
                        LeaveApprovalModel leavemodel = new LeaveApprovalModel();
                        if (result.Item2 != -99)
                        {
                            if (status == "Approved" || status == "Withdrawal")
                            {
                                foreach (var model in modellist.leaveApprovals)
                                {
                                    // Mobileno = model.empmobileno;//mobileno add
                                    #region Message for Approved record to Employee
                                    if (result.Item2 != -99)
                                    {
                                        var msgTextapprover = "";
                                        if (model.OldStatus == "Pending Withdrawal")
                                        {

                                            msgTextapprover = "Your Subordinate wants to cancel/ withdraw below leave/attendance requests which were approved in the system." + Environment.NewLine +
                                                               "Please approve / reject the request accordingly" + Environment.NewLine;
                                            Mobileno = model.ApproverMobNo;// "8149102368";
                                        }
                                        else
                                        {
                                            msgTextapprover = "Your below mentioned leaves/attendance requests submitted have been approved." + Environment.NewLine;
                                            Mobileno = model.empmobileno;//mobileno add "8149102368";//
                                        }
                                        var mesg = msgTextapprover + Environment.NewLine +

                                      "Employee No:-" + model.EmployeeID + Environment.NewLine +
                                      "Request:-" + model.LeaveCategory + Environment.NewLine +
                                      "From Date:-" + model.FromDateString + Environment.NewLine +
                                      "To Date:-" + model.ToDateString + Environment.NewLine +
                                      "Days:-" + model.TotalLeaves + " days";
                                        try
                                        {
                                            _SMSUtility.SendSMS(Mobileno, mesg);
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                        if (result.Item2 != -99)
                        {
                            if (status == "Approved" || status == "Withdrawal")
                            {
                                msg = "LeaveRequest has been Approved  successfully.... ";
                            }
                            else
                            {
                                msg = "LeaveRequest has been Rejected  successfully.... ";
                            }
                            valid = true;
                        }
                        else
                        {
                            msg = "Error Occured while processing the request....";
                            valid = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        valid = false;
                        msg = "Error Occured while processing the request....";
                        _logger.LogError(ex, ex.Message);
                    }
                }
                //for reject code
                else
                {
                    foreach (var model in modellist.leaveApprovals)
                    {
                        model.ErrorMsg = "";
                        model.Indicator = "W";
                        model.ApproverId = Approverid;
                        if (status == "Rejected" && model.OldStatus == "Pending Withdrawal")
                        {
                            model.Status = "Withdrawal Rejected";
                        }
                        else
                        {
                            model.Status = status;
                        }
                    }
                    Tuple<string, int> result = _iLeaveapprovalService.SaveLeaveApproval(Mapper.Map<LeaveApprovalServiceViewModel>(modellist));

                    LeaveApprovalModel leavemodel = new LeaveApprovalModel();

                    if (result.Item2 != -99)
                    {
                        foreach (var model in modellist.leaveApprovals)
                        {
                            if (result.Item2 != -99)
                            {
                                MobilenoR = model.empmobileno;//mobileno add
                                #region Message for Reject record
                                var msgTextapprover =
                                     "Your below mentioned leaves / attendance requests have been rejected." + Environment.NewLine +
                                     "Employee No:-" + model.EmployeeID + Environment.NewLine +
                                     "Request:-" + model.LeaveCategory + Environment.NewLine +
                                     "From Date:-" + model.FromDateString + Environment.NewLine +
                                     "To Date:-" + model.ToDateString + Environment.NewLine +
                                     "Days:-" + model.TotalLeaves + " days";
                                try
                                {
                                    _SMSUtility.SendSMS(MobilenoR.ToString(), msgTextapprover);
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }

                        #endregion
                        msg = "LeaveRequest has been Rejected  successfully.... ";
                        valid = true;
                    }
                    else
                    {
                        msg = "Error Occured while processing the request....";
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                msg = "Error Occured while processing the request....";
                _logger.LogError(ex, ex.Message);
            }
            return valid + "~" + msg;
        }
        public ActionResult DownloadAttachment(string Download)
        {
            string wwwrootPath = _hostingEnvironment.WebRootPath;
            string directory = System.IO.Directory.GetCurrentDirectory();
            FileInfo file = new FileInfo(Path.Combine(wwwrootPath, Download));
            FileInfo fileinfo = new FileInfo(Path.Combine(Download));
            byte[] fileBytes = System.IO.File.ReadAllBytes(file.FullName);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name);
        }
        public JsonResult SaveAlternateApprover(LeaveApprovalModel model)
        {
            try
            {
                string EmpID = model.EmployeeID;
                string LeaveRequestID = model.LeaveRequestId.ToString();
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
                    z.DETAILS[0].PERNR_SUP = model.ApproverId.Trim();//model.Select(x=>x.ApproverId).ToString();
                    z.DETAILS[0].CNAME_SUP = model.ApproverName.Trim();//model.leaveApprovals.Select(x => x.ApproverName).ToString();
                    z.DETAILS[0].CELL_SUP = model.ApproverMobNo.Trim(); //model.leaveApprovals.Select(x => x.ApproverMobNo).ToString();
                    z.DETAILS[0].EMAIL_SUP = model.ApproverMailID.Trim(); //model.leaveApprovals.Select(x => x.ApproverMailID).ToString();
                    z.DETAILS[0].EMAILID_HOD = model.HODMailID.Trim();// model.leaveApprovals.Select(x => x.HODMailID).ToString();
                }
                z.DETAILS[0].PERNR_HOD = model.HODID; //model.leaveApprovals.Select(x => x.HODID).ToString();
                z.DETAILS[0].CNAME_HOD = model.HODName;// model.leaveApprovals.Select(x => x.HODName).ToString();

                z.DETAILS[0].EmployeeID = EmpID;
                z.DETAILS[0].LeaveRequestID = LeaveRequestID.ToString();
                var datareturn = _ILeaveRequestViewService.SaveApproverFromSAP(Mapper.Map<ApproverServiceModel>(z));
                var msg = "saved";
                return Json(new { result = msg, msg = msg });
                //return Json(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(string.Empty);
            }
        }
    }
}