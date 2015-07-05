using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper
{
    public class Parameters
    {
        private CommandType _cmdType = CommandType.Text;
        private int _commandTimeout = 30;

        /// <summary>
        /// 连接字符串名称
        /// </summary>
        public string connectionStringName { get; set; }

        /// <summary>
        /// 操作类型;sql语句或存储过程
        /// </summary>
        public CommandType cmdType 
        {
            get { return _cmdType; }
            set { _cmdType = value; }
        }

        /// <summary>
        /// 参数集合
        /// </summary>
       public  SqlParameterCollection commandParameters{get;set;}

        /// <summary>
        /// 事务
        /// </summary>
       public  SqlTransaction tran {get;set;}

        /// <summary>
        /// 超时时间
        /// </summary>
       public int CommandTimeout 
       {
           get { return _commandTimeout; }
           set { _commandTimeout = value; }
       }
    }
}
