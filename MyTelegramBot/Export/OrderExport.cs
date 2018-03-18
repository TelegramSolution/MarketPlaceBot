using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using MyTelegramBot.BusinessLayer;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace MyTelegramBot.Export
{
    public class OrderExport:ExcelExport
    {

        List<Orders> OrderList { get; set; }

        List<OrderStatus> OrderStatusList { get; set; }

        List<Payment> PaymentList { get; set; }

        ExcelWorksheet OrderWorksheet { get; set; }

        ExcelWorksheet OrderStatusHisotyWorksheet { get; set; }

        ExcelWorksheet PaymentkWorksheet { get; set; }


        public override Stream BuildReport()
        {
            OrderWorksheet = Excel.Workbook.Worksheets.Add("Список заказов");
            CreateOrderWorksheet();

            OrderStatusHisotyWorksheet = Excel.Workbook.Worksheets.Add("История статусов");
            CreateOrderStatusHisotyWorksheet();

            PaymentkWorksheet = Excel.Workbook.Worksheets.Add("Платежи");
            CreatePaymentWorksheet();


            MemoryStream memoryStream = new MemoryStream(Excel.GetAsByteArray());

            return memoryStream;

        }

        /// <summary>
        /// инфо о заказах
        /// </summary>
        private void CreateOrderWorksheet()
        {
            OrderList = OrderFunction.GetAllOrders();

            // шапка
            OrderWorksheet.Cells[1, 1].Value = "Номер заказа";
            OrderWorksheet.Cells[1, 2].Value = "Дата";
            OrderWorksheet.Cells[1, 3].Value = "Комментарий";
            OrderWorksheet.Cells[1, 4].Value = "Пользователь";
            OrderWorksheet.Cells[1, 5].Value = "Телефон";
            OrderWorksheet.Cells[1, 6].Value = "Товар";
            OrderWorksheet.Cells[1, 7].Value = "Цена";
            OrderWorksheet.Cells[1, 8].Value = "Кол-во";
            OrderWorksheet.Cells[1, 9].Value = "Стоимость";
            OrderWorksheet.Cells[1, 10].Value = "Текущий статус заказа";
            OrderWorksheet.Cells[1, 11].Value = "Стоимость доставки";
            OrderWorksheet.Cells[1, 12].Value = "Адрес доставки";
            OrderWorksheet.Cells[1, 13].Value = "Пункт самовывоза";
            OrderWorksheet.Cells[1, 14].Value = "Способ оплаты";
            OrderWorksheet.Cells[1, 15].Value = "Стоимость заказа без учета доставки";

            int row = 2;

            foreach (var order in OrderList)
            {

                foreach (var position in order.OrderProduct.GroupBy(p => p.Product))
                {

                    OrderWorksheet.Cells[row, 1].Value = order.Number;
                    // указываем что число
                    OrderWorksheet.Cells[row, 1].Style.Numberformat.Format = "0";

                    OrderWorksheet.Cells[row, 2].Value = order.DateAdd.ToString();

                    //комментарий к заказу
                    OrderWorksheet.Cells[row, 3].Value = order.Text;

                    //ПОльзователь
                    OrderWorksheet.Cells[row, 4].Value = order.Follower.FirstName + " " + order.Follower.LastName;

                    //Номер телефона
                    if (order.Follower.Telephone != null)
                        OrderWorksheet.Cells[row, 5].Value = order.Follower.Telephone;

                    //товар
                    OrderWorksheet.Cells[row, 6].Value = position.FirstOrDefault().Product.Name;

                    //цена
                    OrderWorksheet.Cells[row, 7].Value = position.FirstOrDefault().Price.Value;
                    OrderWorksheet.Cells[row, 7].Style.Numberformat.Format = @"#,##0.00_ ;\-#,##0.00_ ;0.00_ ;";

                    //кол-во
                    OrderWorksheet.Cells[row, 8].Value = position.Count();
                    OrderWorksheet.Cells[row, 8].Style.Numberformat.Format = "0";

                    //стоимость
                    OrderWorksheet.Cells[row, 9].Value = (position.Count() * position.FirstOrDefault().Price.Value);
                    OrderWorksheet.Cells[row, 9].Style.Numberformat.Format = @"#,##0.00_ ;\-#,##0.00_ ;0.00_ ;";

                    //текущий статус заказа
                    OrderWorksheet.Cells[row, 10].Value = order.CurrentStatusNavigation.Status.Name;

                    //стоимость доставки и адрес
                    if (order.OrderAddress != null && order.OrderAddress.Adress != null)
                    {
                        OrderWorksheet.Cells[row, 11].Value = order.OrderAddress.ShipPriceValue;
                        OrderWorksheet.Cells[row, 11].Style.Numberformat.Format = @"#,##0.00_ ;\-#,##0.00_ ;0.00_ ;";

                        OrderWorksheet.Cells[row, 12].Value = order.OrderAddress.Adress.ToString();

                    }

                    //самовывоз
                    if (order.PickupPoint != null)
                    {
                        OrderWorksheet.Cells[row, 13].Value = order.PickupPoint.Name;
                    }

                    //способ оплаты
                    if (order.Invoice != null && order.Invoice.PaymentType != null)
                        OrderWorksheet.Cells[row, 14].Value = order.Invoice.PaymentType.Name;

                    else
                        OrderWorksheet.Cells[row, 14].Value = "При получении";

                    //стоимость без учета доставки
                    OrderWorksheet.Cells[row, 15].Value = order.TotalPrice();
                    OrderWorksheet.Cells[row, 15].Style.Numberformat.Format = @"#,##0.00_ ;\-#,##0.00_ ;0.00_ ;";

                    row++;
                }

            }
        }


        /// <summary>
        /// история статусов 
        /// </summary>
        private void CreateOrderStatusHisotyWorksheet()
        {
            OrderStatusList = OrderFunction.GetAllHistoryStatus();

            // шапка
            OrderStatusHisotyWorksheet.Cells[1, 1].Value = "Номер заказа";
            OrderStatusHisotyWorksheet.Cells[1, 2].Value = "Дата";
            OrderStatusHisotyWorksheet.Cells[1, 3].Value = "Статус";
            OrderStatusHisotyWorksheet.Cells[1, 4].Value = "Комментарий";
            OrderStatusHisotyWorksheet.Cells[1, 5].Value = "Пользователь";

            int row = 2;

            foreach(var status in OrderStatusList)
            {
                //номер заказа
                OrderStatusHisotyWorksheet.Cells[row, 1].Value = status.Orders.FirstOrDefault().Number;

                //дата
                OrderStatusHisotyWorksheet.Cells[row, 2].Value = status.Timestamp.ToString();

                //Статус
                OrderStatusHisotyWorksheet.Cells[row, 3].Value = status.Status.Name;

                //коммент
                OrderStatusHisotyWorksheet.Cells[row, 4].Value = status.Text;

                //пользователь
                OrderStatusHisotyWorksheet.Cells[row, 5].Value = status.Follower.FirstName + " " +status.Follower.LastName ;

                row++;
            }
        }

        /// <summary>
        /// Платежи
        /// </summary>
        private void CreatePaymentWorksheet()
        {
            PaymentList = PaymentsFunction.GetPaymentsList();

            // шапка
            PaymentkWorksheet.Cells[1, 1].Value = "Номер заказа";
            PaymentkWorksheet.Cells[1, 2].Value = "Дата";
            PaymentkWorksheet.Cells[1, 3].Value = "Id платежа";
            PaymentkWorksheet.Cells[1, 4].Value = "Номер транзакции";
            PaymentkWorksheet.Cells[1, 5].Value = "Комменатрий";
            PaymentkWorksheet.Cells[1, 6].Value = "Адрес счета получателя";
            PaymentkWorksheet.Cells[1, 7].Value = "Сумма";
            PaymentkWorksheet.Cells[1, 8].Value = "Платежная система";

            int row = 2;

            foreach (var payment in PaymentList)
            {
                //Номер заказа
                PaymentkWorksheet.Cells[row, 1].Value = payment.Invoice.Orders.FirstOrDefault().Number;

                //Дата
                PaymentkWorksheet.Cells[row, 2].Value = payment.TimestampDataAdd.ToString();

                //Id платежа
                PaymentkWorksheet.Cells[row, 3].Value = payment.Id;

                //Номер транзакции
                PaymentkWorksheet.Cells[row, 4].Value = payment.TxId;

                //Комменатрий
                PaymentkWorksheet.Cells[row, 5].Value = payment.Comment;

                //Адрес счета получателя
                PaymentkWorksheet.Cells[row, 6].Value = payment.Invoice.AccountNumber;

                //Сумма
                PaymentkWorksheet.Cells[row, 7].Value = payment.Summ;

                //Платежная система
                PaymentkWorksheet.Cells[row, 8].Value = payment.Invoice.PaymentType.Name;
            }
        }
    }
}
