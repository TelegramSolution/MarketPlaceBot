using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class Follower
    {
        public Follower()
        {
            Address = new HashSet<Address>();
            Admin = new HashSet<Admin>();
            Basket = new HashSet<Basket>();
            BlackList = new HashSet<BlackList>();
            HelpDesk = new HashSet<HelpDesk>();
            HelpDeskAnswer = new HashSet<HelpDeskAnswer>();
            HelpDeskInWork = new HashSet<HelpDeskInWork>();
            Notification = new HashSet<Notification>();
            OrderTemp = new HashSet<OrderTemp>();
            Orders = new HashSet<Orders>();
            OrdersInWork = new HashSet<OrdersInWork>();
            ReportsRequestLog = new HashSet<ReportsRequestLog>();
            TelegramMessage = new HashSet<TelegramMessage>();
            OrderHistory = new HashSet<OrderHistory>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public int? ChatType { get; set; }
        public bool Blocked { get; set; }
        public string Telephone { get; set; }
        public int ChatId { get; set; }
        public DateTime? DateAdd { get; set; }

        public ICollection<Address> Address { get; set; }
        public ICollection<Admin> Admin { get; set; }
        public ICollection<Basket> Basket { get; set; }
        public ICollection<BlackList> BlackList { get; set; }
        public ICollection<Notification> Notification { get; set; }
        public ICollection<OrderTemp> OrderTemp { get; set; }
        public ICollection<Orders> Orders { get; set; }
        public ICollection<ReportsRequestLog> ReportsRequestLog { get; set; }

        public ICollection<TelegramMessage> TelegramMessage { get; set; }

        public ICollection<HelpDesk> HelpDesk { get; set; }
        public ICollection<HelpDeskAnswer> HelpDeskAnswer { get; set; }

        public ICollection<OrdersInWork> OrdersInWork { get; set; }

        public ICollection<HelpDeskInWork> HelpDeskInWork { get; set; }

        public ICollection<OrderHistory> OrderHistory { get; set; }
    }
}
