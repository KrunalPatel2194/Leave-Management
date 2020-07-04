using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveSolution.BAL.ServiceModels
{
    public class EmployeeServicemodel
    {

        private Zftm0008Tabservice[] detailsField;

        /// <remarks/>
        public Zftm0008Tabservice[] Details
        {
            get
            {
                return this.detailsField;
            }
            set
            {
                this.detailsField = value;
            }
        }


    }
        public class Zftm0008Tabservice
        {

            private string nameField;

            private string birthdateField;

            private string useridField;

            private string persAreaField;

            private string pSubareaField;
        private string employeeID;

        private string costcenterField;

            private string usridLongField;

            private string zzcardidField;
        private string approverid;
        private string approversapname;
        private string persgField;

        private string perskField;
        private string textField;

        private string indField;

        /// <remarks/>
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        public string Ind
        {
            get
            {
                return this.indField;
            }
            set
            {
                this.indField = value;
            }
        }

        public string Persg
        {
            get
            {
                return this.persgField;
            }
            set
            {
                this.persgField = value;
            }
        }

        /// <remarks/>
        public string Persk
        {
            get
            {
                return this.perskField;
            }
            set
            {
                this.perskField = value;
            }
        }
        /// <remarks/>

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
        public string Name
            {
                get
                {
                    return this.nameField;
                }
                set
                {
                    this.nameField = value;
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
        /// <remarks/>
        public string Birthdate
            {
                get
                {
                    return this.birthdateField;
                }
                set
                {
                    this.birthdateField = value;
                }
            }

            /// <remarks/>
            public string Userid
            {
                get
                {
                    return this.useridField;
                }
                set
                {
                    this.useridField = value;
                }
            }

            /// <remarks/>
            public string PERS_AREA
        {
                get
                {
                    return this.persAreaField;
                }
                set
                {
                    this.persAreaField = value;
                }
            }

            /// <remarks/>
            public string P_SUBAREA
        {
                get
                {
                    return this.pSubareaField;
                }
                set
                {
                    this.pSubareaField = value;
                }
            }

            /// <remarks/>
            public string Costcenter
            {
                get
                {
                    return this.costcenterField;
                }
                set
                {
                    this.costcenterField = value;
                }
            }

            /// <remarks/>
            public string USRID_LONG
        {
                get
                {
                    return this.usridLongField;
                }
                set
                {
                    this.usridLongField = value;
                }
            }

            /// <remarks/>
            public string ZZCARDID
        {
                get
                {
                    return this.zzcardidField;
                }
                set
                {
                    this.zzcardidField = value;
                }
            }
            //public string PersonalArea { get; set; }
            //public string PersonalSubArea { get; set; }
            //public string ApproverId { get; set; }
            //public string ApproverName { get; set; }
            //public string Grade { get; set; }
            //public string Dept { get; set; }
            //public string UserId { get; set; }
            //public string Password { get; set; }
            //public string CostCenter { get; set; }

            //public string CardID { get; set; }
            //public string NewPassword { get; set; }
            //public string MobileNo { get; set; }
            //public string ConfirmPassword { get; set; }
            //public string ReturnUrl { get; set; }
            //public string UserName { get; set; }
            //public int EnterOTP { get; set; }
          
            //// public int EnterOTP { get; set; }
            //public string EmployeeName { get; set; }
            //public string EmployeeMail { get; set; }
            //public string EmployeeDOB { get; set; }
            //public  string GetConnectionString()
            //{
            //    return Startup.ConnectionString;

            //}

            //public bool ViewAssessmentClicked { get; set; }
            //public int otpflag { get; set; }
            //public string EmpEmail { get; set; }
            //public DateTime DateOfBirth { get; set; }
            //public int ReturnsaveValue { get; set; }
            //public int IsPwdExpired { get; set; }


        }

    }

