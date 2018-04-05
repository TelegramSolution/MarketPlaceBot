using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyTelegramBot.BusinessLayer;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace MyTelegramBot.Controllers
{
    [Produces("application/json")]

    public class HostingVersionController : Controller
    {

        TelegramBotClient BotClient { get; set; }

        MarketBotDbContext DbContext { get; set; }

        private string Result { get; set; }

        private Model.HostInfo HostInfo { get; set; }

        public IActionResult Install(string token, string BotName ,bool IsDemo=false)
        {
            string dbname = BotName + "Db";

            HostInfo = new Model.HostInfo();

            if (CreateDb(dbname))
            {
                string read = ReadFile("HostInfo.json");

                if (read != null)
                    HostInfo = JsonConvert.DeserializeObject<Model.HostInfo>(read);

                HostInfo.CreateTimeStamp = DateTime.Now;
                HostInfo.BotName = BotName;
                HostInfo.IsDemo = IsDemo;
                HostInfo.IsFree = false;
                HostInfo.Token = token;
                HostInfo.DbConnectionString = String.Format("Server=localhost;Database={0};Integrated Security = FALSE;Trusted_Connection = True;", dbname);
                HostInfo.DbName = dbname;

                WriteFile("connection.json", HostInfo.DbConnectionString);

                WriteFile("HostInfo.json", JsonConvert.SerializeObject(HostInfo));

                Dictionary<string, string> dictionary = new Dictionary<string, string>
                {
                    { "name", BotName }
                };

                WriteFile("name.json", JsonConvert.SerializeObject(dictionary));

                return Json(HostInfo);

            }

            else
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>
                {
                    { "Ok", "false" },
                    { "Result", Result }
                };

                return Json(dictionary);
            }
        }

        public IActionResult Unistall()
        {
            try
            {
                string read = ReadFile("HostInfo.json");
                Model.HostInfo hostInfo = JsonConvert.DeserializeObject<Model.HostInfo>(read);
                hostInfo.BotName = null;
                hostInfo.DbConnectionString = null;
                hostInfo.Token = null;
                hostInfo.IsFree = true;
                hostInfo.DbName = null;
                hostInfo.Blocked = false;

                WriteFile("HostInfo.json", JsonConvert.SerializeObject(hostInfo));

                return Ok();
            }

            catch
            {
                return NotFound();
            }
        }

        public IActionResult UpdDbConnectionString(string ConnectionString)
        {
            return Ok();
        }

        [HttpGet]
        public IActionResult GetInfo()
        {
            Model.HostInfo hostInfo = JsonConvert.DeserializeObject<Model.HostInfo>(ReadFile("HostInfo.json"));

            return Json(hostInfo);
        }

        private string ReadFile(string Path)
        {
            try
            {
                StreamReader reader = new StreamReader(Path);

                string Result = reader.ReadToEnd();

                reader.Dispose();

                return Result;
            }

            catch
            {
                return null;
            }
        }

        private bool WriteFile(string Path, string Text)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter(Path);

                streamWriter.Write(Text);

                streamWriter.Flush();

                streamWriter.Dispose();

                return true;
            }

            catch
            {
                return false;
            }
        }

        private bool CreateDb(string DbName)
        {
            DbContext = new MarketBotDbContext();

            try
            {
                string CreateSqlQuery = String.Format("CREATE DATABASE [{0}]  CONTAINMENT = NONE " +
                    " ON  PRIMARY (NAME =N'{0}', FILENAME = N'" + @"C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\{0}.mdf' , SIZE = 4288KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )," +
                    "  FILEGROUP [BotDbFs] CONTAINS FILESTREAM  DEFAULT" +
                    " ( NAME = N'{0}_fs', FILENAME = N'" + @"C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\{0}_fs' , MAXSIZE = UNLIMITED)  LOG ON " +
                    " ( NAME = N'{0}_log', FILENAME = N'" + @"C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\{0}_log.ldf' , SIZE = 1072KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)", DbName);

                DbContext.Database.ExecuteSqlCommand(new RawSqlString(CreateSqlQuery));

                DbContext.Database.ExecuteSqlCommand(new RawSqlString("USE " + DbName + " " + ReadFile("SQL\\alter.sql")));

                DbContext.Database.ExecuteSqlCommand(new RawSqlString("USE " + DbName + " " + ReadFile("SQL\\insert.sql")));


                Dictionary<string, string> dictionary = new Dictionary<string, string>();

                
                Result= "Успешно созада база данных " + DbName;

                return true;


            }

            catch (Exception e)
            {
                Result = e.Message;
                return false;
            }

            finally
            {
                DbContext.Dispose();
            }
        }
    }
}