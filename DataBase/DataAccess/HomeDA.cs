using DataBase.Models.ViewModels;
using DBHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBase.DataAccess
{
    public class HomeDA
    {
        public IEnumerable<DataBaseView> GetDatabase()
        {
            string dbSQL = "select dbid as id,name,crdate as createDate,type='database' from master..sysdatabases where dbid > 4 --用户库";
            SqlHelper.connectionString = ConnectionString.connectionString("SqlServerHelper");
            IEnumerable< DataBaseView> list_database = SqlHelper.ExecuteIEnumerable<DataBaseView>(dbSQL);
            return list_database;
        }

        public IEnumerable<TablesView> GetTables(string dbName)
        {
            string tableSQL = @"select t.object_id as id,t.name as name,t.create_date as createdate,
                                    t.modify_date as modifydate,s.value as note,databaseName='{0}', type='table'
                                    from {0}.sys.objects t
                                    left join {0}.sys.extended_properties s on t.object_id=s.major_id and s.minor_id=0  
                                    where [type] = 'u' and is_ms_shipped=0";

            tableSQL = string.Format(tableSQL, dbName); 
            IEnumerable<TablesView> list_tables = SqlHelper.ExecuteIEnumerable<TablesView>(tableSQL, "SqlServerHelper");

            return list_tables;
        }

        public IEnumerable<RowsView> GetRows(string dbName, string TableName)
        {
            string rowSQL = @" SELECT id=C.column_id,name=C.name,primaryKey=ISNULL(IDX.PrimaryKey,N''),
                                    rowType=T.name,lenght=C.max_length,isNull=C.is_nullable,defaultValue=ISNULL(D.definition,N''),
                                    note=ISNULL(PFD.[value],N''), type='row'
                                    FROM {0}.sys.columns C
                                        INNER JOIN {0}.sys.objects O
                                            ON C.[object_id]=O.[object_id] AND O.type='U' AND O.is_ms_shipped=0
                                        INNER JOIN sys.types T
                                            ON C.user_type_id=T.user_type_id
                                        LEFT JOIN sys.default_constraints D
                                            ON C.[object_id]=D.parent_object_id AND C.column_id=D.parent_column_id AND C.default_object_id=D.[object_id]
                                        LEFT JOIN sys.extended_properties PFD
                                            ON PFD.class=1 AND C.[object_id]=PFD.major_id AND C.column_id=PFD.minor_id
                                        LEFT JOIN                       -- 索引及主键信息
                                        (  SELECT IDXC.[object_id],IDXC.column_id,PrimaryKey=IDX.is_primary_key
                                            FROM sys.indexes IDX
		                                    INNER JOIN sys.index_columns IDXC
			                                    ON IDX.[object_id]=IDXC.[object_id] AND IDX.index_id=IDXC.index_id
                                        ) IDX
                                            ON C.[object_id]=IDX.[object_id] AND C.column_id=IDX.column_id 
                                        WHERE O.name=N'{1}'       -- 如果只查询指定表,加上此条件
                                    ORDER BY O.name,C.column_id ";

            rowSQL = string.Format(rowSQL, dbName, TableName);
            //SqlHelper.connectionString = ConnectionString.connectionString("SqlServerHelper");
            IEnumerable<RowsView> list_rows = SqlHelper.ExecuteIEnumerable<RowsView>(rowSQL, "SqlServerHelper");

            return list_rows;
        }

        public IEnumerable<RowsView> GetRowsPaging(string dbName, string TableName,int pageSize,int page)
        {
            string rowSQL = @"  SELECT TOP {2} * 
                                        FROM   (
                                        SELECT ROW_NUMBER() OVER (ORDER BY C.column_id) AS RowNumber,
                                        id=C.column_id,name=C.name,primaryKey=ISNULL(IDX.PrimaryKey,N''),
                                        rowType=T.name,lenght=C.max_length,isNull=C.is_nullable,defaultValue=ISNULL(D.definition,N''),
                                        note=ISNULL(PFD.[value],N''), type='row'
                                        FROM {0}.sys.columns C
                                            INNER JOIN  {0}.sys.objects O
                                                ON C.[object_id]=O.[object_id] AND O.type='U' AND O.is_ms_shipped=0
                                            INNER JOIN {0}.sys.types T
                                                ON C.user_type_id=T.user_type_id
                                            LEFT JOIN {0}.sys.default_constraints D
                                                ON C.[object_id]=D.parent_object_id AND C.column_id=D.parent_column_id AND C.default_object_id=D.[object_id]
                                            LEFT JOIN {0}.sys.extended_properties PFD
                                                ON PFD.class=1 AND C.[object_id]=PFD.major_id AND C.column_id=PFD.minor_id
                                            LEFT JOIN                       -- 索引及主键信息
                                            (  SELECT IDXC.[object_id],IDXC.column_id,PrimaryKey=IDX.is_primary_key
                                                FROM {0}.sys.indexes IDX
		                                        INNER JOIN {0}.sys.index_columns IDXC
			                                        ON IDX.[object_id]=IDXC.[object_id] AND IDX.index_id=IDXC.index_id
                                            ) IDX
                                                ON C.[object_id]=IDX.[object_id] AND C.column_id=IDX.column_id 
                                            WHERE O.name=N'{1}'  
	                                        ) A      
                                        WHERE RowNumber > {2}*({3}-1)";

            rowSQL = string.Format(rowSQL, dbName, TableName,pageSize,page); 
            IEnumerable<RowsView> list_rows = SqlHelper.ExecuteIEnumerable<RowsView>(rowSQL, "SqlServerHelper");

            return list_rows;
        }

        public int GetRowsCount(string dbName, string TableName)
        {
            string rowSQL = @" SELECT  count(*) FROM {0}.sys.columns C INNER JOIN {0}.sys.objects O ON C.[object_id]=O.[object_id] AND O.type='U' AND O.is_ms_shipped=0 WHERE O.name=N'{1}' ";

            rowSQL = string.Format(rowSQL, dbName, TableName);

            int rowCount =(int)SqlHelper.ExecuteScalar(rowSQL, "SqlServerHelper");
            return rowCount;
        }

        /// <summary>
        /// 编辑说明
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="TableName"></param>
        /// <param name="rowName"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public int EditDescription(string dbName,string TableName,string rowName, string Description)
        {
            string sql = @" if exists
                            (  select * FROM  {0}.sys.extended_properties PFD
	                        left join  {0}.sys.columns C 
	                        ON PFD.class=1 AND C.[object_id]=PFD.major_id AND C.column_id=PFD.minor_id        
	                        left JOIN  {0}.sys.objects O
				                        ON C.[object_id]=O.[object_id] AND O.type='U' AND O.is_ms_shipped=0		 
	                        where C.name='{2}' and O.name='{1}' )
                        begin
	                        USE {0};
                            EXEC sp_updateextendedproperty N'MS_Description',   '{3}',   N'user',   N'dbo',   N'table',   N'{1}',   N'column',   N'{2}'
                        end
                        else
                        begin
	                        USE {0};
	                        EXECUTE   sp_addextendedproperty   N'MS_Description',   '{3}',   N'user',   N'dbo',   N'table',   N'{1}',   N'column',   N'{2}' 
                        end ";  //判断是否存在，不存在新增，存在修改

            sql = string.Format(sql, dbName,TableName,rowName,Description); 
            int result = SqlHelper.ExecteNonQuery(sql, "SqlServerHelper");
            return result;
        }

    }
}