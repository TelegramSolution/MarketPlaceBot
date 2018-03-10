﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.InlineQueryResults;
using MyTelegramBot.Bot;
using Newtonsoft.Json;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    public class InvoiceViewMessage:BotMessage
    {
        private Invoice Invoice { get; set; }

        private List<Payment> PaymentList { get; set; }

        private int OrderId { get; set; }

        private MarketBotDbContext db { get; set; }

        private InlineKeyboardCallbackButton CheckPayBtn { get; set; }

        private InlineKeyboardCallbackButton ProceedToСheckoutBtn { get; set; }

        public InvoiceViewMessage (Invoice invoice,int OrderId)
        {
            this.Invoice = invoice;
            this.OrderId = OrderId;

            BackBtn = BuildInlineBtn("Вернуться к заказу", BuildCallData(OrderBot.BackToMyOrderCmd, OrderBot.ModuleName, OrderId),base.Previuos2Emodji,false);

            
        }


        public override BotMessage BuildMsg()
        {
            db = new MarketBotDbContext();

            string IsPaid = "Нет";

            if (PaymentList == null || PaymentList.Count == 0)
                    PaymentList = db.Payment.Where(p => p.InvoiceId == Invoice.Id).ToList();

            if (Invoice != null)
            {
                if (Invoice.PaymentType == null)
                    Invoice.PaymentType = db.PaymentType.Where(p => p.Id == Invoice.PaymentTypeId).FirstOrDefault();

                if (Invoice.Paid && Invoice.Payment.Count>0)
                    IsPaid = "Да (" +Invoice.Payment.LastOrDefault().TimestampDataAdd.ToString()+")";

                base.TextMessage = Bold("Счет на оплату №") + Invoice.InvoiceNumber.ToString() + NewLine() +
                                 Bold("Адрес счета получателя:") + Invoice.AccountNumber + NewLine() +
                                 Bold("Комментарий к платежу:") + Invoice.Comment + NewLine() +
                                 Bold("Сумма: ") + Invoice.Value.ToString() + " " + Invoice.PaymentType.Code + NewLine() +
                                 Bold("Время создания: ") + Invoice.CreateTimestamp.ToString() + NewLine() +
                                 Bold("Способ оплаты: ") + Invoice.PaymentType.Name +NewLine()+
                                 Bold("Оплачено:")+IsPaid
                                 + NewLine() + NewLine() +
                                 "Вы должны оплатить этот счет не позднее " + Invoice.CreateTimestamp.Value.Add(Invoice.LifeTimeDuration.Value).ToString() + NewLine();


                if (Invoice.PaymentTypeId == Bot.Core.ConstantVariable.PaymentTypeVariable.DebitCardForYandexKassa && !Invoice.Paid)
                    ProceedToСheckoutBtn= BuildInlineBtn("Перейти к оплате", BuildCallData(OrderBot.CmdDebitCardСheckout, OrderBot.ModuleName, OrderId),base.Next2Emodji);

                if (Invoice.PaymentTypeId != Bot.Core.ConstantVariable.PaymentTypeVariable.DebitCardForYandexKassa && !Invoice.Paid)
                    CheckPayBtn = new InlineKeyboardCallbackButton("Я оплатил", BuildCallData(Bot.OrderBot.CheckPayCmd, OrderBot.ModuleName, OrderId,Invoice.Id));

                if (Invoice.PaymentTypeId==Bot.Core.ConstantVariable.PaymentTypeVariable.QIWI)
                    base.TextMessage+= NewLine() + "После оплаты нажмите кнопку \"Я оплатил\"";

                if(Invoice.PaymentTypeId!= Bot.Core.ConstantVariable.PaymentTypeVariable.QIWI && Invoice.PaymentTypeId!= Bot.Core.ConstantVariable.PaymentTypeVariable.DebitCardForYandexKassa)
                    base.TextMessage += NewLine() + "После оплаты нажмите кнопку \"Я оплатил\"" +NewLine() + Italic("Для криптовалютных платежей требуется 2 подтверждения");


                if (Invoice.PaymentType != null &&Invoice.PaymentType.Id == Bot.Core.ConstantVariable.PaymentTypeVariable.Litecoin ||
                    Invoice.PaymentType != null && Invoice.PaymentType.Id == Bot.Core.ConstantVariable.PaymentTypeVariable.Bitcoin ||
                    Invoice.PaymentType != null && Invoice.PaymentType.Id == Bot.Core.ConstantVariable.PaymentTypeVariable.Doge)
                    base.TextMessage += NewLine() + NewLine() +
                        HrefUrl("https://live.blockcypher.com/"+Invoice.PaymentType.Code+"/address/" + Invoice.AccountNumber, "Посмотреть платеж");


                if (Invoice.PaymentType != null && Invoice.PaymentType .Id== Bot.Core.ConstantVariable.PaymentTypeVariable.BitcoinCash)
                    base.TextMessage += NewLine() + NewLine() +
                        HrefUrl("https://blockchair.com/bitcoin-cash/address/" + Invoice.AccountNumber, "Посмотреть платеж");

                if (Invoice.PaymentType != null && Invoice.PaymentType.Id == Bot.Core.ConstantVariable.PaymentTypeVariable.Dash)
                    base.TextMessage += NewLine() + NewLine() +
                         HrefUrl("https://explorer.dash.org/address/" + Invoice.AccountNumber, "Посмотреть платеж");

                if (Invoice.PaymentType != null && Invoice.PaymentType.Id == Bot.Core.ConstantVariable.PaymentTypeVariable.Zcash)
                    base.TextMessage += NewLine() + NewLine() +
                         HrefUrl("https://explorer.zcha.in/accounts/" + Invoice.AccountNumber, "Посмотреть платеж");


                if (Invoice.PaymentTypeId == Bot.Core.ConstantVariable.PaymentTypeVariable.QIWI && !Invoice.Paid)
                    base.TextMessage += NewLine() +  HrefUrl(QiwiForm(Invoice.AccountNumber,Convert.ToInt32(Invoice.Value),Invoice.Comment),"Открыть платежную форму")+
                        NewLine()+"Обязательно указывайте комментарий" + base.WarningEmodji;

                SetButtons();
             
            }
                db.Dispose();
                return this;
        }

        private void SetButtons()
        {
            if(Invoice.Paid==true)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                    new[]{
                    new[]
                        {
                            BackBtn,

                        },
                    });

            if (Invoice.Paid == false && ProceedToСheckoutBtn==null)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                    new[]{
                    new[]
                        {
                            BackBtn,

                        },
                    new[]
                        {
                            CheckPayBtn,

                        },
                    });

            if (Invoice.Paid == false && ProceedToСheckoutBtn != null)
                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                    new[]{
                    new[]
                        {
                            BackBtn,

                        },
                    new[]
                        {
                            ProceedToСheckoutBtn,

                        },
                    });
        }

        private string QiwiForm(string tel, int summ, string comm)
        {
            //https://qiwi.com/payment/form/99?extra%5B%27account%27%5D=79991112233&amountInteger=1&extra%5B%27comment%27%5D=test123&currency=643


            return "https://qiwi.com/payment/form/99?"+ "extra%5B%27account%27%5D="+tel+ "&amountInteger="+summ.ToString()+ "&extra%5B%27comment%27%5D="+comm+ "&currency=643";
        }
    }
}
