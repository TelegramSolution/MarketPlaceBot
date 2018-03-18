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
    public class StockHistoryExport:ExcelExport
    {
        List<Stock> StockList { get; set; }

        ExcelWorksheet StockHistoryWorksheet { get; set; }

        public override Stream BuildReport()
        {
            StockHistoryWorksheet = Excel.Workbook.Worksheets.Add("Список Пользователей");

            StockList = StockFunction.GetAllStockHistory();

            // шапка
            StockHistoryWorksheet.Cells[1, 1].Value = "Название товара";
            StockHistoryWorksheet.Cells[1, 2].Value = "Было";
            StockHistoryWorksheet.Cells[1, 3].Value = "Изменение";
            StockHistoryWorksheet.Cells[1, 4].Value = "Стало";
            StockHistoryWorksheet.Cells[1, 5].Value = "Дата";
            StockHistoryWorksheet.Cells[1, 6].Value = "Комментарий";

            int row = 2;

            var ProductGroup = StockList.GroupBy(s => s.ProductId).ToList();

            foreach (var group in ProductGroup)
            {
                group.OrderByDescending(x => x.Id);

                foreach (var stock in group)
                {
                    StockHistoryWorksheet.Cells[row, 1].Value = stock.Product.Name;

                    StockHistoryWorksheet.Cells[row, 2].Value = stock.Balance - stock.Quantity;

                    StockHistoryWorksheet.Cells[row, 3].Value = stock.Quantity;

                    StockHistoryWorksheet.Cells[row, 4].Value = stock.Balance;

                    StockHistoryWorksheet.Cells[row, 5].Value = stock.DateAdd.ToString();

                    StockHistoryWorksheet.Cells[row, 6].Value = stock.Text;

                    row++;
                }
            }

            //foreach(var stock in StockList)
            //{
            //    StockHistoryWorksheet.Cells[row, 1].Value = stock.Product.Name;

            //    StockHistoryWorksheet.Cells[row, 2].Value = stock.Balance + stock.Quantity;

            //    StockHistoryWorksheet.Cells[row, 3].Value = stock.Quantity;

            //    StockHistoryWorksheet.Cells[row, 4].Value = stock.Balance;

            //    StockHistoryWorksheet.Cells[row, 5].Value = stock.DateAdd.ToString();

            //    StockHistoryWorksheet.Cells[row, 6].Value = stock.Text;

            //    row++;

            //}

            return new MemoryStream(Excel.GetAsByteArray());
        }
    }
}
