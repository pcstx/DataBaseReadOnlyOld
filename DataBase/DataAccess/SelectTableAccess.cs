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
        public DataSet Select(string sql, string dbName, string connName)
        {
             DataSet ds = new DataSet();
             try
             {
                 sql = " use [" + dbName + "]; set rowcount 100; " + sql;
                 using (SqlConnection conn = ConnectionString.GetConnection(connName,2) as SqlConnection)
                 {
                     try
                     {
                         using (SqlDataAdapter dapter = new SqlDataAdapter(sql, conn))
                         {
                             dapter.Fill(ds);
                         }
                     }
                     catch (Exception ex)
                     {
                         DataTable dt = new DataTable();                         
                         DataColumn dc = new DataColumn("错误");
                         dt.Columns.Add(dc);
                         DataRow dr = dt.NewRow();
                         dr[0] = ex.Message;
                         dt.Rows.Add(dr);
                         ds.Tables.Add(dt);
                     }
                     conn.Close();
                 }
             }
             catch (Exception ex)
             {
                 DataTable dt = new DataTable();
                 DataColumn dc = new DataColumn("错误");
                 dt.Columns.Add(dc);
                 DataRow dr = dt.NewRow();
                 dr[0] = ex.Message;
                 dt.Rows.Add(dr);
                 ds.Tables.Add(dt);
             }
            return ds;
        }
         

    }
}