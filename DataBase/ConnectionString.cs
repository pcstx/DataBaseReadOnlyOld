using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DataBase
{
    public enum ConnType
    { 
        write=1,
        read=2
    }

    public class ConnectionString
    {
       static string configFilePath = HttpContext.Current.Server.MapPath("/App_Data/connectionStrings.config");
                     
        /// <summary>
        /// 默认选择第一个连接字符串
        /// </summary>
        /// <returns>返回连接字符串</returns>
        public static string connectionString(int connType = 1)
        {
            string connStr = string.Empty;
            string loginType = ConfigurationManager.AppSettings["loginType"];
            if (loginType == "1")
            { 
                XElement root = XElement.Load(configFilePath);
                XElement query = (from item in root.Elements()
                                  where (item.Attribute("default").Value == "true")
                                  select item).FirstOrDefault();
                if (query != null)
                {
                    connStr = query.Element(connType == 1 ? "write" : "read").Attribute("connectionString").Value;
                }              
                //if (ConfigurationManager.ConnectionStrings.Count > 0)
                //{
                //    connStr = ConfigurationManager.ConnectionStrings[0].ConnectionString;
                //} 
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
        public static string connectionString(string connectionStringName,int connType=1)
        { 
            string connStr = string.Empty;
             string loginType = ConfigurationManager.AppSettings["loginType"]; 
             if (loginType == "1") //0登陆，1连接字符串配置
             { 
                 if (string.IsNullOrEmpty(connectionStringName))
                 {
                    connStr=connectionString(connType);
                 }
                 else
                 {
                     //读取connectionStrings.config里面的字段                 
                     XElement root = XElement.Load(configFilePath);
                     XElement query = (from item in root.Elements()
                                       where (item.Attribute("name").Value == connectionStringName)
                                       select item).FirstOrDefault();
                     if (query != null)
                     {
                         connStr = query.Element(connType == 1 ? "write" : "read").Attribute("connectionString").Value;
                     }         
                    // connStr = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
                 }
             }
             else
             {
                 connStr = HttpContext.Current.Session["ConnectionString_login"] as string; 
             } 
             return connStr;
        }

        public static IDbConnection GetConnection(string connectionStringName=null,int connType=1)
        {
            IDbConnection conn = new SqlConnection(connectionString(connectionStringName,connType));
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

        public static List<string> GetConnectionStringCount()
        {
            List<string> list_connStr = new List<string>();
             string loginType = ConfigurationManager.AppSettings["loginType"];
             if (loginType == "1")
             {
                 XElement root = XElement.Load(configFilePath);
                 list_connStr = root.Elements().Select(n => n.Attribute("name").Value).ToList<string>(); 
             } 
             return list_connStr;
        }

        public static string GetDataSource(string connectionStringName)
        {
            SqlConnectionStringBuilder sqlConnStr = new SqlConnectionStringBuilder(connectionString(connectionStringName));
            return sqlConnStr.DataSource;
        }

    }
}