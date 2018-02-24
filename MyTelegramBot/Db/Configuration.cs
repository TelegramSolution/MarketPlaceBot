using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class Configuration
    {
        public int Id { get; set; }
        public string ExampleCsvFileId { get; set; }
        public string TemplateCsvFileId { get; set; }
        public bool? BotBlocked { get; set; }
        public string ManualFileId { get; set; }

        /// <summary>
        /// Id чата куда добавлен бот для отправки уведомлений
        /// </summary>
        public string PrivateGroupChatId { get; set; }
        public int? BotInfoId { get; set; }

        /// <summary>
        /// флаг указывающий нужно ли требовать от пользователя указывать свой номер телефона перед оформлением заказа
        /// </summary>
        public bool VerifyTelephone { get; set; }

        /// <summary>
        /// флаг указывающий будет ли владелец получать уведомления о заказах, заявках в Личку.
        /// </summary>
        public bool OwnerPrivateNotify { get; set; }

        /// <summary>
        /// FileId картиники с подсказкой, как указать никнейм (юзернейм)
        /// </summary>
        public string UserNameFaqFileId { get; set; }

        /// <summary>
        /// Способ получения заказа: Доствка. true - доступен для пользовталей
        /// </summary>
        public bool Delivery { get; set; }

        /// <summary>
        /// Стоимость доставки
        /// </summary>
        public double ShipPrice { get; set; }

        /// <summary>
        /// Стоимость заказа при которой, доставка будет бесплатной
        /// </summary>
        public double FreeShipPrice { get; set; }

        /// <summary>
        /// Способ получения заказа: Самовывоз. true - доступен для пользователей
        /// </summary>
        public bool Pickup { get; set; }

        public int? CurrencyId { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public BotInfo BotInfo { get; set; }
        public Currency Currency { get; set; }


    }
}
