using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using DataBase.Models.ViewModels;
using System.Data;

namespace DataBase.DataAccess
{
    public class DatabaseViewAccess:IDatabaseView
    {
        public IEnumerable<DataBaseView> GetDatabase(string connectionStringName)
        {
            IEnumerable<DataBaseView> list_database = new List<DataBaseView>();
            try
            {
                string dbSQL = " select dbid as id,name,crdate as createDate,type='database',connName='"+connectionStringName+@"' from master..sysdatabases with(nolock) where dbid > 4 order by name;--用户库";
            
                using (IDbConnection conn = ConnectionString.GetConnection(connectionStringName))
                {
                    list_database = conn.Query<DataBaseView>(dbSQL);
                    return list_database;
                }
            }
            catch(Exception ex)
            {
                return list_database;
            }
        }

        public IEnumerable<TablesView> GetTables(string dbName, string connectionStringName,string tableName="")
        {
            IEnumerable<TablesView> list_tables = new List<TablesView>();
            try
            {
                string tableSQL = @" use {0}
                                    select t.object_id as Id,t.name as Name,t.create_date as createdate,
                                    t.modify_date as modifydate,s.value as note,databaseName='{0}', type='table',connName='{1}'
                                    from [{0}].sys.objects t with(nolock)
                                    left join [{0}].sys.extended_properties s with(nolock) on t.object_id=s.major_id and s.minor_id=0  
                                    where [type] = 'u' and is_ms_shipped=0 {2}
                                    order by name;";

                dbName=dbName.Replace('\'',' ');
                string tableSelect = "";
                if (!string.IsNullOrEmpty(tableName))
                {
                    tableSelect = string.Format(" and t.name like '%'+@name+'%' ");
                }
                tableSQL = string.Format(tableSQL, dbName,connectionStringName,tableSelect);
                using (IDbConnection conn = ConnectionString.GetConnection(connectionStringName))
                {
                    list_tables = conn.Query<TablesView>(tableSQL, new { name = tableName });
                    return list_tables;
                }
            }
            catch (Exception ex)
            {
                return list_tables;
            }
        }

        public IEnumerable<RowsView> GetRows(string dbName, string TableName, string connectionStringName)
        {
            IEnumerable<RowsView> list_rows = new List<RowsView>();
            try
            {
                dbName = dbName.Replace('\'', ' ');
                string rowSQL = @" use {0}
                                    SELECT id=C.column_id,name=C.name,primaryKey=ISNULL(IDX.PrimaryKey,N''),
                                    rowType=T.name,lenght=C.max_length,isNull=C.is_nullable,defaultValue=ISNULL(D.definition,N''),
                                    note=ISNULL(PFD.[value],N''), type='row'
                                    FROM [{0}].sys.columns C with(nolock)
                                        INNER JOIN [{0}].sys.objects O with(nolock)
                                            ON C.[object_id]=O.[object_id] AND O.type='U' AND O.is_ms_shipped=0
                                        INNER JOIN sys.types T with(nolock)
                                            ON C.user_type_id=T.user_type_id
                                        LEFT JOIN sys.default_constraints D with(nolock)
                                            ON C.[object_id]=D.parent_object_id AND C.column_id=D.parent_column_id AND C.default_object_id=D.[object_id]
                                        LEFT JOIN sys.extended_properties PFD with(nolock)
                                            ON PFD.class=1 AND C.[object_id]=PFD.major_id AND C.column_id=PFD.minor_id
                                        LEFT JOIN                       -- 索引及主键信息
                                        (  SELECT IDXC.[object_id],IDXC.column_id,PrimaryKey=IDX.is_primary_key
                                            FROM sys.indexes IDX with(nolock)
		                                    INNER JOIN sys.index_columns IDXC with(nolock)
			                                    ON IDX.[object_id]=IDXC.[object_id] AND IDX.index_id=IDXC.index_id
                                        ) IDX
                                            ON C.[object_id]=IDX.[object_id] AND C.column_id=IDX.column_id 
                                        WHERE O.name=@tableName       -- 如果只查询指定表,加上此条件
                                    ORDER BY O.name,C.column_id ";

                rowSQL = string.Format(rowSQL, dbName);

                using (IDbConnection conn = ConnectionString.GetConnection(connectionStringName))
                {
                    list_rows = conn.Query<RowsView>(rowSQL, new { tableName = TableName });
                    return list_rows;
                }
            }
            catch (Exception ex)
            {
                return list_rows;
            }
        }

        public IEnumerable<RowsView> GetRowsPaging(string dbName, string TableName, int pageSize, int page, string connectionStringName)
        {
            IEnumerable<RowsView> list_rows = new List<RowsView>();
            try
            {
                dbName = dbName.Replace('\'', ' ');
                string rowSQL = @"  use {0}
                                        SELECT TOP {1} * 
                                        FROM   (
                                        SELECT ROW_NUMBER() OVER (ORDER BY C.column_id) AS RowNumber,
                                        id=C.column_id,name=C.name,primaryKey=ISNULL(IDX.PrimaryKey,N''),
                                        rowType=T.name,lenght=C.max_length,isNull=C.is_nullable,defaultValue=ISNULL(D.definition,N''),
                                        note=ISNULL(PFD.[value],N''), type='row'
                                        FROM [{0}].sys.columns C with(nolock)
                                            INNER JOIN  [{0}].sys.objects O with(nolock)
                                                ON C.[object_id]=O.[object_id] AND O.type='U' AND O.is_ms_shipped=0
                                            INNER JOIN [{0}].sys.types T with(nolock)
                                                ON C.user_type_id=T.user_type_id 
                                            LEFT JOIN [{0}].sys.default_constraints D with(nolock)
                                                ON C.[object_id]=D.parent_object_id AND C.column_id=D.parent_column_id AND C.default_object_id=D.[object_id]
                                            LEFT JOIN [{0}].sys.extended_properties PFD with(nolock)
                                                ON PFD.class=1 AND C.[object_id]=PFD.major_id AND C.column_id=PFD.minor_id
                                            LEFT JOIN                       -- 索引及主键信息
                                            (  SELECT IDXC.[object_id],IDXC.column_id,PrimaryKey=IDX.is_primary_key
                                                FROM [{0}].sys.indexes IDX with(nolock)
		                                        INNER JOIN [{0}].sys.index_columns IDXC with(nolock)
			                                        ON IDX.[object_id]=IDXC.[object_id] AND IDX.index_id=IDXC.index_id
                                            ) IDX
                                                ON C.[object_id]=IDX.[object_id] AND C.column_id=IDX.column_id 
                                            WHERE O.name=@tableName 
	                                        ) A      
                                        WHERE RowNumber > {1}*({2}-1)";

                rowSQL = string.Format(rowSQL, dbName, pageSize, page);

                using (IDbConnection conn = ConnectionString.GetConnection(connectionStringName))
                {
                    list_rows = conn.Query<RowsView>(rowSQL, new {tableName=TableName });
                    return list_rows;
                }
            }
            catch (Exception ex)
            {
                return list_rows;
            }
        }

        public int GetRowsCount(string dbName, string TableName, string connectionStringName)
        {
            try
            {
                string rowSQL = @" use {0} 
                                         SELECT  count(*) as total FROM [{0}].sys.columns C with(nolock) 
                                         INNER JOIN [{0}].sys.objects O with(nolock) ON C.[object_id]=O.[object_id] AND O.type='U' AND O.is_ms_shipped=0 
                                        WHERE O.name=@tableName ";
                dbName = dbName.Replace('\'', ' ');
                rowSQL = string.Format(rowSQL, dbName);

                using (IDbConnection conn = ConnectionString.GetConnection(connectionStringName))
                {
                    int rowCount = conn.ExecuteScalar<int>(rowSQL, new { tableName=TableName });
                    return rowCount;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public string GetTableDescription(string dbName, string tableName, string connectionStringName)
        {
            string desc = "";
            try
            {
                string sql = @" use {0}
                                SELECT [value]
                            FROM sys.extended_properties a left JOIN  sysobjects b ON a.major_id=b.id
                            WHERE b.name=@tableName and minor_id=0;";
                dbName = dbName.Replace('\'', ' ');
                sql = string.Format(sql, dbName);

                using (IDbConnection conn = ConnectionString.GetConnection(connectionStringName))
                {
                    desc = conn.ExecuteScalar<string>(sql, new { tableName=tableName }); 
                }
            }
            catch (Exception ex)
            {
                desc = "";
            }
            return desc;
        }

        /// <summary>
        /// 编辑说明
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="TableName"></param>
        /// <param name="rowName"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public int EditDescription(string dbName, string TableName, string rowName, string Description, string connectionStringName)
        {
            try
            {
                string sql = @" if exists
                            (  select * FROM  [{0}].sys.extended_properties PFD with(nolock)
	                        left join  [{0}].sys.columns C  with(nolock)
	                        ON PFD.class=1 AND C.[object_id]=PFD.major_id AND C.column_id=PFD.minor_id        
	                        left JOIN  [{0}].sys.objects O with(nolock)
				                        ON C.[object_id]=O.[object_id] AND O.type='U' AND O.is_ms_shipped=0		 
	                        where C.name='{2}' and O.name='{1}' )
                        begin
	                        USE [{0}];
                            EXEC sp_updateextendedproperty N'MS_Description',   '{3}',   N'user',   N'dbo',   N'table',   N'{1}',   N'column',   N'{2}'
                        end
                        else
                        begin
	                        USE [{0}];
	                        EXECUTE   sp_addextendedproperty   N'MS_Description',   '{3}',   N'user',   N'dbo',   N'table',   N'{1}',   N'column',   N'{2}' 
                        end ";  //判断是否存在，不存在新增，存在修改
                dbName = dbName.Replace('\'', ' ');
                sql = string.Format(sql, dbName, TableName, rowName, Description);

                using (IDbConnection conn = ConnectionString.GetConnection(connectionStringName))
                {
                    int result = conn.Execute(sql);
                    return result;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int EditTableDescription(string dbName,string TableName,string Description,string connectionStringName)
        {
            string sql = @"   use [{0}];  
                                  if exists
                                (  
                                SELECT [Value]
                                FROM sys.extended_properties a left JOIN  sysobjects b ON a.major_id=b.id
                                WHERE b.name='{1}' and minor_id=0
                                 )
                                begin
                                  EXEC sp_updateextendedproperty 'MS_Description','{2}',N'user',N'dbo',N'table','{1}',NULL,NULL
                                end
                                else
                                begin  
                                  EXECUTE sp_addextendedproperty N'MS_Description', '{2}', N'user', N'dbo', N'table', N'{1}', NULL, NULL
                                 end ";
            try
            {
                dbName = dbName.Replace('\'', ' ');
                sql = string.Format(sql, dbName, TableName, Description);
                using (IDbConnection conn = ConnectionString.GetConnection(connectionStringName))
                {
                    int result = conn.Execute(sql);
                    return result;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public IEnumerable<ViewsView> GetViews(string dbName, string connectionStringName,string viewName="")
        {
            IEnumerable<ViewsView> list_views = new List<ViewsView>();

            try
            {
                string sql = @" use {0}
                               select name as Name,object_id as Id,create_date as CreateDate,modify_date as ModifyDate, databaseName='{0}',type='view',connName='{1}'
                               from [{0}].sys.objects t with(nolock)
                               where [type] = 'V' {2} order by name ";

                string viewSelect = "";
                if (!string.IsNullOrEmpty(viewName))
                {
                    viewSelect = string.Format(" and Name like '%'+@name+'%' ");
                }
                dbName = dbName.Replace('\'', ' ');
                sql = string.Format(sql, dbName, connectionStringName, viewSelect);
                using (IDbConnection conn = ConnectionString.GetConnection(connectionStringName))
                {
                    list_views = conn.Query<ViewsView>(sql, new { name = viewName });
                    return list_views;
                }
            }
            catch (Exception ex)
            {
                return list_views;
            }
        }

        /// <summary>
        /// 视图的SQL语句
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="viewName"></param>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        public string GetViewSQL(string dbName, string viewName, string connectionStringName)
        {
            string viewText = string.Empty;
            string sql = @" use {0}
                               select text from 
                               [{0}].[dbo].[syscomments] s1 with(nolock)
                               join [{0}].[dbo].[sysobjects] s2 with(nolock)
                               on s1.id=s2.id where name='{1}' ";
            try
            {
                dbName = dbName.Replace('\'', ' ');
                sql = string.Format(sql, dbName, viewName);
                using (IDbConnection conn = ConnectionString.GetConnection(connectionStringName))
                {
                    viewText = conn.ExecuteScalar<string>(sql);
                    return viewText;
                }
            }
            catch (Exception ex)
            {
                return viewText;
            }
        }
        
        /// <summary>
        /// 存储过程列表
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        public IEnumerable<ProcedureView> GetProcedure(string dbName, string connectionStringName,string procedureName="")
        {
            IEnumerable<ProcedureView> list_procedure = new List<ProcedureView>();

            try
            {
                string sql = @" use {0}
                               select name as Name,object_id as Id,create_date as CreateDate,modify_date as ModifyDate,databaseName='{0}',type='procedure',connName='{1}'
                               from [{0}].sys.objects t with(nolock)
                               where [type] = 'P' order by name ";

                string procedureSelect = "";
                if (!string.IsNullOrEmpty(procedureName))
                {
                    procedureSelect = string.Format(" and Name like '%'+@name+'%' ");
                }
                dbName = dbName.Replace('\'', ' ');
                sql = string.Format(sql, dbName, connectionStringName, procedureSelect);
                using (IDbConnection conn = ConnectionString.GetConnection(connectionStringName))
                {
                    list_procedure = conn.Query<ProcedureView>(sql,new {name=procedureName });
                    return list_procedure;
                }
            }
            catch (Exception ex)
            {
                return list_procedure;
            }
        }

        /// <summary>
        /// 存储过程的SQL语句
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="procedureName"></param>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        public string GetProcedureSQL(string dbName, string procedureName, string connectionStringName)
        {
            string procedureText = string.Empty;
            string sql = @" use {0} 
                            select b.[definition] 
                            from [{0}].[sys].[all_objects] a with(nolock),[{0}].[sys].[sql_modules] b with(nolock)
                            where a.is_ms_shipped=0 and a.object_id = b.object_id and a.[type] in ('P') 
                            and a.name='{1}' ";
            try
            {
                dbName = dbName.Replace('\'', ' ');
                sql = string.Format(sql, dbName, procedureName);
                using (IDbConnection conn = ConnectionString.GetConnection(connectionStringName))
                {
                    procedureText = conn.ExecuteScalar<string>(sql);
                    return procedureText;
                }
            }
            catch (Exception ex)
            {
                return procedureText;
            }
        }
        
        public IEnumerable<ProcedureParam> GetProcedureParameters(string dbName, string procedureName, string connectionStringName)
        {
            IEnumerable<ProcedureParam> list_procedure = new List<ProcedureParam>();

            string sql = @"  use {0}
                                select p.name,t.name as [type]
                                from
                                (
                                select name,xusertype from [{0}].sys.syscolumns  with(nolock)
                                    where ID in    
                                    (  
                                    SELECT id FROM [{0}].sys.sysobjects with(nolock)
                                    WHERE OBJECTPROPERTY(id, N'IsProcedure') = 1    
                                    and id = object_id(N'[dbo].[{1}]')   
                                    )
                                ) p  inner join [{0}].sys.types t with(nolock) on p.xusertype=T.user_type_id ";

            try {
                dbName = dbName.Replace('\'', ' ');
                sql = string.Format(sql, dbName, procedureName);
                using (IDbConnection conn = ConnectionString.GetConnection(connectionStringName))
                {
                    list_procedure = conn.Query<ProcedureParam>(sql);
                    return list_procedure;
                }
            }
            catch (Exception ex)
            {
                return list_procedure;
            } 
        }

        public IEnumerable<ColumnsView> GetColumns(string dbName, string tableName, string connectionStringName)
        {
            IEnumerable<ColumnsView> list_columns = new List<ColumnsView>();

            string sql = @"  use {0}
                                SELECT syscolumns.name,systypes.name as rowType,syscolumns.isnullable,syscolumns.length 
                               FROM syscolumns with(nolock), systypes with(nolock)
                               WHERE syscolumns.xusertype = systypes.xusertype 
                               AND syscolumns.id = object_id('{1}') ";

            try
            {
                dbName = dbName.Replace('\'', ' ');
                sql = string.Format(sql, dbName, tableName);
                using (IDbConnection conn = ConnectionString.GetConnection(connectionStringName))
                {
                    list_columns = conn.Query<ColumnsView>(sql);
                    return list_columns;
                }
            }
            catch (Exception ex)
            {
                return list_columns;
            }
        }

    }
}