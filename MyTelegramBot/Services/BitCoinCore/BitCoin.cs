using System;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace MyTelegramBot.Services.BitCoinCore
{
    class BitCoin : ICryptoCurrency
    {
        private string UserNameRpc { get; set; }

        private string PasswordRpc { get; set; }

        private string HostRpc { get; set; }

        private string PortRpc { get; set; }

        
        public BitCoin(string UserName, string Password, string Host="127.0.0.1", string Port= "8332")
        {
            UserNameRpc = UserName;
            PasswordRpc = Password;
            HostRpc = "http://"+ Host;
            PortRpc = Port;
        }

        public IList<string> GetAddressByAccount(string Account=null)
        {
            try
            {
               return RequestRpcServer<ResultResponseArray>("getaddressesbyaccount", Account).result;
                
            }

            catch
            {
                return null;
            }
        }


        public BlockInfo GetBlockInfo<BlockInfo>(string BlockHash)
        {
            try
            {
                return RequestRpcServer<BlockInfo>("getblock", BlockHash);
            }

            catch (Exception exp)
            {
                return default(BlockInfo);
            }
        }

        public string GetNewAddress(string account = null)
        {
            var address= RequestRpcServer<ResultResponse>("getnewaddress", account);

            if (address != null)
                return address.result;

            else
                return String.Empty;

        }

        public TransactionInfo GetTxnInfo<TransactionInfo>(string TxId)
        {
            try
            {
                 return RequestRpcServer<TransactionInfo>("gettransaction", TxId);

            }

            catch
            {
                return default(TransactionInfo);
            }
        }

        public double GetBalance(string Account)
        {
            try
            {
                var res = RequestRpcServer<ResultResponse>("getreceivedbyaddress", Account);

                if (res != null)
                    return Convert.ToDouble(res.result.Replace('.', ','));

                else
                    return 0.0;
            }

            catch
            {
                return 0.0;
            }
        }

        public List<Listtransactions> GetListTransactions(string Account)
        {
            try
            {
                var list = RequestRpcServer<TransactionInfoList>("listtransactions", "*");
                return list.result.Where(l => l.address == Account).ToList();

            }

            catch
            {
                return null;
            }
        }

        private T RequestRpcServer<T>(string methodName, params string [] parameters)
        {
            string respVal = string.Empty;

            var rawRequest = GetRawRequest();
            JObject joe = new JObject();
            joe.Add(new JProperty("jsonrpc", "1.0"));
            joe.Add(new JProperty("id", "1"));
            joe.Add(new JProperty("method", methodName));

            JArray props = new JArray();

            if (parameters != null && parameters.Length > 0)
                foreach (var parameter in parameters)
                    if(parameter!=null)
                        props.Add(parameter);

            else
                props.Add("");

            joe.Add(new JProperty("params", props));

            // serialize json for the request
            string s = JsonConvert.SerializeObject(joe);
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            rawRequest.ContentLength = byteArray.Length;
            Stream dataStream = rawRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            StreamReader streamReader = null;
            try
            {
                WebResponse webResponse = rawRequest.GetResponse();

                streamReader = new StreamReader(webResponse.GetResponseStream(), true);

                respVal = streamReader.ReadToEnd();
                var data = JsonConvert.DeserializeObject(respVal).ToString();
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception exp)
            {
                return default(T);
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                }

            }
            
        }

        private  HttpWebRequest GetRawRequest()
        {

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(HostRpc+":"+PortRpc);
            webRequest.Credentials = new NetworkCredential(UserNameRpc, PasswordRpc);
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";
            return webRequest;
        }
    }
}
