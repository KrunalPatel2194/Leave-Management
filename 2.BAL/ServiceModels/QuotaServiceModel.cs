using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveSolution.BAL.ServiceModels
{
    public class QuotaServiceModel
    {
        
            private Zftm0007ServiceTab[] quotaOverviewField;
            public Zftm0007ServiceTab[] QuotaOverview
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
       
        public class Zftm0007ServiceTab
        {
            private string employeeID;
            private string ktextField;

            private string begdaField;

            private string enddaField;

            private decimal anzhlField;

            private decimal kverbField;
            private string leavecode;
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
            /// <remarks/>
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
}
