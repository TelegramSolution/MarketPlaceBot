using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    /// <summary>
    /// Здесь хранится текст сообщения для рассылки
    /// </summary>
    public partial class Notification
    {
        public int Id { get; set; }
        public int? FollowerId { get; set; }
        public DateTime? DateAdd { get; set; }
        public Follower Follower { get; set; }

        public bool Sended { get; set; }
        public string Text { get;  set; }
    }
}
