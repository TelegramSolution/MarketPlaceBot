using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using Newtonsoft.Json;


namespace MyTelegramBot.Sevices.YandexMap
{
    public static  class YandexMapSearch
    {
        public static  YandexMap Search(Coordinates coordinates)
        {
            string res = "";
            //1 долгота , 2 широта
            string[] spl_longitude;
            string[] spl_latitude;
            spl_latitude = coordinates.latitude.ToString().Split(',');
            spl_longitude = coordinates.longitude.ToString().Split(',');
            string url = "https://geocode-maps.yandex.ru/1.x/?format=json&geocode=" + spl_longitude[0]+"."+spl_longitude[1]+","+spl_latitude[0]+"."+spl_latitude[1];

            // Создаём объект WebClient
            using (var webClient = new WebClient())
            {
                webClient.Encoding = System.Text.Encoding.UTF8;
                var response = webClient.DownloadString(url);
                res = response.ToString();
            }
            YandexMap map = JsonConvert.DeserializeObject<YandexMap>(res);
            return map;
        }


    }
}