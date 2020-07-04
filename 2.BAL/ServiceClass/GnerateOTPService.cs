using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using LeaveSolution.BAL.ServiceModels;
using LeaveSolution.DAL.Interfaces;
using LeaveSolution.DAL.Data;
using System.Linq;
using AutoMapper;
using System.Net.Http;
using Oracle.ManagedDataAccess.Client;
//using Oracle.DataAccess.Types;
using Oracle.ManagedDataAccess.Types;

using LeaveSolution.BAL.ServiceInterface;
using Microsoft.AspNetCore.Mvc;
using LeaveSolution.Core;
using Microsoft.AspNetCore.Http;

namespace LeaveSolution.BAL.ServiceClass
{
    public class GnerateOTPService : ILoginViewService
    {
        private readonly IGenerateOTPRepositary _igenerateotprepositary;
        private readonly ISMSUtility _SMSUtility;
        SMSData smsd = new SMSData();
        public GnerateOTPService(IGenerateOTPRepositary igenerateotp, ISMSUtility SMSUtility)

        {
            _igenerateotprepositary = igenerateotp;
            _SMSUtility = SMSUtility;
        }

        public LoginViewServiceModel LoginCredentials()
        {
            throw new NotImplementedException();
        }

        public long GetMobileno(string employeeid, string fromdata)
        {
            var responseList = _igenerateotprepositary.Getsingledata(employeeid, fromdata);
            var returnvalue = responseList.Select(x => x.Returnvalue);
            var retur = returnvalue.FirstOrDefault();
            long mobno = 1;
            if (retur == 2)
            {
                mobno = 2;

            }
            else if (retur == 1)
            {
                mobno = 1;

            }
            else
            {
                var Mobileno = responseList.Select(x => x.MobileNO).ToList().Distinct().ToList();
                mobno = Mobileno.FirstOrDefault();

            }

            return mobno;

        }
        public long GenerateOtp(string employeeid, long mobileno)
        {

            LoginViewServiceModel l = new LoginViewServiceModel();
            // var responseList = Mapper.Map < LoginViewServiceModel >( _igenerateotprepositary.GetMobileno(employeeid));
            //           var responseList = _igenerateotprepositary.Getsingledata(employeeid);
            //var Mobileno = responseList.Select(x => x.MobileNO).ToList().Distinct().ToList() ;
            //OTP generation code
            long msgsent = 0;



            char[] charArr = "0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new Random();
            int noofcharacters = 4; //Convert.ToInt32(txtCharacters.Text);
            for (int i = 0; i < noofcharacters; i++)
            {
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos).ToString()))
                    strrandom += charArr.GetValue(pos);
                else
                    i--;
            }
            var strotp = strrandom;

            // var item = responseList.FirstOrDefault();
            //               msgsent = item.MobileNO;
            msgsent = mobileno;
            string msgotp = smsd.Otpgeneratesms;
            msgotp = msgotp.Replace("+", strotp);

            //Code for save otp ,empid,otpstarrttime n endtime in table
            var saveotplist = new List<tbl_OTPG>();
            var savedata = new tbl_OTPG
            {
                EmployeeID = Convert.ToString(employeeid),
                OTP = Convert.ToInt32(strotp),
            };
            //uncomment this code for save otp
            saveotplist.Add(savedata);
            var savetorep = _igenerateotprepositary.SaveOTP(saveotplist);
            var ret = savetorep.Item2;
            // temperory comment code as sms utility gets error u should uncomment this for sending sms and make m = 0
            if (ret == 1)
            {
                var sms = _SMSUtility.SendSMS(msgsent.ToString(), msgotp);
                //  var sms = "0";
                if (sms != null)
                {
                    msgsent = 1;
                }
                else
                {
                    msgsent = 0;
                }
            }
            else if(ret == -1)
            {
                msgsent = -1;
            }
            else
            {
                msgsent = 0;
            }
            //}
            return msgsent;
        }

        public int SaveNewUser(LoginViewServiceModel model)
        {
            var saveotplist = new List<tbl_User>();
            var savedata = new tbl_User
            {
                EmployeeID = model.EmployeeID,
                Password = model.Password,
                UserID = model.UserId,

            };
            saveotplist.Add(savedata);
            var responseList = _igenerateotprepositary.SaveNewUser(saveotplist);

            var result = 1;
            if (responseList.Item2 == 1)
            {
                model.ReturnsaveValue = 1;
                return result;
            }
            else if (responseList.Item2 == 2)
            {
                model.ReturnsaveValue = 2;
                result = 2;
                return result;
            }
            else
            {
                result = 0;
                return result;
            }

        }

        public LoginViewServiceModel Authentication(LoginViewServiceModel loginservicemodel)
        {
            try
            {
                string userid = loginservicemodel.UserId;
                var saveotplist = new List<tbl_User>();
                var savedata = new tbl_User
                {
                    EmployeeID = loginservicemodel.EmployeeID,
                    Password = loginservicemodel.Password,
                    UserID = loginservicemodel.UserId,
                };
                saveotplist.Add(savedata);
                var responseList = _igenerateotprepositary.Authenticate(saveotplist);
                var returndata = new LoginViewServiceModel();
                var model = responseList.FirstOrDefault();
                if (model.ReturnValue == 0)
                {
                    //returndata= igenerateotprepositary.Authenticate(saveotplist);
                    returndata.EmployeeName = model.EmployeeName;
                    returndata.EmployeeID = Convert.ToString(model.EmployeeID);
                    returndata.EmpEmail = model.EmpEmail;
                    returndata.DateOfBirth = Convert.ToDateTime(model.DateOfBirth);
                    returndata.ReturnsaveValue = model.ReturnValue;
                    returndata.PersonalArea = model.PersonalArea;
                    returndata.PersonalSubArea = model.PersonalSubArea;
                    returndata.MobileNo = model.MobileNo;
                    returndata.Category = model.Category;
                }
                else if (model.ReturnValue == 1 || model.ReturnValue == 2 || model.ReturnValue == 3 || model.ReturnValue == 4)
                {
                    returndata.ReturnsaveValue = model.ReturnValue;
                    returndata.ReturnValMessg = Convert.ToString(model.ReturnValMessg);
                }

                return returndata;
            }
            catch (Exception ex)
            {

                throw;
            }

            //throw new NotImplementedException();
        }

        public int OTPmatch(int otp, string empid, DateTime dob,string fromdata)
        {
            int ChkOtpMatch = 0;
            int enteredotp = otp;
            var otpmatch = _igenerateotprepositary.Getotp(empid,fromdata);

            string chkexist = otpmatch.ReturnMsg;
            if (chkexist == "Exist")
            {
                return ChkOtpMatch = 4;//empl already exist go to reset
            }
            if (chkexist == "Invalid")
            {
                return ChkOtpMatch = 1;//empl already exist go to reset
            }
            else
            {
                var getotp = otpmatch.OTP;
                DateTime getendtime = otpmatch.OTPEndtime;
                DateTime getdob = otpmatch.DateOfBirth;
                DateTime dt;
                DateTime.TryParseExact(getdob.ToString("dd-MM-yyyy").Replace('/','-'),
                                       "dd-MM-yyyy",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None,
                                       out dt);
               // int ChkOtpMatch = 0;
                DateTime today = DateTime.Now;

                if (enteredotp == getotp && dob.Date == Convert.ToDateTime(dt).Date)

                {
                    if (today > getendtime)
                    {
                        ChkOtpMatch = 2;
                        return ChkOtpMatch;
                    }
                    else
                        ChkOtpMatch = 0;
                    return ChkOtpMatch;
                }

                else
                {
                    ChkOtpMatch = 1;
                    return ChkOtpMatch;
                }
            }
        }

        public string ValidateMifareID(string MifareID, string datafrom)
        {

            string empid = _igenerateotprepositary.ValidateMifareID(MifareID, datafrom);
            return empid;
        }

        public int DOBMatchforKIOSK(string employeeid, DateTime dob)
        {

            var DOBMatch = _igenerateotprepositary.GetDOBForKIOSK(employeeid);

            DateTime getdob = DOBMatch.DateOfBirth;            
            int ChkDOBMatch = 0;
            DateTime dt;
            DateTime.TryParseExact(getdob.ToString("dd/MM/yyyy").Replace('-', '/'),
                                   "dd/MM/yyyy",
                                   CultureInfo.InvariantCulture,
                                   DateTimeStyles.None,
                                   out dt);
            if (dob.Date == Convert.ToDateTime(dt).Date)
            {
                ChkDOBMatch = 1;
                return ChkDOBMatch;
            }
            else
            {
                return ChkDOBMatch;
            }
        }

        public int SaveFromSAP(EmployeeServicemodel model)
        {
            // return 1;
            var saveotplist = new List<tbl_Employee>();
            var savedata = new tbl_Employee
            {

                EmployeeID = model.Details.Select(x => x.EmployeeID).FirstOrDefault(),
                EmployeeName = model.Details.Select(x => x.Name).FirstOrDefault(),
                DateOfBirth = model.Details.Select(x => x.Birthdate).FirstOrDefault(),
                ApproverId = model.Details.Select(x => x.ApproverID).FirstOrDefault(),
                ApproverName = model.Details.Select(x => x.ApproverSAPName).FirstOrDefault(),
                MobileNo = model.Details.Select(x => x.Userid).FirstOrDefault(),
                EmployeeMail = model.Details.Select(x => x.USRID_LONG).FirstOrDefault(),
                CostCenter = model.Details.Select(x => x.Costcenter).FirstOrDefault(),
                PersonalArea = model.Details.Select(x => x.PERS_AREA).FirstOrDefault(),
                PersonalSubArea = model.Details.Select(x => x.P_SUBAREA).FirstOrDefault(),
                CardID = model.Details.Select(x => x.ZZCARDID).FirstOrDefault(),
                EmpGroup = model.Details.Select(x => x.Persg).FirstOrDefault(),
                EmpSubGroup = model.Details.Select(x => x.Persk).FirstOrDefault(),

            };
            saveotplist.Add(savedata);
            Tuple<string, int> result = _igenerateotprepositary.SaveFromSAP(saveotplist);

            //  Tuple<string, int> result = _igenerateotprepositary.SaveFromSAP(Mapper.Map<List<tbl_Employee>>(model));
            var returndata = 0;
            if (result.Item2 == 0)
            {
                returndata = 1;
            }
            else
            {
                returndata = 0;
            }
            return returndata;
        }

        public int SaveApproverFromSAP(ApproverServiceModel model)
        {
            // return 1;
            var saveapproval = new List<tbl_LeaveApproval>();
            var savedata = new tbl_LeaveApproval
            {
                ApproverId = model.DETAILS.Select(x => x.PERNR_SUP).FirstOrDefault(),
                ApproverMobNo = model.DETAILS.Select(x => x.CELL_SUP).FirstOrDefault(),
                ApproverName = model.DETAILS.Select(x => x.CNAME_SUP).FirstOrDefault(),
                HODName = model.DETAILS.Select(x => x.CNAME_HOD).FirstOrDefault(),
                ApproverMailID = model.DETAILS.Select(x => x.EMAIL_SUP).FirstOrDefault(),
                HODID = model.DETAILS.Select(x => x.PERNR_HOD).FirstOrDefault(),
                HODMailID = model.DETAILS.Select(x => x.EMAILID_HOD).FirstOrDefault(),
            };
            saveapproval.Add(savedata);
            Tuple<string, int> result = _igenerateotprepositary.SaveApproverFromSAP(saveapproval);

            //  Tuple<string, int> result = _igenerateotprepositary.SaveFromSAP(Mapper.Map<List<tbl_Employee>>(model));
            var returndata = 0;
            if (result.Item2 == 0)
            {
                returndata = 1;
            }
            else
            {
                returndata = 0;
            }
            return returndata;
        }
        public int SaveFromSAPQuota(QuotaServiceModel model)
        {

            var saveotplist = new List<tbl_LeaveQuotaSAP>();
            foreach (var item in model.QuotaOverview)
            {
                var savedata = new tbl_LeaveQuotaSAP
                {

                    APPROVERID = item.ApproverID,
                    APPROVERNAME = item.ApproverSAPName,
                    EMPLOYEEID = item.EmployeeID,

                    LEAVECODE = item.LeaveCode,
                    LEAVECATEGORY = item.Ktext,
                    STARTDATE = item.Begda,
                    ENDDATE = item.Endda,
                    CARRYFRWD = item.Kverb.ToString(),
                    QUOTA = item.Anzhl.ToString(),
                    BALANCELEAVE = item.AnzhlClose.ToString(),
                    //CardID = model.QuotaOverview.Select(x => x.Zzcardid).FirstOrDefault(),
                };
                saveotplist.Add(savedata);
            }

            Tuple<string, int> result = _igenerateotprepositary.SaveFromSAPQuota(saveotplist);
            var returndata = 0;
            if (result.Item2 == 0)
            {
                returndata = 1;
            }
            else
            {
                returndata = 0;
            }
            return returndata;
        }
        public void DeleteUser(string userid)
        {
           _igenerateotprepositary.DeleteUser(userid);
        }

        public int ExtendSession(string employeeid)
        {
            int i=_igenerateotprepositary.ExtendSession(employeeid);
            return i;
        }
    }
}