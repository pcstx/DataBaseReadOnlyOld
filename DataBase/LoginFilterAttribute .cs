using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataBase
{
    public class LoginFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            string loginType = ConfigurationManager.AppSettings["loginType"];
            if (loginType != "1")
            {
                if (filterContext.HttpContext.Session["ConnectionString_login"] == null)
                {
                   filterContext.HttpContext.Response.Redirect("/Home/Login");
                }
            }
        }
         

    }
}