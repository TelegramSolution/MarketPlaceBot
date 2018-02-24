using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class HelpDeskAnswerAttachment
    {
        public int HelpDeskAnswerId { get; set; }
        public int AttachmentFsId { get; set; }

        public AttachmentFs AttachmentFs { get; set; }
        public HelpDeskAnswer HelpDeskAnswer { get; set; }
    }
}
