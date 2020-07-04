using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveSolution.Models
{
    public class Zftm0006Response
    {
        private ZFTM0006_TAB[] dETAILSField;

        /// <remarks/>
        public ZFTM0006_TAB[] DETAILS
        {
            get
            {
                return this.dETAILSField;
            }
            set
            {
                this.dETAILSField = value;
            }
        }
    }
    public partial class ZFTM0006_TAB
    {

        private string leaverequestid;
        
        private string pERNR_SUPField;

        private string cNAME_SUPField;

        private string cELL_SUPField;

        private string eMAIL_SUPField;

        private string pERNR_HODField;

        private string cNAME_HODField;

        private string eMAILID_HODField;
        private string employeeid;
        public string LeaveRequestID
        {
            get
            {
                return this.leaverequestid;
            }
            set
            {
                this.leaverequestid = value;
            }
        }
        public string EmployeeID
        {
            get
            {
                return this.employeeid;
            }
            set
            {
                this.employeeid = value;
            }
        }
        /// <remarks/>
        public string PERNR_SUP
        {
            get
            {
                return this.pERNR_SUPField;
            }
            set
            {
                this.pERNR_SUPField = value;
            }
        }

        /// <remarks/>
        public string CNAME_SUP
        {
            get
            {
                return this.cNAME_SUPField;
            }
            set
            {
                this.cNAME_SUPField = value;
            }
        }

        /// <remarks/>
        public string CELL_SUP
        {
            get
            {
                return this.cELL_SUPField;
            }
            set
            {
                this.cELL_SUPField = value;
            }
        }

        /// <remarks/>
        public string EMAIL_SUP
        {
            get
            {
                return this.eMAIL_SUPField;
            }
            set
            {
                this.eMAIL_SUPField = value;
            }
        }

        /// <remarks/>
        public string PERNR_HOD
        {
            get
            {
                return this.pERNR_HODField;
            }
            set
            {
                this.pERNR_HODField = value;
            }
        }

        /// <remarks/>
        public string CNAME_HOD
        {
            get
            {
                return this.cNAME_HODField;
            }
            set
            {
                this.cNAME_HODField = value;
            }
        }

        /// <remarks/>
        public string EMAILID_HOD
        {
            get
            {
                return this.eMAILID_HODField;
            }
            set
            {
                this.eMAILID_HODField = value;
            }
        }
    }
}
