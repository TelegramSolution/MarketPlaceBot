using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.Controllers
{
    public class StockController : Controller
    {
        MarketBotDbContext db;

        public IActionResult Index()
        {
            try
            {
                db = new MarketBotDbContext();

                List<Product> list = db.Product.Include(p => p.Unit).Include(p => p.Category).ToList();

                foreach (Product prod in list)
                {
                    var stock = db.Stock.Where(s => s.ProductId == prod.Id).OrderByDescending(s => s.Id).FirstOrDefault();

                    if (stock != null)
                        prod.Stock.Add(stock);

                    else
                    {
                        prod.Stock.Add(new Stock
                        {
                            Balance = 0
                        });
                    }
                }

                return View(list);
            }

            catch
            {
                return Json("Ошибка подключения к базе данных");
            }

        }

        [HttpGet]
        public IActionResult Editor(int Id)
        {
            db = new MarketBotDbContext();

            var product= db.Product.Where(p=>p.Id==Id).Include(p => p.Unit).Include(p => p.Category).Include(p => p.Stock).FirstOrDefault();

            if(product!=null && product.Stock!=null)
                product.Stock=product.Stock.OrderByDescending(s => s.Id).ToList();

            if (product != null)
                return View(product);

            else
                return Json("Товар не найден");
        }

        /// <summary>
        /// Добавить новую запись в таблицу с остатками
        /// </summary>
        /// <param name="Quantity">кол-во</param>
        /// <param name="Id">ид продукта</param>
        /// <param name="Comment">комментарий</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add(int Quantity, int Id, string Comment)
        {

            if(Quantity!=0 && Id > 0)
            {
                Stock new_stock = new Stock
                {
                    ProductId = Id,
                    DateAdd = DateTime.Now,
                    Text = Comment,
                    Quantity=Quantity
                    
                };

                db = new MarketBotDbContext();

                var stock = db.Stock.Where(s => s.ProductId == Id).OrderByDescending(s=>s.Id).FirstOrDefault();

                if (stock != null && stock.Balance + Quantity > 0)
                {
                    new_stock.Balance = stock.Balance + Quantity;
                    db.Stock.Add(new_stock);
                    db.SaveChangesAsync();

                }

                if(stock==null && Quantity > 0)
                {
                    new_stock.Balance = Quantity;
                    db.Stock.Add(new_stock);
                    db.SaveChangesAsync();
                }
                
            }

            RedirectResult redirectResult = new RedirectResult("Editor\\" + Id);

            return redirectResult;
        }
    }
}