using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBase.Models.ViewModels
{
    public class TablesView
    {
        public int Id { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime createDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime modifyDate { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Note { get; set; }
        public string databaseName { get; set; }
        public string type { get; set; }

        public string connName { get; set; }
    }
}