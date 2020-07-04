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
using System.Linq;

namespace LeaveSolution.DAL.Repository
{
    public class LeaveApprovalRepositary : ILeaveApprovalRepositary
    {
        public readonly IConfiguration _config;
        public LeaveApprovalRepositary(IConfiguration config)
        {
            _config = config;
        }
        public List<tbl_LeaveApproval> GetApprovalDetails(string ApproverId)
        {
            DALBase obj = new DALBase(_config);
            // int ApproverId = tblapprover.ApproverId;
            List<tbl_LeaveApproval> listObj = new List<tbl_LeaveApproval>();
            List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            // obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.Varchar2, 20, ApproverId.ToString(), ParameterDirection.Input);
            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.NVarchar2, 20, ApproverId.ToString(), ParameterDirection.Input);
            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EmpDetails, OracleDbType.RefCursor, 1, null, ParameterDirection.Output);
            DataSet ds = obj.GetDatafromDatabase(ConstantStoredProcedure.SP_GETAPPROVERDETAILS, LstTupParameters);
            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var tblleaveapproval = new tbl_LeaveApproval();
                    tblleaveapproval.ApproverMailID = dt.Rows[i][0].ToString();
                    tblleaveapproval.ApproverMobNo = dt.Rows[i][1].ToString();
                    tblleaveapproval.HODMailID = dt.Rows[i][2].ToString();
                    tblleaveapproval.HODMobNo = dt.Rows[i][3].ToString();

                    if (dt.Rows[i][4].ToString() != null || dt.Rows[i][4].ToString() != "")
                    {
                        //tblleaveapproval.deptID = dt.Rows[i][3].ToString();

                    }
                    //tblleaveapproval.FromDateString = tblleaveapproval.FromDate.ToString("dd/MM/yyyy");
                    tblleaveapproval.DeptID = dt.Rows[i][5].ToString();
                    listObj.Add(tblleaveapproval);

                }

            }
            //else
            //{
            //    var tbleaveappr = new tbl_LeaveApproval();
            //    tbleaveappr.ReturnValue = 0;
            //    listObj.Add(tbleaveappr);
            //}
            return listObj;
        }
        //public string GetMappingData(string LeaveCode)
        //{
        //    DALBase obj = new DALBase(_config);
        //    // int ApproverId = tblapprover.ApproverId;
        //    List<tbl_LeaveRequest> listObj = new List<tbl_LeaveRequest>();
        //    List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
        //    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVEID, OracleDbType.NVarchar2, 20, LeaveCode, ParameterDirection.Input);
        //    obj.TupleForOracleParameter(LstTupParameters, "P_QtyType", OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
        //    obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 20, null, ParameterDirection.Output);

        //    DataSet ds = obj.GetDatafromDatabase("BCL_MAPPINGDATAFORAPPROVAL", LstTupParameters);
        //    var ReturnVal = Convert.ToInt32((decimal)(OracleDecimal)(obj.cmd.Parameters["P_RETURNVALUE"].Value));
        //    string Qtytype = "";
        //    if (ReturnVal == 0)
        //    {
        //        DataTable dt = ds.Tables[0];
        //        if (dt.Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                var tblleavereq = new tbl_LeaveRequest();
        //                Qtytype = dt.Rows[i][0].ToString();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //string error = "1";
        //        //listObj.Add(error);
        //    }

        //    return Qtytype;

        //}
        public List<tbl_LeaveApproval> GetLeaveApprovals(string ApproverId)
        {
            DALBase obj = new DALBase(_config);
            // int ApproverId = tblapprover.ApproverId;
            List<tbl_LeaveApproval> listObj = new List<tbl_LeaveApproval>();
            List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERID, OracleDbType.NVarchar2, 20, ApproverId.ToString(), ParameterDirection.Input);
            obj.TupleForOracleParameter(LstTupParameters, "P_RETURNDATA", OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
            obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
            //DataSet ds = obj.GetDatafromDatabase(ConstantStoredProcedure.SP_BCL_GetLeaveForApproval, LstTupParameters);
            DataSet ds = obj.GetDatafromDatabase("BCL_TEST_GETLEAVEFORAPPROVAL", LstTupParameters);
            var ReturnVal = Convert.ToInt32((decimal)(OracleDecimal)(obj.cmd.Parameters["P_RETURNVALUE"].Value));
            if (ReturnVal == 0)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        var tblleaveapproval = new tbl_LeaveApproval();
                        tblleaveapproval.LeaveRequestId = Convert.ToInt32(dt.Rows[i][0]);
                        tblleaveapproval.LeaveId = Convert.ToInt32(dt.Rows[i][1]);
                        tblleaveapproval.LeaveCode = dt.Rows[i][2].ToString();
                        tblleaveapproval.FromDate = Convert.ToDateTime(dt.Rows[i][3]);
                        tblleaveapproval.FromDateString = tblleaveapproval.FromDate.ToString("dd/MM/yyyy");
                        tblleaveapproval.ToDate = Convert.ToDateTime(dt.Rows[i][4]);
                        tblleaveapproval.ToDateString = tblleaveapproval.ToDate.ToString("dd/MM/yyyy");
                        tblleaveapproval.LeaveShift = dt.Rows[i][5].ToString();
                        tblleaveapproval.Remarks = dt.Rows[i][6].ToString();
                        tblleaveapproval.Status = dt.Rows[i][7].ToString();
                        tblleaveapproval.TotalLeaves = Convert.ToDouble(dt.Rows[i][8]);
                        tblleaveapproval.LeaveAppliedDate = Convert.ToDateTime(dt.Rows[i][9]);
                        tblleaveapproval.LeaveAppliedDateString = tblleaveapproval.LeaveAppliedDate.ToString("dd/MM/yyyy");
                        tblleaveapproval.EmployeeID = Convert.ToString(dt.Rows[i][10]);
                        tblleaveapproval.EmployeeName = dt.Rows[i][11].ToString();
                        tblleaveapproval.LeaveCategory = dt.Rows[i][12].ToString();

                        if (dt.Rows[i][13].ToString() != null)
                        {
                            tblleaveapproval.FileName = dt.Rows[i][13].ToString();
                        }
                        //tblleaveapproval.FromData = dt.Rows[i][19].ToString();
                        listObj.Add(tblleaveapproval);

                    }
                }
            }
            else if (ReturnVal == 1 || ReturnVal == 2)
            {
                var tbleaveappr = new tbl_LeaveApproval();
                tbleaveappr.ReturnValue = ReturnVal;
                listObj.Add(tbleaveappr);
            }
            return listObj;
        }
        public List<tbl_LeaveApproval> GetLeaveApprovalsForDashboard(string ApproverId)
        {
            DALBase obj = new DALBase(_config);
            // int ApproverId = tblapprover.ApproverId;
            List<tbl_LeaveApproval> listObj = new List<tbl_LeaveApproval>();
            List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERID, OracleDbType.NVarchar2, 20, ApproverId.ToString(), ParameterDirection.Input);
            obj.TupleForOracleParameter(LstTupParameters, "P_RETURNDATA", OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
            obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
            //DataSet ds = obj.GetDatafromDatabase(ConstantStoredProcedure.SP_BCL_GETLEAVE_APR_DASHBOARD, LstTupParameters);
            DataSet ds = obj.GetDatafromDatabase("BCL_TEST_GETAPR_DASHBOARD", LstTupParameters);
            var ReturnVal = Convert.ToInt32((decimal)(OracleDecimal)(obj.cmd.Parameters["P_RETURNVALUE"].Value));
            if (ReturnVal == 0)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        var tblleaveapproval = new tbl_LeaveApproval();
                        tblleaveapproval.LeaveRequestId = Convert.ToInt32(dt.Rows[i][0]);
                        tblleaveapproval.LeaveId = Convert.ToInt32(dt.Rows[i][1]);
                        tblleaveapproval.LeaveCode = Convert.ToString(dt.Rows[i][2]);
                        tblleaveapproval.FromDate = Convert.ToDateTime(dt.Rows[i][3]);
                        tblleaveapproval.FromDateString = tblleaveapproval.FromDate.ToString("dd/MM/yyyy");
                        tblleaveapproval.ToDate = Convert.ToDateTime(dt.Rows[i][4]);
                        tblleaveapproval.ToDateString = tblleaveapproval.ToDate.ToString("dd/MM/yyyy");
                        tblleaveapproval.LeaveShift = dt.Rows[i][5].ToString();
                        tblleaveapproval.Remarks = dt.Rows[i][8].ToString();
                        tblleaveapproval.Status = dt.Rows[i][9].ToString();
                        tblleaveapproval.TotalLeaves = Convert.ToDouble(dt.Rows[i][10]);
                        tblleaveapproval.LeaveAppliedDate = Convert.ToDateTime(dt.Rows[i][11]);
                        tblleaveapproval.LeaveAppliedDateString = tblleaveapproval.LeaveAppliedDate.ToString("dd/MM/yyyy");
                        tblleaveapproval.EmployeeID = Convert.ToString(dt.Rows[i][12]);
                        tblleaveapproval.EmployeeName = dt.Rows[i][13].ToString();
                        tblleaveapproval.LeaveCategory = dt.Rows[i][14].ToString();
                        tblleaveapproval.ApprovedDate = dt.Rows[i][15].ToString();
                        tblleaveapproval.Indicator = dt.Rows[i]["SAPSUCCESSFLAG"].ToString();
                        tblleaveapproval.ErrorMsg = dt.Rows[i]["ERRORMSG"].ToString();


                        //  tblleaveapproval.LeaveAppliedDateString =Convert.ToDateTime tblleaveapproval.ApprovedDate.ToString("dd/MM/yyyy");
                        listObj.Add(tblleaveapproval);

                    }
                }
            }
            else if (ReturnVal == 1 || ReturnVal == 2)
            {
                var tbleaveappr = new tbl_LeaveApproval();
                tbleaveappr.ReturnValue = ReturnVal;
                listObj.Add(tbleaveappr);
            }
            return listObj;
        }
        public Tuple<string, int> SaveAprrovedLeaveRequest(List<tbl_LeaveApproval> tblLeaveApprovals)
        {
            int i = 0;

            Tuple<string, int> Returnstr = new Tuple<string, int>("Transaction", 1);
            List<Tuple<string, OracleDbType, int, object, ParameterDirection>> Lsttuples = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            OracleConnection con;
            OracleCommand cmd;
            DALBase obj = new DALBase(_config);
            //Initialise Objects
            con = new OracleConnection(obj.DBConnection);
            cmd = new OracleCommand();
            try
            {

                cmd.Connection = con;
                cmd.CommandText = "BCL_TEST_SAVELEAVEAPPROVAL";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection.Open();

                cmd.ArrayBindCount = tblLeaveApprovals.Select(x => x.LeaveRequestId).ToArray().Length;
                // cmd.ArrayBindCount = UID.Count;
                // For specifying Oracle Data Types for Parameters you need to use "Oracle.DataAccess.Types" namespace.
                cmd.Parameters.Add(ConstantsVariables.P_LEAVEREQUESTID, OracleDbType.NVarchar2);
                cmd.Parameters[0].Value = tblLeaveApprovals.Select(x => x.LeaveRequestId).ToArray();

                cmd.Parameters.Add(ConstantsVariables.P_APPROVERID, OracleDbType.NVarchar2);
                cmd.Parameters[1].Value = tblLeaveApprovals.Select(x => x.ApproverId).ToArray();

                cmd.Parameters.Add(ConstantsVariables.P_STATUS, OracleDbType.NVarchar2);
                cmd.Parameters[2].Value = tblLeaveApprovals.Select(x => x.Status).ToArray();

                cmd.Parameters.Add(ConstantsVariables.P_EMPLOYEEID, OracleDbType.NVarchar2);
                cmd.Parameters[3].Value = tblLeaveApprovals.Select(x => x.EmployeeID).ToArray();

                cmd.Parameters.Add("P_RETURNVALUE", OracleDbType.Int32, ParameterDirection.Output);
                cmd.Parameters[4].Value = null;

                cmd.Parameters.Add("P_ErrorMsg", OracleDbType.NVarchar2, 3000);
                cmd.Parameters[5].Value = tblLeaveApprovals.Select(x => x.ErrorMsg).ToArray();

                cmd.Parameters.Add("P_Indicator", OracleDbType.NVarchar2);
                cmd.Parameters[6].Value = tblLeaveApprovals.Select(x => x.Indicator).ToArray();
                try
                {
                    i = cmd.ExecuteNonQuery();
                    Returnstr = new Tuple<string, int>("Transaction", i);
                }
                catch (Exception ex)
                {
                    i = -99;
                    Returnstr = new Tuple<string, int>("Transaction", i);
                    throw ex;
                }
                //  string[] RetunVal =  (string[])cmd.Parameters["P_RETURNVALUE"];
                //returnMessage =(cmd.Parameters[4].Value).ToString());
                return Returnstr;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //Close Connection
                // i = -99;
                con.Close();
                cmd.Dispose();
                con.Dispose();
            }




        }
        //public Tuple<string, int> SaveFromSAP(List<tbl_LeaveQuotaSAP> tbl_User)
        //{
        //    try
        //    {
        //        DALBase obj = new DALBase(_config);
        //        List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();

        //        foreach (var l in tbl_User)
        //        {
        //            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.NVarchar2, 50, Convert.ToString(l.EMPLOYEEID), ParameterDirection.Input);
        //            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVEID, OracleDbType.NVarchar2, 50, Convert.ToString(l.LEAVECODE), ParameterDirection.Input);
        //            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_LEAVECATEGORY, OracleDbType.NVarchar2, 1000, Convert.ToString(l.LEAVECATEGORY), ParameterDirection.Input);
        //            string startdate = Convert.ToDateTime(l.STARTDATE).ToString("dd/MM/yyyy");
        //            string endate = Convert.ToDateTime(l.ENDDATE).ToString("dd/MM/yyyy");
        //            obj.TupleForOracleParameter(LstTupParameters, "P_STARTDATE", OracleDbType.NVarchar2, 120, Convert.ToString(startdate), ParameterDirection.Input);
        //            obj.TupleForOracleParameter(LstTupParameters, "P_ENDDATE", OracleDbType.NVarchar2, 120, Convert.ToString(endate), ParameterDirection.Input);
        //            obj.TupleForOracleParameter(LstTupParameters, "P_QUOTA", OracleDbType.Double, 100, l.QUOTA, ParameterDirection.Input);
        //            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_BALANCELEAVE, OracleDbType.Double, 100, l.BALANCELEAVE, ParameterDirection.Input);
        //            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERID, OracleDbType.NVarchar2, 50, Convert.ToString(l.APPROVERID), ParameterDirection.Input);
        //            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERNAME, OracleDbType.NVarchar2, 50, Convert.ToString(l.APPROVERNAME), ParameterDirection.Input);
        //            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_TOTALLEAVE, OracleDbType.Double, 50, l.CARRYFRWD, ParameterDirection.Input);
        //            obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
        //        }
        //        Tuple<string, int> Returnstr = obj.savedataFSAP("BCL_SAVELEAVEQUOTASAP", LstTupParameters);
        //        return Returnstr;
        //    }
        //    catch (Exception ex)
        //    {
        //        Tuple<string, int> returndata = new Tuple<string, int>("Failed", 1);
        //        return returndata;
        //        throw;
        //    }

        //}
        public List<tbl_LeaveApproval> GetEmpLeaveDetails(string SchedulerType)
        {

            DALBase obj = new DALBase(_config);
            List<tbl_LeaveApproval> list = new List<tbl_LeaveApproval>();
            DataSet ds = obj.GetEmpLeaveDetails(ConstantStoredProcedure.SP_BCL_GETEMPDETAILS_E, SchedulerType);
            DataTable dt = ds.Tables[0];

            tbl_LeaveApproval objtblLeaveRequest = new tbl_LeaveApproval();
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        objtblLeaveRequest = new tbl_LeaveApproval();
                        objtblLeaveRequest.EmployeeID = Convert.ToString(dtRow["EMPLOYEEID"]);
                        objtblLeaveRequest.FromDate = Convert.ToDateTime(dtRow["FROMDATE"]);
                        objtblLeaveRequest.FromDateString = objtblLeaveRequest.FromDate.ToString("dd/MM/yyyy");
                        objtblLeaveRequest.FromDateString = objtblLeaveRequest.FromDateString.Replace('-', '/');
                        objtblLeaveRequest.ToDate = Convert.ToDateTime(dtRow["TODATE"]);
                        objtblLeaveRequest.ToDateString = objtblLeaveRequest.ToDate.ToString("dd/MM/yyyy");
                        objtblLeaveRequest.ToDateString = objtblLeaveRequest.ToDateString.Replace('-', '/');
                        objtblLeaveRequest.LeaveShift = Convert.ToString(dtRow["LEAVESHIFT"]);
                        objtblLeaveRequest.LeaveCode = Convert.ToString(dtRow["LEAVECODE"]);
                        objtblLeaveRequest.LeaveRequestId = Convert.ToInt32(dtRow["LEAVEREQUESTID"]);
                        list.Add(objtblLeaveRequest);
                    }
                }
            }

            return list;
        }
        public List<tbl_LeaveApproval> GetMulitpleEmployeeDetails(string strLeaveReqID)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<tbl_LeaveApproval> listObj = new List<tbl_LeaveApproval>();
                tbl_LeaveApproval objLeave = new tbl_LeaveApproval();
                DataSet ds = obj.GetMulipleDataBYId(ConstantStoredProcedure.SP_BCL_GETEMPDETAILS_Muliple, strLeaveReqID);
                DataTable dt = ds.Tables[0];
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dtRow in dt.Rows)
                        {
                            var objLeaverequestid = dtRow[0].ToString();
                            var objEmployeeID = dtRow[1].ToString();
                            objLeave = new tbl_LeaveApproval();
                            objLeave.LeaveRequestId = Convert.ToInt32(objLeaverequestid);
                            objLeave.EmployeeID = objEmployeeID;
                            objLeave.FromDate = Convert.ToDateTime(dtRow[2]);
                            objLeave.FromDateString = objLeave.FromDate.ToString("dd/MM/yyyy");
                            objLeave.ToDate = Convert.ToDateTime(dtRow[3]);
                            objLeave.ToDateString = objLeave.ToDate.ToString("dd/MM/yyyy");
                            objLeave.TotalLeaves = Convert.ToDouble(dtRow[4]);
                            objLeave.LeaveShift = dtRow[5].ToString();
                            objLeave.LeaveCode = dtRow[6].ToString();
                            objLeave.OldStatus = dtRow[9].ToString();
                            objLeave.LeaveId = Convert.ToInt32(dtRow[10]);
                            objLeave.empmobileno = Convert.ToString(dtRow[11]);
                            objLeave.LeaveCategory = Convert.ToString(dtRow[12]);
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

        public Tuple<string, int> SaveLeaveApprovalScheduler(List<tbl_LeaveApproval> tblLeaveApprovals)
        {
            int i = 0;

            Tuple<string, int> Returnstr = new Tuple<string, int>("Transaction", 1);
            List<Tuple<string, OracleDbType, int, object, ParameterDirection>> Lsttuples = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            OracleConnection con;
            OracleCommand cmd;
            DALBase obj = new DALBase(_config);
            //Initialise Objects
            con = new OracleConnection(obj.DBConnection);
            cmd = new OracleCommand();
            try
            {

                cmd.Connection = con;
                cmd.CommandText = "BCL_TEST_UPDATEFLAGMESSAGE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection.Open();

                cmd.ArrayBindCount = tblLeaveApprovals.Select(x => x.LeaveRequestId).ToArray().Length;
                // cmd.ArrayBindCount = UID.Count;
                // For specifying Oracle Data Types for Parameters you need to use "Oracle.DataAccess.Types" namespace.
                cmd.Parameters.Add(ConstantsVariables.P_LEAVEREQUESTID, OracleDbType.NVarchar2);
                cmd.Parameters[0].Value = tblLeaveApprovals.Select(x => x.LeaveRequestId).ToArray();

                cmd.Parameters.Add(ConstantsVariables.P_EMPLOYEEID, OracleDbType.NVarchar2);
                cmd.Parameters[1].Value = tblLeaveApprovals.Select(x => x.EmployeeID).ToArray();

                cmd.Parameters.Add("P_ErrorMsg", OracleDbType.NVarchar2, 3000);
                cmd.Parameters[2].Value = tblLeaveApprovals.Select(x => x.ErrorMsg).ToArray();

                cmd.Parameters.Add("P_Indicator", OracleDbType.NVarchar2);
                cmd.Parameters[3].Value = tblLeaveApprovals.Select(x => x.Indicator).ToArray();

                cmd.Parameters.Add("P_FromData", OracleDbType.NVarchar2);
                cmd.Parameters[3].Value = tblLeaveApprovals.Select(x => x.Indicator).ToArray();


                try
                {
                    i = cmd.ExecuteNonQuery();
                    //var RetunVal = Convert.ToInt32((decimal)(OracleDecimal)(cmd.Parameters["P_RETURNVALUE"].Value));
                    Returnstr = new Tuple<string, int>("Transaction", i);
                }
                catch (Exception ex)
                {
                    i = -99;
                    Returnstr = new Tuple<string, int>("Transaction", i);
                    throw ex;
                }
                //  string[] RetunVal =  (string[])cmd.Parameters["P_RETURNVALUE"];
                //returnMessage =(cmd.Parameters[4].Value).ToString());
                return Returnstr;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //Close Connection
                // i = -99;
                con.Close();
                cmd.Dispose();
                con.Dispose();
            }
        }

        public Tuple<string, int> InsertEmpId(string Empid)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
                obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.NVarchar2, 50, Empid.ToString(), ParameterDirection.Input);
                obj.TupleForOracleParameter(LstTupParameters, "P_MESSAGE", OracleDbType.NVarchar2, 100, null, ParameterDirection.Output);
                obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
                Tuple<string, int> Returnstr = obj.savedata(ConstantStoredProcedure.SP_BCL_InsertApproverID, LstTupParameters);
                return Returnstr;
            }
            catch (Exception ex)
            {
                Tuple<string, int> returndata = new Tuple<string, int>("Failed", 1);
                return returndata;
                throw;
            }

        }
    }
}
