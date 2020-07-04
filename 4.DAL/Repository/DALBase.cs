using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Data;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
//using Oracle.DataAccess.Types;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace LeaveSolution.DAL.Repository
{
    public class DALBase
    {


        //private readonly IConfiguration configuration;
        public readonly IConfiguration _config;
        public DALBase(IConfiguration config)
        {
            _config = config;
        }





        public void TupleForOracleParameter(List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters, string paraname, OracleDbType OraclePara, int lenght, string value, ParameterDirection paraDirection)
        {
            LstTupParameters.Add(new Tuple<string, OracleDbType, int, object, ParameterDirection>(paraname, OraclePara, lenght, value, paraDirection));
        }
        //public GenearteOTP()
        //{

        //}
        public OracleCommand cmd;
        public DataTable dt;
        public OracleDataAdapter da;
        public OracleConnection ocon;
        DataSet dataSet = new DataSet();

        // DataSet dataSet = new DataSet();
        public string DBConnection
        {
            get
            {
                return _config.GetSection("Data").GetSection("DefaultConnection").Value;
            }
        }
        //this is common function to get data from databse single or multiple



        public DataSet GetDatafromDatabase(string storedProcedureName, List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters)
        {

            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(storedProcedureName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            foreach (var item in LstTupParameters)
            {
                cmd.Parameters.Add(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5);
            }

            try
            {
                da.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                ocon.Close();
            }


        }
        public DataSet GetById(string storedProcedureName, string id, string fromdata)
        {

            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(storedProcedureName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;

            cmd.Parameters.Add("P_DATAFROM", OracleDbType.Varchar2, 20, fromdata, ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_EMPID, OracleDbType.Varchar2, 20, id.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add("P_RETURNVALUE", OracleDbType.Int32, 20, null, ParameterDirection.Output);
            cmd.Parameters.Add(ConstantsVariables.P_PHONENO, OracleDbType.RefCursor, 20, null, ParameterDirection.Output);

            try
            {
                da.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ocon.Close();
            }


        }
        //EMployeeReport

        public DataSet GetEmployeeReport(string storedProcedureName, List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters)
        {

            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(storedProcedureName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            foreach (var item in LstTupParameters)
            {
                cmd.Parameters.Add(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5);
            }

           // cmd.Parameters.Add(ConstantsVariables.P_EMPID, OracleDbType.Varchar2, 20, empid.ToString(), ParameterDirection.Input);
           // cmd.Parameters.Add(ConstantsVariables.P_FROMDATE, OracleDbType.Varchar2, 20, fromdata.ToString(), ParameterDirection.Input);
           // cmd.Parameters.Add(ConstantsVariables.P_TODATE, OracleDbType.Varchar2, 20, todate.ToString(), ParameterDirection.Input);
            //cmd.Parameters.Add(ConstantsVariables.P_LeaveDetails, OracleDbType.RefCursor, 100, null, ParameterDirection.Output);

            try
            {
                da.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ocon.Close();
            }


        }

        //save data
        public int Saveconfirm(string StoredProcedure,string AdminID,string userdata)
        {
            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(StoredProcedure, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            ocon.Open();
            try
            {
                cmd.Parameters.Add("P_AdminID", OracleDbType.NVarchar2, 500, AdminID, ParameterDirection.Input);
                cmd.Parameters.Add("P_Userdata", OracleDbType.NVarchar2, 500, userdata, ParameterDirection.Input);
                cmd.Parameters.Add("P_RETURNVALUE", OracleDbType.Int32, 100, null, ParameterDirection.Output);
                 cmd.ExecuteNonQuery();
                var RetunVal = Convert.ToInt32((decimal)(OracleDecimal)(cmd.Parameters["P_RETURNVALUE"].Value));
                return Convert.ToInt32(RetunVal);

            }
            catch (Exception ex)
            {

                throw ex;
            }
            {

            }
        }
        //   public IList<tbl_OTPG> savedata(string storedProcedureName) 
        public Tuple<string, int> savedata(string storedProcedureName, List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters)

        {
            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(storedProcedureName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            // tbl_OTPG t = new tbl_OTPG();
            foreach (var item in LstTupParameters)
            {
                cmd.Parameters.Add(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5);

            }
            try
            {
                ocon.Open();
                cmd.ExecuteNonQuery();
                var statusoftrans = 0;
                var ReturnMessage = Convert.ToString((string)(OracleString)(cmd.Parameters["P_MESSAGE"].Value));
                var RetunVal = Convert.ToInt32((decimal)(OracleDecimal)(cmd.Parameters["P_RETURNVALUE"].Value));
                //  var IsPreviousPassword = Convert.ToInt32((decimal)(OracleDecimal)(cmd.Parameters["v_IsPreviousPass"].Value));
                // var RetunVal = Convert.ToInt32((decimal)(OracleDecimal)(cmd.Parameters["P_RETURNVALUE"].Value));
                Tuple<string, int> returndata = new Tuple<string, int>(ReturnMessage, RetunVal);
                return returndata;
            }
            catch (Exception ex)
            {
                Tuple<string, int> returndata = new Tuple<string, int>("Failed", 1);
                return returndata;
                throw;
            }
        }
        public Tuple<string, int> savedataFSAP(string storedProcedureName, List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters)

        {
            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(storedProcedureName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            // tbl_OTPG t = new tbl_OTPG();
            foreach (var item in LstTupParameters)
            {
                cmd.Parameters.Add(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5);

            }
            try
            {
                ocon.Open();
                cmd.ExecuteNonQuery();
                var statusoftrans = 0;
                //var ReturnMessage = Convert.ToString((string)(OracleString)(cmd.Parameters["P_MESSAGE"].Value));
                var RetunVal = Convert.ToInt32((decimal)(OracleDecimal)(cmd.Parameters["P_RETURNVALUE"].Value));
                //  var IsPreviousPassword = Convert.ToInt32((decimal)(OracleDecimal)(cmd.Parameters["v_IsPreviousPass"].Value));
                // var RetunVal = Convert.ToInt32((decimal)(OracleDecimal)(cmd.Parameters["P_RETURNVALUE"].Value));
                Tuple<string, int> returndata = new Tuple<string, int>("", RetunVal);
                return returndata;
            }
            catch (Exception ex)
            {
                Tuple<string, int> returndata = new Tuple<string, int>("Failed", 1);
                return returndata;
                throw;
            }
        }
        public DataSet GetLeaveRequestBYIdForDept(string spName,string PA,String PSA)
        {
            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(spName, ocon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Add(ConstantsVariables.P_PA, OracleDbType.Varchar2, 20, PA.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_PSA, OracleDbType.Varchar2, 20, PSA.ToString(), ParameterDirection.Input);
            //cmd.Parameters.Add(ConstantsVariables.P_Category, OracleDbType.Varchar2, 20, PSA.ToString(), ParameterDirection.Input);

            cmd.Parameters.Add(ConstantsVariables.P_LeaveDetails, OracleDbType.RefCursor, 100, null, ParameterDirection.Output);
            // cmd.Parameters.Add(ConstantsVariables.P_DESCRIPTION, OracleDbType.RefCursor, 100, null, ParameterDirection.Output);
            try
            {
                da.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                ocon.Close();
            }
        }
        public DataSet GetLeaveRequestBYId(string spName, string PA, string PSA,string Category)
        {
            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(spName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Add(ConstantsVariables.P_PA, OracleDbType.Varchar2, 20, PA.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_PSA, OracleDbType.Varchar2, 20, PSA.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_Category, OracleDbType.Varchar2, 20, Category.ToString(), ParameterDirection.Input);

            cmd.Parameters.Add(ConstantsVariables.P_LeaveDetails, OracleDbType.RefCursor, 100, null, ParameterDirection.Output);
            
            // cmd.Parameters.Add(ConstantsVariables.P_DESCRIPTION, OracleDbType.RefCursor, 100, null, ParameterDirection.Output);
            try
            {
                da.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
                {

                throw;
            }
            finally
            {
                ocon.Close();
            }
        }

        public DataSet GetApproverEmail(string spName, string EmpID)
        {
            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(spName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Add("EmpID", OracleDbType.Varchar2, 20, EmpID.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_LeaveDetails, OracleDbType.RefCursor, 100, null, ParameterDirection.Output);
            try
            {
                da.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                ocon.Close();
            }
        }

        public DataSet GetMulipleDataBYId(string spName, string strLeaveRqtId)
        {
            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(spName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Add(ConstantsVariables.P_LEAVEREQUESTID, OracleDbType.Varchar2, strLeaveRqtId.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_EmpDetails, OracleDbType.RefCursor, 100, null, ParameterDirection.Output);
            try
            {
                da.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                ocon.Close();
            }
        }
        //For Department 
        public DataSet GetLeaveRequestBYIdForDept(string spName, string PA, string PSA, string DeptMasterID)
        {
            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(spName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Add(ConstantsVariables.P_PA, OracleDbType.Varchar2, 20, PA.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_PSA, OracleDbType.Varchar2, 20, PSA.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_LeaveDetails, OracleDbType.RefCursor, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(ConstantsVariables.P_DeptMasterID, OracleDbType.Varchar2, 20, DeptMasterID.ToString(), ParameterDirection.Input);
            // cmd.Parameters.Add(ConstantsVariables.P_DESCRIPTION, OracleDbType.RefCursor, 100, null, ParameterDirection.Output);
            try
            {
                da.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                ocon.Close();
            }
        }
        public DataSet GetHolidaylist(string spName, string EmpId)
        {
            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(spName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Add(ConstantsVariables.P_EMPID, OracleDbType.Varchar2, 20, EmpId.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_LeaveDetails, OracleDbType.RefCursor, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(ConstantsVariables.P_DESCRIPTION, OracleDbType.RefCursor, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(ConstantsVariables.P_STATUS, OracleDbType.RefCursor, 100, null, ParameterDirection.Output);
            cmd.Parameters.Add(ConstantsVariables.P_STATUS, OracleDbType.RefCursor, 100, null, ParameterDirection.Output);
            try
            {
                da.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                ocon.Close();
            }
        }
        /// <summary>
        /// For Get Balance From LeaveQuota
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="employeeid"></param>
        /// <param name="leavecode"></param>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <returns></returns>
        public double GetBalanceByEmpId(string storedProcedureName, string employeeid, string leaveid)
        {

            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(storedProcedureName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Add(ConstantsVariables.P_EMPID, OracleDbType.Varchar2, 20, employeeid.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_LEAVEID, OracleDbType.Varchar2, 20, leaveid.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_BALANCELEAVE, OracleDbType.RefCursor, 20, null, ParameterDirection.Output);

            try
            {
                da.Fill(dataSet);
                if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    return Convert.ToDouble(dataSet.Tables[0].Rows[0][0]);
                }
                else
                    return 0;

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                ocon.Close();
            }
        }
        public DataSet GetBalanceByEmpIdDept(string storedProcedureName, string employeeid, string leaveid)
        {

            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(storedProcedureName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Add(ConstantsVariables.P_EMPID, OracleDbType.Varchar2, 20, employeeid.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_LEAVEID, OracleDbType.Varchar2, 20, leaveid.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_BALANCELEAVE, OracleDbType.RefCursor, 20, null, ParameterDirection.Output);

            try
            {
                da.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                ocon.Close();
            }
        }
        /// <summary>
        /// For Dashboard
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="employeeid"></param>
        /// <returns></returns>
        public DataSet GetDashboardDetailsEmpId(string storedProcedureName, string empid)
        {
            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(storedProcedureName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Add(ConstantsVariables.P_EMPID, OracleDbType.Varchar2, 20, empid.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_DashboardDetails, OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
            cmd.Parameters.Add(ConstantsVariables.P_DashboardDetailsforPending, OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
            try
            {
                da.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                ocon.Close();
            }
        }
        public DataSet GetLeaveCode(string storedProcedureName, int employeeid, string Description)
        {

            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(storedProcedureName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Add(ConstantsVariables.P_EMPID, OracleDbType.Varchar2, 20, employeeid.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_LEAVECODE, OracleDbType.Varchar2, 20, Description.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_LEAVECODE, OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
            try
            {
                da.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                ocon.Close();
            }


        }
        public DataSet GetAutoCompleteList(string storedProcedureName, string search)
        {
            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(storedProcedureName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Add(ConstantsVariables.P_EMPID, OracleDbType.Varchar2, 20, search.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_EmpDetails, OracleDbType.RefCursor, 100, null, ParameterDirection.Output);
            // cmd.Parameters.Add(ConstantsVariables.P_DESCRIPTION, OracleDbType.RefCursor, 100, null, ParameterDirection.Output);
            try
            {
                da.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                ocon.Close();
            }
        }

        public Tuple<string, int> ExecuteQueryWithOutputParameterWithTable(string storedProcedureName, List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters, out DataTable Dt)
        {
            string ReturnMessage = string.Empty;
            int RetunVal;
            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(storedProcedureName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;

            foreach (var item in LstTupParameters)
            {
                cmd.Parameters.Add(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5);
            }
            try
            {
                ocon.Open();
                da.Fill(dataSet);
                if (((System.Data.InternalDataCollectionBase)(dataSet.Tables)).Count > 0)
                {
                    Dt = dataSet.Tables[0];
                }
                else
                {
                    Dt = new DataTable();
                }

                ReturnMessage = Convert.ToString((string)(OracleString)(cmd.Parameters["P_MESSAGE"].Value));
                RetunVal = Convert.ToInt32((decimal)(OracleDecimal)(cmd.Parameters["P_RETURNVALUE"].Value));

                Tuple<string, int> returndata = new Tuple<string, int>(ReturnMessage, RetunVal);

                return returndata;
            }
            catch (Exception x)
            {
                throw;
            }
            finally
            {
                ocon.Close();
            }

        }
        public DataSet GetEmpDetails(string storedProcedureName, int objLeaveRequestId)
        {
            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(storedProcedureName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Add(ConstantsVariables.P_EMPID, OracleDbType.Int32, 20, objLeaveRequestId, ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_EmpDetails, OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
            try
            {
                da.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                ocon.Close();
            }
        }
        public DataSet GetEmpLeaveDetails(string storedProcedureName,string SchedulerType)
        {
            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(storedProcedureName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            
                  cmd.Parameters.Add("P_Schedulertype", OracleDbType.NVarchar2, 20, SchedulerType, ParameterDirection.Input);
            cmd.Parameters.Add(ConstantsVariables.P_EmpDetails, OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
            try
            {
                da.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                ocon.Close();
            }
        }

        public DataSet GetDEPTID(string storedProcedureName, string empDeptID)
        {

            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(storedProcedureName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            //      ConstantsVariables.P_EMPID  ConstantsVariables.P_LEAVECODE
            cmd.Parameters.Add("P_SPEMPLOYEEID", OracleDbType.Varchar2, 20, empDeptID.ToString(), ParameterDirection.Input);
            cmd.Parameters.Add("P_DeptID", OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
            try
            {
                da.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                ocon.Close();
            }


        }
        public void DeleteUser(string storedProcedureName, string userid)
        {           
            ocon = new OracleConnection(DBConnection);
            cmd = new OracleCommand(storedProcedureName, ocon);
            da = new OracleDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            ocon.Open();
            cmd.Parameters.Add(ConstantsVariables.P_EMPID, OracleDbType.Varchar2, 20, userid.ToString(), ParameterDirection.Input);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                ocon.Close();
            }
        }
    }
}

