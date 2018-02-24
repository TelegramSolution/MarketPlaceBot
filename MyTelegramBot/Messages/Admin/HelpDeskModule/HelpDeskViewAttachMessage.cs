using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;
using MyTelegramBot.Bot.AdminModule;

namespace MyTelegramBot.Messages.Admin
{
    public class HelpDeskViewAttachMessage:Bot.BotMessage
    {
        private InlineKeyboardCallbackButton GetHelpDeskBtn { get; set; }

        HelpDesk HelpDesk { get; set; }

        Bot.BotMessage [] BotMessage { get; set;}

        List<HelpDeskAttachment> ListAttachFs { get; set; }

        private int BotId { get; set; }

        public HelpDeskViewAttachMessage(HelpDesk helpDesk, List<HelpDeskAttachment> list, int BotId)
        {
            this.HelpDesk = helpDesk;
            this.ListAttachFs = list;
            this.BotId = BotId;
        }

        public Bot.BotMessage[] BuildMessage()
        {
            if (ListAttachFs != null && ListAttachFs.Count > 0)
            {
                BotMessage = new Bot.BotMessage[ListAttachFs.Count];
                int counter = 0;

                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    foreach (var attachFs in ListAttachFs)
                    {
                        var telegram_attach= db.AttachmentTelegram.Where(a => a.AttachmentFsId == attachFs.AttachmentFsId && a.BotInfoId == BotId).FirstOrDefault();
                        GetHelpDeskBtn = new InlineKeyboardCallbackButton("Вернуться к заявке №" + HelpDesk.Number.ToString(),
                                                                            BuildCallData(Bot.AdminModule.HelpDeskProccessingBot.GetHelpDeskCmd, Bot.AdminModule.HelpDeskProccessingBot.ModuleName, HelpDesk.Id));
                        try
                        {
                            BotMessage[counter] = new Bot.BotMessage();
                            BotMessage[counter].MediaFile = GetMediaFile(db, telegram_attach);
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
                }

                return BotMessage;
            }

            else
                return null;

        }

        private Bot.MediaFile GetMediaFile(MarketBotDbContext db,AttachmentTelegram telegram_attach)
        {
            try
            {
                if (telegram_attach != null & telegram_attach.FileId != null)
                {
                     MediaFile = new Bot.MediaFile
                    {
                        FileTo = new Telegram.Bot.Types.FileToSend { FileId = telegram_attach.FileId, Filename = "File" },
                        Caption = db.AttachmentFs.Where(a => a.Id == telegram_attach.AttachmentFsId).FirstOrDefault().Caption,
                        TypeFileTo = Bot.MediaFile.HowMediaType(db.AttachmentFs.Where(a => a.Id == telegram_attach.AttachmentFsId).FirstOrDefault().AttachmentTypeId)
                    };

                    return MediaFile;
                }
                else
                {
                    var fs = db.AttachmentFs.Where(a => a.Id == telegram_attach.AttachmentFsId).FirstOrDefault();

                    MediaFile = new Bot.MediaFile
                    {
                        FileTo = new Telegram.Bot.Types.FileToSend
                        {
                            Content = new System.IO.MemoryStream(fs.Fs),
                            Filename = "File"
                        },
                        Caption = fs.Name,
                        TypeFileTo = Bot.MediaFile.HowMediaType(fs.AttachmentTypeId),
                        AttachmentFsId=Convert.ToInt32(fs.Id)
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
