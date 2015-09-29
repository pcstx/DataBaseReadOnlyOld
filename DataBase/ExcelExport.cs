using DataBase.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;

namespace DataBase
{
    public class ExcelExport
    {
        public static bool ExportExcelWithAspose(DataTable dt, string path)
        {
            bool succeed = false;
            if (dt != null)
            {
                try
                {
                    Aspose.Cells.License li = new Aspose.Cells.License();
                    var lic = Resources.License;
                    Stream s = new MemoryStream(lic);
                    li.SetLicense(s);

                    Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();
                    Aspose.Cells.Worksheet cellSheet = workbook.Worksheets[0];

                    cellSheet.Name = dt.TableName;

                    int rowIndex = 0;
                    int colIndex = 0;
                    int colCount = dt.Columns.Count;
                    int rowCount = dt.Rows.Count;

                    //列名的处理
                    for (int i = 0; i < colCount; i++)
                    {
                        cellSheet.Cells[rowIndex, colIndex].PutValue(dt.Columns[i].ColumnName);
                        cellSheet.Cells[rowIndex, colIndex].Style.Font.IsBold = true;
                        cellSheet.Cells[rowIndex, colIndex].Style.Font.Name = "宋体";
                        colIndex++;
                    }

                    Aspose.Cells.Style style = workbook.Styles[workbook.Styles.Add()];
                    style.Font.Name = "Arial";
                    style.Font.Size = 10;
                    Aspose.Cells.StyleFlag styleFlag = new Aspose.Cells.StyleFlag();
                    cellSheet.Cells.ApplyStyle(style, styleFlag);

                    rowIndex++;

                    for (int i = 0; i < rowCount; i++)
                    {
                        colIndex = 0;
                        for (int j = 0; j < colCount; j++)
                        {
                            cellSheet.Cells[rowIndex, colIndex].PutValue(dt.Rows[i][j].ToString());
                            colIndex++;
                        }
                        rowIndex++;
                    }
                    cellSheet.AutoFitColumns();

                    path = Path.GetFullPath(path);
                    workbook.Save(path);
                    succeed = true;
                }
                catch (Exception ex)
                {
                    succeed = false;
                }
            }

            return succeed;
        }

        public static bool ExportExcelWithAspose(DataSet ds, string path)
        {
            bool succeed = false;
            if (ds != null&&ds.Tables.Count>0)
            {
                try
                {
                    Aspose.Cells.License li = new Aspose.Cells.License();
                    var lic = Resources.License;
                    Stream s = new MemoryStream(lic);
                    li.SetLicense(s);

                    Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();

                    for (int tableCount = 0; tableCount < ds.Tables.Count; tableCount++)
                    {
                        workbook.Worksheets.Add(ds.Tables[tableCount].TableName);
                        Aspose.Cells.Worksheet cellSheet = workbook.Worksheets[tableCount];
                      //  cellSheet.Name = ds.Tables[tableCount].TableName;

                        int rowIndex = 0;
                        int colIndex = 0;
                        int colCount = ds.Tables[tableCount].Columns.Count;
                        int rowCount = ds.Tables[tableCount].Rows.Count;

                        //列名的处理
                        for (int i = 0; i < colCount; i++)
                        {
                            cellSheet.Cells[rowIndex, colIndex].PutValue(ds.Tables[tableCount].Columns[i].ColumnName);
                            cellSheet.Cells[rowIndex, colIndex].Style.Font.IsBold = true;
                            cellSheet.Cells[rowIndex, colIndex].Style.Font.Name = "宋体";
                            colIndex++;
                        }

                        Aspose.Cells.Style style = workbook.Styles[workbook.Styles.Add()];
                        style.Font.Name = "Arial";
                        style.Font.Size = 10;
                        Aspose.Cells.StyleFlag styleFlag = new Aspose.Cells.StyleFlag();
                        cellSheet.Cells.ApplyStyle(style, styleFlag);

                        rowIndex++;

                        for (int i = 0; i < rowCount; i++)
                        {
                            colIndex = 0;
                            for (int j = 0; j < colCount; j++)
                            {
                                cellSheet.Cells[rowIndex, colIndex].PutValue(ds.Tables[tableCount].Rows[i][j].ToString());
                                colIndex++;
                            }
                            rowIndex++;
                        }
                        cellSheet.AutoFitColumns();
                    } 
                    path = Path.GetFullPath(path);
                    workbook.Save(path);
                    succeed = true;
                }
                catch (Exception ex)
                {
                    succeed = false;
                }
            }

            return succeed;
        }

    }
}