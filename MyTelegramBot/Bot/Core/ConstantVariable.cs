using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot.Bot.Core
{
    public class ConstantVariable
    {
        /// <summary>
        /// способ оплаты
        /// </summary>
       public class PaymentTypeVariable
        {
            /// <summary>
            /// при получении
            /// </summary>
            public const int PaymentOnReceipt = 1;

            public const int QIWI = 2;

            public const int Litecoin = 3;

            public const int BitcoinCash = 4;

            public const int Doge = 5;

            public const int Bitcoin=6;

            /// <summary>
            /// оплата банковской карторй внутри бота, через яндекс кассу
            /// </summary>
            public const int DebitCardForYandexKassa = 7;

            public const int Dash = 8;

            public const int Zcash = 9;

        }

        /// <summary>
        /// тип медиа файл в телеграм
        /// </summary>
        public class MediaTypeVariable
        {
            public const int Photo = 1;

            public const int Video = 2;

            public const int Audio = 3;

            public const int Voice = 4;

            public const int VideoNote=5;

            /// <summary>
            /// zip, doc, exe, pdf и прочии файлы
            /// </summary>
            public const int Document = 6;

        }

        /// <summary>
        /// тип валюты которую использует бот
        /// </summary>
        public class CurrencyTypeVariable
        {
            /// <summary>
            /// Рубль
            /// </summary>
            public const int Rub = 1;

            /// <summary>
            /// Гривна
            /// </summary>
            public const int Uah = 2;
        }

        /// <summary>
        /// Основные статусы заказа
        /// </summary>
        public class OrderStatusVariable
        {
            /// <summary>
            /// Ожидает обработки
            /// </summary>
            public const int PendingProcessing = 1;

            /// <summary>
            /// согласован
            /// </summary>
            public const int AgreedUpon = 2;

            /// <summary>
            /// Отменен
            /// </summary>
            public const int Canceled = 3;

            /// <summary>
            /// выполнен
            /// </summary>
            public const int Completed = 4;

        }

    }
}
