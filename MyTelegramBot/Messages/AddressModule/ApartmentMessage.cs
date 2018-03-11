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
    /// сообщение с клавиатурой для ввода номера квартиры
    /// </summary>
    public class ApartmentMessage:BotMessage
    {

        private InlineKeyboardCallbackButton OneBtn { get; set; }

        private InlineKeyboardCallbackButton TwoBtn { get; set; }

        private InlineKeyboardCallbackButton ThreeBtn { get; set; }

        private InlineKeyboardCallbackButton FourBtn { get; set; }

        private InlineKeyboardCallbackButton FiveBtn { get; set; }

        private InlineKeyboardCallbackButton SixBtn { get; set; }

        private InlineKeyboardCallbackButton SevenBtn { get; set; }

        private InlineKeyboardCallbackButton EightBtn { get; set; }

        private InlineKeyboardCallbackButton NineBtn { get; set; }

        private InlineKeyboardCallbackButton ZeroBtn { get; set; }

        private InlineKeyboardCallbackButton ClearBtn { get; set; }

        private InlineKeyboardCallbackButton DoneApartmentBtn { get; set; }

        private int SelectNumber { get; set; }

        private int ApartmentNumber { get; set; }

        /// <summary>
        /// id адреса к которому добавляют квартиру
        /// </summary>
        private int AddressId { get; set; }

        private Address Address { get; set; }

        public ApartmentMessage(int AddressId)
        {
            this.AddressId = AddressId;
        }

        public ApartmentMessage(int AddressId, int SelectNumber)
        {
            this.AddressId = AddressId;
            this.SelectNumber = SelectNumber;
        }



        public override BotMessage BuildMsg()
        {
            OneBtn = new InlineKeyboardCallbackButton("1", BuildCallData(Bot.AddressBot.SelectApartmentNumberCmd, Bot.AddressBot.ModuleName, AddressId,1));

            TwoBtn = new InlineKeyboardCallbackButton("2", BuildCallData(Bot.AddressBot.SelectApartmentNumberCmd, Bot.AddressBot.ModuleName, AddressId,2));

            ThreeBtn = new InlineKeyboardCallbackButton("3", BuildCallData(Bot.AddressBot.SelectApartmentNumberCmd, Bot.AddressBot.ModuleName, AddressId,3));

            FourBtn = new InlineKeyboardCallbackButton("4", BuildCallData(Bot.AddressBot.SelectApartmentNumberCmd, Bot.AddressBot.ModuleName, AddressId,4));

            FiveBtn = new InlineKeyboardCallbackButton("5", BuildCallData(Bot.AddressBot.SelectApartmentNumberCmd, Bot.AddressBot.ModuleName, AddressId,5));

            SixBtn = new InlineKeyboardCallbackButton("6", BuildCallData(Bot.AddressBot.SelectApartmentNumberCmd, Bot.AddressBot.ModuleName, AddressId,6));

            SevenBtn = new InlineKeyboardCallbackButton("7", BuildCallData(Bot.AddressBot.SelectApartmentNumberCmd, Bot.AddressBot.ModuleName, AddressId,7));

            EightBtn = new InlineKeyboardCallbackButton("8", BuildCallData(Bot.AddressBot.SelectApartmentNumberCmd, Bot.AddressBot.ModuleName, AddressId, 8));

            NineBtn = new InlineKeyboardCallbackButton("9", BuildCallData(Bot.AddressBot.SelectApartmentNumberCmd, Bot.AddressBot.ModuleName, AddressId, 9));

            ZeroBtn = new InlineKeyboardCallbackButton("0", BuildCallData(Bot.AddressBot.SelectApartmentNumberCmd, Bot.AddressBot.ModuleName, AddressId, 0));

            ClearBtn = new InlineKeyboardCallbackButton("Очистить", BuildCallData(Bot.AddressBot.ClearApartmentNumberCmd, Bot.AddressBot.ModuleName, AddressId,-1));

            DoneApartmentBtn = new InlineKeyboardCallbackButton("Продолжить", BuildCallData(Bot.AddressBot.DoneApartmentCmd, Bot.AddressBot.ModuleName, AddressId));


            if (AddressId > 0)
            {
                using (MarketBotDbContext db=new MarketBotDbContext())
                {
                    string spl = String.Empty;

                    Address = db.Address.Find(AddressId);

                    Address.House = db.House.Find(Address.HouseId);
                   
                    //Пользователь нажад на кнопку с цифрой
                    if (Address != null && Address.House != null && SelectNumber>0 && AddressId>0)
                    {
                        spl = Address.House.Apartment.ToString() + this.SelectNumber.ToString();
                        Address.House.Apartment = spl;
                        db.SaveChanges();
                        base.TextMessage = Bold("Номер квартиры:") + spl;
                        base.CallBackTitleText= "Номер квартиры:" +spl;
                        SetInlineKeyBoard();
                        return this;
                    }

                    //Очистил значение
                    if (Address != null && Address.House != null && SelectNumber <0 && AddressId >0 && Address.House.Apartment!="")
                    {
                        spl = Address.House.Apartment.ToString() + this.SelectNumber.ToString();
                        Address.House.Apartment = String.Empty;
                        db.SaveChanges();
                        base.TextMessage = Bold("Номер квартиры:") ;
                        base.CallBackTitleText = "Введите номер квартиры";
                        SetInlineKeyBoard();
                        return this;
                    }

                    // сообщение после потверждения адреса
                    if (Address != null && Address.House != null && SelectNumber == 0)
                    {
                        base.TextMessage = Bold("Номер квартиры:");
                        SetInlineKeyBoard();
                        return this;
                    }

                    else // пользователь нажал очистить,но значение итак пустое. Возвращаем нулл, что бы ни чего не отправлял
                        return null;

                    

                }

             
            }

            else
                return null;


        }

        private void SetInlineKeyBoard()
        {
            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            OneBtn,TwoBtn,ThreeBtn
                        },
                new[]
                        {
                            FourBtn,FiveBtn,SixBtn
                        },

                new[]
                        {
                            SevenBtn,EightBtn,NineBtn
                        },

                new[]
                        {
                            ClearBtn,ZeroBtn,DoneApartmentBtn
                        },

                 });


        }
    }
}
