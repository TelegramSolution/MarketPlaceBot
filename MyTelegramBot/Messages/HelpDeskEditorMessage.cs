using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot;

namespace MyTelegramBot.Messages
{
    public class HelpDeskEditorMessage:BotMessage
    {
        private InlineKeyboardCallbackButton AddAttachBtn { get; set; }

        private InlineKeyboardCallbackButton SendBtn { get; set; }

        private int HelpDeskId { get; set; }

        private HelpDesk HelpDesk { get; set; }

        public HelpDeskEditorMessage(int HelpDeskId)
        {
            this.HelpDeskId = HelpDeskId;
        }

        public HelpDeskEditorMessage (HelpDesk helpDesk)
        {
            this.HelpDesk = helpDesk;

        }

        public override BotMessage BuildMsg()
        {

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                if (this.HelpDesk == null)
                    this.HelpDesk = db.HelpDesk.Where(h => h.Id == HelpDeskId).Include(h => h.HelpDeskAttachment).FirstOrDefault();

                if (this.HelpDesk.HelpDeskAttachment == null || this.HelpDesk.HelpDeskAttachment.Count==0)
                    this.HelpDesk = db.HelpDesk.Where(h => h.Id == this.HelpDesk.Id).Include(h => h.HelpDeskAttachment).FirstOrDefault();
            }

            if (HelpDesk!=null && !HelpDesk.Send==true)
            {
                AddAttachBtn = new InlineKeyboardCallbackButton("Добавить файл", BuildCallData("AddAttachHelpDesk",Bot.HelpDeskBot.ModuleName));
                SendBtn = new InlineKeyboardCallbackButton("Отправить заявку", BuildCallData("SendHelpDesk", Bot.HelpDeskBot.ModuleName,HelpDesk.Id));

                if (HelpDesk.HelpDeskAttachment != null)
                    base.TextMessage = Bold("Описание проблемы: ")+ HelpDesk.Text + NewLine() + Bold("Прикрепленных файлов: ") + HelpDesk.HelpDeskAttachment.Count.ToString();

                else
                    base.TextMessage = HelpDesk.Text + NewLine() + Bold("Прикрепленных файлов: ") + "0";

                base.MessageReplyMarkup = new InlineKeyboardMarkup(
                    new[]{
                new[]
                        {
                            AddAttachBtn
                        },
                new[]
                        {
                            SendBtn
                        },

                    });
            }

            if (HelpDesk!=null && HelpDesk.Send==true)
            {
                if (HelpDesk.HelpDeskAttachment != null)
                    base.TextMessage =Bold("Номер заявки: ")+HelpDesk.Number.ToString()+NewLine()
                            +Bold("Дата: ") +HelpDesk.Timestamp.ToString()+ NewLine() +
                            Bold("Описание проблемы: ")+ HelpDesk.Text + NewLine() + Bold("Прикрепленных файлов: ") + HelpDesk.HelpDeskAttachment.Count.ToString();

                else
                    base.TextMessage = HelpDesk.Text + NewLine() + Bold("Прикрепленных файлов: ") + "0";
            }
            return this;
        }

    }
}
