using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.BAL.ServiceModels;
using LeaveSolution.DAL.Interfaces;
using LeaveSolution.DAL.Data;
using System.Linq;
using AutoMapper;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using LeaveSolution.BAL.ServiceInterface;
using LeaveSolution.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveSolution.BAL.ServiceClass
{
    public class LeaveRequestService : ILeaveRequestViewService
    {
        private readonly ILeaveRequestRepository _iLeaveRequestRepository;
        public LeaveRequestService(ILeaveRequestRepository ileaverequest)
        {
            _iLeaveRequestRepository = ileaverequest;
        }
        public List<SelectListItem> GetLeaveRequest(string PA, string PSA, string Category)
        {
            var responseList = _iLeaveRequestRepository.GetLeaveRequest(PA, PSA, Category).Select(s => new SelectListItem { Value = s.LeaveCode.ToString(), Text = s.LeaveCategory.ToString() }).ToList();
            return responseList;
        }
        public string[] GetApproverEmail(string EmpID)
        {
            return _iLeaveRequestRepository.GetApproverEmail(EmpID);
        }
        public List<SelectListItem> GetLeaveRequestList(string PA, string PSA, string Category)
        {
            var responseList = _iLeaveRequestRepository.GetLeaveRequest(PA, PSA, Category).Select(s => new SelectListItem { Value = s.LeaveCode.ToString(), Text = s.LeaveCategory.ToString() }).ToList();
            return responseList;
        }
        public List<LeaveRequestViewtServiceModel> GetLeaveDetailsForChange(int objLeaverequestId)
        {
            List<LeaveRequestViewtServiceModel> listmodel = new List<LeaveRequestViewtServiceModel>();
            // listmodel = Mapper.Map < List<DeptLeaveRequestViewtServiceModel> >(_iLeaveRequestRepository.GetLeaveDetailsForChange(objLeaverequestId));
            listmodel = Mapper.Map<List<LeaveRequestViewtServiceModel>>(_iLeaveRequestRepository.GetLeaveDetailsForChange(objLeaverequestId));
            return listmodel;
        }
        public List<string> GetAutoCompleteList(string search)
        {
            List<string> _listRoleId = _iLeaveRequestRepository.GetAutoCompleteList(search);
            return _listRoleId;
        }
        public double GetBalanceForSingleEmp(string employeeid, string leaveId)
        {
            return _iLeaveRequestRepository.GetBalanceForSingleEmp(employeeid, leaveId);
        }
        public int SaveApproverFromSAP(ApproverServiceModel model)
        {
            // return 1;
            var saveapproval = new List<tbl_LeaveApproval>();
            var savedata = new tbl_LeaveApproval
            {
                EmployeeID = model.DETAILS.Select(x => x.EmployeeID).FirstOrDefault(),
                ApproverId = model.DETAILS.Select(x => x.PERNR_SUP).FirstOrDefault(),
                ApproverMobNo = model.DETAILS.Select(x => x.CELL_SUP).FirstOrDefault(),
                ApproverName = model.DETAILS.Select(x => x.CNAME_SUP).FirstOrDefault(),
                HODName = model.DETAILS.Select(x => x.CNAME_HOD).FirstOrDefault(),
                ApproverMailID = model.DETAILS.Select(x => x.EMAIL_SUP).FirstOrDefault(),
                HODID = model.DETAILS.Select(x => x.PERNR_HOD).FirstOrDefault(),
                HODMailID = model.DETAILS.Select(x => x.EMAILID_HOD).FirstOrDefault(),
                LeaveRequestId=Convert.ToInt32(model.DETAILS.Select(x=>x.LeaveRequestID).FirstOrDefault()),
            };
            saveapproval.Add(savedata);
            Tuple<string, int> result = _iLeaveRequestRepository.SaveApproverFromSAP(saveapproval);

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
        public Tuple<string, int> SaveLeaveRequest(LeaveRequestViewtServiceModel objModel)
        {

            Tuple<string, int> itemSaveData;
            if (objModel.LeaveRequestId == null)
            {
                itemSaveData = _iLeaveRequestRepository.SaveLeaveRequest(Mapper.Map<tbl_LeaveRequest>(objModel));
            }
            else
            {
                itemSaveData = _iLeaveRequestRepository.UpdateLeaveRequest(Mapper.Map<tbl_LeaveRequest>(objModel));
            }

            //Tuple<string, int> itemSaveData = _iLeaveRequestRepository.SaveLeaveRequest(Mapper.Map<tbl_LeaveRequest>(objModel));
            //var result = 1;
            //if (itemSaveData.Item2 == 1)
            //{
            //    result = ;
            //}
            //else if (itemSaveData.Item2 == 0)
            //{
            //    result = 1;
            //}

            return itemSaveData;
        }
        public List<HolidayServiceModel> GetHolidayList(string Empid)
        {
            var responseList = _iLeaveRequestRepository.GetHolidayList(Empid).Select(s => new HolidayServiceModel { HolidayDate = s.HolidayDate.ToString(), HolidayName = s.HolidayName.ToString(), Text = s.Text.ToString(), Text1 = s.Text1.ToString() }).ToList();
            return responseList;
        }
        public int SaveFromSAP(QuotaServiceModel model)
        {

            var saveotplist = new List<tbl_LeaveQuotaSAP>();
            var savedata = new tbl_LeaveQuotaSAP
            {

                APPROVERID = model.QuotaOverview.Select(x => x.ApproverID).FirstOrDefault(),
                APPROVERNAME = model.QuotaOverview.Select(x => x.ApproverSAPName).FirstOrDefault(),
                EMPLOYEEID = model.QuotaOverview.Select(x => x.EmployeeID).FirstOrDefault(),

                LEAVECODE = model.QuotaOverview.Select(x => x.LeaveCode).FirstOrDefault(),
                LEAVECATEGORY = model.QuotaOverview.Select(x => x.Ktext).FirstOrDefault(),
                STARTDATE = model.QuotaOverview.Select(x => x.Begda).FirstOrDefault(),
                ENDDATE = model.QuotaOverview.Select(x => x.Endda).FirstOrDefault(),
                CARRYFRWD = model.QuotaOverview.Select(x => x.Kverb).FirstOrDefault().ToString(),
                QUOTA = model.QuotaOverview.Select(x => x.Anzhl).FirstOrDefault().ToString(),
                BALANCELEAVE = model.QuotaOverview.Select(x => x.AnzhlClose).FirstOrDefault().ToString(),
                //CardID = model.QuotaOverview.Select(x => x.Zzcardid).FirstOrDefault(),
            };
            saveotplist.Add(savedata);
            Tuple<string, int> result = _iLeaveRequestRepository.SaveFromSAP(saveotplist);
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

        public string GetMappingData(string Leavecode)
        {
            string qtytype = _iLeaveRequestRepository.GetMappingData(Leavecode).ToString();
            return qtytype;
        }

        public string GetLeaveCode(string LeaveID)
        {
            string LeaveCode = _iLeaveRequestRepository.GetLeaveCode(LeaveID);
            return LeaveCode;
        }
        //public List<LeaveApprovalServiceModel> GetEmpLeaveDetails()
        //{
        //    var responseList = Mapper.Map<List<LeaveApprovalServiceModel>>(_iLeaveRequestRepository.GetEmpLeaveDetails()).ToList();
        //    return responseList;
        //}
    }
}
