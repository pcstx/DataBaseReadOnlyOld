using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBase.Models.ViewModels
{
    public class RowsGridView
    {
        public int Total { get; set; }
        public IEnumerable<RowsView> Rows { get; set; }

    }
}