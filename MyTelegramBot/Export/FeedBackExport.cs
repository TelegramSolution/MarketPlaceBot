using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using MyTelegramBot.BusinessLayer;
using OfficeOpenXml;
using OfficeOpenXml.Style;
namespace MyTelegramBot.Export
{
    public class FeedBackExport : ExcelExport
    {
        List<FeedBack> FeedBackList { get; set; }

        ExcelWorksheet FeedBackWorksheet { get; set; }

        public override Stream BuildReport()
        {
            FeedBackWorksheet = Excel.Workbook.Worksheets.Add("Список Отзывов");

            FeedBackList = FeedbackFunction.GetAllFeedBack();

            // шапка
            FeedBackWorksheet.Cells[1, 1].Value = "Номер отзыва";
            FeedBackWorksheet.Cells[1, 2].Value = "Дата";
            FeedBackWorksheet.Cells[1, 3].Value = "Номер заказа";
            FeedBackWorksheet.Cells[1, 4].Value = "Название товара";
            FeedBackWorksheet.Cells[1, 5].Value = "Оценка";
            FeedBackWorksheet.Cells[1, 6].Value = "Комментарий";

            int row = 2;

            foreach(var feedback in FeedBackList)
            {
                FeedBackWorksheet.Cells[row, 1].Value = feedback.Id;

                FeedBackWorksheet.Cells[row, 2].Value = feedback.DateAdd.ToString();

                FeedBackWorksheet.Cells[row, 3].Value = feedback.Order.Number;

                FeedBackWorksheet.Cells[row, 4].Value = feedback.Product.Name;

                FeedBackWorksheet.Cells[row, 5].Value = feedback.RaitingValue;

                FeedBackWorksheet.Cells[row, 6].Value = feedback.Text;

                row++;
            }

            return new MemoryStream(Excel.GetAsByteArray());
        }
    }
}
