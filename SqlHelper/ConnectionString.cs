using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DBHelper
{
    public class ConnectionString
    {
        /// <summary>
        /// 默认选择第一个连接字符串
        /// </summary>
        /// <returns>返回连接字符串</returns>
        public static string connectionString()
        {
            if (ConfigurationManager.ConnectionStrings.Count > 0)
            {
                return ConfigurationManager.ConnectionStrings[0].ConnectionString;
            }
            else
            {
                return null;
            }           
        } 

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="connectionStringName">连接字符串名称</param>
        /// <returns>返回连接字符串</returns>
        public static string connectionString(string connectionStringName)
        {
            if(string.IsNullOrEmpty(connectionStringName))
            {
                return connectionString();
            }
            else
            {
                return ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            }
            
        } 
    }
}
