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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveSolution.BAL.ServiceClass
{
    public class LeaveApprovalService : ILeaveApprovalService
    {

        private ILeaveApprovalRepositary _leaveApprovalRepositary;
        private ISMSUtility _SMSUtility;
        SMSData smsd = new SMSData();
        public LeaveApprovalService(ILeaveApprovalRepositary leaveApprovalRepositary, ISMSUtility utility)
        {
            _leaveApprovalRepositary = leaveApprovalRepositary;
            _SMSUtility = utility;
        }


        public LeaveApprovalServiceViewModel GetApprovalDetails(string ApproverID)
        {

            var serviceModel = new LeaveApprovalServiceViewModel
            {
                leaveApprovals = Mapper.Map<List<LeaveApprovalServiceModel>>(_leaveApprovalRepositary.GetApprovalDetails(ApproverID))
            };

            return serviceModel;
        }


        public LeaveApprovalServiceViewModel GetLeaveRequestForApproval(string ApproverId)
        {

            var serviceModel = new LeaveApprovalServiceViewModel
            {
                leaveApprovals = Mapper.Map<List<LeaveApprovalServiceModel>>(_leaveApprovalRepositary.GetLeaveApprovals(ApproverId))
            };
            return serviceModel;
        }
        //public string GetMappingData(string Leavecode)
        //{
        //    string qtytype = _leaveApprovalRepositary.GetMappingData(Leavecode).ToString();
        //    return qtytype;
        //}
        public Tuple<string, int> SaveLeaveApproval(LeaveApprovalServiceViewModel model)
        {
            Tuple<string, int> itemSaveData = _leaveApprovalRepositary.SaveAprrovedLeaveRequest(Mapper.Map<List<tbl_LeaveApproval>>(model.leaveApprovals.Select(Mapper.Map<LeaveApprovalServiceModel>)));
            var result = 1;

            if (itemSaveData.Item2 != -99)
            {
                result = 1;
                SMSData sms = new SMSData();

                //  var sms=_SMSUtility.SendSMS(model.leaveApprovals.FirstOrDefault(x=>x.ApproverMobNo.ToString()))
                return itemSaveData;
            }
            else
            {
                result = 0;
                return itemSaveData;
            }
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
        //    Tuple<string, int> result = _leaveApprovalRepositary.SaveFromSAP(saveotplist);
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
        public LeaveApprovalServiceViewModel GetLeaveApprovalsForDashboard(string ApproverId)
        {

            var serviceModel = new LeaveApprovalServiceViewModel
            {
                leaveApprovals = Mapper.Map<List<LeaveApprovalServiceModel>>(_leaveApprovalRepositary.GetLeaveApprovalsForDashboard(ApproverId))
            };

            return serviceModel;
        }

        public LeaveApprovalServiceViewModel GetMulitpleEmployeeDetails(string strleavereqid)
        {
            var serviceModel = new LeaveApprovalServiceViewModel
            {
                leaveApprovals = Mapper.Map<List<LeaveApprovalServiceModel>>(_leaveApprovalRepositary.GetMulitpleEmployeeDetails(strleavereqid))
            };
            return serviceModel;
        }
        public Tuple<string, int> InsertEmpId(string empid)
        {
            Tuple<string, int> itemSaveData;
            itemSaveData = _leaveApprovalRepositary.InsertEmpId(empid);
            return itemSaveData;
        }
    }
}
