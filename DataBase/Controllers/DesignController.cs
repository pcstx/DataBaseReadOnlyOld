using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc; 

namespace DataBase.Controllers
{
    public class DesignController : Controller
    {
        IDatabaseView homeDA = new DataAccess.DatabaseViewAccess();

        public ActionResult ViewDesign(string dbName, string viewName, string connectionStringName = "SqlServerHelper")
        {
            ViewBag.SQL = homeDA.GetViewSQL(dbName, viewName, connectionStringName);

            return View();
        }

        public ActionResult ProcedureDesign(string dbName, string viewName, string connectionStringName = "SqlServerHelper")
        {
            ViewBag.SQL = homeDA.GetProcedureSQL(dbName, viewName, connectionStringName);

            return View("ViewDesign");
        }

        public ActionResult ProcedureSelect(string dbName, string tableName, string connectionStringName = "SqlServerHelper")
        {
            ViewBag.dbName = dbName;
            ViewBag.Conn = connectionStringName;
            ViewBag.tableName = tableName;
            return View();
        }

        public ActionResult EntityDesign(string dbName, string tableName, string connectionStringName = "SqlServerHelper")
        {
            var entity = homeDA.GetColumns(dbName, tableName, connectionStringName); 
            string entityStr = JsonConvert.SerializeObject(entity).Replace('\"', '\'');
             ViewBag.Data = HttpUtility.UrlEncode(entityStr);
            ViewBag.TableName = tableName;
            return View();
        }

    }
}