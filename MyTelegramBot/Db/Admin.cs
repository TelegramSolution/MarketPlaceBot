using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot
{
    public partial class Admin
    {
        public int Id { get; set; }
        public int? FollowerId { get; set; }
        public DateTime? DateAdd { get; set; }
        public int? AdminKeyId { get; set; }

        public bool Enable { get; set; }

        /// <summary>
        /// Флаг указывет будет ли оператор получать уведомления об изменения в статусах заказа и Тех. поддержки
        /// </summary>
        public bool NotyfiActive { get; set; }

        public AdminKey AdminKey { get; set; }
        public Follower Follower { get; set; }
    }
}
