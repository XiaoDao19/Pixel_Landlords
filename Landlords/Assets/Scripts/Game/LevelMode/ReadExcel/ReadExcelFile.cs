using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excel;
using System.Data;
using System.IO;

namespace PIXEL.Landlords.Game.LevelMode
{
    public static class ReadExcelFile
    {
        //将已经读取得Excel关卡表，作为一个LevelInformations类。
        public static LevelInformations ReadLevelExcel(string _filePath, int _levels)
        {
            //行数，列数
            int rowCounts = 0;
            int columnCounts = 0;

            //创建excel表的所有行的集合
            DataRowCollection rowCollection = ReadExcelFiles(_filePath, ref columnCounts, ref rowCounts);

            LevelInformations levelInformations = new LevelInformations();

            //读取每一行的每列信息

            //int 类型的参数的读取方式
            uint.TryParse(rowCollection[_levels][0].ToString(), out levelInformations.LevelId);

            //string类型参数的读取方式
            levelInformations.PlayerCards = rowCollection[_levels][1].ToString();
            levelInformations.AINo1Cards = rowCollection[_levels][2].ToString();
            levelInformations.AINo2Cards = rowCollection[_levels][3].ToString();

            return levelInformations;
        }

        //读取Excel文件
        private static DataRowCollection ReadExcelFiles(string _filePath, ref int _columnCounts, ref int _rowCounts)
        {
            //创建文件流，打开Excel文件
            FileStream fileStream = File.Open(_filePath, FileMode.Open, FileAccess.Read);

            //创建Excel文件读取器
            IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);

            //将读取得文件转换为Excel文件
            DataSet dataSet = excelDataReader.AsDataSet();

            //获取指定sheet表的行列的数量，以及信息
            _columnCounts = dataSet.Tables[0].Columns.Count;
            _rowCounts = dataSet.Tables[0].Rows.Count;

            fileStream.Close();
            fileStream.Dispose();

            //返回第一张表的所有行的信息
            return dataSet.Tables[0].Rows;
        }
    }
}