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
    public class ProductExport:ExcelExport
    {
        List<Product> ProductList { get; set; }

        ExcelWorksheet ProductWorksheet { get; set; }

        public override Stream BuildReport()
        {
            ProductWorksheet = Excel.Workbook.Worksheets.Add("Список товаров");

            ProductList = ProductFunction.GetAllProductList();
           
            // шапка
            ProductWorksheet.Cells[1, 1].Value = "Id";
            ProductWorksheet.Cells[1, 2].Value = "Название";
            ProductWorksheet.Cells[1, 3].Value = "Категория";
            ProductWorksheet.Cells[1, 4].Value = "Описание";
            ProductWorksheet.Cells[1, 5].Value = "Дата добавления";
            ProductWorksheet.Cells[1, 6].Value = "Артикул";
            ProductWorksheet.Cells[1, 7].Value = "Стоимость";
            ProductWorksheet.Cells[1, 8].Value = "Ссылка на подробное описание";
            ProductWorksheet.Cells[1, 9].Value = "Статус";
            ProductWorksheet.Cells[1, 10].Value = "Ед. измерения";

            int row = 2;

            foreach(var product in ProductList)
            {
                ProductWorksheet.Cells[row, 1].Value = product.Id;

                ProductWorksheet.Cells[row, 2].Value = product.Name;

                ProductWorksheet.Cells[row, 3].Value = product.Category.Name;

                ProductWorksheet.Cells[row, 4].Value = product.Text;

                ProductWorksheet.Cells[row, 5].Value = product.DateAdd.ToString();

                ProductWorksheet.Cells[row, 6].Value = product.Code;

                ProductWorksheet.Cells[row, 7].Value = product.CurrentPrice.Value;

                ProductWorksheet.Cells[row, 8].Value = product.TelegraphUrl;

                if (product.Enable)
                    ProductWorksheet.Cells[row, 9].Value = "Активно";

                else
                    ProductWorksheet.Cells[row, 9].Value = "Скрыто от пользователей";

                ProductWorksheet.Cells[row, 10].Value = product.Unit.Name;

                row++;
            }

            return new MemoryStream(Excel.GetAsByteArray());
        }
    }
}
