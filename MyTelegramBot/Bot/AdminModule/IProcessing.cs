using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTelegramBot.Bot.AdminModule
{
    interface IProcessing
    {
        /// <summary>
        /// Заявка не обрабатывается ни одним из пользователей
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<bool> CheckNotInWork<T>(T t);

        /// <summary>
        /// /// Проверям находиться ли завявка в обработке у текущего пользователя
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<bool> CheckInWork<T>(T t);

        /// <summary>
        /// Проверяем закрыта заявка или нет
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<bool> CheckIsDone<T>(T t);

        /// <summary>
        /// Увдомить всех пользвоателей об изменениях в заявке (новый коммент, заявка закрыта, взята кем то в обработку)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<bool> NotifyChanges(string text, int Id);

        /// <summary>
        /// Находится ли заявка в обработке у кого-то другого
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<bool> CheckInWorkOfAnotherUser<T>(T t);

    }
}
