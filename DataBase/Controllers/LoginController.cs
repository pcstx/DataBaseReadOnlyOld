using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataBase.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string server, string uid, string password)
        {
            bool isConn = ConnectionString.TestConnection(server, uid, password);
            if (isConn)
            {
                Session["ConnectionString_login"] = string.Format("server={0};uid={1};pwd={2};", server, uid, password);
                return Content("1");
            }
            else
            {
                Session["ConnectionString_login"] = null;
                return Content("0");
            }
        }

        /// <summary>
        /// 功能说明
        /// </summary>
        /// <returns></returns>
        public ActionResult Info()
        {
            return View();
        }

    }
}