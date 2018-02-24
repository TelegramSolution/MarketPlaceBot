using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class AttachmentFs
    {
        public AttachmentFs()
        {
            AttachmentTelegram = new HashSet<AttachmentTelegram>();
            FeedBackAttachmentFs = new HashSet<FeedBackAttachmentFs>();
            HelpDeskAnswerAttachment = new HashSet<HelpDeskAnswerAttachment>();
            HelpDeskAttachment = new HashSet<HelpDeskAttachment>();
            ProductPhoto = new HashSet<ProductPhoto>();
        }

        public int Id { get; set; }
        public Guid GuId { get; set; }
        public byte[] Fs { get; set; }
        public string Caption { get; set; }
        public int? AttachmentTypeId { get; set; }
        public string Name { get; set; }

        public AttachmentType AttachmentType { get; set; }
        public ICollection<AttachmentTelegram> AttachmentTelegram { get; set; }
        public ICollection<FeedBackAttachmentFs> FeedBackAttachmentFs { get; set; }
        public ICollection<HelpDeskAnswerAttachment> HelpDeskAnswerAttachment { get; set; }
        public ICollection<HelpDeskAttachment> HelpDeskAttachment { get; set; }
        public ICollection<ProductPhoto> ProductPhoto { get; set; }
    }
}
