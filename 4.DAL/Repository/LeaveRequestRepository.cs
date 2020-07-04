using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Interfaces;
using LeaveSolution.DAL.Data;
using System.Data;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Globalization;

namespace LeaveSolution.DAL.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        //Common to all page for connection
        public Tuple<string, int> SaveApproverFromSAP(List<tbl_LeaveApproval> Approverdata)
        {
            try
            {
             
                DALBase obj = new DALBase(_config);
                List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
                foreach (var l in Approverdata)
                {
                    if (l.ApproverMailID == null)
                    {
                        l.ApproverMailID = "EmailID not Updated";
                    }
                    string FromData = "";
                    if (l.LeaveRequestId==0 || l.LeaveRequestId == null)
                    {
                        FromData = "EMP";
                    }
                    else
                    {
                        FromData = "APR";
                    }
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERID, OracleDbType.NVarchar2, 50, l.ApproverId.ToString(), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERNAME, OracleDbType.NVarchar2, 1000, l.ApproverName.ToString(), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMAILID, OracleDbType.NVarchar2, 100, l.ApproverMailID, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_PHONENO, OracleDbType.Varchar2, 100, l.ApproverMobNo, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_HODID", OracleDbType.Varchar2, 100, l.HODID, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_HODName", OracleDbType.Varchar2, 100, l.HODName, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_HODMailID", OracleDbType.Varchar2, 100, l.HODMailID, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_MESSAGE", OracleDbType.NVarchar2, 100, null, ParameterDirection.Output);
                    obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.NVarchar2, 50, l.EmployeeID.ToString(), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "FromData", OracleDbType.NVarchar2, 50, FromData, ParameterDirection.Input);
                    
                        obj.TupleForOracleParameter(LstTupParameters, "P_LeaveRequestID", OracleDbType.NVarchar2, 50, l.LeaveRequestId.ToString(), ParameterDirection.Input);
                }
                Tuple<string, int> Returnstr = obj.savedata("BCL_TEST_SAVEAPPROVERMASTER", LstTupParameters);
                return Returnstr;
            }
            catch (Exception ex)
            {
                Tuple<string, int> returndata = new Tuple<string, int>("Failed", 1);
                return returndata;
                throw;
            }
        }
        public static void TupleForOracleParameter(List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters, string paraname, OracleDbType OraclePara, int lenght, string value, ParameterDirection paraDirection)
        {
            LstTupParameters.Add(new Tuple<string, OracleDbType, int, object, ParameterDirection>(paraname, OraclePara, lenght, value, paraDirection));
        }
        public readonly IConfiguration _config;
        public LeaveRequestRepository(IConfiguration config)//constructor
        {
            _config = config;
        }
        public List<tbl_LeaveRequest> GetLeaveRequest(string PA, string PSA, string Category)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<tbl_LeaveRequest> listObj = new List<tbl_LeaveRequest>();
                tbl_LeaveRequest objLeave = new tbl_LeaveRequest();
                DataSet ds = obj.GetLeaveRequestBYId("BCL_TEST_GETLEAVEMASTER", PA, PSA, Category);
                DataTable dt = ds.Tables[0];
                if (dt != null)
                {
                    objLeave.LeaveCode = "";
                    objLeave.LeaveCategory = "--Please Select--";
                    listObj.Add(objLeave);
                    if (dt.Rows.Count > 0)
                    {                       
                        foreach (DataRow dtRow in dt.Rows)
                        {
                            string LeaveID = dtRow["LEAVEID"].ToString() + "~" + dtRow["Qttype"].ToString() + "~" + dtRow["LEAVECODE"].ToString();
                            string Descr = dtRow["Description"].ToString();
                            objLeave = new tbl_LeaveRequest();
                            objLeave.LeaveCode = LeaveID;
                            objLeave.LeaveCategory = Descr;
                            listObj.Add(objLeave);
                        }
                    }
                }
                return listObj;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public string[] GetApproverEmail(string EmpID)
        {
            string[] email = new string[2];
            try
            {
                DALBase obj = new DALBase(_config);
                DataSet ds = obj.GetApproverEmail("BCL_GETApproverEmail", EmpID);
                DataTable dt = ds.Tables[0];
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        email[0] = dt.Rows[0]["Approvername"].ToString();
                        email[1]= dt.Rows[0]["Emailid"].ToString();
                    }
                }
                return email;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> GetAutoCompleteList(string search)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<string> listObj = new List<string>();
                DataSet ds = obj.GetAutoCompleteList(ConstantStoredProcedure.SP_BCL_GETEMPLOYEENAME, search);
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        var EmpName = dtRow["EmpNameId"].ToString();
                        listObj.Add(EmpName);
                    }
                }
                return listObj;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        /// <summary>
        /// For HolidayList Bind
        /// </summary>
        /// <returns></returns>
        public List<tblHoliday> GetHolidayList(string Empid)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<tblHoliday> listObj = new List<tblHoliday>();
                DataSet ds = obj.GetHolidaylist(ConstantStoredProcedure.SP_BCL_HOLIDAYLIST, Empid);
                if (ds != null && ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                        {
                            foreach (DataRow dtRow in ds.Tables[i].Rows)
                            {
                                var date = dtRow["FROMDATE"].ToString();
                                var Descr = dtRow["TODATE"].ToString();
                                var Descr1 = dtRow["DESCRIPTION"].ToString();
                                var Descr2 = dtRow["Status"].ToString();
                                var objLeave = new tblHoliday();
                                objLeave.HolidayDate = Convert.ToDateTime(date).ToString("yyyy/MM/dd").Replace('-','/');
                                objLeave.HolidayName = Convert.ToDateTime(Descr).ToString("yyyy/MM/dd").Replace('-', '/');
                                objLeave.Text = Descr1;
                                objLeave.Text1 = Descr2;
                                listObj.Add(objLeave);
                            }
                        }
                    }
                }
                return listObj;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        /// <summary>
        /// For Get BalanceLeave
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public double GetBalanceForSingleEmp(string employeeid, string leaveid)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                return obj.GetBalanceByEmpId(ConstantStoredProcedure.SP_BCLDB_sp_GetLeaveBalance, employeeid, leaveid);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public Tuple<string, int> SaveLeaveRequest(tbl_LeaveRequest objleave_request)
        {
                DALBase obj = new DALBase(_config);
             
                objleave_request.LeaveAppliedDate = Convert.ToDateTime(DateTime.Now.Date).ToString("dd/MM/yyyy");
                objleave_request.StatusUpdate = Convert.ToDateTime(DateTime.Now.Date).ToString("dd/MM/yyyy");               
                List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVEID, OracleDbType.Int32, 50, Convert.ToString(objleave_request.LeavelistId), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_FROMDATE, OracleDbType.NVarchar2, 120,Convert.ToString(objleave_request.FromDate), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_TODATE, OracleDbType.NVarchar2, 120, Convert.ToString(objleave_request.ToDate), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVETYPE, OracleDbType.NVarchar2, 50, Convert.ToString(objleave_request.LeaveShift), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERID, OracleDbType.NVarchar2, 50, Convert.ToString(objleave_request.ApproverId), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERNAME, OracleDbType.NVarchar2, 50, Convert.ToString(objleave_request.ApproverName), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_REMARKS, OracleDbType.NVarchar2, 100, Convert.ToString(objleave_request.Remarks), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_TOTALAPPLIEDLEAVE, OracleDbType.Double, 100, Convert.ToString(objleave_request.TotalAppliedLeave), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVEAPPLIEDDATE, OracleDbType.NVarchar2, 120, Convert.ToString(objleave_request.LeaveAppliedDate), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPLOYEEID, OracleDbType.NVarchar2, 50, Convert.ToString(objleave_request.EmployeeID), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, "P_MESSAGE", OracleDbType.NVarchar2, 100, null, ParameterDirection.Output);
                TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
                if (objleave_request.UploadFileName == null)
                {
                    TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_FILENAME, OracleDbType.NVarchar2, 50, null, ParameterDirection.Input);
                }
                else
                {
                    TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_FILENAME, OracleDbType.NVarchar2, 50, Convert.ToString(objleave_request.UploadFileName), ParameterDirection.Input);
                }

            TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPLOYEENAME, OracleDbType.NVarchar2, 50, Convert.ToString(objleave_request.EmployeeName), ParameterDirection.Input);
            //  Tuple<string, int> Retunstr = obj.savedata(ConstantStoredProcedure.SP_BCL_SAVELEAVEREQUEST, LstTupParameters);
            Tuple<string, int> Retunstr = obj.savedata("BCL_SAVELEAVEREQUEST", LstTupParameters); 
            return Retunstr;
        }
        public Tuple<string, int> UpdateLeaveRequest(tbl_LeaveRequest objleave_request)
        {
            try
            {
                DALBase obj = new DALBase(_config);
               

                List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPLOYEEID, OracleDbType.NVarchar2, 50, objleave_request.EmployeeID.ToString(), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVEREQUESTID, OracleDbType.NVarchar2, 50, objleave_request.LeaveRequestID.ToString(), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERNAME, OracleDbType.NVarchar2, 100, objleave_request.ApproverName.ToString(), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERID, OracleDbType.NVarchar2, 50, objleave_request.ApproverId.ToString(), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, "P_MESSAGE", OracleDbType.NVarchar2, 100, null, ParameterDirection.Output);
                TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
                
                Tuple<string, int> Retunstr = obj.savedata(ConstantStoredProcedure.SP_BCL_UPDATELEAVEREQUEST, LstTupParameters);
                return Retunstr;
            }
            catch (Exception ex)
            {
                Tuple<string, int> returndata = new Tuple<string, int>("Failed", 1);
                return returndata;
                throw;
            }
        }
        public List<tbl_LeaveRequest> GetLeaveDetailsForChange(int objLeaverequestId)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<tbl_LeaveRequest> objList = new List<tbl_LeaveRequest>();
                DataSet ds = obj.GetEmpDetails(ConstantStoredProcedure.SP_BCL_GETLEAVEDETAILSFORCHNAGE, objLeaverequestId);
                DataTable dt = ds.Tables[0];
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var objtblLeaveRequest = new tbl_LeaveRequest();
                            objtblLeaveRequest.LeaveRequestID = Convert.ToString(dt.Rows[i]["LEAVEREQUESTID"]);
                            objtblLeaveRequest.LeaveId = Convert.ToInt32(dt.Rows[i]["LEAVEID"]);
                            objtblLeaveRequest.LeaveCategory = Convert.ToString(dt.Rows[i]["LEAVECATEGORY"]);
                            objtblLeaveRequest.FromDate = Convert.ToDateTime(dt.Rows[i]["FROMDATE"]).ToString("dd/MM/yyyy");
                            objtblLeaveRequest.ToDate = Convert.ToDateTime(dt.Rows[i]["TODATE"]).ToString("dd/MM/yyyy");
                            objtblLeaveRequest.LeaveShift = Convert.ToString(dt.Rows[i]["LEAVESHIFT"]);
                            objtblLeaveRequest.ApproverId = Convert.ToString(dt.Rows[i]["APPROVERID"]);
                            objtblLeaveRequest.ApproverName = Convert.ToString(dt.Rows[i]["APPROVERNAME"]);
                            //objtblLeaveRequest.AbsenceCEHour = Convert.ToDouble(dt.Rows[i][8]);
                            objtblLeaveRequest.Remarks = Convert.ToString(dt.Rows[i]["REMARKS"]);
                            objList.Add(objtblLeaveRequest);
                        }
                    }
                }
                return objList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Tuple<string, int> SaveFromSAP(List<tbl_LeaveQuotaSAP> tbl_User)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
                foreach (var l in tbl_User)
                {
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.NVarchar2, 50, Convert.ToString(l.EMPLOYEEID), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVEID, OracleDbType.NVarchar2, 50, Convert.ToString(l.LEAVECODE), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVECATEGORY, OracleDbType.NVarchar2, 1000, Convert.ToString(l.LEAVECATEGORY), ParameterDirection.Input);
                    string startdate = Convert.ToDateTime(l.STARTDATE).ToString("dd/MM/yyyy");
                    string endate = Convert.ToDateTime(l.ENDDATE).ToString("dd/MM/yyyy");
                    obj.TupleForOracleParameter(LstTupParameters, "P_STARTDATE", OracleDbType.NVarchar2, 120, Convert.ToString(startdate), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_ENDDATE", OracleDbType.NVarchar2, 120, Convert.ToString(endate), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_QUOTA", OracleDbType.Double, 100, l.QUOTA, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_BALANCELEAVE, OracleDbType.Double, 100, l.BALANCELEAVE, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERID, OracleDbType.NVarchar2, 50, Convert.ToString(l.APPROVERID), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERNAME, OracleDbType.NVarchar2, 50, Convert.ToString(l.APPROVERNAME), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_TOTALLEAVE, OracleDbType.Double, 50, l.CARRYFRWD, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
                }
                Tuple<string, int> Returnstr = obj.savedataFSAP("BCL_SAVELEAVEQUOTASAP", LstTupParameters);
                return Returnstr;
            }
            catch (Exception ex)
            {
                Tuple<string, int> returndata = new Tuple<string, int>("Failed", 1);
                return returndata;
                throw;
            }
        }
        public string GetMappingData(string LeaveCode)
        {
            DALBase obj = new DALBase(_config);
            List<tbl_LeaveRequest> listObj = new List<tbl_LeaveRequest>();
            List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVEID, OracleDbType.NVarchar2, 20, Convert.ToString(LeaveCode), ParameterDirection.Input);
            obj.TupleForOracleParameter(LstTupParameters, "P_QtyType", OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
            obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 20, null, ParameterDirection.Output);
            DataSet ds = obj.GetDatafromDatabase(ConstantStoredProcedure.SPBCL_MAPPINGDATA, LstTupParameters);
            var ReturnVal = Convert.ToInt32((decimal)(OracleDecimal)(obj.cmd.Parameters["P_RETURNVALUE"].Value));
            string Qtytype = "";
            if (ReturnVal == 0)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var tblleavereq = new tbl_LeaveRequest();
                        Qtytype = dt.Rows[i]["QTTYPE"].ToString();
                    }
                }
            }
            return Qtytype;
        }
        public string GetLeaveCode(string LeaveID)
        {
            DALBase obj = new DALBase(_config);
            List<tbl_LeaveRequest> listObj = new List<tbl_LeaveRequest>();
            List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVEID, OracleDbType.NVarchar2, 20, Convert.ToString(LeaveID), ParameterDirection.Input);
            obj.TupleForOracleParameter(LstTupParameters, "LeaveCode", OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
            obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 20, null, ParameterDirection.Output);
            DataSet ds = obj.GetDatafromDatabase(ConstantStoredProcedure.SP_BCL_GETLEAVECODE, LstTupParameters);
            var ReturnVal = Convert.ToInt32((decimal)(OracleDecimal)(obj.cmd.Parameters["P_RETURNVALUE"].Value));
            string LeaveCode = "";
            if (ReturnVal == 0)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var tblleavereq = new tbl_LeaveRequest();
                        LeaveCode = dt.Rows[i]["LEAVECODE"].ToString();
                    }
                }
            }
            else
            {
                //string error = "1";
                //listObj.Add(error);
            }
            return LeaveCode;
        }
       
    }
}
