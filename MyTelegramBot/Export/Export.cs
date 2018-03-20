using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using MyTelegramBot.BusinessLayer;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace MyTelegramBot.Export
{
    public abstract class ExcelExport
    {
        protected ExcelPackage Excel { get; set; }

        public abstract  Stream BuildReport();

        public ExcelExport()
        {
            Excel = new ExcelPackage();

            //Excel.Workbook.Properties.Author = "https://github.com/TelegramSolution/MarketPlaceBot";
            //Excel.Workbook.Properties.Title = "https://github.com/TelegramSolution/MarketPlaceBot";
            //Excel.Workbook.Properties.Company = "https://github.com/TelegramSolution/MarketPlaceBot";
        }
    }
}
