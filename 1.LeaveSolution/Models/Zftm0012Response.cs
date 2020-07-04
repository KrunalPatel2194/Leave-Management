using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveSolution.Models
{
    public class Zftm0012Response
    {
        private Zftm0012Tab[] outputField;

        /// <remarks/>
        public Zftm0012Tab[] Output
        {
            get
            {
                return this.outputField;
            }
            set
            {
                this.outputField = value;
            }
        }
    }

    public partial class Zftm0012Tab
    {

        private string leaveCodeField;

        private string appliedDateField;

        private string begdaField;

        private string enddaField;

        private string leaveTypeField;

        private string totLeavesField;

        private string submittedByField;

        private string approverField;

        private string statusField;

        private string sapStatusField;

        private string errorMessageField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LeaveCode
        {
            get
            {
                return this.leaveCodeField;
            }
            set
            {
                this.leaveCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AppliedDate
        {
            get
            {
                return this.appliedDateField;
            }
            set
            {
                this.appliedDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LeaveType
        {
            get
            {
                return this.leaveTypeField;
            }
            set
            {
                this.leaveTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TotLeaves
        {
            get
            {
                return this.totLeavesField;
            }
            set
            {
                this.totLeavesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SubmittedBy
        {
            get
            {
                return this.submittedByField;
            }
            set
            {
                this.submittedByField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Approver
        {
            get
            {
                return this.approverField;
            }
            set
            {
                this.approverField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SapStatus
        {
            get
            {
                return this.sapStatusField;
            }
            set
            {
                this.sapStatusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ErrorMessage
        {
            get
            {
                return this.errorMessageField;
            }
            set
            {
                this.errorMessageField = value;
            }
        }
    }
}
