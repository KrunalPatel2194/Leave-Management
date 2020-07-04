using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveSolution.Models
{
    public class Zftm0011Response
    {
        private Zftm0010Tab[] outputField;

        /// <remarks/>
        public Zftm0010Tab[] Output
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

    public partial class Zftm0010Tab
    {

        private string ldateField;

        private System.DateTime swipeInField;

        private System.DateTime swipeOutField;

        private string shiftField;

        private string dayTypeField;

        /// <remarks/>
        public string Ldate
        {
            get
            {
                return this.ldateField;
            }
            set
            {
                this.ldateField = value;
            }
        }

        /// <remarks/>
        public System.DateTime SwipeIn
        {
            get
            {
                return this.swipeInField;
            }
            set
            {
                this.swipeInField = value;
            }
        }

        /// <remarks/>
        public System.DateTime SwipeOut
        {
            get
            {
                return this.swipeOutField;
            }
            set
            {
                this.swipeOutField = value;
            }
        }

        /// <remarks/>
        public string Shift
        {
            get
            {
                return this.shiftField;
            }
            set
            {
                this.shiftField = value;
            }
        }

        /// <remarks/>
        public string DayType
        {
            get
            {
                return this.dayTypeField;
            }
            set
            {
                this.dayTypeField = value;
            }
        }
    }
}
