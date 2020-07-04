using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveSolution.Models
{
    public class Zfpa0025Response
    {
        private string personnalNumberField;

        /// <remarks/>
        public string PersonnalNumber
        {
            get
            {
                return this.personnalNumberField;
            }
            set
            {
                this.personnalNumberField = value;
            }
        }
    }
    public class EmployeId
    {
        public string PERSONNAL_NUMBER { get; set; }
    }

}
