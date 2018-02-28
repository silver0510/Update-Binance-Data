using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;


namespace Update_Binance_Data
{
    public class ReadAPI
    {
        //symbol : ký hiệu cặp coin muốn lấy. vd: ethbtc
        //interval: lấy dữ liệu theo nến gì?. vd: 15m, 30m, 1h, 2h, 4h...
        //limit: số lượng nến muốn lấy
        //startTime: thời gian bắt đầu lấy dữ liệu. nếu để trống thì lấy đến thời điểm hiện tại
        //endTime: thời kết thúc lấy dữ liệu. nếu để trống thì lấy đến thời điểm hiện tại
        public string getCoinDetailCandle(string symbol,string interval = "1h",int limit = 500,long startTime = 0, long endTime = 0)
        {
            string url = @"https://api.binance.com/api/v1/klines?symbol=" + symbol.ToUpper() + "&interval=" + interval + "&limit=" + limit.ToString();
            if ((startTime != 0)&&(endTime != 0))
            {
                url += "&startTime=" + startTime.ToString() + "&endTime=" + endTime.ToString();
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    Console.WriteLine(ex.ToString());
                }
                throw;
            }
        }

        //Lấy thông tin trong vòng 24h của 1 coin
        public string getCoinDetail24h(string symbol)
        {
            string url = @"https://api.binance.com/api/v1/ticker/24hr?symbol=" + symbol.ToUpper();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    Console.WriteLine(ex.ToString());
                }
                throw;
            }
        }

        //Lấy thông tin của tất cả các coin của sàn binance trong 24h
        public string getExchangeData24h()
        {
            string url = @"https://api.binance.com/api/v1/ticker/24hr";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    Console.WriteLine(ex.ToString());
                }
                throw;
            }
        }
    }
}
