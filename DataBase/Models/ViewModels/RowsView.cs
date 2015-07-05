using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBase.Models.ViewModels
{
    public class RowsView
    {
        public int id { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public int primaryKey { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string rowType { get; set; }
        /// <summary>
        /// 类型长度
        /// </summary>
        public int lenght { get; set; }
        /// <summary>
        /// 是否为空
        /// </summary>
        public int isNull { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string defaultValue { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string note { get; set; }
        public string type { get; set; }
    }
}