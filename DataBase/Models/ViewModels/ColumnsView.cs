using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBase.Models.ViewModels
{
    public class ColumnsView
    {
        public string name { get; set; }

        public string rowType { get; set; }

        public int isnullable { get; set; }

        public int length { get; set; }
    }
}