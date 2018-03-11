using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.BusinessLayer
{
    public class AttachmentTelegramFunction
    {
        public static AttachmentTelegram AddAttachmentTelegram(int AttachFsId,int BotId ,string FileId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                AttachmentTelegram attachment = new AttachmentTelegram
                {
                    AttachmentFsId = AttachFsId,
                    FileId = FileId,
                    BotInfoId = BotId,

                };

                db.AttachmentTelegram.Add(attachment);
                db.SaveChanges();
                return attachment;

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
