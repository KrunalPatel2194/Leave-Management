using System;
//using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using LeaveSolution.BAL.ServiceModels;
using LeaveSolution.DAL.Interfaces;
using LeaveSolution.DAL.Data;
using System.Linq;
using AutoMapper;
using Oracle.ManagedDataAccess.Client;
//using Oracle.DataAccess.Types;
using Oracle.ManagedDataAccess.Types;
using LeaveSolution.BAL.ServiceInterface;
using LeaveSolution.Core;
using Microsoft.AspNetCore.Http;

namespace LeaveSolution.BAL.ServiceInterface
{
    public class MenuServices : IMenuServices
    {

        private IMenumasterRepositay _menumasterRepositay;
        public MenuServices(IMenumasterRepositay menumasterRepositay)
        {
            _menumasterRepositay = menumasterRepositay;
        }

        public List <MenuServiceModel> GetMenu(string ModuleName,string EmpId)
        {
            //string ModuleName = HttpContext.
            List<MenuServiceModel> listmodel = new List<MenuServiceModel>();
            listmodel =Mapper.Map<List<MenuServiceModel>>( _menumasterRepositay.GetMenuMaster(ModuleName,EmpId));
            return listmodel;
        }
    }
}
