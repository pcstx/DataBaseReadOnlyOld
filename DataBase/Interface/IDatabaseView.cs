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
        IEnumerable<DataBaseView> GetDatabase();

        IEnumerable<TablesView> GetTables(string dbName);

        IEnumerable<RowsView> GetRows(string dbName, string TableName);

        IEnumerable<RowsView> GetRowsPaging(string dbName, string TableName, int pageSize, int page);

        int GetRowsCount(string dbName, string TableName);

        string GetTableDescription(string dbName, string tableName);
        /// <summary>
        /// 编辑说明
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="TableName"></param>
        /// <param name="rowName"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        int EditDescription(string dbName, string TableName, string rowName, string Description);
    }
}
