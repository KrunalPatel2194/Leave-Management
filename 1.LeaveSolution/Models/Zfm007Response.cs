using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveSolution.Models
{
    public class Zfm007Response
    {
        private Zftm0007Tab[] quotaOverviewField;
        public Zftm0007Tab[] QuotaOverview
        { 
           
        get
            {
                return this.quotaOverviewField;
            }
            set
            {
                this.quotaOverviewField = value;
            }
        }
    }
    public  class Zftm0007Tab
    {

        private string ktextField;
        private string employeeID;
        private string begdaField;
        private string leavecode;
        private string enddaField;

        private decimal anzhlField;

        private decimal kverbField;

        private decimal anzhlCloseField;
        private string approverid;
        private string approversapname;

        /// <remarks/>
        /// 

        public string ApproverID
        {
            get
            {
                return this.approverid;
            }
            set
            {
                this.approverid = value;
            }
        }
        public string ApproverSAPName
        {
            get
            {
                return this.approversapname;
            }
            set
            {
                this.approversapname = value;
            }
        }

        public string LeaveCode
        {
            get
            {
                return this.leavecode;
            }
            set
            {
                this.leavecode = value;
            }
        }
        public string EmployeeID
        {
            get
            {
                return this.employeeID;
            }
            set
            {
                this.employeeID = value;
            }
        }
        public string Ktext
        {
            get
            {
                return this.ktextField;
            }
            set
            {
                this.ktextField = value;
            }
        }

        /// <remarks/>
        public string Begda
        {
            get
            {
                return this.begdaField;
            }
            set
            {
                this.begdaField = value;
            }
        }

        /// <remarks/>
        public string Endda
        {
            get
            {
                return this.enddaField;
            }
            set
            {
                this.enddaField = value;
            }
        }

        /// <remarks/>
        public decimal Anzhl
        {
            get
            {
                return this.anzhlField;
            }
            set
            {
                this.anzhlField = value;
            }
        }

        /// <remarks/>
        public decimal Kverb
        {
            get
            {
                return this.kverbField;
            }
            set
            {
                this.kverbField = value;
            }
        }

        /// <remarks/>
        public decimal AnzhlClose
        {
            get
            {
                return this.anzhlCloseField;
            }
            set
            {
                this.anzhlCloseField = value;
            }
        }
    }
}
