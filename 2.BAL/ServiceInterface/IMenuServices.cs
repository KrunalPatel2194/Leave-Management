using System;
using System.Collections.Generic;
using System.Text;
using LeaveSolution.BAL.ServiceModels;

namespace LeaveSolution.BAL.ServiceInterface
{
    public interface IMenuServices
    {
       List< MenuServiceModel> GetMenu(string ModuleName, string empID);
    }
}
