using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace MyTelegramBot.BusinessLayer
{
    public class StockFunction
    {
        public static int CurrentBalance(int ProductId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var CurrentStock = db.Stock.Where(s => s.ProductId == ProductId).LastOrDefault();

                if (CurrentStock != null)
                    return Convert.ToInt32(CurrentStock.Balance);

                else
                    return 0;
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

        public static List<Stock> GetAllStockHistory()
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.Stock.Include(s => s.Product).OrderByDescending(s=>s.Id).ToList();
            }

            catch
            {
                return null;
            }

            finally
            {
                db.Dispose();
            }
        }
    }
}
