using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Interfaces;
using LeaveSolution.DAL.Data;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace LeaveSolution.DAL.Repository
{
    public class LeaveDashboardRepository : ILeaveDashboardRepositary
    {
        public static void TupleForOracleParameter(List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters, string paraname, OracleDbType oraclePara, int length, string value, ParameterDirection paraDirection)
        {
            LstTupParameters.Add(new Tuple<string, OracleDbType, int, object, ParameterDirection>(paraname, oraclePara, length, value, paraDirection));
        }
        public readonly IConfiguration _config;
        public LeaveDashboardRepository(IConfiguration config)
        {
            _config = config;
        }
        public List<tbl_LeaveDashboard> GetDashboardLeaveList(string empid)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<tbl_LeaveDashboard> listobj = new List<tbl_LeaveDashboard>();
                List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
                obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.NVarchar2, 20, empid.ToString(), ParameterDirection.Input);
                obj.TupleForOracleParameter(LstTupParameters, "P_DashboardDetails", OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
                DataSet ds = obj.GetDatafromDatabase(ConstantStoredProcedure.SP_BCL_DashboardDetails, LstTupParameters);
                DataTable dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        var objLeave = new tbl_LeaveDashboard();
                        var objLeaveCat = dtRow["LEAVECATEGORY"].ToString();
                        var objQuota = dtRow["QUOTA"].ToString();
                        var objApprovedLeave = dtRow["Approved"].ToString();
                        var objPendingLeave = dtRow["Pending"].ToString();
                        var objEndDate = dtRow["ENDDATE"].ToString();

                        objLeave.LeaveCategory = objLeaveCat;
                        objLeave.Quota = Convert.ToDouble(objQuota);
                        objLeave.ApprovedLeave = objApprovedLeave;
                        objLeave.PendingLeave = Convert.ToDouble(objPendingLeave);
                        objLeave.EndDate = objEndDate;
                        objLeave.EndDate = Convert.ToDateTime(objEndDate).ToString("dd/MM/yyyy");
                        listobj.Add(objLeave);
                    }
                }
                return listobj;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
