using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBase.Models.ViewModels
{
    public class ConnectionStrings
    {
        public int id { get; set; }
        public string connectionStringName { get; set; }
        public string connectionString { get; set; }
    }
}