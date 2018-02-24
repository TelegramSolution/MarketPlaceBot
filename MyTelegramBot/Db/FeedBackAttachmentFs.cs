using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class FeedBackAttachmentFs
    {
        public int FeedBackId { get; set; }
        public int AttachmentFsId { get; set; }

        public AttachmentFs AttachmentFs { get; set; }
        public FeedBack FeedBack { get; set; }
    }
}
