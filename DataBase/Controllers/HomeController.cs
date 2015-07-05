using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc; 
using DataBase.Models.ViewModels;
using System.Configuration;

namespace DataBase.Controllers
{
    public class HomeController : Controller
    {
        //DataAccess.HomeDA homeDA = new DataAccess.HomeDA();
        IDatabaseView homeDA = new DataAccess.DatabaseViewAccess();

        [LoginFilterAttribute]
        public ActionResult Index()
        { 
            return View();
        }

        /// <summary>
        /// 返回数据库json
        /// </summary>
        /// <returns></returns>
        [LoginFilterAttribute]
        public ActionResult DataBaseJson()
        {
            IEnumerable<DataBase.Models.ViewModels.DataBaseView> list_database = null;
            string connectionString = ConnectionString.connectionString("SqlServerHelper");
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                list_database = CacheHelper.GetCache(connectionString) as IEnumerable<DataBase.Models.ViewModels.DataBaseView>;
                if (list_database == null || list_database.Count() <= 0)
                {
                    list_database = homeDA.GetDatabase();
                    CacheHelper.SetCache(connectionString, list_database,TimeSpan.FromHours(2));
                }
            } 
            return Json(list_database, "text/plain", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 返回表格json
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        /// <returns></returns>
        [LoginFilterAttribute]
        public ActionResult TablesJson(string dbName)
        {
              IEnumerable<TablesView> list_tables = null;
              string connectionString = ConnectionString.connectionString("SqlServerHelper");
              if (!string.IsNullOrWhiteSpace(connectionString))
              {
                  list_tables = CacheHelper.GetCache(connectionString + "_Table_" + dbName) as IEnumerable<TablesView>;
                  if (list_tables == null || list_tables.Count() <= 0)
                  {
                      list_tables = homeDA.GetTables(dbName);
                      CacheHelper.SetCache(connectionString + "_Table_" + dbName, list_tables, TimeSpan.FromHours(2));
                  }
              } 
            return Json(list_tables, "text/plain", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 返回列json
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        /// <param name="TableName">表格名称</param>
        /// <returns></returns>
        [LoginFilterAttribute]
        public JsonResult RowsJson(string dbName, string TableName)
        { 
            IEnumerable<RowsView> list_rows = homeDA.GetRows(dbName, TableName);

            return Json(list_rows, "text/plain", JsonRequestBehavior.AllowGet);
        }


        [LoginFilterAttribute]
        public JsonResult RowsGridJson(string dbName, string TableName)
        {
            int page =Convert.ToInt32(HttpContext.Request["page"]);
            int pageSize = Convert.ToInt32(HttpContext.Request["pageSize"]);

            RowsGridView rgv = new RowsGridView();
            rgv.Rows = homeDA.GetRowsPaging(dbName, TableName, pageSize, page);
            rgv.Total = homeDA.GetRowsCount(dbName,TableName);

            return Json(rgv, "text/plain", JsonRequestBehavior.AllowGet);
        }


        [LoginFilterAttribute]
        public ActionResult RowsGrid(string dbName, string TableName)
        {
            ViewBag.dbName = dbName;
            ViewBag.TableName = TableName; 
            string desc = homeDA.GetTableDescription(dbName, TableName);
            ViewBag.Desc = desc; 
            return View();
        }

        [HttpPost]
        public ActionResult EditConfig(ConnectionStrings conn)
        {
            if (!string.IsNullOrEmpty(conn.connectionStringName))
            {
                DataAccess.ConfigurationOperator configOper = new DataAccess.ConfigurationOperator();
                configOper.SetConnectionString(conn.connectionStringName, conn.connectionString);
                configOper.Save();
            }
         
            return RedirectToAction("Config");
        }
        public ActionResult Config()
        { 
            DataAccess.ConfigDA config = new DataAccess.ConfigDA();
            List<ConnectionStrings> list_conn= config.GetConnectionStrings();
            return View(list_conn);
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult Tree()
        {
            return View();
        } 

        public ActionResult Login()
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

        [LoginFilterAttribute]
        public ActionResult Logout()
        {
            Session["ConnectionString_login"] = null;
            return RedirectToAction("Login");
        }

        public static bool haveLogout()
        { 
             string loginType = ConfigurationManager.AppSettings["loginType"];
             if (loginType == "1")
             {
                 return false;
             }

             return true;
        }

         [LoginFilterAttribute]
        public ActionResult RefreshCache()
        { 
                CacheHelper.RemoveAllCache();
                return RedirectToAction("Index");
        }

    }
}