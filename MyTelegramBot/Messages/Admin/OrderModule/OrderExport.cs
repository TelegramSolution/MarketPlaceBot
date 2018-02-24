using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.Messages.Admin
{
    /// <summary>
    /// Экспорт всех заказов в CSV
    /// </summary>
    public class OrderExport
    {

        public Stream Export()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                var Orders = db.Orders.Include(o => o.Confirm).Include(o => o.Done).Include(o => o.Delete).Include(o => o.OrderProduct).Include(o => o.OrderAddress).Include(o => o.Follower).Include(o=>o.FeedBack).ToList();

                string data = "Номер заказа;"+ 
                              "Время;"+
                              "Комментарий;"+ 
                              "Телефон;"+
                              "Адрес (город);"+
                              "Адрес (улица);" + 
                              "Адрес (дом);"+ 
                              "Согласован;" +
                              "Время согласования;" +
                              "Время выполнения;"+ 
                              "Позиция заказа;" +
                              "Название товара;"+ 
                              "Цена;" +
                              "Количество;"
                              +"Отзыв к заказу;";

                string newLine = "";
                foreach (Orders order in Orders)
                {
                    try
                    {
                        var Address = db.Address.Where(a => a.Id == order.OrderAddress.AdressId).Include(a => a.House).Include(a => a.House.Street).Include(a => a.House.Street.City).FirstOrDefault();
                        int counter = 1;
                        string Line = "";

                        string feedback = String.Empty;

                        if (order.FeedBack != null)
                            feedback = order.FeedBack.Text;

                        foreach (OrderProduct position in order.OrderProduct)
                        {
                            

                            Line = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};",
                                                    order.Number.ToString(), //  Номер 0
                                                    order.DateAdd.ToString(), // Дата 1
                                                    order.Text, // Комментарий 2
                                                    order.Follower.Telephone, // Телефон 3
                                                    Address.House.Street.City.Name,  // Город 4
                                                    Address.House.Street.Name, // УЛица 5
                                                    Address.House.Number, // Дом 6
                                                    order.Confirm.Text, // 7 Комментарий к согласованному заказу
                                                    order.Confirm.Timestamp.ToString(),// Дата согласования 8
                                                    order.Done.Timestamp.ToString(), // Время выполнения 9
                                                    counter.ToString(), // Позиция 10
                                                    db.Product.Where(p=>p.Id==position.ProductId).FirstOrDefault().Name, // Название товара 11
                                                    db.ProductPrice.Where(p=>p.Id==position.PriceId).FirstOrDefault().Value.ToString(), // Стоистомть 12
                                                    position.Count.ToString(),
                                                    feedback// Кол-во  13
                                                    ) + Bot.BotMessage.NewLine();
                             counter++;
                             newLine += Line;
                        }

                       
                    }

                    catch
                    {

                    }
                }


                if (WriteToFile(data + Bot.BotMessage.NewLine() + newLine) > 0)
                    return ReadFile();

                else
                    return null;

            }
        }

        private int WriteToFile(string data)
        {

                using (StreamWriter sw = new StreamWriter("Orders.csv"))
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
                var file = File.OpenRead("Orders.csv");

                return file;
            }

            catch
            {
                return null;
            }
        }
    }
}
