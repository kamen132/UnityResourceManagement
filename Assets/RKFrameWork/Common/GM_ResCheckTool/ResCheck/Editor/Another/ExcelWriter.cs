//using NPOI.HSSF.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
//using NPOI.SS.UserModel;

public class ExcelWriter
{
    //public static void WriteListDataToExcel(List<List<string>> excelData, string fileName, string outputDir)
    //{
    //    if (excelData.Count > 1)
    //    {
    //        string excelPath = ExportExcelData(excelData, fileName, outputDir);
    //        System.Diagnostics.Process.Start(excelPath);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("写入Excel流无数据");
    //    }
    //}

    //private static string ExportExcelData(List<List<string>> data, string fileName, string outputDir)
    //{
    //    var workbook = new HSSFWorkbook();
    //    var modifyFilesSheet = workbook.CreateSheet("main");

    //    IRow currRow;
    //    int lineCount = 0;
    //    foreach (List<string> row in data)
    //    {
    //        currRow = modifyFilesSheet.CreateRow(lineCount++);//创建第i行
    //        int colCount = 0;
    //        foreach (string colStr in row)
    //        {
    //            modifyFilesSheet.AutoSizeColumn(colCount);
    //            var currCell = currRow.CreateCell(colCount++);
    //            currCell.SetCellValue(colStr);
    //        }

    //    }

    //    string s_ExportDir = outputDir;

    //    if (string.IsNullOrEmpty(fileName))
    //    {
    //        fileName = System.DateTime.Now.ToShortDateString();
    //    }

    //    string excelOutputPath = string.Format("{0}/{1}.xls", s_ExportDir, fileName);
    //    if (File.Exists(excelOutputPath))
    //    {
    //        File.Delete(excelOutputPath);
    //        AssetDatabase.Refresh();
    //    }

    //    using (var fs = File.Create(excelOutputPath))
    //    {
    //        workbook.Write(fs);
    //    }

    //    AssetDatabase.Refresh();
    //    UnityEngine.Debug.LogFormat("Excel导出完成 : {0}", excelOutputPath);
    //    return excelOutputPath;
    //}
}
