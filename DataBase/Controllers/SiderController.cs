using DataBase.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataBase.Controllers
{
    [LoginFilter]
    public class SiderController : Controller
    {
        IDatabaseView homeDA = new DataAccess.DatabaseViewAccess();
         
        public ActionResult Index()
        {
            List<string> list_connStr = ConnectionString.GetConnectionStringCount();             
            return View(list_connStr);
        }
         
        public ActionResult SiderDatabase(string connectionStringName = "SqlServerHelper")
        {
            IEnumerable<DataBase.Models.ViewModels.DataBaseView> list_database = null;
            string connectionString = ConnectionString.connectionString(connectionStringName);
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                list_database = CacheHelper.GetCache(connectionString) as IEnumerable<DataBase.Models.ViewModels.DataBaseView>;
                if (list_database == null || list_database.Count() <= 0)
                {
                    list_database = homeDA.GetDatabase(connectionStringName);
                    CacheHelper.SetCache(connectionString, list_database, TimeSpan.FromHours(2));
                }
            }

            ViewBag.conn = connectionStringName;
            return PartialView(list_database);
        }
         
        public ActionResult TablesJson(string dbName, string connectionStringName = "SqlServerHelper",string tableName="")
        {
            IEnumerable<TablesView> list_tables = null;
            string connectionString = ConnectionString.connectionString(connectionStringName);
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                list_tables = CacheHelper.GetCache(connectionString + "_Table_" + dbName) as IEnumerable<TablesView>;
                if (list_tables == null || list_tables.Count() <= 0)
                {
                    list_tables = homeDA.GetTables(dbName, connectionStringName, tableName);
                    CacheHelper.SetCache(connectionString + "_Table_" + dbName, list_tables, TimeSpan.FromHours(2));
                }
                else
                {
                    if (!string.IsNullOrEmpty(tableName))
                    { 
                        list_tables = list_tables.Where(t => t.Name.ToLower().Contains(tableName.ToLower()));
                    } 
                }
            } 
            return Json(list_tables, "text/plain", JsonRequestBehavior.AllowGet);
        }
         
        public ActionResult ViewsJson(string dbName, string connectionStringName = "SqlServerHelper",string viewName="")
        {
            IEnumerable<ViewsView> list_Views = null;
            string connectionString = ConnectionString.connectionString(connectionStringName);
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                list_Views = CacheHelper.GetCache(connectionString + "_View_" + dbName) as IEnumerable<ViewsView>;
                if (list_Views == null || list_Views.Count() <= 0)
                {
                    list_Views = homeDA.GetViews(dbName, connectionStringName,viewName);
                    CacheHelper.SetCache(connectionString + "_View_" + dbName, list_Views, TimeSpan.FromHours(2));
                }
                else
                {
                    if (!string.IsNullOrEmpty(viewName))
                    {
                        list_Views = list_Views.Where(t => t.Name.ToLower().Contains(viewName.ToLower()));
                    }
                }
            }
            return Json(list_Views, "text/plain", JsonRequestBehavior.AllowGet);
        }
         
        public ActionResult ProcedureJson(string dbName, string connectionStringName = "SqlServerHelper",string procedureName="")
        {
            IEnumerable<ProcedureView> list_Procedure = null;
            string connectionString = ConnectionString.connectionString(connectionStringName);
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                list_Procedure = CacheHelper.GetCache(connectionString + "_Procedure_" + dbName) as IEnumerable<ProcedureView>;
                if (list_Procedure == null || list_Procedure.Count() <= 0)
                {
                    list_Procedure = homeDA.GetProcedure(dbName, connectionStringName, procedureName);
                    CacheHelper.SetCache(connectionString + "_Procedure_" + dbName, list_Procedure, TimeSpan.FromHours(2));
                }
                else
                {
                    if (!string.IsNullOrEmpty(procedureName))
                    {
                        list_Procedure = list_Procedure.Where(t => t.Name.ToLower().Contains(procedureName.ToLower()));
                    }
                }
            }
            return Json(list_Procedure, "text/plain", JsonRequestBehavior.AllowGet);
        }
         
    }
}