using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTelegramBot
{
    public partial class Orders
    {
        public Orders()
        {
            OrderProduct = new HashSet<OrderProduct>();
            OrdersInWork = new HashSet<OrdersInWork>();
            FeedBack = new HashSet<FeedBack>();
        }

        public int Id { get; set; }
        public int? Number { get; set; }
        public int FollowerId { get; set; }
        public string Text { get; set; }
        public DateTime? DateAdd { get; set; }
        public bool? Paid { get; set; }
        public int? BotInfoId { get; set; }
        public int? InvoiceId { get; set; }

        public int? PickupPointId { get; set; }

        public int? CurrentStatus { get; set; }

        public BotInfo BotInfo { get; set; }

        public Follower Follower { get; set; }

        public Invoice Invoice { get; set; }

        public PickupPoint PickupPoint { get; set; }

        /// <summary>
        /// флаг  указывающий на то, что после выполнения заказа, остатки на складе были обнавлены
        /// </summary>
        public bool StockUpdate { get; set; }
        public ICollection<FeedBack> FeedBack
        {
            get; set;
        }
        public OrderAddress OrderAddress { get; set; }
        public ICollection<OrderProduct> OrderProduct { get; set; }
        public ICollection<OrdersInWork> OrdersInWork { get; set; }

        public OrderStatus CurrentStatusNavigation { get; set; }

        /// <summary>
        /// Посчитать стоимость без учета стоимости доставки
        /// </summary>
        /// <returns></returns>
        public double TotalPrice()
        {
            int counter = 0;
            double total = 0.0;
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                foreach (OrderProduct p in OrderProduct) // Вытаскиваем все товары из заказа
                {
                    counter++;

                    if (p.Product == null)
                        p.Product = db.Product.Where(x => x.Id == p.ProductId).Include(x => x.CurrentPrice).FirstOrDefault();

                    if (p.Price == null)
                        p.Price = p.Product.CurrentPrice;


                    total += p.Price.Value * p.Count;
                }
            }

            return total;
        }


        /// <summary>
        /// Кол-во товара в заказке
        /// </summary>
        /// <param name="ProductId">id Товара</param>
        /// <returns></returns>
        public int PositionCount(int ProductId)
        {
            if (OrderProduct != null)
            {
                return OrderProduct.Where(p => p.ProductId == ProductId).ToList().Count;
            }

            else
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    OrderProduct = db.OrderProduct.Where(p => p.OrderId == Id).ToList();
                    return OrderProduct.Where(p => p.ProductId == ProductId).ToList().Count;
                }
            }
        }

        /// <summary>
        /// Общая стоимость нужного нам товара в заказе
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public double PositionPrice(int ProductId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            int counter = 0;
            double total = 0.0;

            try
            {
                if (OrderProduct == null)
                    OrderProduct = db.OrderProduct.Where(p => p.ProductId == ProductId).ToList();

                else
                {
                    var list = OrderProduct.Where(p => p.ProductId == ProductId).ToList();

                    foreach (OrderProduct p in list)
                    {
                        counter++;

                        if (p.Product == null)
                            p.Product = db.Product.Where(x => x.Id == p.ProductId).Include(x => x.CurrentPrice).FirstOrDefault();

                        if (p.Price == null)
                            p.Price = p.Product.CurrentPrice;

                        total += p.Price.Value * p.Count;
                    }
                }

                return total;
            }

            catch
            {
                return -1;
            }

            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Все позиции заказа в строковое значение для сообщения
        /// Пример.
        /// 1) Товар1 52руб. x 3 шт. = 156 руб.
        /// ......
        /// n) Товар2 40руб. x 1 шт. = 40 руб.
        /// </summary>
        /// <returns></returns>
        public string PositionToString()
        {
            MarketBotDbContext db = new MarketBotDbContext();
            try
            {

                int counter = 1;

                ///групируем позиции по id товара
                List<IGrouping<int, OrderProduct>> PositionGroup = new List<IGrouping<int, OrderProduct>>();

                string Positions = String.Empty;


                foreach (OrderProduct op in OrderProduct)
                {
                    if (op.Product == null)
                        op.Product = db.Product.Where(p => p.Id == op.ProductId).Include(p => p.Unit).FirstOrDefault();

                    if (op.Product != null && op.Product.Unit == null)
                        op.Product.Unit = db.Units.Find(op.Product.UnitId);

                    if (op.Price == null)
                        op.Price = db.ProductPrice.Where(p => p.Id == op.PriceId).Include(p => p.Currency).FirstOrDefault();

                    if (op.Price != null && op.Price.Currency == null)
                        op.Price.Currency = db.Currency.Find(op.Price.CurrencyId);
                }

                PositionGroup = OrderProduct.GroupBy(p => p.ProductId).ToList();

                foreach (var pos in PositionGroup)
                {

                    Positions += counter.ToString() + ") " + pos.FirstOrDefault().Product.Name + " " + pos.FirstOrDefault().Price.Value.ToString() +
                        pos.FirstOrDefault().Price.Currency.ShortName + " x " + pos.Count().ToString() + " " + pos.FirstOrDefault().Product.Unit.ShortName + " = "
                        + (pos.Count() * pos.FirstOrDefault().Price.Value).ToString() + " " + pos.FirstOrDefault().Price.Currency.ShortName + "\r\n";
                    counter++;
                }

                return Positions;
            }

            catch
            {
                return String.Empty;
            }

            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// После того как заказ выполнен, обновяем данные на складе
        /// </summary>
        /// <returns>возвращает ассоциативный массив Ид продукта - > массив вставленных в таблицу Stock значений</returns>
        public List<IGrouping<Product, Stock>> UpdateStock()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            List<Stock> list = new List<Stock>();

            List<IGrouping<Product, Stock>> result=new List<IGrouping<Product, Stock>>();


                if (OrderProduct == null || OrderProduct != null && OrderProduct.Count == 0)
                    OrderProduct = db.OrderProduct.Where(op => op.OrderId == Id).ToList();

                if(!StockUpdate && OrderProduct!=null && OrderProduct.Count > 0)
                {
                    
                    foreach(OrderProduct op in OrderProduct)
                    {
                        var last = db.Stock.Where(s => s.ProductId == op.ProductId).LastOrDefault().Balance;

                        if (op.Product == null)
                            op.Product = db.Product.Where(p=>p.Id==op.ProductId).Include(o=>o.Unit).FirstOrDefault();

                        Stock stock = new Stock
                        {
                            Balance = last - op.Count,
                            Quantity = -op.Count,
                            DateAdd = DateTime.Now,
                            Text = "Заказ " + Number.ToString(),
                            ProductId=op.ProductId

                        };

                        db.Stock.Add(stock);

                        db.SaveChanges();

                        list.Add(stock);
                    }

                    this.StockUpdate = true;

                    db.Update<Orders>(this);

                    db.SaveChanges();

                    result = list.GroupBy(s => s.Product).ToList();
                

                db.Dispose();
                return result;
            }

            else
                return null;
        }
    }
}
