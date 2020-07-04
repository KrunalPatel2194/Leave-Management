using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Data;
using LeaveSolution.DAL.Interfaces;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
namespace LeaveSolution.DAL.Repository
{
    public class LeaveHistoryRepository : ILeaveHistoryRepository
    {
        public static void TupleForOracelParameter(List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters, string paraname, OracleDbType oraclePara, int length, string value, ParameterDirection paraDirection)
        {
            LstTupParameters.Add(new Tuple<string, OracleDbType, int, object, ParameterDirection>(paraname, oraclePara, length, value, paraDirection));
        }
        public readonly IConfiguration _config;
        public LeaveHistoryRepository(IConfiguration config)
        {
            _config = config;
        }
        public List<tbl_LeaveHistory> GetLeaveHistory(string empid,string year)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<tbl_LeaveHistory> objList = new List<tbl_LeaveHistory>();
                List<Tuple<string, OracleDbType, int, object, ParameterDirection>> lstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
                obj.TupleForOracleParameter(lstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.NVarchar2, 20, empid.ToString(), ParameterDirection.Input);
                obj.TupleForOracleParameter(lstTupParameters, "P_LeaveHistory", OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
                obj.TupleForOracleParameter(lstTupParameters, "P_Year", OracleDbType.NVarchar2, 20, year, ParameterDirection.Input);
                DataSet ds = obj.GetDatafromDatabase("BCL_TEST2_GETLEAVEHISTORY", lstTupParameters);
                DataTable dt = ds.Tables[0];
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var objTblLeaveHistory = new tbl_LeaveHistory();
                            objTblLeaveHistory.LeaveCategory = Convert.ToString(dt.Rows[i]["LEAVECATEGORY"]);
                            objTblLeaveHistory.LeaveAppliedDate = Convert.ToDateTime(dt.Rows[i]["LEAVEAPPLIEDDATE"]).ToString("dd/MM/yyyy").Replace('-','/');
                            objTblLeaveHistory.FromDate = Convert.ToDateTime(dt.Rows[i]["FROMDATE"]).ToString("dd/MM/yyyy").Replace('-', '/'); ;
                            objTblLeaveHistory.ToDate = Convert.ToDateTime(dt.Rows[i]["TODATE"]).ToString("dd/MM/yyyy").Replace('-', '/'); ;
                            objTblLeaveHistory.TotalLeaves = Convert.ToDouble(dt.Rows[i]["TOTALAPPLIEDLEAVE"]);
                            objTblLeaveHistory.ApproverName = Convert.ToString(dt.Rows[i]["APPROVERNAME"]);
                            objTblLeaveHistory.Status = Convert.ToString(dt.Rows[i]["STATUS"]);
                            if(Convert.ToString(dt.Rows[i]["LEAVESHIFT"]) == "F")
                            {
                                objTblLeaveHistory.LeaveType = "First Half";

                            }
                            else if(Convert.ToString(dt.Rows[i]["LEAVESHIFT"]) == "S")
                            {
                                objTblLeaveHistory.LeaveType = "Second Half";

                            }
                            else
                            {
                                objTblLeaveHistory.LeaveType = "Full Day";

                            }
                            objTblLeaveHistory.Indicator = dt.Rows[i]["SAPSUCCESSFLAG"].ToString();
                            objTblLeaveHistory.ErrorMsg = dt.Rows[i]["ERRORMSG"].ToString();
                            objTblLeaveHistory.LeaveCode = dt.Rows[i]["Leavecode"].ToString();
                            objList.Add(objTblLeaveHistory);
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
        //For Report
        public List<tbl_LeaveHistory> GetEmployeeReport(string empid, string fromdate, string todate)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<tbl_LeaveHistory> objList = new List<tbl_LeaveHistory>();
                List<Tuple<string, OracleDbType, int, object, ParameterDirection>> lstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
                obj.TupleForOracleParameter(lstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.NVarchar2, 20, empid.ToString(), ParameterDirection.Input);
                obj.TupleForOracleParameter(lstTupParameters, ConstantsVariables.P_FROMDATE, OracleDbType.NVarchar2, 20, fromdate.ToString(), ParameterDirection.Input);
                obj.TupleForOracleParameter(lstTupParameters, ConstantsVariables.P_TODATE, OracleDbType.NVarchar2, 20, todate.ToString(), ParameterDirection.Input);
                obj.TupleForOracleParameter(lstTupParameters, "P_LeaveHistory", OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
                DataSet ds = obj.GetEmployeeReport(ConstantStoredProcedure.SP_BCL_EMPLOYEEREPORT, lstTupParameters);
                DataTable dt = ds.Tables[0];
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var objTblLeaveHistory = new tbl_LeaveHistory();
                            objTblLeaveHistory.LeaveCategory = Convert.ToString(dt.Rows[i]["LEAVECATEGORY"]);
                            objTblLeaveHistory.LeaveAppliedDate = Convert.ToDateTime(dt.Rows[i]["LEAVEAPPLIEDDATE"]).ToString("dd/MM/yyyy");
                            objTblLeaveHistory.FromDate = Convert.ToDateTime(dt.Rows[i]["FROMDATE"]).ToString("dd/MM/yyyy");
                            objTblLeaveHistory.ToDate = Convert.ToDateTime(dt.Rows[i]["TODATE"]).ToString("dd/MM/yyyy");
                            objTblLeaveHistory.TotalLeaves = Convert.ToDouble(dt.Rows[i]["TOTALAPPLIEDLEAVE"]);
                            objTblLeaveHistory.ApproverName = Convert.ToString(dt.Rows[i]["APPROVERNAME"]);
                            objTblLeaveHistory.Status = Convert.ToString(dt.Rows[i]["STATUS"]);
                            objTblLeaveHistory.LeaveType = Convert.ToString(dt.Rows[i]["LEAVESHIFT"]);
                            objTblLeaveHistory.Indicator = dt.Rows[i]["SAPSUCCESSFLAG"].ToString();
                            objTblLeaveHistory.ErrorMsg = dt.Rows[i]["ERRORMSG"].ToString();
                            objList.Add(objTblLeaveHistory);
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
        public void LeaveHistoryExcel(string empid)
        {
            DALBase obj = new DALBase(_config);
            List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.NVarchar2, 20, empid, ParameterDirection.Input);
            obj.DeleteUser(ConstantStoredProcedure.SP_BCL_GETLEAVEHISTORYExcel, empid);
        }
    }
}

