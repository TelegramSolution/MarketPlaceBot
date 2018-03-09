using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.BusinessLayer
{
    public class HelpDeskFunction
    {

        /// <summary>
        /// Кто обрабатывает заявку в данный момент
        /// </summary>
        /// <param name="HelpDeskId"></param>
        /// <returns></returns>
        public static HelpDeskInWork WhoItWorkNow(int HelpDeskId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var InWork = db.HelpDeskInWork.Where(h => h.HelpDeskId == HelpDeskId).Include(h => h.Follower).LastOrDefault();

                if (InWork != null && InWork.InWork == true)
                    return InWork;

                if (InWork != null && InWork.InWork == false) // кто то обрабатывал но освободил
                    return null;

                else
                    return null;
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

        /// <summary>
        /// Добавить ответ на заявку 
        /// </summary>
        /// <param name="HelpDeskId"></param>
        /// <param name="FollowerId"></param>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static HelpDesk AddAnswerComment(int HelpDeskId, int FollowerId, string Text)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var Help = db.HelpDesk.Where(h => h.Id == HelpDeskId)
                                    .Include(h=>h.Follower)
                                    .Include(h => h.HelpDeskAnswer)
                                    .Include(h => h.HelpDeskAttachment)
                                    .Include(h => h.HelpDeskInWork).FirstOrDefault();


                HelpDeskAnswer helpDeskAnswer = new HelpDeskAnswer
                    {
                        Timestamp = DateTime.Now,
                        Closed = false,
                        FollowerId = FollowerId,
                        Text = Text,
                        HelpDeskId = HelpDeskId
                    };

                    db.HelpDeskAnswer.Add(helpDeskAnswer);
                    db.SaveChanges();
                    Help.HelpDeskAnswer.Add(helpDeskAnswer);
                    return Help;


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

        /// <summary>
        /// Закрыть заявку
        /// </summary>
        /// <param name="HelpDeskId"></param>
        /// <param name="FollowerId"></param>
        /// <returns></returns>
        public static HelpDesk HelpDeskClosed(int HelpDeskId,int FollowerId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var Help = db.HelpDesk.Where(h => h.Id == HelpDeskId).Include(h => h.HelpDeskAttachment).
                         Include(h => h.HelpDeskAnswer)
                        .Include(h => h.HelpDeskInWork)
                        .FirstOrDefault();

                HelpDeskAnswer helpDeskAnswer = new HelpDeskAnswer
                {
                    Closed = true,
                    ClosedTimestamp = DateTime.Now,
                    FollowerId = FollowerId,
                    HelpDeskId = HelpDeskId,
                    Timestamp = DateTime.Now,
                    Text = "Заявка закрыта",

                };

                db.HelpDeskAnswer.Add(helpDeskAnswer);
                Help.Closed = true;
                db.Update<HelpDesk>(Help);
                db.SaveChanges();
                Help.HelpDeskAnswer.Add(helpDeskAnswer);
                return Help;
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

        /// <summary>
        /// Взять заявку в работу
        /// </summary>
        /// <param name="HelpDeskId"></param>
        /// <param name="FollowerId"></param>
        /// <returns></returns>
        public static HelpDeskInWork TakeToWork(int HelpDeskId, int FollowerId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                HelpDeskInWork helpDeskInWork = new HelpDeskInWork
                {
                    FollowerId = FollowerId,
                    HelpDeskId = HelpDeskId,
                    InWork = true,
                    Timestamp = DateTime.Now

                };

                db.HelpDeskInWork.Add(helpDeskInWork);
                db.SaveChanges();
                return helpDeskInWork;
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

        /// <summary>
        /// освободить заявку.
        /// </summary>
        /// <param name="HelpDeskId"></param>
        /// <param name="FollowerId"></param>
        /// <returns></returns>
        public static HelpDeskInWork FreeHelpDesk(int HelpDeskId, int FollowerId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                HelpDeskInWork helpDeskInWork = new HelpDeskInWork
                {
                    FollowerId = FollowerId,
                    HelpDeskId = HelpDeskId,
                    InWork = false,
                    Timestamp = DateTime.Now

                };

                db.HelpDeskInWork.Add(helpDeskInWork);
                db.SaveChanges();
                return helpDeskInWork;
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

        public static HelpDesk GetHelpDesk(int HelpDeskId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.HelpDesk.Where(h => h.Id == HelpDeskId)
                                    .Include(h=>h.Follower)
                                    .Include(h => h.HelpDeskAnswer)
                                    .Include(h => h.HelpDeskAttachment)
                                    .Include(h => h.HelpDeskInWork).FirstOrDefault();
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

        public static HelpDesk GetHelpDesk(int FollowerId,int BotInfoId, bool Send=false)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.HelpDesk.Where(h => h.FollowerId == FollowerId && h.BotInfoId==BotInfoId && h.Send==Send)
                                    .Include(h => h.Follower)
                                    .Include(h => h.HelpDeskAnswer)
                                    .Include(h => h.HelpDeskAttachment)
                                    .Include(h => h.HelpDeskInWork).FirstOrDefault();
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


        public static HelpDesk HelpDeskFindByNumber(int Number)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return db.HelpDesk.Where(h => h.Number == Number)
                                    .Include(h=>h.Follower)
                                    .Include(h => h.HelpDeskAnswer)
                                    .Include(h => h.HelpDeskAttachment)
                                    .Include(h => h.HelpDeskInWork).FirstOrDefault();
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

        public static List<HelpDeskAttachment> GetHelpDeskAttachment(int HelpDeskId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                return  db.HelpDeskAttachment.Where(h => h.HelpDeskId == HelpDeskId).ToList();
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

        public static HelpDesk InsertHelpDesk(int FollowerId, int BotId, string Text)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
              var NoSendHelp = db.HelpDesk.Where(h => h.Send == false 
                                                && h.FollowerId == FollowerId 
                                                && h.BotInfoId == BotId)
                                                .Include(h => h.HelpDeskAttachment).FirstOrDefault();

                //У пользователя есть не отправленная заявка. Работаем с ней
                if (NoSendHelp != null && Text != null || NoSendHelp != null && Text != null)
                {
                    NoSendHelp.Text = Text;
                    db.Update<HelpDesk>(NoSendHelp);
                    db.SaveChanges();
                    return NoSendHelp;
                }

                //У пользователя нет не отправленных заявок. Создаем новую, но не даем Номер и делам статус не отправлена
                if (NoSendHelp == null && Text != null || NoSendHelp == null && Text != null)
                {
                    HelpDesk help = new HelpDesk
                    {
                        FollowerId = FollowerId,
                        Text = Text,
                        Send = false,
                        BotInfoId = BotId
                    };

                    db.HelpDesk.Add(help);
                    db.SaveChanges();
                    return help;
                }

                else
                    return null;

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

        /// <summary>
        /// Присвоить заявке номер и поставить Флаг "Отправлена"
        /// </summary>
        /// <param name="HelpDeskId"></param>
        /// <returns></returns>
        public static HelpDesk SaveHelpDesk(int HelpDeskId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                var Help = db.HelpDesk.Where(h => h.Id == HelpDeskId && h.Send == false).FirstOrDefault();

                var LastHelp = db.HelpDesk.Where(h => h.Send == true).OrderByDescending(h => h.Number).Include(h => h.HelpDeskAttachment).FirstOrDefault();

                if (Help != null)
                {
                    int number = 1;

                    if (LastHelp != null)
                        number = Convert.ToInt32(LastHelp.Number) + 1;

                    Help.Number = number;
                    Help.Send = true;
                    Help.Timestamp = DateTime.Now;
                    Help.InWork = false;
                    Help.Closed = false;

                    db.SaveChanges();

                    return Help;
                }

                else
                    return null;
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

        /// <summary>
        /// Добавить файл к заявке
        /// </summary>
        /// <param name="AttachFsId"></param>
        /// <param name="HelpDeskId"></param>
        /// <returns></returns>
        public static HelpDeskAttachment AddAttachToHelpDesk(int AttachFsId, int HelpDeskId)
        {
            MarketBotDbContext db = new MarketBotDbContext();

            try
            {
                if (AttachFsId > 0 && HelpDeskId > 0)
                {
                    HelpDeskAttachment attachment = new HelpDeskAttachment
                    {
                        AttachmentFsId = AttachFsId,
                        HelpDeskId = HelpDeskId
                    };

                    db.HelpDeskAttachment.Add(attachment);
                    db.SaveChanges();
                    return attachment;
                }

                else
                    return null;
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
