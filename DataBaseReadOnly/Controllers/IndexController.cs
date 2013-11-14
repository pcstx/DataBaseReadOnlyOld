using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc; 
using System.Data.SqlClient;
using DBHelper;
using System.Data;

namespace DataBaseReadOnly.Controllers
{
    public class user
    {
        public int ID { get; set; }
        public string UserName { get; set; }
    }

    public class database
    { 
        public int dbid{get;set;}
        public string name{get;set;}
        public DateTime crdate{get;set;} 
    }

    public class IndexController : Controller
    {
        //
        // GET: /Index/
        Models.masterEntities master = new Models.masterEntities();

        public ActionResult Index()
        {
            #region SQL
            string sql = "select dbid,name,crdate from master..sysdatabases where dbid > 4 --用户库";

            string tableSQL = " Select TABLE_NAME FROM website.INFORMATION_SCHEMA.TABLES Where TABLE_TYPE='BASE TABLE'";

            string rowSQL = @"SELECT 
    TableName=CASE WHEN C.column_id=1 THEN O.name ELSE N'' END,
    TableDesc=ISNULL(CASE WHEN C.column_id=1 THEN PTB.[value] END,N''),
    Column_id=C.column_id,
    ColumnName=C.name,
    PrimaryKey=ISNULL(IDX.PrimaryKey,N''),
    [IDENTITY]=CASE WHEN C.is_identity=1 THEN N'√'ELSE N'' END,
    Computed=CASE WHEN C.is_computed=1 THEN N'√'ELSE N'' END,
    Type=T.name,
    Length=C.max_length,
    Precision=C.precision,
    Scale=C.scale,
    NullAble=CASE WHEN C.is_nullable=1 THEN N'√'ELSE N'' END,
    [Default]=ISNULL(D.definition,N''),
    ColumnDesc=ISNULL(PFD.[value],N''),
    IndexName=ISNULL(IDX.IndexName,N''),
    IndexSort=ISNULL(IDX.Sort,N''),
    Create_Date=O.Create_Date,
    Modify_Date=O.Modify_date
FROM sys.columns C
    INNER JOIN sys.objects O
        ON C.[object_id]=O.[object_id]
            AND O.type='U'
            AND O.is_ms_shipped=0
    INNER JOIN sys.types T
        ON C.user_type_id=T.user_type_id
    LEFT JOIN sys.default_constraints D
        ON C.[object_id]=D.parent_object_id
            AND C.column_id=D.parent_column_id
            AND C.default_object_id=D.[object_id]
    LEFT JOIN sys.extended_properties PFD
        ON PFD.class=1 
            AND C.[object_id]=PFD.major_id 
            AND C.column_id=PFD.minor_id
--             AND PFD.name='Caption'  -- 字段说明对应的描述名称(一个字段可以添加多个不同name的描述)
    LEFT JOIN sys.extended_properties PTB
        ON PTB.class=1 
            AND PTB.minor_id=0 
            AND C.[object_id]=PTB.major_id
--             AND PFD.name='Caption'  -- 表说明对应的描述名称(一个表可以添加多个不同name的描述) 
    LEFT JOIN                       -- 索引及主键信息
    (
        SELECT 
            IDXC.[object_id],
            IDXC.column_id,
            Sort=CASE INDEXKEY_PROPERTY(IDXC.[object_id],IDXC.index_id,IDXC.index_column_id,'IsDescending')
                WHEN 1 THEN 'DESC' WHEN 0 THEN 'ASC' ELSE '' END,
            PrimaryKey=CASE WHEN IDX.is_primary_key=1 THEN N'√'ELSE N'' END,
            IndexName=IDX.Name
        FROM sys.indexes IDX
        INNER JOIN sys.index_columns IDXC
            ON IDX.[object_id]=IDXC.[object_id]
                AND IDX.index_id=IDXC.index_id
        LEFT JOIN sys.key_constraints KC
            ON IDX.[object_id]=KC.[parent_object_id]
                AND IDX.index_id=KC.unique_index_id
        INNER JOIN  -- 对于一个列包含多个索引的情况,只显示第1个索引信息
        (
            SELECT [object_id], Column_id, index_id=MIN(index_id)
            FROM sys.index_columns
            GROUP BY [object_id], Column_id
        ) IDXCUQ
            ON IDXC.[object_id]=IDXCUQ.[object_id]
                AND IDXC.Column_id=IDXCUQ.Column_id
                AND IDXC.index_id=IDXCUQ.index_id
    ) IDX
        ON C.[object_id]=IDX.[object_id]
            AND C.column_id=IDX.column_id 
-- WHERE O.name=N'要查询的表'       -- 如果只查询指定表,加上此条件
ORDER BY O.name,C.column_id 
";

            string indexSQL = @"SELECT 
    TableId=O.[object_id],
    TableName=O.Name,
    IndexId=ISNULL(KC.[object_id],IDX.index_id),
    IndexName=IDX.Name,
    IndexType=ISNULL(KC.type_desc,'Index'),
    Index_Column_id=IDXC.index_column_id,
    ColumnID=C.Column_id,
    ColumnName=C.Name,
    Sort=CASE INDEXKEY_PROPERTY(IDXC.[object_id],IDXC.index_id,IDXC.index_column_id,'IsDescending')
        WHEN 1 THEN 'DESC' WHEN 0 THEN 'ASC' ELSE '' END,
    PrimaryKey=CASE WHEN IDX.is_primary_key=1 THEN N'√'ELSE N'' END,
    [UQIQUE]=CASE WHEN IDX.is_unique=1 THEN N'√'ELSE N'' END,
    Ignore_dup_key=CASE WHEN IDX.ignore_dup_key=1 THEN N'√'ELSE N'' END,
    Disabled=CASE WHEN IDX.is_disabled=1 THEN N'√'ELSE N'' END,
    Fill_factor=IDX.fill_factor,
    Padded=CASE WHEN IDX.is_padded=1 THEN N'√'ELSE N'' END
FROM sys.indexes IDX
    INNER JOIN sys.index_columns IDXC
        ON IDX.[object_id]=IDXC.[object_id]
            AND IDX.index_id=IDXC.index_id
    LEFT JOIN sys.key_constraints KC
        ON IDX.[object_id]=KC.[parent_object_id]
            AND IDX.index_id=KC.unique_index_id
    INNER JOIN sys.objects O
        ON O.[object_id]=IDX.[object_id]
    INNER JOIN sys.columns C
        ON O.[object_id]=C.[object_id]
            AND O.type='U'
            AND O.is_ms_shipped=0
            AND IDXC.Column_id=C.Column_id
--    INNER JOIN  -- 对于一个列包含多个索引的情况,只显示第1个索引信息
--    (
--        SELECT [object_id], Column_id, index_id=MIN(index_id)
--        FROM sys.index_columns
--        GROUP BY [object_id], Column_id
--    ) IDXCUQ
--        ON IDXC.[object_id]=IDXCUQ.[object_id]
--            AND IDXC.Column_id=IDXCUQ.Column_id
";
            #endregion

            SqlHelper.connectionString = ConnectionString.connectionString("SqlServerHelper");
  
             IEnumerable<database> IUser= SqlHelper.ExecuteIEnumerable<database>(sql);
            DataTable dt= SqlHelper.ExecuteDataTable(tableSQL);
             
            return View(IUser);
        }


        public JsonResult DataBaseJson()
        {
            var dbObject = master.showDataBase().ToList();

            return Json(dbObject, "text/html", JsonRequestBehavior.AllowGet);
        }

    }
}
