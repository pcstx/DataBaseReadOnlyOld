using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;
using System.Data.SqlClient;

namespace DataBase.DataAccess
{
    public class SelectTableAccess
    { 
        public DataSet Select(string sql, string connName)
        {
            sql = " set rowcount 100; " + sql;
            DataSet ds = new DataSet();
            using (SqlConnection conn = ConnectionString.GetConnection(connName) as SqlConnection)
            {
                using (SqlDataAdapter dapter = new SqlDataAdapter(sql,conn))
                {
                    dapter.Fill(ds);                
                }
                conn.Close();
            }
            return ds;
        }

    }
}