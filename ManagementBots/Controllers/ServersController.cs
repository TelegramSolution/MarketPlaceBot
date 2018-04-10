using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ManagementBots.Db;

namespace ManagementBots.Controllers
{
    [Produces("application/json")]
    [Route("Servers")]
    public class ServersController : Controller
    {
        // GET: api/Servers
        [HttpGet]
        public IActionResult Index()
        {
            BotMngmntDbContext botMngmntDb = new BotMngmntDbContext();

            var list = botMngmntDb.Server.ToList();

            botMngmntDb.Dispose();

            return View("Index2",list);
        }

        // GET: api/Servers/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/Servers
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/Servers/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
