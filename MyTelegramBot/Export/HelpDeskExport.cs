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
    public class HelpDeskExport:ExcelExport
    {
        private List<HelpDesk> HelpDeskList { get; set; }

        private ExcelWorksheet HelpWorksheet { get; set; }
        public override Stream BuildReport()
        {
            HelpDeskList = HelpDeskFunction.GetAllHelpDeskList();

            HelpWorksheet = Excel.Workbook.Worksheets.Add("Заявки");

            // шапка
            HelpWorksheet.Cells[1, 1].Value = "Номер заявки";
            HelpWorksheet.Cells[1, 2].Value = "Дата";
            HelpWorksheet.Cells[1, 3].Value = "Пользователь";
            HelpWorksheet.Cells[1, 4].Value = "Описание проблемы";
            HelpWorksheet.Cells[1, 5].Value = "Комментарий оператора";
            HelpWorksheet.Cells[1, 6].Value = "Время";
            HelpWorksheet.Cells[1, 7].Value = "Оператор";

            int row = 2;

            foreach (var help in HelpDeskList)
            {

                foreach(var answer in help.HelpDeskAnswer)
                {
                    HelpWorksheet.Cells[row, 1].Value = help.Number;

                    HelpWorksheet.Cells[row, 2].Value = help.Timestamp.ToString();

                    HelpWorksheet.Cells[row, 3].Value = help.Follower.FirstName + " " + help.Follower.LastName;

                    HelpWorksheet.Cells[row, 4].Value = help.Text;

                    HelpWorksheet.Cells[row, 5].Value = answer.Text;

                    HelpWorksheet.Cells[row, 6].Value = answer.Timestamp.ToString();

                    HelpWorksheet.Cells[row, 7].Value = answer.Follower.FirstName + " " + answer.Follower.LastName;

                    row++;
                }
            }

            return new MemoryStream(Excel.GetAsByteArray());
        }
    }
}
