using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение со списком адресов
    /// </summary>
    public class AddressListMessage:BotMessage
    {
        private InlineKeyboardCallbackButton [][] AddressListBtn { get; set; }

        private List<Address> AddressIdList { get; set; }

        private int FollowerId { get; set; }

        public AddressListMessage (int FollowerId)
        {
            this.FollowerId = FollowerId;
            BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData(Bot.BasketBot.BackToBasketCmd, Bot.BasketBot.ModuleName));
        }
        public override BotMessage BuildMsg()
        {
            
            using (MarketBotDbContext db=new MarketBotDbContext())
                AddressIdList=db.Address.Where(a => a.FollowerId == FollowerId).Include(a=>a.House).Include(a=>a.House.Street).Include(a=>a.House.Street.House).Include(a=>a.House.Street.City).ToList();
            

            if (AddressIdList != null && AddressIdList.Count() > 0)
            {
                AddressListBtn = new InlineKeyboardCallbackButton[AddressIdList.Count() + 2][];
                int counter = 0;

                foreach (Address address in AddressIdList)
                {
                    int? HouseId = address.HouseId;
                    var House = address.House;
                    var Street = address.House.Street;
                    var City = address.House.Street.City;

                    string Adr = City.Name + ", " + Street.Name + ", д. " + House.Number+", " +House.Apartment;
                    AddressListBtn[counter] = new InlineKeyboardCallbackButton[1];
                    AddressListBtn[counter][0] = base.BuildInlineBtn(Adr, BuildCallData(Bot.OrderBot.CmdSelectAddress, Bot.OrderBot.ModuleName, address.Id), base.HouseEmodji);
                    counter++;
                }

                AddressListBtn[counter] = new InlineKeyboardCallbackButton[1];
                AddressListBtn[counter][0] = AddAddress();

                AddressListBtn[counter + 1] = new InlineKeyboardCallbackButton[1];
                AddressListBtn[counter + 1][0] = BackBtn;


                base.MessageReplyMarkup = new InlineKeyboardMarkup(AddressListBtn);
                base.TextMessage = "Выберите адрес";

            }

            else
            {
                AddressListBtn = new InlineKeyboardCallbackButton[1][];
                AddressListBtn[0] = new InlineKeyboardCallbackButton[1];
                AddressListBtn[0][0] = AddAddress();
                base.MessageReplyMarkup = new InlineKeyboardMarkup(AddressListBtn);
                base.TextMessage = "Выберите адрес";
    
            }

            return this;
        }

        private InlineKeyboardCallbackButton AddressBtn(string text, int Id)
        {
            string data = BuildCallData(Bot.OrderBot.CmdSelectAddress, Bot.OrderBot.ModuleName,Id);
            InlineKeyboardCallbackButton  btn = new InlineKeyboardCallbackButton(text, data);
            return btn;
        }

        private InlineKeyboardCallbackButton AddAddress()
        {
            InlineKeyboardCallbackButton btn = new InlineKeyboardCallbackButton("Добавить новый", BuildCallData(Bot.AddressBot.CmdAddAddress, Bot.AddressBot.ModuleName));
            return btn;
        }
    }
}
