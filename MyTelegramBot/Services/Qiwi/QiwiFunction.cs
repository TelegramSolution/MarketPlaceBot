using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QiwiApiSharp;
using System.Net.Http;
using System.Text;
using QiwiApiSharp.Enumerations;

namespace MyTelegramBot.Services.Qiwi
{
    public class QiwiFunction
    {
        private const string API_ENDPOINT = "edge.qiwi.com";

        public static bool Initialized { get; private set; }

        private static string _token;
        private static HttpClient _httpClient;


        public static async Task<PaymentHistory.DataItem> SearchPayment (string Comment, string token, string telephone)
        {

            try
            {
                QiwiApiSharp.Enumerations.Source[] source = new QiwiApiSharp.Enumerations.Source[1];
                source[0] = QiwiApiSharp.Enumerations.Source.QW_RUB;

                Initialize(token);
                var history = await PaymentHistoryAsync(telephone, 10, Operation.IN, source);
                var payment= history.data.Where(h => h.comment == Comment).FirstOrDefault();
                return payment;
            }

            catch
            {
                return null;
            }
        }

        public static void GetTransaction(int TxtId)
        {

        }

        private static void Initialize(string token)
        {
            _token = token;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://" + API_ENDPOINT + "/");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);
            Initialized = true;
        }

        private static async Task<HttpResponseMessage> ApiCallAsync(string request, Dictionary<string, object> query)
        {
            string queryString = null;
            if (query != null && query.Count > 0)
            {
                var queryBuilder = new StringBuilder();
                foreach (var param in query)
                {
                    queryBuilder.Append(param.Key);
                    queryBuilder.Append("=");
                    queryBuilder.Append(param.Value);
                    queryBuilder.Append("&");
                }
                queryString = queryBuilder.ToString();
            }
            queryString = string.IsNullOrEmpty(queryString) ? "" : "?" + queryString;
            return await _httpClient.GetAsync(request + queryString);
        }

        public static async Task<bool> TestConnection(string PhoneNumber, string ApiKey)
        {
            try
            {
                QiwiApiSharp.QiwiApi.Initialize(ApiKey);

                var user = await QiwiApiSharp.QiwiApi.UserProfileAsync();

                if (user.authInfo.personId == Convert.ToInt64(PhoneNumber))
                    return true;

                else
                    return false;
            }

            catch
            {
                return false;
            }
        }

        private static async Task<PaymentHistory> PaymentHistoryAsync(string walletId, int rows, Operation operation, Source[] sources, DateTime? startDate = null, DateTime? endDate = null, DateTime? nextTxnDate = null, long? nextTxnId = null)
        {

            var query = new Dictionary<string, object>
            {
                {"rows", rows },
                {"operation", operation.ToString() }
            };

            for (int i = 0; i < sources.Length; i++)
                query.Add("source[" + i + "]", sources[i].ToString());

            if (startDate != null && endDate != null)
            {
                query.Add("startDate", startDate.Value.ToString("s") + "Z");
                query.Add("endDate", endDate.Value.ToString("s") + "Z");
            }

            if (nextTxnDate != null && nextTxnId != null)
            {
                query.Add("nextTxnDate", nextTxnDate.Value.ToString("s") + "Z");
                query.Add("nextTxnId", nextTxnId);
            }

            var response = await ApiCallAsync("payment-history/v1/persons/" + walletId + "/payments", query);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentHistory>(data);
               
            }


            response.EnsureSuccessStatusCode();
            return null;
        }
    }
}
