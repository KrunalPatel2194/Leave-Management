using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Interfaces;
using LeaveSolution.DAL.Data;
using System.Data;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Linq;
namespace LeaveSolution.DAL.Repository
{
    public class OTPGenerateRepositary : IGenerateOTPRepositary
    {
        public readonly IConfiguration _config;
        public OTPGenerateRepositary(IConfiguration config)
        {
            _config = config;
        }
        public List<tbl_OTPG> Getsingledata(string id, string fromdata)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<tbl_OTPG> listObj = new List<tbl_OTPG>();
                DataSet dt = obj.GetById(ConstantStoredProcedure.SP_GETMOBILENO, id, fromdata);
                var RetunVal = Convert.ToInt32((decimal)(OracleDecimal)(obj.cmd.Parameters["P_RETURNVALUE"].Value));
                var tbuser = new tbl_OTPG();
                if (RetunVal == 1)
                {
                    tbuser.Returnvalue = RetunVal;
                    listObj.Add(tbuser);
                }
                else if (RetunVal == 2)
                {
                    tbuser.Returnvalue = RetunVal;
                    listObj.Add(tbuser);
                }
                else if (RetunVal == 0)
                {
                    DataTable Dt1 = dt.Tables[0];
                    if (Dt1.Rows.Count > 0)
                    {
                        string MobileNo = Convert.ToString(Dt1.Rows[0]["PHONENO"]);
                        if (string.IsNullOrEmpty(MobileNo))
                        {
                            tbuser.Returnvalue = 2;
                            listObj.Add(tbuser);
                        }
                        else
                        {
                            foreach (var item in Dt1.Rows)
                            {
                                var tbOtg = new tbl_OTPG();
                                tbOtg.MobileNO = Convert.ToInt64(MobileNo);
                                listObj.Add(tbOtg);
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
        public Tuple<string, int> SaveOTP(List<tbl_OTPG> tbl_Otps)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
                foreach (var l in tbl_Otps)
                {
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.NVarchar2, 50, l.EmployeeID.ToString(), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_OTP, OracleDbType.Int32, 100, l.OTP.ToString(), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_MESSAGE", OracleDbType.NVarchar2, 100, null, ParameterDirection.Output);
                    obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
                    obj.TupleForOracleParameter(LstTupParameters, "v_IsPreviousPass", OracleDbType.Int32, 1, null, ParameterDirection.Output);
                }
                Tuple<string, int> Retunstr = obj.savedata(ConstantStoredProcedure.SP_SaveOTPGEN, LstTupParameters);
                // var statusoftrans = 0;
                // Tuple<string, int> returndata = new Tuple<string, int>("Success", 0);

                return Retunstr;
            }
            catch (Exception ex)
            {
                Tuple<string, int> returndata = new Tuple<string, int>("Failed", 1);
                return returndata;
                throw;
            }

        }

        public Tuple<string, int> SaveNewUser(List<tbl_User> tbl_USERs)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();

                foreach (var l in tbl_USERs)
                {
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.NVarchar2, 50, l.EmployeeID.ToString(), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_USERID, OracleDbType.NVarchar2, 50, l.UserID.ToString(), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_PASSWORD, OracleDbType.Varchar2, 100, l.Password.ToString(), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_ISACTIVE, OracleDbType.Int32, 100, 1.ToString(), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_MESSAGE", OracleDbType.NVarchar2, 100, null, ParameterDirection.Output);
                    obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
                    //   TupleForOracleParameter(LstTupParameters, "v_IsPreviousPass", OracleDbType.Int32, 1, null, ParameterDirection.Output);
                }
                Tuple<string, int> Returnstr = obj.savedata(ConstantStoredProcedure.SP_SAVEUSER, LstTupParameters);
                return Returnstr;
            }
            catch (Exception ex)
            {
                Tuple<string, int> returndata = new Tuple<string, int>("Failed", 1);
                return returndata;
                throw;
            }

        }

        public List<tbl_User> Authenticate(List<tbl_User> tbl_USERs)
        {
            DALBase obj = new DALBase(_config);
            List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            List<tbl_User> listObj = new List<tbl_User>();

            foreach (var l in tbl_USERs)
            {
                obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_USERID, OracleDbType.NVarchar2, 50, l.UserID.ToString(), ParameterDirection.Input);
                obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_ISACTIVE, OracleDbType.Int32, 50, 1.ToString(), ParameterDirection.Input);
                obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_PASSWORD, OracleDbType.Varchar2, 100, l.Password, ParameterDirection.Input);
                obj.TupleForOracleParameter(LstTupParameters, "P_MESSAGE", OracleDbType.NVarchar2, 100, null, ParameterDirection.Output);
                obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
                obj.TupleForOracleParameter(LstTupParameters, "P_EMPDETAILS", OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
            }
            //DataSet ds = obj.GetDatafromDatabase(ConstantStoredProcedure.SP_AUTHENTICATEUSER, LstTupParameters);
            DataSet ds = obj.GetDatafromDatabase("BCL_TEST_AUTHENTICATIONUSER", LstTupParameters);
            var RetunVal = Convert.ToInt32((decimal)(OracleDecimal)(obj.cmd.Parameters["P_RETURNVALUE"].Value));
            var RetunValMessg = Convert.ToString((OracleString)(obj.cmd.Parameters["P_MESSAGE"].Value));
            if (RetunVal == 0)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {

                    foreach (var item in dt.Rows)
                    {
                        var tbuser = new tbl_User();
                        tbuser.EmployeeID = Convert.ToString(dt.Rows[0][0]);
                        tbuser.EmployeeName = dt.Rows[0][1].ToString();
                        tbuser.EmpEmail = Convert.ToString(dt.Rows[0][2]);
                        tbuser.DateOfBirth = Convert.ToDateTime(dt.Rows[0][3]);
                        tbuser.PersonalArea = Convert.ToString(dt.Rows[0][4]);
                        tbuser.PersonalSubArea = dt.Rows[0][5].ToString();
                        tbuser.MobileNo = Convert.ToString(dt.Rows[0][6]);
                        tbuser.ReturnValue = RetunVal;
                        tbuser.Category = dt.Rows[0][7].ToString();
                        listObj.Add(tbuser);
                    }
                }
            }
            else if (RetunVal == 1 || RetunVal == 2 || RetunVal == 3 || RetunVal == 4)
            {
                var tbuser = new tbl_User();
                tbuser.ReturnValue = RetunVal;
                tbuser.ReturnValMessg = Convert.ToString(RetunValMessg);
                listObj.Add(tbuser);
            }
            return listObj;
        }
        public tbl_OTPG Getotp(string employeeid,string fromdata)
        {
            DALBase obj = new DALBase(_config);
            List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            tbl_OTPG listObj = new tbl_OTPG();
            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.Varchar2, 20, employeeid.ToString(), ParameterDirection.Input);
            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_OTP, OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
            obj.TupleForOracleParameter(LstTupParameters, "P_DATAFROM", OracleDbType.NVarchar2, 20, fromdata, ParameterDirection.Input);
            obj.TupleForOracleParameter(LstTupParameters, "P_RETURNMSG", OracleDbType.NVarchar2, 20, null, ParameterDirection.Output);
              DataSet ds = obj.GetDatafromDatabase("BCL_TEST_GETOTP", LstTupParameters);
            string RetunMsg = (obj.cmd.Parameters["P_RETURNMSG"].Value.ToString());

            if (RetunMsg == "Reset" || RetunMsg == "null"|| RetunMsg == "NotExist")
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    listObj.OTP = Convert.ToInt32(dt.Rows[0][0]);
                    listObj.OTPEndtime = Convert.ToDateTime(dt.Rows[0][1]);
                    listObj.DateOfBirth = Convert.ToDateTime(dt.Rows[0][2]);
                    listObj.ReturnMsg = RetunMsg;
                }
            }
            else
            {
                listObj.ReturnMsg = RetunMsg;
            }
            return listObj;
        }
        public string ValidateMifareID(string Mifareid, string datafrom)
        {
            DALBase obj = new DALBase(_config);
            List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            obj.TupleForOracleParameter(LstTupParameters, "P_DATAFROM", OracleDbType.NVarchar2, 20, datafrom, ParameterDirection.Input);
            obj.TupleForOracleParameter(LstTupParameters, "P_MifareID", OracleDbType.NVarchar2, 20, Mifareid.ToString(), ParameterDirection.Input);
            obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 20, null, ParameterDirection.Output);
            obj.TupleForOracleParameter(LstTupParameters, "P_Empdetails", OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
            DataSet ds = obj.GetDatafromDatabase("BCL_VALIDATEMIFAREID", LstTupParameters);
            var RetunVal = Convert.ToInt32(Convert.ToString(obj.cmd.Parameters["p_returnvalue"].Value));
            string empid = "";
            if (RetunVal == 0)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    empid = Convert.ToString(dt.Rows[0]["EMPID"]);
                }
            }
            if (RetunVal == 2 || RetunVal == 1)
            {
                empid = RetunVal.ToString();
            }
            return empid;
        }
        public tbl_OTPG GetDOBForKIOSK(string employeeid)
        {
            DALBase obj = new DALBase(_config);

            List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            tbl_OTPG listObj = new tbl_OTPG();

            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.Varchar2, 20, employeeid.ToString(), ParameterDirection.Input);
            obj.TupleForOracleParameter(LstTupParameters, "P_Empdetails", OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
            DataSet ds = obj.GetDatafromDatabase(ConstantStoredProcedure.SP_BCL_GETDOBFORKIOSK, LstTupParameters);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                DateTime dob = Convert.ToDateTime(dt.Rows[0]["DATEOFBIRTH"]);
                listObj.DateOfBirth = dob;
            }
            return listObj;
        }
        public Tuple<string, int> SaveFromSAPQuota(List<tbl_LeaveQuotaSAP> tbl_User)
        {
            int i = 0;
            Tuple<string, int> Returnstr = new Tuple<string, int>("Transaction", 1);
            OracleConnection con;
            OracleCommand cmd;
            DALBase obj = new DALBase(_config);
            con = new OracleConnection(obj.DBConnection);
            cmd = new OracleCommand();
            try
            {
                cmd.Connection = con;
                cmd.CommandText = "BCL_ALLQUOTASAVESAP";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection.Open();
                cmd.ArrayBindCount = tbl_User.Select(x => x.EMPLOYEEID).ToArray().Length;
                cmd.Parameters.Add(ConstantsVariables.P_EMPID, OracleDbType.NVarchar2);
                cmd.Parameters[0].Value = tbl_User.Select(x => x.EMPLOYEEID).ToArray();
                cmd.Parameters.Add(ConstantsVariables.P_LEAVECATEGORY, OracleDbType.NVarchar2);
                cmd.Parameters[1].Value = tbl_User.Select(x => x.LEAVECATEGORY).ToArray();
                cmd.Parameters.Add("P_STARTDATE", OracleDbType.NVarchar2);
                cmd.Parameters[2].Value = tbl_User.Select(x => x.STARTDATE).ToArray();
                cmd.Parameters.Add("P_ENDDATE", OracleDbType.NVarchar2);
                cmd.Parameters[3].Value = tbl_User.Select(x => x.ENDDATE).ToArray();
                cmd.Parameters.Add("P_QUOTA", OracleDbType.Double);
                cmd.Parameters[4].Value = tbl_User.Select(x => x.QUOTA).ToArray();
                cmd.Parameters.Add(ConstantsVariables.P_BALANCELEAVE, OracleDbType.Double);
                cmd.Parameters[5].Value = tbl_User.Select(x => x.BALANCELEAVE).ToArray();
                cmd.Parameters.Add(ConstantsVariables.P_APPROVERID, OracleDbType.NVarchar2);
                cmd.Parameters[6].Value = tbl_User.Select(x => x.APPROVERID).ToArray();
                cmd.Parameters.Add(ConstantsVariables.P_APPROVERNAME, OracleDbType.NVarchar2);
                cmd.Parameters[7].Value = tbl_User.Select(x => x.ApproverSAPName).ToArray();
                cmd.Parameters.Add(ConstantsVariables.P_TOTALLEAVE, OracleDbType.Double);
                cmd.Parameters[8].Value = tbl_User.Select(x => x.CARRYFRWD).ToArray();
                cmd.Parameters.Add("P_RETURNVALUE", OracleDbType.Int32, ParameterDirection.Output);
                cmd.Parameters[9].Value = null;
                try
                {
                    i = cmd.ExecuteNonQuery();
                    Returnstr = new Tuple<string, int>("Transaction", i);
                }
                catch (Exception)
                {
                    i = -99;
                    Returnstr = new Tuple<string, int>("Transaction", i);
                    throw;
                }
                return Returnstr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
                cmd.Dispose();
                con.Dispose();
            }
        }
        public Tuple<string, int> SaveFromSAP(List<tbl_Employee> tbl_User)
        {
            try
            {
                DALBase obj = new DALBase(_config);
                List<Tuple<string, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
                foreach (var l in tbl_User)
                {
                    if (l.EmployeeMail == null || l.EmployeeMail == "")
                    {
                        l.EmployeeMail = "EmailID not Updated";
                    }
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPID, OracleDbType.NVarchar2, 50, l.EmployeeID.ToString(), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMPNAME, OracleDbType.NVarchar2, 50, l.EmployeeName.ToString(), ParameterDirection.Input);
                    string DOB = l.DateOfBirth;
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_DATEOFBIRTH, OracleDbType.NVarchar2, 100, DOB, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_PA, OracleDbType.Varchar2, 100, l.PersonalArea, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_PSA, OracleDbType.Varchar2, 100, l.PersonalSubArea, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_PHONENO, OracleDbType.Varchar2, 100, l.MobileNo, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "CostCenter", OracleDbType.Varchar2, 100, l.CostCenter, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "CardID", OracleDbType.Varchar2, 100, l.CardID, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_MESSAGE", OracleDbType.NVarchar2, 100, null, ParameterDirection.Output);
                    obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
                    obj.TupleForOracleParameter(LstTupParameters, "P_EMAIL", OracleDbType.Varchar2, 500, l.EmployeeMail, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERID, OracleDbType.Varchar2, 500, Convert.ToString(l.ApproverId), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERNAME, OracleDbType.Varchar2, 500, Convert.ToString(l.ApproverName), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_EMPSUBGROUP", OracleDbType.Varchar2, 500, l.EmpSubGroup, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_EMPGROUP", OracleDbType.Varchar2, 500, l.EmpGroup, ParameterDirection.Input);
                }
                Tuple<string, int> Returnstr = obj.savedata("SaveEMPMaster", LstTupParameters);
                return Returnstr;
            }
            catch (Exception ex)
            {
                Tuple<string, int> returndata = new Tuple<string, int>("Failed", 1);
                return returndata;
                throw;
            }
        }
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
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERID, OracleDbType.NVarchar2, 50, l.ApproverId.ToString(), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_APPROVERNAME, OracleDbType.NVarchar2, 1000, l.ApproverName.ToString(), ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_EMAILID, OracleDbType.NVarchar2, 100, l.ApproverMailID, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_PHONENO, OracleDbType.Varchar2, 100, l.ApproverMobNo, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_HODID", OracleDbType.Varchar2, 100, l.HODID, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_HODName", OracleDbType.Varchar2, 100, l.HODName, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_HODMailID", OracleDbType.Varchar2, 100, l.HODMailID, ParameterDirection.Input);
                    obj.TupleForOracleParameter(LstTupParameters, "P_MESSAGE", OracleDbType.NVarchar2, 100, null, ParameterDirection.Output);
                    obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
                }
                Tuple<string, int> Returnstr = obj.savedata("BCL_SaveApproverMaster", LstTupParameters);
                return Returnstr;
            }
            catch (Exception ex)
            {
                Tuple<string, int> returndata = new Tuple<string, int>("Failed", 1);
                return returndata;
                throw;
            }
        }
        public void DeleteUser(string userid)
        {
            DALBase obj = new DALBase(_config);
            List<tbl_LeaveRequest> listObj = new List<tbl_LeaveRequest>();
            List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            obj.TupleForOracleParameter(LstTupParameters, ConstantsVariables.P_USERID, OracleDbType.NVarchar2, 20, Convert.ToString(userid), ParameterDirection.Input);
            obj.DeleteUser(ConstantStoredProcedure.SP_BCL_Delete_ConcurrentLogin, userid);
        }

        public int ExtendSession(string employeeid)
        {
            DALBase obj = new DALBase(_config);
            OracleConnection con;
            OracleCommand cmd;
            int i = 0;
            con = new OracleConnection(obj.DBConnection);
            cmd = new OracleCommand();
            cmd.Connection = con;
            cmd.CommandText = "BCL_AUTHENTICATIONUSERTIME";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.Parameters.Add(ConstantsVariables.P_EMPLOYEEID, OracleDbType.NVarchar2, employeeid, ParameterDirection.Input);
            try
            {
                i = cmd.ExecuteNonQuery();
                //Returnstr = new Tuple<string, int>("Transaction", i);
            }
            catch (Exception ex)
            {
                i = -99;
                //Returnstr = new Tuple<string, int>("Transaction", i);
                throw ex;
            }
            return i;
        }
    }
}
