using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.AdminModule;
using MyTelegramBot.Bot.Core;

namespace MyTelegramBot.Messages.Admin
{
    public class HelpDeskViewAttachMessage:BotMessage
    {
        private InlineKeyboardCallbackButton GetHelpDeskBtn { get; set; }

        HelpDesk HelpDesk { get; set; }

        BotMessage [] BotMessage { get; set;}

        List<HelpDeskAttachment> ListAttachFs { get; set; }

        private int BotId { get; set; }

        MarketBotDbContext db = new MarketBotDbContext();

        public HelpDeskViewAttachMessage(HelpDesk helpDesk, List<HelpDeskAttachment> list, int BotId)
        {
            this.HelpDesk = helpDesk;
            this.ListAttachFs = list;
            this.BotId = BotId;
        }

        public BotMessage[] BuildMessage()
        {
            if (ListAttachFs != null && ListAttachFs.Count > 0)
            {
                BotMessage = new BotMessage[ListAttachFs.Count];
                int counter = 0;
                db = new MarketBotDbContext();

                foreach (var attachFs in ListAttachFs)
                {
                    var telegram_attach= db.AttachmentTelegram.Where(a => a.AttachmentFsId == attachFs.AttachmentFsId && a.BotInfoId == BotId).FirstOrDefault();
                    GetHelpDeskBtn = new InlineKeyboardCallbackButton("Вернуться к заявке №" + HelpDesk.Number.ToString(),
                                                                        BuildCallData(Bot.AdminModule.HelpDeskProccessingBot.GetHelpDeskCmd, Bot.AdminModule.HelpDeskProccessingBot.ModuleName, HelpDesk.Id));
                    try
                    {
                        BotMessage[counter] = new BotMessage();
                        BotMessage[counter].MediaFile = GetMediaFile(telegram_attach);
                        BotMessage[counter].MessageReplyMarkup = new InlineKeyboardMarkup(
                        new[]{
                        new[]
                                    {
                                        GetHelpDeskBtn
                                    },

                         });

                    }
                    catch (Exception e)
                    {

                    }
                    counter++;
                }

                db.Dispose();
                return BotMessage;
            }

            else
                return null;

        }

        private MediaFile GetMediaFile(AttachmentTelegram telegram_attach)
        {
            try
            {
                if (telegram_attach != null & telegram_attach.FileId != null) // файл уже загруже на сервер телеграм 
                {
                     MediaFile = new MediaFile
                    {
                        FileTo = new Telegram.Bot.Types.FileToSend { FileId = telegram_attach.FileId, Filename = "File" },
                        Caption = db.AttachmentFs.Where(a => a.Id == telegram_attach.AttachmentFsId).FirstOrDefault().Caption,
                        FileTypeId=Convert.ToInt32(db.AttachmentFs.Where(a => a.Id == telegram_attach.AttachmentFsId).FirstOrDefault().AttachmentTypeId)
                     };

                    return MediaFile;
                }
                else
                {
                    var fs = db.AttachmentFs.Where(a => a.Id == telegram_attach.AttachmentFsId).FirstOrDefault();

                    MediaFile = new MediaFile
                    {
                        FileTo = new Telegram.Bot.Types.FileToSend
                        {
                            Content = new System.IO.MemoryStream(fs.Fs),
                            Filename = "File"
                        },
                        Caption = fs.Name,
                        
                        AttachmentFsId=Convert.ToInt32(fs.Id),
                        FileTypeId=Convert.ToInt32(fs.AttachmentTypeId)
                    };

                    return MediaFile;
                }
            }

            catch (Exception e)
            {
                return null;
            }
        }
    }
}
