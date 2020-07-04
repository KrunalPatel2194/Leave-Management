using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveSolution.Models
{
    public class MenuviewModel
    {
        public List<MenuModel> MenuModels { get; set; }
   
    }
    public class MenuModel
    {
        public int RollID { get; set; }
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int ParentMenuID { get; set; }
    }
}
