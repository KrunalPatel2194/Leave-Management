using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office;
using LeaveSolution.DAL.Interfaces;
using LeaveSolution.DAL.Data;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Linq;
using OfficeOpenXml;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Globalization;
using System.IO;

namespace LeaveSolution.DAL.Repository
{
    public class DeptLeaveRequestRepository : IDeptLeaveRequestRepository
    {
        public static void TupleForOracleParameter(List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters, string paraname, OracleDbType OraclePara, int lenght, string value, ParameterDirection paraDirection)
        {
            LstTupParameters.Add(new Tuple<string, OracleDbType, int, object, ParameterDirection>(paraname, OraclePara, lenght, value, paraDirection));
        }
        public readonly IConfiguration _config;

        public DeptLeaveRequestRepository(IConfiguration config)
        {
            _config = config;
        }
        public string GetMappingData(string LeaveCode)
        {
            DALBase obj = new DALBase(_config);
            List<tbl_LeaveRequest> listObj = new List<tbl_LeaveRequest>();
            List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVEID, OracleDbType.NVarchar2, 20, LeaveCode, ParameterDirection.Input);
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
        public int Saveuploadedleave(List<tbl_DeptLeaveRequest> dept)
        {
            int i = 0;
            List<tbl_DeptLeaveRequest> ErrorList = new List<tbl_DeptLeaveRequest>();
            try
            {
                List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
                Tuple<string, int> Returnstr = new Tuple<string, int>("Failed", 1);
                {
                    DALBase obj = new DALBase(_config);
                    OracleConnection con;
                    OracleCommand cmd;
                    con = new OracleConnection(obj.DBConnection);
                    cmd = new OracleCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "BCL_TEST_SAVE_MULTILEAVE_DEPTV";//BCL_SAVE_MULTIPLELEAVE_DEPT";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.ArrayBindCount = dept.Select(x => x.EmployeeID).ToArray().Length;
                    cmd.Parameters.Add(ConstantsVariables.P_FROMDATE, OracleDbType.NVarchar2, 1000);
                    cmd.Parameters[0].Value = dept.Select(x => x.FromDate).ToArray();
                    cmd.Parameters.Add(ConstantsVariables.P_TODATE, OracleDbType.NVarchar2, 1000);
                    cmd.Parameters[1].Value = dept.Select(x => x.ToDate).ToArray();
                    cmd.Parameters.Add(ConstantsVariables.P_LEAVECODE, OracleDbType.NVarchar2, 100);
                    cmd.Parameters[2].Value = dept.Select(x => x.LeaveCode).ToArray();
                    cmd.Parameters.Add("P_REMARK", OracleDbType.NVarchar2, 1000);
                    cmd.Parameters[3].Value = dept.Select(x => x.Remarks).ToArray();
                    cmd.Parameters.Add(ConstantsVariables.P_TOTALAPPLIEDLEAVE, OracleDbType.Decimal, 200);
                    cmd.Parameters[4].Value = dept.Select(x => x.TotalAppliedLeave).ToArray();
                    cmd.Parameters.Add(ConstantsVariables.P_EMPLOYEEID, OracleDbType.NVarchar2, 200);
                    cmd.Parameters[5].Value = dept.Select(x => x.EmployeeID).ToArray();
                    cmd.Parameters.Add("P_RETURNVALUE", OracleDbType.Int32, 20);
                    cmd.Parameters[6].Value = null;
                    cmd.Parameters.Add(ConstantsVariables.P_FILENAME, OracleDbType.NVarchar2, 1000);
                    cmd.Parameters[7].Value = dept.Select(x => x.UploadFileName).ToArray();
                    cmd.Parameters.Add(ConstantsVariables.P_CreatedBy, OracleDbType.NVarchar2, 200);
                    cmd.Parameters[8].Value = dept.Select(x => x.CreatedBy).ToArray();
                    cmd.Parameters.Add("P_AdminID", OracleDbType.NVarchar2, 200);
                    cmd.Parameters[9].Value = dept.Select(x => x.AdminID).ToArray();
                    cmd.Parameters.Add("P_ERRORMESSAGE", OracleDbType.NVarchar2, 1000);
                    cmd.Parameters[10].Value = dept.Select(x => x.ErrorMsg).ToArray();
                    cmd.Parameters.Add("P_Indicator", OracleDbType.NVarchar2, 100);
                    cmd.Parameters[11].Value = dept.Select(x => x.Indicator).ToArray();
                    cmd.Parameters.Add(ConstantsVariables.P_LEAVESHIFT, OracleDbType.NVarchar2, 50);
                    cmd.Parameters[12].Value = dept.Select(x => x.LeaveShift).ToArray();
                    try
                    {
                        i = cmd.ExecuteNonQuery();
                        con.Close();
                        Returnstr = new Tuple<string, int>("Transaction", i);                        
                    }
                    catch (Exception ex)
                    {
                        con.Close();
                        i = -99;
                        Returnstr = new Tuple<string, int>("Transaction", i);
                        throw ex;
                    }
                }
                return i;
            }
            catch (Exception ex)
            {
                return i;
            }
        }
        public List<tbl_DeptLeaveRequest> GetAutoCompleteList(string search)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<tbl_DeptLeaveRequest> listObj = new List<tbl_DeptLeaveRequest>();
                DataSet ds = obj.GetAutoCompleteList(ConstantStoredProcedure.SP_BCL_GETEMPLOYEENAMEDEPT, search);
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        string[] vals = dtRow["EmpNameId"].ToString().Split('_');
                        var EmpID = Convert.ToString(vals[0]);
                        var EmpName =Convert.ToString(vals[1]);
                        var objAutoId = new tbl_DeptLeaveRequest();
                        objAutoId.EmployeeID = EmpID;
                        objAutoId.EmployeeName = EmpName;
                        listObj.Add(objAutoId);
                    }
                }
                return listObj;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public List<tbl_DeptLeaveRequest> GetLeaveRequest(string PA, string PSA, string DeptMasterID)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<tbl_DeptLeaveRequest> listObj = new List<tbl_DeptLeaveRequest>();
                DataSet ds = obj.GetLeaveRequestBYIdForDept(ConstantStoredProcedure.SP_GetLeaveRequestMasterDept, PA, PSA, DeptMasterID);
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        var LeaveID = dtRow["LEAVEID"].ToString();
                        var Descr = dtRow["Description"].ToString();
                        var objLeave = new tbl_DeptLeaveRequest();
                        objLeave.LeaveCode = LeaveID;
                        objLeave.LeaveCategory = Descr;
                        listObj.Add(objLeave);
                    }
                }
                return listObj;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public List<tbl_DeptLeaveRequest> GetBalanceForSingleEmp(string employeeid, string leaveid)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<tbl_DeptLeaveRequest> listObj = new List<tbl_DeptLeaveRequest>();
                DataSet dt = obj.GetBalanceByEmpIdDept(ConstantStoredProcedure.SP_BCLDB_sp_GetLeaveBalance, employeeid, leaveid);
                DataTable Dt1 = dt.Tables[0];
                if (Dt1.Rows.Count > 0)
                {
                    int balanceLeave = Convert.ToInt32(Dt1.Rows[0][0]);
                    foreach (var item in Dt1.Rows)
                    {
                        var getBalance = new tbl_DeptLeaveRequest();
                        getBalance.BalanceLeave = balanceLeave;
                        listObj.Add(getBalance);
                    }
                }
                return listObj;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public List<tblHolidayDept> GetHolidayList(string Empid)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<tblHolidayDept> listObj = new List<tblHolidayDept>();
                DataSet ds = obj.GetHolidaylist("BCL_DEPTHOLIDAYLIST", Empid);
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
                                var objLeave = new tblHolidayDept();
                                objLeave.HolidayDate = Convert.ToDateTime(date).ToString("yyyy/MM/dd");
                                objLeave.HolidayName = Convert.ToDateTime(Descr).ToString("yyyy/MM/dd");
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
        public Tuple<string, int> SaveLeaveRequest(tbl_DeptLeaveRequest objleave_request)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                
               
                List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVEID, OracleDbType.Int32, 50, objleave_request.LeavelistId.ToString(), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_FROMDATE, OracleDbType.NVarchar2, 120, Convert.ToString(objleave_request.FromDate), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_TODATE, OracleDbType.NVarchar2, 120, Convert.ToString(objleave_request.ToDate), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVESHIFT, OracleDbType.NVarchar2, 50, objleave_request.LeaveShift.ToString(), ParameterDirection.Input);
                //TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_ABSENCEHOUR, OracleDbType.Double, 50, objleave_request.AbsenceCEHour.ToString(), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERNAME, OracleDbType.NVarchar2, 1000, objleave_request.ApproverName.ToString(), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERID, OracleDbType.NVarchar2, 50, objleave_request.ApproverId.ToString(), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_REMARKS, OracleDbType.NVarchar2, 100, objleave_request.Remarks.ToString(), ParameterDirection.Input);
                //TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_BALANCELEAVE, OracleDbType.Double, 100, objleave_request.BalanceLeave.ToString(), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_TOTALAPPLIEDLEAVE, OracleDbType.Double, 100, objleave_request.TotalAppliedLeave.ToString(), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPLOYEEID, OracleDbType.NVarchar2, 50, objleave_request.EmployeeID.ToString(), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPLOYEENAME, OracleDbType.NVarchar2, 1000, objleave_request.EmployeeName.ToString(), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, "P_MESSAGE", OracleDbType.NVarchar2, 100, null, ParameterDirection.Output);
                TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_PA, OracleDbType.NVarchar2, 50, objleave_request.PA.ToString(), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_PSA, OracleDbType.NVarchar2, 50, objleave_request.PSA.ToString(), ParameterDirection.Input);
                if (objleave_request.UploadFileName == null)
                {
                    TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_FILENAME, OracleDbType.NVarchar2, 50, null, ParameterDirection.Input);
                }
                else
                {
                    TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_FILENAME, OracleDbType.NVarchar2, 50, objleave_request.UploadFileName.ToString(), ParameterDirection.Input);
                }
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_CreatedBy, OracleDbType.NVarchar2, 50, objleave_request.CreatedBy.ToString(), ParameterDirection.Input);
                TupleForOracleParameter(LstTupParameters, "P_AdminID", OracleDbType.NVarchar2, 50, objleave_request.AdminID.ToString(), ParameterDirection.Input);
                Tuple<string, int> Retunstr = obj.savedata("BCL_TEST_SAVEDEPTLEAVEREQUEST", LstTupParameters);
                return Retunstr;
            }
            catch (Exception ex)
            {
                Tuple<string, int> returndata = new Tuple<string, int>("Failed", 1);
                return returndata;
                throw;
            }
        }
        public string GetLeaveCode(string LeaveID)
        {
            DALBase obj = new DALBase(_config);
            List<tbl_LeaveRequest> listObj = new List<tbl_LeaveRequest>();
            List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVEID, OracleDbType.NVarchar2, 20, LeaveID, ParameterDirection.Input);
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
                        LeaveCode = dt.Rows[i][0].ToString();
                    }
                }
            }
            return LeaveCode;
        }
        public static void WriteDataTableToExcel(System.Data.DataTable dataTable, string worksheetName, string saveAsLocation, string ReporType)
        {
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook excelworkBook;
            Microsoft.Office.Interop.Excel.Worksheet excelSheet;
            Microsoft.Office.Interop.Excel.Range excelCellrange;
            byte[] fileContents;
            string ExcelFileName = worksheetName + '_' + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xlsx";
            var path = Path.Combine(
                       Directory.GetCurrentDirectory(), "wwwroot" + "/Documents/",
                       "sslm");
            FileInfo files = new FileInfo(Path.Combine(path));
            try
            {
                using (ExcelPackage package = new ExcelPackage(files))
                {
                    // add a new worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Employee");
                    worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                    package.Save(); //Save the workbook.
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public tbl_DeptLeaveRequest GetEmpDetails(string EmpID)
        {
            DALBase obj = new DALBase(_config);
            List<tbl_DeptLeaveRequest> listObj = new List<tbl_DeptLeaveRequest>();
            List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.NVarchar2, 20, EmpID.ToString(), ParameterDirection.Input);
            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EmpDetails, OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
            DataSet ds = obj.GetDatafromDatabase(ConstantStoredProcedure.SP_BCL_GETEMPDETAILS, LstTupParameters);
            tbl_DeptLeaveRequest tblempdetails = new tbl_DeptLeaveRequest();
            DataTable dt = ds.Tables[0];
            if (dt!=null && dt.Rows.Count > 0)
            {
                tblempdetails.EmployeeName =Convert.ToString(dt.Rows[0]["EMPNAME"]);
                tblempdetails.Grade = Convert.ToString(dt.Rows[0]["CostCenter"]);
                tblempdetails.PA = Convert.ToString(dt.Rows[0]["PA"]);
                tblempdetails.PSA = Convert.ToString(dt.Rows[0]["PSA"]);
                tblempdetails.ApproverId = Convert.ToString(dt.Rows[0]["ApproverIDName"]);
                tblempdetails.empPhoneNo = Convert.ToString(dt.Rows[0]["PHONENO"]);
                listObj.Add(tblempdetails);
            }
            return tblempdetails;
        }
        public Tuple<string, int> SaveFromSAP(List<tbl_LeaveQuotaSAP> tbl_User)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();

                foreach (var l in tbl_User)
                {
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.NVarchar2, 50, l.EMPLOYEEID.ToString(), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVEID, OracleDbType.NVarchar2, 50, l.LEAVECODE.ToString(), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVECATEGORY, OracleDbType.NVarchar2, 1000, l.LEAVECATEGORY.ToString(), ParameterDirection.Input);
                    string startdate = Convert.ToDateTime(l.STARTDATE).ToString("dd/MM/yyyy");
                    string endate = Convert.ToDateTime(l.ENDDATE).ToString("dd/MM/yyyy");
                    obj.TupleForOracleParameter(LstTupParameters, "P_STARTDATE", OracleDbType.NVarchar2, 120, startdate, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_ENDDATE", OracleDbType.NVarchar2, 120, endate, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_QUOTA", OracleDbType.Double, 100, l.QUOTA, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_BALANCELEAVE, OracleDbType.Double, 100, l.BALANCELEAVE, ParameterDirection.Input);
                    //string name = "vc1.ttl";
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERID, OracleDbType.NVarchar2, 50, l.APPROVERID, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERNAME, OracleDbType.NVarchar2, 50,l.APPROVERNAME, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_TOTALLEAVE, OracleDbType.Double, 50, l.CARRYFRWD, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_MESSAGE", OracleDbType.NVarchar2, 100, null, ParameterDirection.Output);
                    obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
                }
                Tuple<string, int> Returnstr = obj.savedata("BCL_SAVELEAVEQUOTASAP", LstTupParameters);
                return Returnstr;
            }
            catch (Exception ex)
            {
                Tuple<string, int> returndata = new Tuple<string, int>("Failed", 1);
                return returndata;
                throw;
            }
        }
        public int SaveConfirm(string AdminID, string userdata)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                var i = obj.Saveconfirm("BCL__SAVEDEPTLEAVE_CONFIRM", AdminID, userdata);
                return i;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int GetDEPTID(string spempdeptid)
        {
            DALBase obj = new DALBase(_config);
            DataSet ds = obj.GetDEPTID(ConstantStoredProcedure.SP_BCL_DEPTID, spempdeptid);
            DataTable dt = ds.Tables[0];
            int objdtval = 0;
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    objdtval = Convert.ToInt32(dt.Rows[0][0]);
                }
            }
            return objdtval;
        }
        public List<tbl_DeptLeaveRequest> GetLeaveDetailsForChange(int objLeaverequestId)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<tbl_DeptLeaveRequest> objList = new List<tbl_DeptLeaveRequest>();
                DataSet ds = obj.GetEmpDetails(ConstantStoredProcedure.SP_BCL_GETLEAVEFORCHNAGE_DEPT, objLeaverequestId);
                DataTable dt = ds.Tables[0];
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var objtblLeaveRequest = new tbl_DeptLeaveRequest();
                            objtblLeaveRequest.LeaveRequestId = Convert.ToString(dt.Rows[i]["LEAVEREQUESTID"]);
                            objtblLeaveRequest.LeaveId = Convert.ToInt32(dt.Rows[i]["LEAVEID"]);
                            objtblLeaveRequest.LeaveCategory = Convert.ToString(dt.Rows[i]["LEAVECATEGORY"]);
                            objtblLeaveRequest.FromDate = Convert.ToDateTime(dt.Rows[i]["FROMDATE"]).ToString("dd/MM/yyyy");
                            objtblLeaveRequest.ToDate = Convert.ToDateTime(dt.Rows[i]["TODATE"]).ToString("dd/MM/yyyy");
                            objtblLeaveRequest.LeaveShift = Convert.ToString(dt.Rows[i]["LEAVESHIFT"]);
                            objtblLeaveRequest.ApproverId = Convert.ToString(dt.Rows[i]["APPROVERID"]);
                            objtblLeaveRequest.ApproverName = Convert.ToString(dt.Rows[i]["APPROVERNAME"]);
                            objtblLeaveRequest.AbsenceCEHour = Convert.ToDouble(dt.Rows[i]["ABSENCEHOUR"]);
                            objtblLeaveRequest.Remarks = Convert.ToString(dt.Rows[i]["REMARKS"]);
                            objtblLeaveRequest.EmployeeID = Convert.ToString(dt.Rows[i]["EMPLOYEEID"]);
                            objtblLeaveRequest.PA = Convert.ToString(dt.Rows[i]["PA"]);
                            objtblLeaveRequest.PSA = Convert.ToString(dt.Rows[i]["PSA"]);
                            objtblLeaveRequest.EmployeeName = Convert.ToString(dt.Rows[i]["EMPLOYEENAME"]);
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
    }
}
