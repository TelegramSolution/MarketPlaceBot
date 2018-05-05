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
    public class ProductQuestionExport:ExcelExport
    {
        private List<ProductQuestion> List { get; set; }

        ExcelWorksheet ProductQuestionWorksheet { get; set; }
        public override Stream BuildReport()
        {
            List = BusinessLayer.ProductFunction.GetProductQuestionList();


            ProductQuestionWorksheet = Excel.Workbook.Worksheets.Add("Список Отзывов");


            // шапка
            ProductQuestionWorksheet.Cells[1, 1].Value = "Номер вопроса";
            ProductQuestionWorksheet.Cells[1, 2].Value = "Дата";
            ProductQuestionWorksheet.Cells[1, 3].Value = "Название товара";
            ProductQuestionWorksheet.Cells[1, 4].Value = "Пользователь";
            ProductQuestionWorksheet.Cells[1, 5].Value = "Текст вопроса";
            ProductQuestionWorksheet.Cells[1, 6].Value = "Дата ответа";
            ProductQuestionWorksheet.Cells[1, 7].Value = "Оператор";
            ProductQuestionWorksheet.Cells[1, 8].Value = "Тест ответа";
            

            int row = 2;

            foreach (var question in List)
            {
                ProductQuestionWorksheet.Cells[row, 1].Value = question.Id;

                ProductQuestionWorksheet.Cells[row, 2].Value = question.TimeStamp.ToString();

                ProductQuestionWorksheet.Cells[row, 3].Value = question.Product.Name;

                ProductQuestionWorksheet.Cells[row, 4].Value = question.Follower.ToString();

                ProductQuestionWorksheet.Cells[row, 5].Value = question.Text;

                if (question.Answer != null)
                {
                    ProductQuestionWorksheet.Cells[row, 6].Value = question.Answer.TimeStamp.ToString();

                    ProductQuestionWorksheet.Cells[row, 7].Value = question.Answer.Follower.ToString();

                    ProductQuestionWorksheet.Cells[row, 8].Value = question.Answer.Text;
                }
                row++;
            }

            return new MemoryStream(Excel.GetAsByteArray());
        }
    }
}
