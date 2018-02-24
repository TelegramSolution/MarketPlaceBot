using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.Messages.Admin
{
    public class StockExport
    {

        public Stream Export()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                var stock = db.Stock.Include(s=>s.Product).Include(s=>s.Product.Category).OrderBy(s=>s.ProductId).ToList();

                string data = "Название товара;Категория;Дата;Добавлено / Убавлено; Остаток;Комментарий;";

                string line = "";

                foreach(Stock s in stock)
                {
                    var NewLine = string.Format("{0};{1};{2};{3};{4};{5};", s.Product.Name, s.Product.Category.Name, s.DateAdd.ToString(), s.Quantity.ToString(), s.Balance.ToString(),s.Text);
                    line +=NewLine+Bot.BotMessage.NewLine();
                }

                if (WriteToFile(data + Bot.BotMessage.NewLine() + line) > 0)
                    return ReadFile();

                else
                    return null;

            }
        }

        private int WriteToFile(string data)
        {

            using (StreamWriter sw = new StreamWriter("Stock.csv"))
            {
                try
                {
                    sw.Write(data);
                    sw.Flush();
                    sw.Close();
                    return 1;
                }

                catch
                {
                    sw.Dispose();
                    return -1;
                }
            }


        }

        private Stream ReadFile()
        {
            try
            {
                var file = File.OpenRead("Stock.csv");

                return file;
            }

            catch
            {
                return null;
            }
        }
    }
}
