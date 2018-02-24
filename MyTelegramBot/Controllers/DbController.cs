using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json;
namespace MyTelegramBot.Controllers
{
    [Produces("application/json")]
    public class DbController : Controller
    {
        public IActionResult Index()
        {
            //"Server=localhost;Database=MarketBotDb;Integrated Security = FALSE;USER ID=bot;PASSWORD=bot;Trusted_Connection = True;"

            string sql = ReadFile("sql.json");

            if (sql==null || sql!=null && sql == "")
            {
                Model.SettingsDbConnection settingsDb = new Model.SettingsDbConnection
                {
                    DbName = "MarketBotDb",
                    Host = "localhost",
                    UserName = "",
                    Password = ""
                };

                return View(settingsDb);
            }

            else
            {
                var dbconnection = JsonConvert.DeserializeObject<Model.SettingsDbConnection>(sql);
                return View(dbconnection);
            }

            
        }

        [HttpPost]
       
        public IActionResult Update([FromBody] Model.SettingsDbConnection settings)
        {
            if (settings != null && settings.DbName!=null && settings.Host!=null)
            {
                //открываем файл sql.json и сохраняем изменени

                WriteFile("sql.json", JsonConvert.SerializeObject(settings));

                //"Server=localhost;Database=MarketBotDb;Integrated Security = FALSE;USER ID=bot;PASSWORD=bot;Trusted_Connection = True;"

                string SqlConnection = "Server=" + settings.Host+ ";Database=" + settings.DbName + ";Integrated Security = FALSE;Trusted_Connection = True;";

                WriteFile("connection.json", SqlConnection);


                if (TestConnection())
                  return  Json("Успех!");

                else
                    return Json("Ошибка подключения");
            }

            else
                return Json("Ошибка ввода данных");
        }

        private bool TestConnection()
        {
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var list= db.Units.ToList();

                    return true;

                }
            }

            catch
            {
                return false;
            }
        }


        private string ReadFile (string path)
        {
            try
            {
                using(StreamReader sr =new StreamReader(path))
                {
                    return sr.ReadToEnd();
                }
            }

            catch
            {
                return null;
            }
        }

        private void WriteFile(string path, string value)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write(value);
                    sw.Flush();
                    sw.Dispose();
                }
            }
            catch
            {

            }
        }
    }


    
}