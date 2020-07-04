using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Interfaces;
using LeaveSolution.DAL.Data;
using System.Data;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
//using Oracle.DataAccess.Types;
using Oracle.ManagedDataAccess.Types;

namespace LeaveSolution.DAL.Repository
{
    public class LeaveWithdrawalRepositary : ILeaveWithdrawalRepository
    {
        public readonly IConfiguration _config;
        public LeaveWithdrawalRepositary(IConfiguration config)
        {
            _config = config;
        }

        public List<tbl_Leavewithdrawal> GetLeavesforWithdrawal(string EmployeeId)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<tbl_Leavewithdrawal> listObj = new List<tbl_Leavewithdrawal>();
                List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
                obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPLOYEEID, OracleDbType.NVarchar2, 20, EmployeeId.ToString(), ParameterDirection.Input);
                obj.TupleForOracleParameter(LstTupParameters, "P_RETURNDATA", OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
                obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
                DataSet ds = obj.GetDatafromDatabase("BCL_TEST_GETLEAVEFORWITHDRAWAL", LstTupParameters);
                var ReturnVal = Convert.ToInt32((decimal)(OracleDecimal)(obj.cmd.Parameters["P_RETURNVALUE"].Value));
                if (ReturnVal == 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var tblleavewithdrawal = new tbl_Leavewithdrawal();
                            tblleavewithdrawal.LeaveRequestId = Convert.ToInt32(dt.Rows[i]["LEAVEREQUESTID"]);
                            tblleavewithdrawal.LeaveId = Convert.ToInt32(dt.Rows[i]["LEAVEID"]);
                            tblleavewithdrawal.LeaveCode = Convert.ToString(dt.Rows[i]["LEAVECODE"]);
                            tblleavewithdrawal.FromDate = Convert.ToDateTime(dt.Rows[i]["FROMDATE"]);
                            tblleavewithdrawal.FromDateString = tblleavewithdrawal.FromDate.ToString("dd/MM/yyyy").Replace('-','/');
                            tblleavewithdrawal.ToDate = Convert.ToDateTime(dt.Rows[i]["TODATE"]);
                            tblleavewithdrawal.ToDateString = tblleavewithdrawal.ToDate.ToString("dd/MM/yyyy").Replace('-', '/');
                            tblleavewithdrawal.LeaveShift = Convert.ToString(dt.Rows[i]["LEAVESHIFT"]);
                            tblleavewithdrawal.ApproverName = Convert.ToString(dt.Rows[i]["APPROVERNAME"]);
                            tblleavewithdrawal.ApproverId = Convert.ToString(dt.Rows[i]["APPROVERID"]);
                            tblleavewithdrawal.Status = Convert.ToString(dt.Rows[i]["STATUS"]);
                            tblleavewithdrawal.TotalLeaves = Convert.ToDouble(dt.Rows[i]["TOTALAPPLIEDLEAVE"]);
                            tblleavewithdrawal.LeaveAppliedDate = Convert.ToDateTime(dt.Rows[i]["LEAVEAPPLIEDDATE"]);
                            tblleavewithdrawal.LeaveAppliedDateString = tblleavewithdrawal.LeaveAppliedDate.ToString("dd/MM/yyyy").Replace('-', '/'); ;
                            tblleavewithdrawal.EmployeeID = Convert.ToString(dt.Rows[i]["EMPLOYEEID"]);
                            tblleavewithdrawal.EmployeeName = Convert.ToString(dt.Rows[i]["EMPLOYEENAME"]);
                            tblleavewithdrawal.LeaveCategory = Convert.ToString(dt.Rows[i]["LEAVECATEGORY"]);
                            tblleavewithdrawal.UploadFileName = Convert.ToString(dt.Rows[i]["FILENAME"]);
                            listObj.Add(tblleavewithdrawal);
                        }
                    }
                }
                else if (ReturnVal == 1 || ReturnVal == 2)
                {
                    var tbleavewithdrawal = new tbl_Leavewithdrawal();
                    tbleavewithdrawal.ReturnValue = ReturnVal;
                    listObj.Add(tbleavewithdrawal);
                }
                return listObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public Tuple<string, int> SaveWithdrawalLeaveRequest(tbl_Leavewithdrawal tblLeaveWithdrawal)
        {
            try
            {
                Tuple<string, int> returnStr = new Tuple<string, int>("Failed", 1);
                List<Tuple<string, OracleDbType, int, object, ParameterDirection>> Lsttuples = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
                DALBase obj = new DALBase(_config);
                Lsttuples = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
                obj.TupleForOracleParameter(Lsttuples, ConstantsVariables.P_LEAVEREQUESTID, OracleDbType.NVarchar2, 20, tblLeaveWithdrawal.LeaveRequestId.ToString(), ParameterDirection.Input);
                obj.TupleForOracleParameter(Lsttuples, ConstantsVariables.P_STATUS, OracleDbType.NVarchar2, 1000, tblLeaveWithdrawal.Status, ParameterDirection.Input);
                obj.TupleForOracleParameter(Lsttuples, ConstantsVariables.P_EMPLOYEEID, OracleDbType.NVarchar2, 500, tblLeaveWithdrawal.EmployeeID.ToString(), ParameterDirection.Input);
                obj.TupleForOracleParameter(Lsttuples, ConstantsVariables.P_TOTALAPPLIEDLEAVE, OracleDbType.Double, 20, tblLeaveWithdrawal.TotalLeaves.ToString(), ParameterDirection.Input);
                obj.TupleForOracleParameter(Lsttuples, "P_MESSAGE", OracleDbType.NVarchar2, 100, null, ParameterDirection.Output);
                obj.TupleForOracleParameter(Lsttuples, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
               obj.TupleForOracleParameter(Lsttuples, "P_ErrorMsg", OracleDbType.NVarchar2, 1000,Convert.ToString(tblLeaveWithdrawal.ErrorMsg), ParameterDirection.Input);
                obj.TupleForOracleParameter(Lsttuples, "P_Indicator", OracleDbType.NVarchar2, 20, Convert.ToString(tblLeaveWithdrawal.Indicator), ParameterDirection.Input);
                returnStr = obj.savedata("BCL_TEST_SAVELEAVEWITHDRAWAL", Lsttuples);
                return returnStr;
            }
            catch (Exception ex)
            {
                Tuple<string, int> returnData = new Tuple<string, int>("Failed", 0);
                return returnData;
                throw ex;
            }
        }
    }
}