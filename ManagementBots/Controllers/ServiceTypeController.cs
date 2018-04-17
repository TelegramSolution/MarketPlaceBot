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

    public class ServiceTypeController : Controller
    {

        BotMngmntDbContext Context { get; set; }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                Context = new BotMngmntDbContext();

                return View(Context.ServiceType.ToList());
            }

            catch (Exception e)
            {
               return Json(e.Message);
            }

            finally
            {
                Context.Dispose();
            }
            
        }

        [HttpPost]
        public IActionResult Save([FromBody] ServiceType serviceType)
        {
            try
            {
                if (serviceType != null && serviceType.Price < 0)
                    return Json("Ошибка. Стоимость должна быль больше нуля");

                if(serviceType!=null && serviceType.Name=="")
                    return Json("Ошибка. Укажите название");


                Context = new BotMngmntDbContext();

                if (serviceType != null && serviceType.Id == 0 && Insert(serviceType).Id > 0)
                    return Json("Добавлено");

                if (serviceType != null && serviceType.Id > 0)
                {
                    Update(serviceType);
                    return Json("Сохранено");
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
                if(Context!=null)
                    Context.Dispose();
            }
        }

        [HttpGet]
        public IActionResult Delete(int Id)
        {
            try
            {
                Context = new BotMngmntDbContext();

                var service_type = Context.ServiceType.Find(Id);

                Context.Remove<ServiceType>(service_type);

                Context.SaveChanges();

                return Json("Удалено");
            }

            catch (Exception e)
            {
                return Json(e.Message);
            }

            finally
            {
                Context.Dispose();
            }
        }

        private ServiceType Insert(ServiceType serviceType)
        {
            Context.ServiceType.Add(serviceType);

            Context.SaveChanges();

            return serviceType;
        }

        private ServiceType Update(ServiceType _serviceType)
        {
            //var serviceType = Context.ServiceType.Find(_serviceType.Id);

            Context.Update<ServiceType>(_serviceType);

            Context.SaveChanges();

            return _serviceType;
        }
    }

}