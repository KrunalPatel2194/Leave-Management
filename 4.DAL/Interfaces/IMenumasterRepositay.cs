using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.DAL.Data;

namespace LeaveSolution.DAL.Interfaces
{
    public interface IMenumasterRepositay
    {
        List<tbl_MenuMaster> GetMenuMaster(string ModuleName,string Empid);
    }
}
