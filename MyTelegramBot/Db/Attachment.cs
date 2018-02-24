using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class AttachmentTelegram
    {
        public int Id { get; set; }
        public string FileId { get; set; }
        public int? AttachmentFsId { get; set; }
        public int? BotInfoId { get; set; }

        public AttachmentFs AttachmentFs { get; set; }
        public BotInfo BotInfo { get; set; }
    }
}
