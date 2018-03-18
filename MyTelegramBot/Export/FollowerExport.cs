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
    public class FollowerExport : ExcelExport
    {
        ExcelWorksheet FollowerWorksheet { get; set; }

        List<Follower> FollowerList { get; set; }

        public override Stream BuildReport()
        {
            FollowerWorksheet = Excel.Workbook.Worksheets.Add("Список Пользователей");

            FollowerList = FollowerFunction.GetFollowerList();

            // шапка
            FollowerWorksheet.Cells[1, 1].Value = "Имя";
            FollowerWorksheet.Cells[1, 2].Value = "Фамилия";
            FollowerWorksheet.Cells[1, 3].Value = "UserName";
            FollowerWorksheet.Cells[1, 4].Value = "Телефон";
            FollowerWorksheet.Cells[1, 5].Value = "Дата регистрации";            
            FollowerWorksheet.Cells[1, 6].Value = "Заблокирован";

            int row = 2;

            foreach (var follower in FollowerList)
            {
                //имя
                FollowerWorksheet.Cells[row, 1].Value = follower.FirstName;

                //фамилия
                FollowerWorksheet.Cells[row, 2].Value = follower.LastName;


                //username
                FollowerWorksheet.Cells[row, 3].Value = follower.UserName;

                //телефон 
                FollowerWorksheet.Cells[row, 4].Value = follower.Telephone;


                FollowerWorksheet.Cells[row, 5].Value = follower.DateAdd.ToString();

                if (follower.Blocked)
                    FollowerWorksheet.Cells[row, 6].Value = "Да";

                else
                    FollowerWorksheet.Cells[row, 6].Value = "Нет";


                row++;

            }

            return new MemoryStream(Excel.GetAsByteArray());
        }
    }
}
