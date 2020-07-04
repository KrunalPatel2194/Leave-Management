using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveSolution.DAL.Data
{
    public class tbl_MenuMaster
    {
        public int RollID { get; set; }
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public string Controller { get; set; }
        public string  Action { get; set; }

        public int ParentMenuID { get; set; }
    }
}
