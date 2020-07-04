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
using System.Globalization;
namespace LeaveSolution.DAL.Repository
{
    public class LeaveQuotaRepository : ILeaveQuotaRepository
    {
        public static void TupleForOracleParameter(List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters, string paraname, OracleDbType OraclePara, int lenght, string value, ParameterDirection paraDirection)
        {
            LstTupParameters.Add(new Tuple<string, OracleDbType, int, object, ParameterDirection>(paraname, OraclePara, lenght, value, paraDirection));
        }
        public readonly IConfiguration _config;
        public LeaveQuotaRepository(IConfiguration config)
        {
            _config = config;
        }
        public List<tbl_LeaveQuota> GetLeaveQuota(string objEmpID)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<tbl_LeaveQuota> listObj = new List<tbl_LeaveQuota>();
                List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
                TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPLOYEEID, OracleDbType.NVarchar2, 50, objEmpID.ToString(), ParameterDirection.Input);
                obj.TupleForOracleParameter(LstTupParameters, "P_QuotaDetails", OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
                DataSet ds = obj.GetDatafromDatabase(ConstantStoredProcedure.SP_BCL_GETLeaveQuotaDetails, LstTupParameters);
                DataTable dt = ds.Tables[0];
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var objtblLeaveQuota = new tbl_LeaveQuota();
                            objtblLeaveQuota.LeaveCategory = Convert.ToString(dt.Rows[i]["LEAVECATEGORY"]);
                            objtblLeaveQuota.BalanceLeave = Convert.ToInt32(dt.Rows[i]["BALANCELEAVE"]);
                            objtblLeaveQuota.Quota = Convert.ToInt32(dt.Rows[i]["QUOTA"]);
                            objtblLeaveQuota.FromDate = Convert.ToDateTime(dt.Rows[i]["STARTDATE"]).ToString("dd/MM/yyyy");
                            objtblLeaveQuota.ToDate = Convert.ToDateTime(dt.Rows[i]["ENDDATE"]).ToString("dd/MM/yyyy");
                            objtblLeaveQuota.ApproverName = dt.Rows[i]["APPROVERNAME"].ToString();
                            objtblLeaveQuota.TotalLeaves = Convert.ToInt32(dt.Rows[i]["TOTALLEAVES"]);
                            listObj.Add(objtblLeaveQuota);
                        }
                    }
                }
                return listObj;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
