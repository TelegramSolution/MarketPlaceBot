using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class HelpDeskAttachment
    {
        public int HelpDeskId { get; set; }
        public int AttachmentFsId { get; set; }

        public AttachmentFs AttachmentFs { get; set; }
        public HelpDesk HelpDesk { get; set; }
    }
}
