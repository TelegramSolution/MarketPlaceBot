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
        public override Stream BuildReport()
        {
            throw new NotImplementedException();
        }
    }
}
