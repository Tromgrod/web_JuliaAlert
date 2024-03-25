// ------------------------------public --------------------------------------------------------------------------------------
// <copyright file="Converter.cs" company="GalexStudio">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the Converter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.Tools.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Weblib.Helpers
{
    public class ExcelExportHelper
    {
        public static string ExcelContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public static DataTable ListToDataTable(Dictionary<long, ItemBase> data, Dictionary<string, decimal> columSum, AdvancedProperties properties, bool includeSystemColumns)
        {
            DataTable dataTable = new DataTable();

            if (includeSystemColumns)
                dataTable.Columns.Add("Id", typeof(long));
            foreach (AdvancedProperty property in properties)
            {
                dataTable.Columns.Add(property.Common.PrintName, typeof(string));
            }
            if (includeSystemColumns)
            {
                dataTable.Columns.Add("Создан", typeof(string));
                dataTable.Columns.Add("Создатель", typeof(string));
            }
            if (columSum.Count > 0)
            {
                object[] values = new object[properties.Count + (includeSystemColumns ? 3 : 0)];
                var i = 0;

                foreach (AdvancedProperty property in properties)
                {
                    if (property.Common.TotalSum)
                    {
                        values[i] = Math.Round(columSum[property.Db.ParamName], property.Common.DecimalRound, MidpointRounding.AwayFromZero).ToString();
                    }
                    else if (i == 0)
                    {
                        values[i] = "Общая сумма";
                    }
                    i++;
                }

                dataTable.Rows.Add(values);
            }
            foreach (var pitem in data.Values)
            {
                object[] values = new object[properties.Count + (includeSystemColumns ? 3 : 0)];
                var i = 0;
                if (includeSystemColumns)
                {
                    values[i++] = pitem.Id;
                }
                foreach (AdvancedProperty property in properties)
                {
                    values[i++] = property.GetDataProcessor().ToString(property.PropertyDescriptor.GetValue(pitem), property, pitem, DisplayMode.Excell).Replace("&nbsp;", "").Trim().Replace("<br>", "\n");
                }
                if (includeSystemColumns)
                {
                    values[i++] = pitem.DateCreated.ToString();
                    values[i] = pitem.CreatedBy.Login;
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static DataTable ListToDataTable(Dictionary<long, ItemBase> data, AdvancedProperties properties, bool includeSystemColumns)
        {
            DataTable dataTable = new DataTable();

            if (includeSystemColumns)
                dataTable.Columns.Add("Id", typeof(long));
            foreach (AdvancedProperty property in properties)
            {
                dataTable.Columns.Add(property.Common.PrintName, typeof(string));
            }
            if (includeSystemColumns)
            {
                dataTable.Columns.Add("Создан", typeof(string));
                dataTable.Columns.Add("Создатель", typeof(string));
            }
            foreach (var pitem in data.Values)
            {
                object[] values = new object[properties.Count + (includeSystemColumns ? 3 : 0)];
                var i = 0;
                if (includeSystemColumns)
                {
                    values[i++] = pitem.Id;
                }
                foreach (AdvancedProperty property in properties)
                {
                    values[i++] = property.GetDataProcessor().ToString(property.PropertyDescriptor.GetValue(pitem), property, pitem, DisplayMode.Excell).Replace("&nbsp;", "").Trim();
                }
                if (includeSystemColumns)
                {
                    values[i++] = pitem.DateCreated.ToString();
                    values[i] = pitem.CreatedBy.Login;
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static byte[] ExportExcel(DataTable dataTable, string heading = "", params string[] columnsToTake)
            => ExportExcel(dataTable, heading, false, columnsToTake);

        public static byte[] ExportExcel(DataTable dataTable, string heading = "", bool totalSum = false, params string[] columnsToTake)
        {
            byte[] result = null;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(string.Format("{0} Data", heading));
                int startRowFrom = 1;

                // add the content into the Excel file  
                workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);

                // autofit width of cells with small content  
                int columnIndex = 1;
                foreach (DataColumn column in dataTable.Columns)
                {
                    ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                    {
                        workSheet.Column(columnIndex).AutoFit();
                    }

                    columnIndex++;
                }

                // format header - bold, yellow on black  
                using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
                }

                // format cells - add borders  
                using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                {
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);

                    r.Style.WrapText = true;

                    r.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                }

                // export total sum
                if (totalSum)
                {
                    using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + 1, dataTable.Columns.Count])
                    {
                        r.Style.Font.Bold = true;
                        r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                    }
                }

                if (columnsToTake != null && columnsToTake.Length > 0)
                {
                    // removed ignored columns  
                    for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                    {
                        if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                            workSheet.DeleteColumn(i + 1);
                    }
                }

                result = package.GetAsByteArray();
            }

            return result;
        }

        public static byte[] ExportExcel(Dictionary<long, ItemBase> data, Dictionary<string, decimal> columSum, AdvancedProperties properties, string Heading = "", bool includeSystemColumns = false, params string[] ColumnsToTake)
        {
            if (columSum != null && columSum.Count > 0)
                return ExportExcel(ListToDataTable(data, columSum, properties, includeSystemColumns), Heading, true, ColumnsToTake);
            else
                return ExportExcel(ListToDataTable(data, properties, includeSystemColumns), Heading, ColumnsToTake);
        }

        public static byte[] ExportExcel(Dictionary<long, ItemBase> data, AdvancedProperties properties, string Heading = "", bool includeSystemColumns = false, params string[] ColumnsToTake)
        {
            return ExportExcel(ListToDataTable(data, properties, includeSystemColumns), Heading, ColumnsToTake);
        }
    }
}