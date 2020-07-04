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
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Data;
using System.IO;
using OfficeOpenXml;
using Microsoft.Extensions.Options;
using LeaveSolution.RESTAPI;
using System.Globalization;

namespace LeaveSolution.Controllers
{
    public class LeaveHistoryController : Controller
    {
        private readonly ILeaveRequestViewService _ILeaveRequestViewService;
        private readonly ILogger<LeaveHistoryController> _logger;
        private readonly ILeaveHistoryService _ILeaveHistoryService;
        private readonly IOptions<MySettingsModel> appSettings;
        public readonly IConfiguration _rfcconfig;
        public LeaveHistoryController(IOptions<MySettingsModel> app, ILeaveHistoryService leaveHistoryService, ILeaveRequestViewService leaveRequestViewService, ILogger<LeaveHistoryController> logger, IConfiguration rfc)
        {
            appSettings = app;
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
            _ILeaveHistoryService = leaveHistoryService;
            _ILeaveRequestViewService = leaveRequestViewService;
            _logger = logger;
            _rfcconfig = rfc;
        }
        [HttpGet]
        [Authentication]
        public IActionResult LeaveHistory()
        {
            return View();
        }

        public IActionResult GetApproverName()
        {
            string[] approverdetails = new string[2];
            try
            {
                var empId = HttpContext.Session.GetString(Constant.EmployeeID);
                approverdetails = _ILeaveRequestViewService.GetApproverEmail(empId);
            }
            catch (Exception ex)
            {
                approverdetails = null;
                _logger.LogError(ex, ex.Message);
            }
            return Json(new { data = approverdetails });
        }

        public JsonResult GetHolidayListByYear(string Year, string download,string email=null)
        {
            LeaveHistoryModelViewModel objModel = new LeaveHistoryModelViewModel();
            LeaveHistoryModelViewModel objNewModel = new LeaveHistoryModelViewModel();
            objNewModel.LeaveHistory = new List<LeaveHistoryModelViewModel>();
            try
            {
                var objEmployeeId = HttpContext.Session.GetString(Constant.EmployeeID);
                string year = Year;
                var EmployeeName = HttpContext.Session.GetString(Constant.SessionUserName);
                objModel = Mapper.Map<LeaveHistoryModelViewModel>(_ILeaveHistoryService.GetLeaveHistoryDetails(objEmployeeId, year));
                string param = "LeaveHistory?pernr=" + objEmployeeId + "&year=" + year;
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
                        if (QuotaOverviewDetails != null && QuotaOverviewDetails.OUTPUT != null)
                        {
                            foreach (var item in QuotaOverviewDetails.OUTPUT)
                            {
                                LeaveHistoryModelViewModel objTempModel = new LeaveHistoryModelViewModel();
                                objTempModel.LeaveCode = Convert.ToString(item.LEAVE_CODE);                                
                                objTempModel.LeaveAppliedDate = "<span>" + item.APPLIED_DATE.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "</span>" + item.APPLIED_DATE.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Replace('-','/');
                                objTempModel.FromDate = item.BEGDA.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Replace('-', '/'); ;
                                objTempModel.ToDate = item.ENDDA.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Replace('-', '/'); ;
                                objTempModel.TotalLeaves = string.IsNullOrEmpty(item.TOT_LEAVES) ? 0 : Convert.ToDouble(item.TOT_LEAVES);
                                objTempModel.Status = Convert.ToString(item.STATUS);
                                objTempModel.LeaveType = Convert.ToString(item.LEAVE_TYPE);
                                objTempModel.Indicator = item.SAP_STATUS;
                                if (item.SAP_STATUS == "S")
                                {
                                    objTempModel.ErrorMsg = "SAP updated successfully";
                                }
                                else
                                {
                                    objTempModel.ErrorMsg = item.ERROR_MESSAGE;
                                }
                                objNewModel.LeaveHistory.Add(objTempModel);
                            }
                        }
                    }
                }
                string PA = HttpContext.Session.GetString(Constant.PersonalArea);
                string PSA = HttpContext.Session.GetString(Constant.PersonalSubArea);
                string Category = HttpContext.Session.GetString(Constant.Category);
                var LeaveCategoryList = _ILeaveRequestViewService.GetLeaveRequest(PA, PSA, Category);
                LeaveCategoryList.RemoveAt(0);
                LeaveHistoryModelViewModel tempmodeladd = new LeaveHistoryModelViewModel();
                tempmodeladd.LeaveHistory = new List<LeaveHistoryModelViewModel>();
                foreach (var subitem in objNewModel.LeaveHistory)
                {
                    var urlNameExists = LeaveCategoryList.Any(s => s.Value.Split('~')[2] == subitem.LeaveCode);
                    if (urlNameExists)
                        subitem.LeaveCategory = LeaveCategoryList.FirstOrDefault(s => s.Value.Split('~')[2] == subitem.LeaveCode).Text;
                    else
                    {
                        subitem.LeaveCategory = subitem.LeaveCode;
                    }
                    LeaveHistoryModelViewModel model = objModel.LeaveHistory.FirstOrDefault(t =>
                    t.LeaveCode == subitem.LeaveCode &&
                                  t.LeaveType == subitem.LeaveType &&
                                  t.FromDate == subitem.FromDate &&
                                  t.ToDate == subitem.ToDate);
                    if (model != null)
                    {
                        //record exists
                        subitem.ApproverName = model.ApproverName;
                        subitem.LeaveAppliedDate = model.LeaveAppliedDate;
                        objModel.LeaveHistory.Remove(model);
                    }
                }

                foreach (var subitem in objModel.LeaveHistory)
                {
                    string date =  subitem.LeaveAppliedDate.Replace('-', '/');
                    DateTime dtdate = DateTime.ParseExact(date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    subitem.LeaveAppliedDate = "<span>" + dtdate.ToString("yyyyMMdd") + "</span>" + dtdate.ToString("dd/MM/yyyy").Replace('-', '/');
                    subitem.FromDate = subitem.FromDate.Replace('-', '/');
                    subitem.ToDate = subitem.ToDate.Replace('-', '/');
                    objNewModel.LeaveHistory.Add(subitem);
                }
                if (download == "yes")
                {
                    string empname = HttpContext.Session.GetString("FirstName");
                    SendEmail(objNewModel.LeaveHistory.OrderByDescending(a => a.LeaveAppliedDate).ToList(), empname, email);
                }
                return Json(new { result = true, data = objNewModel });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Json(new { result = false, data = objNewModel });
            }
        }

        public void SendEmail(List<LeaveHistoryModelViewModel> objmodel, string empname, string emailaddress)
        {
            string Host = _rfcconfig.GetSection("EmailUtility").GetSection("Host").Value;
            int Port = Convert.ToInt32(_rfcconfig.GetSection("EmailUtility").GetSection("Port").Value);
            string EmailFrom = _rfcconfig.GetSection("EmailUtility").GetSection("EmailFrom").Value;
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><div style='font-size:17px; font-family:Calibri (Body);'>");
            sb.Append("Hi Sir/Madam, <br/>");
            sb.Append("Please find attached leave history sheet for " + empname + ".");
            sb.Append("</div></html>");
            string htmlString = sb.ToString();
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient(Host, Port);
            message.From = new MailAddress(EmailFrom);
            message.To.Add(new MailAddress(emailaddress));
            message.Subject = "Leave history for " + empname;
            message.IsBodyHtml = true;
            message.Body = htmlString;

            DataTable table = new DataTable();
            table.Columns.Add("LEAVE CATEGORY", typeof(string));
            table.Columns.Add("LEAVE APPLIEDDATE", typeof(string));
            table.Columns.Add("FROM DATE", typeof(string));
            table.Columns.Add("TO DATE", typeof(string));
            table.Columns.Add("TOTAL APPLIED LEAVE", typeof(string));
            table.Columns.Add("APPROVER NAME", typeof(string));
            table.Columns.Add("STATUS", typeof(string));
            table.Columns.Add("LEAVE SHIFT", typeof(string));
            table.Columns.Add("SAP SUCCESS FLAG", typeof(string));
            table.Columns.Add("ERROR MSG", typeof(string));
            foreach (LeaveHistoryModelViewModel model in objmodel)
            {
                int index = model.LeaveAppliedDate.LastIndexOf(">");
                string newdate = (index < 0) ? model.LeaveAppliedDate : model.LeaveAppliedDate.Remove(0, index + 1);
                table.Rows.Add(model.LeaveCategory, newdate, model.FromDate, model.ToDate, model.TotalLeaves, model.ApproverName,
                    model.Status, model.LeaveType, model.Indicator, model.ErrorMsg);
            }
            var attachment = GetAttachment(table, empname);
            message.Attachments.Add(attachment);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(message);
        }

        public static Attachment GetAttachment(DataTable dataTable, string fileName)
        {
            MemoryStream outputStream = new MemoryStream();
            using (ExcelPackage package = new ExcelPackage(outputStream))
            {
                ExcelWorksheet facilityWorksheet = package.Workbook.Worksheets.Add("history");
                facilityWorksheet.Cells.LoadFromDataTable(dataTable, true);
                package.Save();
            }
            outputStream.Position = 0;
            Attachment attachment = new Attachment(outputStream, fileName + ".xlsx", "application/vnd.ms-excel");
            return attachment;
        }
    }
}

