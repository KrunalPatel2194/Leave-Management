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
using System;
using Oracle.ManagedDataAccess.Client;
//using Oracle.DataAccess.Types;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace LeaveSolution.BAL.ServiceClass
{
    public class DeptLeaveRequestService : IDeptLeaveRequestViewService
    {
        private IDeptLeaveRequestRepository _DeptLeaveRequestRepository;

        public DeptLeaveRequestService(IDeptLeaveRequestRepository deptLeaveRequestRepository)
        {
            _DeptLeaveRequestRepository = deptLeaveRequestRepository;
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
            Tuple<string, int> result = _DeptLeaveRequestRepository.SaveFromSAP(saveotplist);
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
        public string GetLeaveCode(string LeaveID)
        {
            string LeaveCode = _DeptLeaveRequestRepository.GetLeaveCode(LeaveID);
            return LeaveCode;

        }
        public string GetMappingData(string Leavecode)
        {
            string qtytype = _DeptLeaveRequestRepository.GetMappingData(Leavecode).ToString();
            return qtytype;
        }
        public int SaveUploadedLeave(List<DeptLeaveRequestViewtServiceModel> objModel)
        {
            DataTable dtout = new DataTable();

            // int result = Mapper.Map<List<DeptLeaveRequestViewtServiceModel>>(, dtout));
            int result = _DeptLeaveRequestRepository.Saveuploadedleave(Mapper.Map<List<tbl_DeptLeaveRequest>>(objModel));
            return result;

        }


        public int GetDEPTID(string spempdeptid)
        {
            var ReturnVal = _DeptLeaveRequestRepository.GetDEPTID(spempdeptid);
            return ReturnVal;
        }

        public List<SelectListItem> GetLeaveRequest(string PA,string PSA,string deptMasterID)
        {
            var responseList = _DeptLeaveRequestRepository.GetLeaveRequest(PA,PSA,deptMasterID).Select(s => new SelectListItem { Value = s.LeaveCode.ToString(), Text = s.LeaveCategory.ToString() }).ToList();
            return responseList;
        }
       
        public List<HolidayServiceModel> GetHolidayList(string Empid)
        {
            //var responseList = _DeptLeaveRequestRepository.GetHolidayList(Empid).Select(s => new SelectListItem { Value = s.HolidayDate.ToString(), Text = s.HolidayName.ToString() }).ToList();
            //return responseList;
            var responseList = _DeptLeaveRequestRepository.GetHolidayList(Empid).Select(s => new HolidayServiceModel { HolidayDate = s.HolidayDate.ToString(), HolidayName = s.HolidayName.ToString(), Text = s.Text.ToString(), Text1 = s.Text1.ToString() }).ToList();
            return responseList;
        }
        public List<SelectListItem> GetAutoCompleteList(string search)
        {
            var responseList = _DeptLeaveRequestRepository.GetAutoCompleteList(search).Select(s => new SelectListItem { Value = s.EmployeeID.ToString(), Text = s.EmployeeName.ToString() }).ToList();
            return responseList;
        }
        public DeptLeaveRequestViewtServiceModel GetEmpDetails(string EmpID)
        { 
            var returndata = _DeptLeaveRequestRepository.GetEmpDetails(EmpID);
            var result = new DeptLeaveRequestViewtServiceModel();
            result.EmployeeName = returndata.EmployeeName;
            result.Grade = returndata.Grade;
            result.PA = returndata.PA;
            result.PSA = returndata.PSA;
            result.ApproverId = returndata.ApproverId;
            result.empPhoneNo = returndata.empPhoneNo;
            return result;
        }
        public Tuple<string, int> SaveLeaveRequest(DeptLeaveRequestViewtServiceModel objModel)
        {

            Tuple<string, int> itemSaveData = _DeptLeaveRequestRepository.SaveLeaveRequest(Mapper.Map<tbl_DeptLeaveRequest>(objModel));
           // var result = 1;
            //if (itemSaveData.Item2 == 1)
            //{
            //    result = 0;
            //}
            //else if (itemSaveData.Item2 == 2)
            //{
            //    result = 1;
            //    //return result;
            //}
            //else
            //{
            //    result = 2;
            //}
            return itemSaveData;
        }
        public double GetBalanceForSingleEmp(string employeeid, string leaveId)
        {
            var responseList = _DeptLeaveRequestRepository.GetBalanceForSingleEmp(employeeid, leaveId);
            var balanceLeave = responseList.Select(x => x.BalanceLeave).ToList().Distinct().ToList();
            double balLeave = balanceLeave.FirstOrDefault();
            return balLeave;
        }

        public int SaveConfirm(string AdminID,string userdata)
        {
            var ReturnVal = _DeptLeaveRequestRepository.SaveConfirm(AdminID, userdata);
            return ReturnVal;
        }

        public List<DeptLeaveRequestViewtServiceModel> GetLeaveDetailsForChange(int objLeaverequestId)
        {
            List<DeptLeaveRequestViewtServiceModel> listmodel = new List<DeptLeaveRequestViewtServiceModel>();
            listmodel = Mapper.Map<List<DeptLeaveRequestViewtServiceModel>>(_DeptLeaveRequestRepository.GetLeaveDetailsForChange(objLeaverequestId));
            return listmodel;

        }
    }
}

