using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBase.Models.ViewModels
{
    public class DataBaseView:TableModels.DataBase
    {
        public string type { get; set; }

        public string connName { get; set; }
    }
}