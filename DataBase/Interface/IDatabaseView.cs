using DataBase.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public interface IDatabaseView
    {
        IEnumerable<DataBaseView> GetDatabase(string connectionStringName);

        IEnumerable<TablesView> GetTables(string dbName, string connectionStringName,string tableName="");

        IEnumerable<RowsView> GetRows(string dbName, string TableName, string connectionStringName);

        IEnumerable<RowsView> GetRowsPaging(string dbName, string TableName, int pageSize, int page, string connectionStringName);

        int GetRowsCount(string dbName, string TableName, string connectionStringName);

        string GetTableDescription(string dbName, string tableName, string connectionStringName);
        /// <summary>
        /// 编辑说明
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="TableName"></param>
        /// <param name="rowName"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        int EditDescription(string dbName, string TableName, string rowName, string Description, string connectionStringName);
        int EditTableDescription(string dbName, string TableName, string Description, string connectionStringName);

        IEnumerable<ViewsView> GetViews(string dbName, string connectionStringName, string viewName = "");

        IEnumerable<ProcedureView> GetProcedure(string dbName, string connectionStringName, string procedureName="");

        string GetViewSQL(string dbName, string viewName, string connectionStringName);

        string GetProcedureSQL(string dbName, string procedureName, string connectionStringName);

        IEnumerable<ProcedureParam> GetProcedureParameters(string dbName, string procedureName, string connectionStringName);

        IEnumerable<ColumnsView> GetColumns(string dbName, string tableName, string connectionStringName);
    }
}
