using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Interfaces;
using LeaveSolution.DAL.Data;
using System.Data;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using LeaveSolution.DAL.Repository;
using Microsoft.AspNetCore.Http;

namespace LeaveSolution.DAL.Interfaces
{
    public class MenuRepositary : IMenumasterRepositay
    {
        public readonly IConfiguration _config;
        public MenuRepositary(IConfiguration config)
        {
            _config = config;
        }
        public List<tbl_MenuMaster> GetMenuMaster(string ModuleName, string EmpID)
        {
            DALBase obj = new DALBase(_config);
            List<tbl_MenuMaster> listObj = new List<tbl_MenuMaster>();
            List<Tuple<String, OracleDbType, int, object, ParameterDirection>> LstTupParameters = new List<Tuple<string, OracleDbType, int, object, ParameterDirection>>();
            obj.TupleForOracleParameter(LstTupParameters, "P_EmployeeID", OracleDbType.NVarchar2, 500, EmpID, ParameterDirection.Input);
            obj.TupleForOracleParameter(LstTupParameters, "P_ROLLName", OracleDbType.NVarchar2, 500, ModuleName, ParameterDirection.Input);
            obj.TupleForOracleParameter(LstTupParameters, "P_RETURNDATA", OracleDbType.RefCursor, 20, null, ParameterDirection.Output);
            obj.TupleForOracleParameter(LstTupParameters, "P_RETURNVALUE", OracleDbType.Int32, 1, null, ParameterDirection.Output);
            DataSet ds = obj.GetDatafromDatabase(ConstantStoredProcedure.SP_BCL_MENUMASTERDETAILS, LstTupParameters);
            var ReturnVal = Convert.ToInt32((decimal)(OracleDecimal)(obj.cmd.Parameters["P_RETURNVALUE"].Value));
            if (ReturnVal == 0)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var tblmenu = new tbl_MenuMaster();
                        tblmenu.MenuID = Convert.ToInt32(dt.Rows[i]["MENUID"]);
                        tblmenu.MenuName = Convert.ToString(dt.Rows[i]["MENUNAME"]);
                        tblmenu.Controller = Convert.ToString(dt.Rows[i]["CONTROLLER"]);
                        tblmenu.Action = Convert.ToString(dt.Rows[i]["Action"]);
                        tblmenu.ParentMenuID = Convert.ToInt32(dt.Rows[i]["PARENTMENUID"]);
                        // tblmenu.RollID = Convert.ToInt32(dt.Rows[i][5]);
                        listObj.Add(tblmenu);
                    }
                }
            }
            return listObj;
        }
    }
}
