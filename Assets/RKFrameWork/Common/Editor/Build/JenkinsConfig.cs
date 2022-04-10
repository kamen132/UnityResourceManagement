//using NPOI.HSSF.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JenkinsConfig
{
    private const string BaseWorkPath = "WorkAssets/";
    public static IDictionary<string, List<string>> NeedPackDirectorys = new Dictionary<string, List<string>>()
    {
        {
            "*.prefab",new List<string>()
            {
                BaseWorkPath + "UIPrefab",
                BaseWorkPath + "Effect/Prefabs",
            }
        },
        {
            "*.png",new List<string>()
            {
                BaseWorkPath + "textures_dynamic",//动态图集，每个图片会单独打成一个ab，rgba32压缩格式
                BaseWorkPath + "textures_rgba32",//不想被压缩（图集以及textures_separate就不支持了,为什么不能放到动态图集：因为动态图集单个图为一个ab，这个文件夹的png，可以随着prefab一起打到一个ab。所以，大图小图都可能在这个文件夹下

                 //BaseWorkPath + "textures_separate",//大图，不方便放到图集中，比如bg，panelbg。但是他比图集压缩损失更小
                 //BaseWorkPath + "textures_rgba32_dazzlingPin",//暂时先使用，当改变下载方式的时候，要去掉
                 //BaseWorkPath + "textures_download",//暂时先使用，当改变下载方式的时候，要去掉
            }
        },
        {
            "*.mp3",new List<string>()
            {
                BaseWorkPath + "sound",
                BaseWorkPath + "sound_bg",
            }
        } ,
        {
            "*.mat",new List<string>()
            {
                BaseWorkPath + "Materials",
            }
        } ,
    };

    public static void ExportExcle(string fileName, string sheetName, List<string> titleList, List<List<string>> data)
    {
        //var workbook = new HSSFWorkbook();
        //var sheet = workbook.CreateSheet(sheetName);

        //var typeRow = sheet.CreateRow(0);
        //for (int i = 0; i < titleList.Count; i++)
        //{
        //    var typeCell = typeRow.CreateCell(i);
        //    typeCell.SetCellValue(titleList[i]);
        //}

        //for(int i = 0;i < data.Count;i++)
        //{
        //    var row = sheet.CreateRow( i + 1);
        //    var itemData = data[i];
        //    for (int j = 0; j < itemData.Count; j++)
        //    {
        //        var cell = row.CreateCell(j);
        //        cell.SetCellValue(itemData[j]);
        //    }
        //}

        //string excelOutputPath = Application.dataPath + "/../" + fileName + ".xls";
        //if (File.Exists(excelOutputPath))
        //{
        //    File.Delete(excelOutputPath);
        //}

        //using (var fs = File.Create(excelOutputPath))
        //{
        //    workbook.Write(fs);
        //}
    }
}
