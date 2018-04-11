using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ManagementBots.Db;
using Microsoft.EntityFrameworkCore;
using ManagementBots.BusinessLayer;


namespace ManagementBots.Controllers
{
    public class DNSController : Controller
    {
        BotMngmntDbContext DbContext { get; set; }

        [HttpGet]
        public IActionResult Index()
        {
            DbContext = new BotMngmntDbContext();

            var list = DbContext.Dns.ToList();

            DbContext.Dispose();

            return View(list);
        }

        [HttpPost]
        public IActionResult Post ([FromBody] Dns dns)
        {
            try
            {
                DbContext = new BotMngmntDbContext();

                if (dns != null && dns.Name != null && dns.Name != "" && dns.Ip != "" && dns.Id > 0)
                {
                    UpdateDns(dns);
                    return Json("Сохранено");
                }

                var reapet = DbContext.Dns.Where(d => d.Name == dns.Name).FirstOrDefault();

                if (dns.Id == 0 && reapet != null)
                    return Json("Домен с таким именем уже существует");

                if (dns != null && dns.Name != null && dns.Name != "" && dns.Ip != "" && dns.Id == 0)
                {
                    dns.SslPath = BusinessLayer.SSL.GenerateSSL(GeneralFunction.SslNetworkFolder(), dns.Name);
                    InsertDns(dns);
                    return Json("Добавлено");
                }

                else
                    return Json("Ошибка");
            }

            catch (Exception e)
            {
                return Json(e.Message);
            }

            finally
            {
                DbContext.Dispose();
            }
        }


        [HttpGet]
        public IActionResult History (int Id)
        {
            DbContext = new BotMngmntDbContext();

            try
            {
                var Historys = DbContext.DnsHistory.Where(h => h.DnsId == Id).Include(h=>h.Bot).ToList();

                List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

                foreach(var history in Historys)
                {
                    Dictionary<string, string> row = new Dictionary<string, string>();

                    row.Add("Id", history.Id.ToString());
                    row.Add("TimeStamp", history.TimeStamp.ToString());
                    row.Add("BotId", history.BotId.ToString());
                    row.Add("BotName", history.Bot.BotName.ToString());

                    result.Add(row);
                }

                return Json(result);
            }

            catch
            {
                return NotFound();
            }

            finally
            {
                DbContext.Dispose();
            }
        }

        [HttpGet]
        public IActionResult Delete (int Id)
        {
            DbContext = new BotMngmntDbContext();

            try
            {
                var dns = DbContext.Dns.Find(Id);

                DbContext.Remove<Dns>(dns);

                DbContext.SaveChanges();

                return Json("Удалено");
            }

            catch (Exception e)
            {
                return Json(e.Message);
            }

            finally
            {
                DbContext.Dispose();
            }
        }

        [HttpGet]
        public IActionResult NewSsl(int DomainId)
        {
            try
            {
                return Json(SSL.GenerateSSL("C:\\MYcert", "ya.ru"));
            }

            catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        private Dns InsertDns(Dns dns)
        {
            dns.TimeStamp = DateTime.Now;

            
            DbContext.Add(dns);

            DbContext.SaveChanges();

            return dns;
        }


        private Dns UpdateDns(Dns dns)
        {
            DbContext.Update<Dns>(dns);

            DbContext.SaveChanges();

            return dns;
        }
    }
}
