using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyTelegramBot.Services;
namespace MyTelegramBot.Controllers
{
    [Produces("application/json")]
    public class PayConfigController : Controller
    {
        PaymentTypeConfig PaymentTypeConfig;

        PaymentTypeEnum PaymentTypeEnum;

        MarketBotDbContext db;

        public IActionResult Qiwi()
        {
            db = new MarketBotDbContext();

            PaymentTypeEnum = PaymentTypeEnum.Qiwi;

            List<PaymentTypeConfig> list = new List<PaymentTypeConfig>();

            list = db.PaymentTypeConfig.Where(p => p.PaymentId == PaymentType.GetTypeId(PaymentTypeEnum)).OrderByDescending(p => p.Id).ToList();

            PaymentType paymentType = db.PaymentType.Find(PaymentType.GetTypeId(PaymentTypeEnum));

            Tuple<List<PaymentTypeConfig>, PaymentType> model = new Tuple<List<PaymentTypeConfig>, PaymentType>(list, paymentType);

            return View("Qiwi", model);
        }

        public IActionResult Litecoin()
        {
            db = new MarketBotDbContext();

            PaymentTypeEnum =  PaymentTypeEnum.Litecoin;


            PaymentTypeConfig = db.PaymentTypeConfig.Where(p => p.PaymentId == PaymentType.GetTypeId(PaymentTypeEnum)).OrderByDescending(p => p.Id).FirstOrDefault();

            if (PaymentTypeConfig == null)
            {
                PaymentTypeConfig = new PaymentTypeConfig
                {
                    Host = "127.0.0.1",
                    Login = "root",
                    Pass = "toor",
                    Port = "9332",
                    Enable = true,
                    PaymentId = PaymentType.GetTypeId(PaymentTypeEnum)
                };

            }

            ViewBag.Title = "Litecoin";
            ViewBag.Text = "В папке с установленными Litecoin Core создайте бат файл.Сохраните и запустите этот бат файл и дождитесь синхронизации базы данных (Размер базы данных более 10гб)." +
                "Содержимое бат файла:";
            ViewBag.Bat = "litecoin-qt.exe -server -rest -rpcuser=root -rpcpassword=toor -rpcport=9332";
            
            return View("CryptoCurrency", PaymentTypeConfig);
        }

        public IActionResult BitcoinCash()
        {
            db = new MarketBotDbContext();

            PaymentTypeEnum = PaymentTypeEnum.BitcoinCash;

            PaymentTypeConfig = db.PaymentTypeConfig.Where(p => p.PaymentId == PaymentType.GetTypeId(PaymentTypeEnum)).OrderByDescending(p => p.Id).FirstOrDefault();

            if (PaymentTypeConfig == null)
            {
                PaymentTypeConfig = new PaymentTypeConfig
                {
                    Host = "127.0.0.1",
                    Login = "root",
                    Pass = "toor",
                    Port = "8332",
                    Enable = true,
                    PaymentId = PaymentType.GetTypeId(PaymentTypeEnum)
                };

            }

            ViewBag.Title = "Bitcoin Cash";
            ViewBag.Text = "В папке с установленными Bitcoin Cash Core создайте бат файл.Сохраните и запустите этот бат файл и дождитесь синхронизации базы данных (Размер базы данных более 150гб)." +
                "Содержимое бат файла:";
            ViewBag.Bat = "bitcoin-qt.exe -server -rest -rpcuser=root -rpcpassword=toor -rpcport=8332";

            return View("CryptoCurrency", PaymentTypeConfig);
        }

        public IActionResult Bitcoin()
        {
            db = new MarketBotDbContext();

            PaymentTypeEnum = PaymentTypeEnum.Bitcoin;

            PaymentTypeConfig = db.PaymentTypeConfig.Where(p => p.PaymentId == PaymentType.GetTypeId(PaymentTypeEnum)).OrderByDescending(p => p.Id).FirstOrDefault();

            if (PaymentTypeConfig == null)
            {
                PaymentTypeConfig = new PaymentTypeConfig
                {
                    Host = "127.0.0.1",
                    Login = "root",
                    Pass = "toor",
                    Port = "8332",
                    Enable = true,
                    PaymentId = PaymentType.GetTypeId(PaymentTypeEnum)
                };

            }

            ViewBag.Title = "Bitcoin Cash";
            ViewBag.Text = "В папке с установленными Bitcoin Core создайте бат файл.Сохраните и запустите этот бат файл и дождитесь синхронизации базы данных (Размер базы данных более 180гб)." +
                "Содержимое бат файла:";
            ViewBag.Bat = "bitcoin-qt.exe -server -rest -rpcuser=root -rpcpassword=toor -rpcport=8332";

            return View("CryptoCurrency", PaymentTypeConfig);
        }

        public IActionResult Doge()
        {
            db = new MarketBotDbContext();

            PaymentTypeEnum = PaymentTypeEnum.Doge;

            PaymentTypeConfig = db.PaymentTypeConfig.Where(p => p.PaymentId == PaymentType.GetTypeId(PaymentTypeEnum)).OrderByDescending(p => p.Id).FirstOrDefault();

            if (PaymentTypeConfig == null)
            {
                PaymentTypeConfig = new PaymentTypeConfig
                {
                    Host = "127.0.0.1",
                    Login = "root",
                    Pass = "toor",
                    Port = "8332",
                    Enable = true,
                    PaymentId = PaymentType.GetTypeId(PaymentTypeEnum)
                };

            }

            ViewBag.Title = "Doge";
            ViewBag.Text = "В папке с установленными Doge Core создайте бат файл.Сохраните и запустите этот бат файл и дождитесь синхронизации базы данных (Размер базы данных более 20гб)." +
                "Содержимое бат файла:";
            ViewBag.Bat = "dogecoin-qt.exe -server -rest -rpcuser=root -rpcpassword=toor -rpcport=8332";

            return View("CryptoCurrency", PaymentTypeConfig);
        }

        public IActionResult PaymentOnReceipt()
        {
            db = new MarketBotDbContext();

            return View(db.PaymentType.Where(p => p.Id == 1).FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult SavePaymentOnReceipt([FromBody] PaymentType _paymentType)
        {
            db = new MarketBotDbContext();

            var OnReceipt = db.PaymentType.Where(p => p.Id == 1).FirstOrDefault();

            if (OnReceipt != null && _paymentType != null)
            {
                OnReceipt.Enable = _paymentType.Enable;
                db.SaveChanges();
                return Json("Сохранено");
            }

            else
                return Json("Ошибка");
        }

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Save([FromBody] PaymentTypeConfig config)
        {
            if(config!=null)
                PaymentTypeEnum = PaymentType.GetPaymentTypeEnum(config.PaymentId);

            if (config != null && config.Login == ""  && PaymentTypeEnum == PaymentTypeEnum.Qiwi)
                return Json("Заполните поле Номер телефона");

            if (config != null && config.Pass == "" && PaymentTypeEnum == PaymentTypeEnum.Qiwi)
                return Json("Заполните поле Токен доступа");

            if (config != null && config.Pass == "" && PaymentTypeEnum == PaymentTypeEnum.Litecoin)
                return Json("Заполните поле Пароль");

            if (config != null && config.Login == "" && PaymentTypeEnum == PaymentTypeEnum.Litecoin)
                return Json("Заполните поле Логин");

            if (config != null && config.Host == "" && PaymentTypeEnum == PaymentTypeEnum.Litecoin)
                return Json("Заполните поле Адрес RPC сервера");

            if (config != null && config.Port == "" && PaymentTypeEnum == PaymentTypeEnum.Litecoin)
                return Json("Заполните поле Порт RPC сервера");

            if (config != null && SaveChanges(config)>=0)
                return Json("Сохранено");
            
            else
                return Json("Ошибка"); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public IActionResult Delete (int id)
        {
            if (id > 0)
            {
                db = new MarketBotDbContext();

                var config = db.PaymentTypeConfig.Find(id);

                db.PaymentTypeConfig.Remove(config);

                db.SaveChanges();
              
            }

            return RedirectToAction("Qiwi");
        }

        [HttpPost]

        public IActionResult EnableQiwi([FromBody] PaymentType paymentType)
        {
            if (paymentType != null)
            {
                db = new MarketBotDbContext();

                var type = db.PaymentType.Find(paymentType.Id);

                type.Enable = paymentType.Enable;

                db.SaveChanges();

                return Json("Сохранено");
            }

            else
                return Json("Ошибка");
        }

        [HttpGet]

        public IActionResult AddQiwi(string telephone, string token)
        {
            db = new MarketBotDbContext();

            if (telephone != null && token != null)
            {
                PaymentTypeConfig = new PaymentTypeConfig
                {
                    Host = "https://qiwi.com/api",
                    Login = telephone,
                    Pass = token,
                    Port = "80",
                    Enable = true,
                    PaymentId = PaymentType.GetTypeId(PaymentTypeEnum.Qiwi)
                };

                db.PaymentTypeConfig.Add(PaymentTypeConfig);

                db.SaveChanges();
            }

            return RedirectToAction("Qiwi");
        }

        /// <summary>
        /// Сохранить изменения в бд
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private int SaveChanges(PaymentTypeConfig config)
        {
            if (db == null)
                db = new MarketBotDbContext();

            if (config != null && config.Id > 0) // Есле уже сущестуюущая запись, то редактируем ее
            {
                var OldCgf = db.PaymentTypeConfig.Where(c => c.Id == config.Id).FirstOrDefault();

                if (OldCgf != null)
                {
                    OldCgf.Login = config.Login;
                    OldCgf.Pass = config.Pass;
                    OldCgf.Enable = config.Enable;
                    OldCgf.Host = config.Host;
                    OldCgf.Port = config.Port;
                    return EnablePaymentType(config.Enable, Convert.ToInt32(config.PaymentId));
                    
                }

                else
                    return 0;
            }

            if(config!=null && config.Id == 0) // Если это новая запись то добавляем ее в бд
            {
                config.TimeStamp = DateTime.Now;
                db.PaymentTypeConfig.Add(config);
                return EnablePaymentType(config.Enable, Convert.ToInt32(config.PaymentId));
            }

            else
                return -1;
        }


        /// <summary>
        /// Включить/отключить метод оплаты
        /// </summary>
        /// <param name="value"></param>
        /// <param name="typePaymentId"></param>
        /// <returns></returns>
        private int EnablePaymentType(bool value, int typePaymentId)
        {
            if (db == null)
                db = new MarketBotDbContext();

            var payment = db.PaymentType.Where(p => p.Id == typePaymentId).FirstOrDefault();

            if (payment != null)
            {
                payment.Enable = value;
                return db.SaveChanges();
            }

            else
                return -1;

        }

        /// <summary>
        /// Проверка соединения с платежной системой
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> TestConnection([FromBody] PaymentTypeConfig config)
        {
            if (config != null)
                PaymentTypeEnum = PaymentType.GetPaymentTypeEnum(config.PaymentId);

            if (PaymentTypeEnum == PaymentTypeEnum.Qiwi)
            {
                if (await Services.Qiwi.QiwiFunction.TestConnection(config.Login, config.Pass))
                    return new JsonResult("Успех");

                else
                    return new JsonResult("Ошибка соединения");
            }

            if (PaymentTypeEnum != PaymentTypeEnum.Qiwi && config!=null)
            {
                string FirstBlockHash = String.Empty;

                if (PaymentTypeEnum == PaymentTypeEnum.Litecoin)
                    FirstBlockHash = "80ca095ed10b02e53d769eb6eaf92cd04e9e0759e5be4a8477b42911ba49c78f";

                if (PaymentTypeEnum == PaymentTypeEnum.BitcoinCash)
                    FirstBlockHash = "00000000839a8e6886ab5951d76f411475428afc90947ee320161bbf18eb6048";

                if (PaymentTypeEnum == PaymentTypeEnum.Doge)
                    FirstBlockHash = "82bc68038f6034c0596b6e313729793a887fded6e92a31fbdf70863f89d9bea2";

                if(PaymentTypeEnum==PaymentTypeEnum.Bitcoin)
                    FirstBlockHash = "00000000839a8e6886ab5951d76f411475428afc90947ee320161bbf18eb6048";

                Services.BitCoinCore.BitCoin ltc = new Services.BitCoinCore.BitCoin(config.Login, config.Pass, config.Host, config.Port);
                var block = ltc.GetBlockInfo<Services.BitCoinCore.BlockInfo>(FirstBlockHash);

                if (block != null)
                    return new JsonResult("Успех");

                else
                    return new JsonResult("Ошибка соединения");
            }

            else
                return new JsonResult("Ошибка соединения");
        }


    }
}