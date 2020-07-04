using Microsoft.AspNetCore.Mvc.Filters;
using System;
using LeaveSolution.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using LeaveSolution.Models;

namespace LeaveSolution.CustomFilter
{
    public class Authentication : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (string.IsNullOrEmpty(Convert.ToString(context.HttpContext.Session.GetString(Constant.SessionUserName))))
            {
                context.Result = new RedirectToRouteResult(new Microsoft.AspNetCore.Routing.RouteValueDictionary
                    {
                        {"Controller","Account"},
                        {"Action","SignOut"}
                    });
            }

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string controllername = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ControllerName;
            string actionname = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ActionName;
            if (string.IsNullOrEmpty(Convert.ToString(context.HttpContext.Session.GetString(Constant.SessionUserName))))
            {
                context.Result = new RedirectToRouteResult(new Microsoft.AspNetCore.Routing.RouteValueDictionary
                    {
                        {"Controller","Account"},
                        {"Action","SignOut"}
                    });
            }
            else if (Convert.ToString(context.HttpContext.Session.GetString(Constant.SessionUserName)) != null)
            {
                MenuviewModel menuModel = JsonConvert.DeserializeObject<MenuviewModel>(context.HttpContext.Session.GetString(Constant.Menu));
                int count = menuModel.MenuModels.Where(x => x.Controller.ToUpper() == controllername.ToUpper() && x.Action.ToUpper() == actionname.ToUpper()).Count();               
                if (count == 0)
                {
                    context.Result = new RedirectToRouteResult(new Microsoft.AspNetCore.Routing.RouteValueDictionary
                    {
                        {"Controller","Home"},
                        {"Action","Accessdenied"}
                    });
                }
            }
        }
    }
}
