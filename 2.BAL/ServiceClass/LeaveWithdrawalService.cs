using System;
//using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using LeaveSolution.BAL.ServiceModels;
using LeaveSolution.DAL.Interfaces;
using LeaveSolution.DAL.Data;
using System.Linq;
using AutoMapper;
using Oracle.ManagedDataAccess.Client;
//using Oracle.DataAccess.Types;
using Oracle.ManagedDataAccess.Types;
using LeaveSolution.BAL.ServiceInterface;
using LeaveSolution.Core;
using Microsoft.AspNetCore.Http;

namespace LeaveSolution.BAL.ServiceClass
{
   public class LeaveWithdrawalService : ILeaveWithdrawalService
    {
        private ILeaveWithdrawalRepository _leaveWithdrawalRepository;
        private readonly ISMSUtility _SMSUtility;
        SMSData ObjSmsData = new SMSData();

        public LeaveWithdrawalService(ILeaveWithdrawalRepository leaveWithdrawalRepository, ISMSUtility SMSUtility)
        {
            _leaveWithdrawalRepository = leaveWithdrawalRepository;
           
        }
        //public int SaveFromSAP(QuotaServiceModel model)
        //{

        //    var saveotplist = new List<tbl_LeaveQuotaSAP>();
        //    var savedata = new tbl_LeaveQuotaSAP
        //    {

        //        APPROVERID = model.QuotaOverview.Select(x => x.ApproverID).FirstOrDefault(),
        //        APPROVERNAME = model.QuotaOverview.Select(x => x.ApproverSAPName).FirstOrDefault(),
        //        EMPLOYEEID = model.QuotaOverview.Select(x => x.EmployeeID).FirstOrDefault(),

        //        LEAVECODE = model.QuotaOverview.Select(x => x.LeaveCode).FirstOrDefault(),
        //        LEAVECATEGORY = model.QuotaOverview.Select(x => x.Ktext).FirstOrDefault(),
        //        STARTDATE = model.QuotaOverview.Select(x => x.Begda).FirstOrDefault(),
        //        ENDDATE = model.QuotaOverview.Select(x => x.Endda).FirstOrDefault(),
        //        CARRYFRWD = model.QuotaOverview.Select(x => x.Kverb).FirstOrDefault().ToString(),
        //        QUOTA = model.QuotaOverview.Select(x => x.Anzhl).FirstOrDefault().ToString(),
        //        BALANCELEAVE = model.QuotaOverview.Select(x => x.AnzhlClose).FirstOrDefault().ToString(),
        //        //CardID = model.QuotaOverview.Select(x => x.Zzcardid).FirstOrDefault(),
        //    };
        //    saveotplist.Add(savedata);
        //    Tuple<string, int> result = _leaveWithdrawalRepository.SaveFromSAP(saveotplist);
        //    var returndata = 0;
        //    if (result.Item2 == 0)
        //    {
        //        returndata = 1;
        //    }
        //    else
        //    {
        //        returndata = 0;
        //    }
        //    return returndata;
        //}
        public LeaveWithdrawalServiceViewModel GetLeaveRequestForWithdrawal(string EmployeeId)
        {
            var serviceModel = new LeaveWithdrawalServiceViewModel
            {
                leavewithdrawal = Mapper.Map<List<LeaveWithdrawalServiceModel>>(_leaveWithdrawalRepository.GetLeavesforWithdrawal(EmployeeId))
            };
            return serviceModel;
        }
        public int SaveLeaveWithdrawal(LeaveWithdrawalServiceModel model)
        {           
            Tuple<string, int> itemSaveData = _leaveWithdrawalRepository.SaveWithdrawalLeaveRequest(Mapper.Map<tbl_Leavewithdrawal>(model));           
            var mobileNo = model.MobileNo;
            var result = 1;
            if (itemSaveData.Item2 == 1)
            {
                result = 1;
                return result;
            }
            else
            {
                result = 0;
                return result;
            }
        }

        //public string GetMappingData(string Leavecode)
        //{
        //    string qtytype = _leaveWithdrawalRepository.GetMappingData(Leavecode).ToString();
        //    return qtytype;
        //}

    }
}
