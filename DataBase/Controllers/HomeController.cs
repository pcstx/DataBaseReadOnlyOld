using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataBase.Models.ViewModels;
using System.Configuration;
using System.Text;

namespace DataBase.Controllers
{
    public class HomeController : Controller
    {
        //DataAccess.HomeDA homeDA = new DataAccess.HomeDA();
        IDatabaseView homeDA = new DataAccess.DatabaseViewAccess();

        [LoginFilterAttribute]
        public ActionResult Index()
        {
            List<string> list_connStr = ConnectionString.GetConnectionStringCount();
            
            return View(list_connStr);
        }

        [LoginFilter]
        public ActionResult Sider()
        {
            List<string> list_connStr = ConnectionString.GetConnectionStringCount();
             
            return PartialView(list_connStr);
        }

        /// <summary>
        /// 返回数据库json
        /// </summary>
        /// <returns></returns>
        [LoginFilterAttribute]
        public ActionResult DataBaseJson(string connectionStringName = "SqlServerHelper")
        {
            IEnumerable<DataBase.Models.ViewModels.DataBaseView> list_database = null;
            string connectionString = ConnectionString.connectionString(connectionStringName);
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                list_database = CacheHelper.GetCache(connectionString) as IEnumerable<DataBase.Models.ViewModels.DataBaseView>;
                if (list_database == null || list_database.Count() <= 0)
                {
                    list_database = homeDA.GetDatabase(connectionStringName);
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
        public ActionResult TablesJson(string dbName, string connectionStringName = "SqlServerHelper")
        {
              IEnumerable<TablesView> list_tables = null;
              string connectionString = ConnectionString.connectionString(connectionStringName);
              if (!string.IsNullOrWhiteSpace(connectionString))
              {
                  list_tables = CacheHelper.GetCache(connectionString + "_Table_" + dbName) as IEnumerable<TablesView>;
                  if (list_tables == null || list_tables.Count() <= 0)
                  {
                      list_tables = homeDA.GetTables(dbName,connectionStringName);
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
        public JsonResult RowsJson(string dbName, string TableName, string connectionStringName = "SqlServerHelper")
        { 
            IEnumerable<RowsView> list_rows = homeDA.GetRows(dbName, TableName,connectionStringName);

            return Json(list_rows, "text/plain", JsonRequestBehavior.AllowGet);
        }


        [LoginFilterAttribute]
        public JsonResult RowsGridJson(string dbName, string TableName, string connectionStringName = "SqlServerHelper")
        {
            int page =Convert.ToInt32(HttpContext.Request["page"]);
            int pageSize = Convert.ToInt32(HttpContext.Request["pageSize"]);

            RowsGridView rgv = new RowsGridView();
            rgv.Rows = homeDA.GetRowsPaging(dbName, TableName, pageSize, page,connectionStringName);
            rgv.Total = homeDA.GetRowsCount(dbName,TableName,connectionStringName);

            return Json(rgv, "text/plain", JsonRequestBehavior.AllowGet);
        }


        [LoginFilterAttribute]
        public ActionResult RowsGrid(string dbName, string TableName, string connectionStringName = "SqlServerHelper")
        {
            ViewBag.dbName = dbName;
            ViewBag.TableName = TableName; 
            string desc = homeDA.GetTableDescription(dbName, TableName,connectionStringName);
            ViewBag.Desc = desc;
            ViewBag.ConnName = connectionStringName;
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

        /// <summary>
        /// 查询界面
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <param name="connectionStringName"></param>
        /// <param name="selectType">1是表视图；2是存储过程</param>
        /// <returns></returns>
         public ActionResult Select(string dbName, string tableName,string connectionStringName = "SqlServerHelper", int selectType=1)
         {
             ViewBag.dbName = dbName;
             ViewBag.Conn = connectionStringName;
             ViewBag.tableName = tableName;

            if (selectType == 1)
            {
                ViewBag.SQL = string.Format("select top 100 * from [{0}] with(nolock)", tableName);
            }
            else if (selectType == 2) {
                ///查询存储过程参数
                var param = homeDA.GetProcedureParameters(dbName, tableName, connectionStringName);
                StringBuilder procedureParam = new StringBuilder();
                procedureParam.AppendFormat(" EXEC  [{0}] \r\n ", tableName);
                foreach (var p in param)
                {
                    procedureParam.AppendFormat(" {0} = {1}, \r\n ",p.name,p.type); 
                }
                string procedureStr = procedureParam.ToString();
                if (procedureStr.LastIndexOf(',') > 0)
                {
                    procedureStr = procedureStr.Substring(0, procedureStr.LastIndexOf(','));
                }
                ViewBag.SQL = procedureStr;
            }
            
             return View();
         }

        [LoginFilter]
        [HttpPost]        
         public ActionResult SelectTable(string sql,string dbName, string connectionStringName = "SqlServerHelper")
         {
             DataAccess.SelectTableAccess access = new DataAccess.SelectTableAccess();
             var dy = access.Select(sql, dbName, connectionStringName);
             return PartialView(dy);
         }

        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dbName"></param>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportTable(string sql, string dbName, string connectionStringName = "SqlServerHelper")
        {
            DataAccess.SelectTableAccess access = new DataAccess.SelectTableAccess();
            var dy = access.Select(sql, dbName, connectionStringName);
            string path = Server.MapPath(@"~/App_Data/Export/export.xls");
            bool result= ExcelExport.ExportExcelWithAspose(dy, path);

            return File(path, "application/ms-excel", "export.xls");
        }

        [LoginFilter]
        [HttpPost]
        public ActionResult EditRowDescription(string dbName, string tableName, string rowName, string description, string connectionStringName= "SqlServerHelper")
        {
            int result= homeDA.EditDescription(dbName, tableName, rowName, description, connectionStringName);
            return Content(result.ToString());
        }

        [LoginFilter]
        [HttpPost]
        public ActionResult EditTableDescription(string dbName, string tableName, string description, string connectionStringName)
        {
            int result = homeDA.EditTableDescription(dbName, tableName, description, connectionStringName);
            return Content(result.ToString());
        }

        public ActionResult Test()
        {
            return PartialView();
        }

    }
}