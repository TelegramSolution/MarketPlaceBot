using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Telegram.Bot;

namespace MyTelegramBot.Controllers
{
    public class HelpController : Controller
    {
        MarketBotDbContext db;

        HelpDesk HelpDesk;

        TelegramBotClient TelegramBotClient { get; set; }

        public IActionResult Index()
        {
            db = new MarketBotDbContext();

            var list = db.HelpDesk.Where(h=>h.Number>0).Include(h=>h.Follower).Include(h=>h.BotInfo).OrderByDescending(h=>h.Id).ToList();

            return View(list);
        }

        [HttpGet]
        public IActionResult Get(int Number)
        {
            if (db == null)
                db = new MarketBotDbContext();

            if (Number > 0)
                HelpDesk = db.HelpDesk.Where(h => h.Number == Number).Include(h=>h.HelpDeskInWork).Include(h=>h.HelpDeskAnswer).Include(h=>h.Follower).Include(h=>h.HelpDeskAttachment).FirstOrDefault();


            if (HelpDesk != null)
            {
                if (HelpDesk.HelpDeskInWork != null)
                    foreach (HelpDeskInWork work in HelpDesk.HelpDeskInWork)
                        work.Follower = db.Follower.Where(f => f.Id == work.FollowerId).FirstOrDefault();

                if (HelpDesk.HelpDeskAnswer != null)
                    foreach (HelpDeskAnswer answer in HelpDesk.HelpDeskAnswer)
                        answer.Follower = db.Follower.Where(f => f.Id == answer.FollowerId).FirstOrDefault();

                return View(HelpDesk);
            }



            else
                return NotFound();
        }

        /// <summary>
        /// Отправить файлы прикрепленные к заявке в Лс владельцу. Нужно доработать, если нет FileId то ни чего не отправит
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
      
        public IActionResult ViewAttach(int id)
        {
            db = new MarketBotDbContext();

            var BotInfo = db.BotInfo.Where(b => b.Name == Bot.GeneralFunction.GetBotName()).FirstOrDefault();

            var Help = db.HelpDeskAttachment.Where(h => h.HelpDeskId == id).ToList();

            TelegramBotClient = new TelegramBotClient(BotInfo.Token);

             foreach(HelpDeskAttachment hda in Help)
            {
                hda.AttachmentFs = db.AttachmentFs.Where(a => a.Id == hda.AttachmentFsId).Include(a => a.AttachmentTelegram).FirstOrDefault();

                if(hda!=null && hda.AttachmentFs.AttachmentTelegram.Where(a=>a.BotInfoId==BotInfo.Id).FirstOrDefault()!=null && hda.AttachmentFs.AttachmentTypeId==1)
                {
                    string file_id = hda.AttachmentFs.AttachmentTelegram.Where(a => a.BotInfoId == BotInfo.Id).FirstOrDefault().FileId;
                    TelegramBotClient.SendPhotoAsync(BotInfo.OwnerChatId, new Telegram.Bot.Types.FileToSend { FileId = file_id, Filename = "photo.png" },hda.AttachmentFs.Caption);
                }


                if (hda != null && hda.AttachmentFs.AttachmentTelegram.Where(a => a.BotInfoId == BotInfo.Id).FirstOrDefault() != null && hda.AttachmentFs.AttachmentTypeId == 2)
                {
                    string file_id = hda.AttachmentFs.AttachmentTelegram.Where(a => a.BotInfoId == BotInfo.Id).FirstOrDefault().FileId;
                    TelegramBotClient.SendVideoAsync(BotInfo.OwnerChatId, new Telegram.Bot.Types.FileToSend { FileId = file_id, Filename = "" }, caption: hda.AttachmentFs.Caption);
                }

                if (hda != null && hda.AttachmentFs.AttachmentTelegram.Where(a => a.BotInfoId == BotInfo.Id).FirstOrDefault() != null && hda.AttachmentFs.AttachmentTypeId == 3)
                {
                    string file_id = hda.AttachmentFs.AttachmentTelegram.Where(a => a.BotInfoId == BotInfo.Id).FirstOrDefault().FileId;
                    TelegramBotClient.SendVoiceAsync(BotInfo.OwnerChatId, new Telegram.Bot.Types.FileToSend { FileId = file_id, Filename = "" }, caption: hda.AttachmentFs.Caption);
                }
            }

            if (Help.Count > 0)
                return Json("Файл отправлены через Telegram");

            else
                return Json("Не вложенных файлов");
        }

        [HttpPost]

        public IActionResult AddComment ([FromBody] HelpDeskAnswer helpDeskAnswer)
        {
            db = new MarketBotDbContext();

            if (helpDeskAnswer != null && helpDeskAnswer.Text != null && helpDeskAnswer.HelpDeskId>0)
            {
                HelpDesk = db.HelpDesk.Find(helpDeskAnswer.HelpDeskId);

                if (!HelpDesk.Closed)
                {

                    var BotInfo = db.BotInfo.Where(b => b.Name == Bot.GeneralFunction.GetBotName()).FirstOrDefault();

                    helpDeskAnswer.FollowerId = db.Follower.Where(f => f.ChatId == BotInfo.OwnerChatId).FirstOrDefault().Id;

                    helpDeskAnswer.Timestamp = DateTime.Now;

                    HelpDesk = db.HelpDesk.Find(helpDeskAnswer.HelpDeskId);

                    HelpDesk.Closed = helpDeskAnswer.Closed;

                    if (helpDeskAnswer.Closed)
                        helpDeskAnswer.ClosedTimestamp = DateTime.Now;

                    db.HelpDeskAnswer.Add(helpDeskAnswer);

                    db.SaveChanges();

                    return Json("Добавлено");
                }

                else
                    return Json("Заявка уже выполнена");
            }

            else
                return Json("Ошибка");
        }
    }
}