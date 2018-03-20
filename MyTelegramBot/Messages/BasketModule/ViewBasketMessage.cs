using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using MyTelegramBot.Bot;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообещние с содержание корзины
    /// </summary>
    public class ViewBasketMessage:BotMessage
    {
        private InlineKeyboardCallbackButton ClearBasketBtn { get; set; }

        private InlineKeyboardCallbackButton ToCheckOutBtn { get; set; }

        private InlineKeyboardCallbackButton ReturnToCatalogBtn { get; set; }

        private InlineKeyboardCallbackButton BasketEditBtn { get; set; }

        private InlineKeyboardCallbackButton MainMenuBtn { get; set; }

        private Follower Follower { get; set; }

        private List<Basket> Basket { get; set; }
        private int FollowerId { get; set; }

        private int BotId { get; set; }

        public ViewBasketMessage(int FollowerId, int BotId)
        {
            this.FollowerId = FollowerId;
            this.BotId = BotId;
        }

        public override BotMessage BuildMsg ()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                Follower = db.Follower.Where(f => f.Id == FollowerId).FirstOrDefault();
                Basket = db.Basket.Where(b => b.FollowerId == Follower.Id && b.Enable).ToList();
            }

            if (Basket!=null && Basket.Count()>0)
            {
                ClearBasketBtn = ClearBasket(Follower.Id);
                ToCheckOutBtn = ToCheckOut(Follower.Id);
                BasketEditBtn = BasketEdit(Follower.Id);
                MainMenuBtn = BuildInlineBtn("На главную", BuildCallData(MainMenuBot.ToMainMenuCmd, MainMenuBot.ModuleName));
                SetInlineKeyBoard();
                string Info = BasketPositionInfo.GetPositionInfo(Follower.Id, BotId);
                base.TextMessage = Bold("Ваша корзина:") + NewLine() + Info;
                
            }

            else
            {
                base.CallBackTitleText = "Ваша Корзина пуста";
            }


            return this;
        }

        private void SetInlineKeyBoard()
        {
            

            base.MessageReplyMarkup = new InlineKeyboardMarkup(
                new[]{
                new[]
                        {
                            BasketEditBtn
                        },
                new[]
                        {
                            ToCheckOutBtn
                        },
                new[]
                        {
                            ClearBasketBtn
                        },
                new[]
                        {
                            MainMenuBtn
                        }


                 });
        }

        private InlineKeyboardCallbackButton ClearBasket (int FollowerId)
        {
            string data = BuildCallData(Bot.BasketBot.ClearBasketCmd, Bot.BasketBot.ModuleName, FollowerId);
            InlineKeyboardCallbackButton button = base.BuildInlineBtn("Очистить корзину", data,base.CrossEmodji);
            return button;
        }

        private InlineKeyboardCallbackButton ToCheckOut(int FollowerId)
        {
            string data = BuildCallData(Bot.OrderBot.MethodOfObtainingListCmd,Bot.OrderBot.ModuleName ,FollowerId);
            InlineKeyboardCallbackButton button = base.BuildInlineBtn("Перейти к оформлению", data,base.Next2Emodji);
            return button;
        }

        private InlineKeyboardCallbackButton BasketEdit(int FollowerId)
        {
            string data = BuildCallData(Bot.BasketBot.BasketEditCmd,Bot.BasketBot.ModuleName ,FollowerId);
            InlineKeyboardCallbackButton button = base.BuildInlineBtn("Изменить", data,base.PenEmodji);
            return button;
        }

    }
}
