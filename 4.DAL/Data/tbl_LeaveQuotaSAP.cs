using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveSolution.DAL.Data
{
   public class tbl_LeaveQuotaSAP
    {
        public string LEAVECODE { get; set; }
        public string LEAVECATEGORY { get; set; }
        public string EMPLOYEEID { get; set; }
        public string BALANCELEAVE { get; set; }
        public string ApproverSAPName { get; set; }

        
        public string QUOTA { get; set; }
        public string STARTDATE { get; set; }

        public string CARRYFRWD { get; set; }
        public string ENDDATE { get; set; }
        public string APPROVERID { get; set; }
        public string APPROVERNAME { get; set; }


        //CARRYCOUNT,
        //,
        //,
        //,
        //REMARKS,
        //LEAVEID,
        //TOTALLEAVES,
        //CREATEDBY,
        //CREATEDDATE,
        //MODIFYBY,
        //MODIFYDATE
    }
}
