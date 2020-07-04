using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
//using StackExchange.Redis;

namespace LeaveSolution.Models
{
    public class LeaveHistoryModelViewModel
    {
        public string Year { get; set; }
        public List<LeaveHistoryModelViewModel> LeaveHistory { get; set; }
        public string ErrorMsg { get; set; }
        public string Indicator { get; set; }
        public string LeaveCategory { get; set; }
        public string LeaveAppliedDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public double TotalLeaves { get; set; }
        public string ApproverName { get; set; }
        public double TotalAppliedLeave { get; set; }
        public string Status { get; set; }
        public string LeaveType { get; set; }
        public string EmployeeID { get; set; }
        public string LeaveCode { get; set; }
        public DateTime AppliedDate { get; set; }
    }
    public class DataTableAjaxPostModel
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<Column> columns { get; set; }
        public Search search { get; set; }
    }
    public class EMPLEAVEHISTORY
    {
        public HistoryDetails[] OUTPUT;
    }
    public class HistoryDetails
    {
        private string _LEAVE_CODE;

        private System.DateTime _APPLIED_DATE;

        private System.DateTime _BEGDA;
        private System.DateTime _ENDDA;

        private string _LEAVE_TYPE;

        private string _TOT_LEAVES;
        private string _SUBMITTED_BY;
        private string _APPROVER;
        private string _STATUS;
        private string _SAP_STATUS;
        private string _ERROR_MESSAGE;

        public string LEAVE_CODE
        {
            get
            {
                return this._LEAVE_CODE;
            }
            set
            {
                this._LEAVE_CODE = value;
            }
        }

        public System.DateTime APPLIED_DATE
        {
            get
            {
                return this._APPLIED_DATE;
            }
            set
            {
                this._APPLIED_DATE = value;
            }
        }

        public System.DateTime BEGDA
        {
            get
            {
                return this._BEGDA;
            }
            set
            {
                this._BEGDA = value;
            }
        }

        public System.DateTime ENDDA
        {
            get
            {
                return this._ENDDA;
            }
            set
            {
                this._ENDDA = value;
            }
        }

        public string LEAVE_TYPE
        {
            get
            {
                return this._LEAVE_TYPE;
            }
            set
            {
                this._LEAVE_TYPE = value;
            }
        }

        public string TOT_LEAVES
        {
            get
            {
                return this._TOT_LEAVES;
            }
            set
            {
                this._TOT_LEAVES = value;
            }
        }

        public string SUBMITTED_BY
        {
            get
            {
                return this._SUBMITTED_BY;
            }
            set
            {
                this._SUBMITTED_BY = value;
            }
        }

        public string APPROVER
        {
            get
            {
                return this._APPROVER;
            }
            set
            {
                this._APPROVER = value;
            }
        }

        public string STATUS
        {
            get
            {
                return this._STATUS;
            }
            set
            {
                this._STATUS = value;
            }
        }

        public string SAP_STATUS
        {
            get
            {
                return this._SAP_STATUS;
            }
            set
            {
                this._SAP_STATUS = value;
            }
        }

        public string ERROR_MESSAGE
        {
            get
            {
                return this._ERROR_MESSAGE;
            }
            set
            {
                this._ERROR_MESSAGE = value;
            }
        }

        

    }
}


