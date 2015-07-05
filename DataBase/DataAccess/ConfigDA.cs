using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
using DataBase.Models.ViewModels;
using System.Configuration;

namespace DataBase.DataAccess
{
    public class ConfigDA
    {

        public List<ConnectionStrings> GetConnectionStrings()
        {
            int id = 1;
         //   int total = ConfigurationManager.ConnectionStrings.Count;
            List<ConnectionStrings> list_conn = new List<ConnectionStrings>();
            foreach (ConnectionStringSettings connStr in ConfigurationManager.ConnectionStrings)
            {
                ConnectionStrings c = new ConnectionStrings();
                c.connectionString = connStr.ConnectionString;
                c.connectionStringName = connStr.Name;
                c.id = id;
                id++;
                list_conn.Add(c);
            }

            return list_conn;
        }

        public void EditConnectionString()
        {
             
        }

    }
}