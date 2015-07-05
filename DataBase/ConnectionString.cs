using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DataBase
{
    public class ConnectionString
    {
        /// <summary>
        /// 默认选择第一个连接字符串
        /// </summary>
        /// <returns>返回连接字符串</returns>
        public static string connectionString()
        {
            string connStr = string.Empty;
            string loginType = ConfigurationManager.AppSettings["loginType"];
            if (loginType == "1")
            {
                if (ConfigurationManager.ConnectionStrings.Count > 0)
                {
                    connStr = ConfigurationManager.ConnectionStrings[0].ConnectionString;
                } 
            }
            else
            {
                connStr = HttpContext.Current.Session["ConnectionString_login"] as string;
            } 
            return connStr;
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="connectionStringName">连接字符串名称</param>
        /// <returns>返回连接字符串</returns>
        public static string connectionString(string connectionStringName)
        {
            string connStr = string.Empty;
             string loginType = ConfigurationManager.AppSettings["loginType"]; 
             if (loginType == "1") //0登陆，1连接字符串配置
             {
                 if (string.IsNullOrEmpty(connectionStringName))
                 {
                    connStr=connectionString();
                 }
                 else
                 {
                     connStr = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
                 }
             }
             else
             {
                 connStr = HttpContext.Current.Session["ConnectionString_login"] as string; 
             } 
             return connStr;
        }

        public static IDbConnection GetConnection(string connectionStringName=null)
        {
            IDbConnection conn = new SqlConnection(connectionString(connectionStringName));
            conn.Open();
            return conn;
        }

        public static bool TestConnection(string server, string uid, string password)
        {
            string connectionString = string.Format("server={0};uid={1};pwd={2};", server, uid, password);
            try
            {
                IDbConnection conn = new SqlConnection(connectionString);
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}