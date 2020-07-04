using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveSolution.DAL.Data
{
    public class tbl_OTPG
    {
        public string EmployeeID { get; set; }
        public int OTP { get; set; }
        public long MobileNO { get; set; }
        public DateTime OTPStarttime { get; set; }
        public DateTime OTPEndtime { get; set; }
        public DateTime DateOfBirth { get; set; }

        public int Returnvalue { get; set; }
        public string ReturnMsg { get; set; }
    }
}
