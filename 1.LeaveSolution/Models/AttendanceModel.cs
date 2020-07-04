using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace LeaveSolution.Models
{
    public class AttendanceModel
    {
        public  int EmployeeID { get; set; }
        public List<SelectListItem> MonthList { get; set; }
        public List<SelectListItem> YearList { get; set; }
        public string DayName { get; set; }
        public string Date { get; set; }
        public string Day { get; set; }
        public string Intime { get; set; }
        public string Outtime { get; set; }
        public string Firsthalf { get; set; }
        public string Secondhalf { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public List<AttendanceModel> ListOfAttendance { get; set; }
        public string AttendanceRegularization
        {
            get; set;
        }
    }
    public class EMPATTENDANCE
    {
        public EmployeAttendance[] OUTPUT;
    }
    public class EmployeAttendance
    {
        private string LDATE;

        private System.DateTime SWIPE_IN;

        private System.DateTime SWIPE_OUT;

        private string SHIFT;

        private string DAY_TYPE;


        public string Ldate
        {
            get
            {
                return this.LDATE;
            }
            set
            {
                this.LDATE = value;
            }
        }

        public System.DateTime Swipe_In
        {
            get
            {
                return this.SWIPE_IN;
            }
            set
            {
                this.SWIPE_IN = value;
            }
        }

        public System.DateTime Swipe_Out
        {
            get
            {
                return this.SWIPE_OUT;
            }
            set
            {
                this.SWIPE_OUT = value;
            }
        }

        public string Shift
        {
            get
            {
                return this.SHIFT;
            }
            set
            {
                this.SHIFT = value;
            }
        }

        public string Day_Type
        {
            get
            {
                return this.DAY_TYPE;
            }
            set
            {
                this.DAY_TYPE = value;
            }
        }
    }
}
