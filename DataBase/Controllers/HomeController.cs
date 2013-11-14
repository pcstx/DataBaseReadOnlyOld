using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBHelper;
using DataBase.Models.ViewModels;

namespace DataBase.Controllers
{
    public class HomeController : Controller
    {
        DataAccess.HomeDA homeDA = new DataAccess.HomeDA();
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 返回数据库json
        /// </summary>
        /// <returns></returns>
        public JsonResult DataBaseJson()
        {
            IEnumerable<DataBase.Models.ViewModels.DataBaseView> list_database = homeDA.GetDatabase();
            return Json(list_database, "text/plain", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 返回表格json
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        /// <returns></returns>
        public JsonResult TablesJson(string dbName)
        {
            IEnumerable<TablesView> list_tables = homeDA.GetTables(dbName);

            return Json(list_tables, "text/plain", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 返回列json
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        /// <param name="TableName">表格名称</param>
        /// <returns></returns>
        public JsonResult RowsJson(string dbName, string TableName)
        { 
            IEnumerable<RowsView> list_rows = homeDA.GetRows(dbName, TableName);

            return Json(list_rows, "text/plain", JsonRequestBehavior.AllowGet);
        }

        public JsonResult RowsGridJson(string dbName, string TableName)
        {
            int page =Convert.ToInt32(HttpContext.Request["page"]);
            int pageSize = Convert.ToInt32(HttpContext.Request["pageSize"]);

            RowsGridView rgv = new RowsGridView();
            rgv.Rows = homeDA.GetRowsPaging(dbName, TableName, pageSize, page);
            rgv.Total = homeDA.GetRowsCount(dbName,TableName); 
            return Json(rgv, "text/plain", JsonRequestBehavior.AllowGet);
        }

        public ActionResult RowsGrid(string dbName, string TableName)
        {
            ViewBag.dbName = dbName;
            ViewBag.TableName = TableName;
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}