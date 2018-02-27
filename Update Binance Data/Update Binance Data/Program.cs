using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Update_Binance_Data
{
    class Program
    {
        static DatabaseAccess db = new DatabaseAccess(DatabaseAccess.onlineServer);
        static ReadAPI api = new ReadAPI();
        private static System.Timers.Timer updateTime;
        static void Main(string[] args)
        {
            //getDataAllCoinEveryHour();
            //Console.WriteLine(ConvertToTimestamp(DateTime.Now));
            if (db.checkNewDBOrNot()) getAllCoinData500h();

            updateTime = new System.Timers.Timer();
            updateTime.Elapsed += updateAllCoinData;
            updateTime.Interval = 500;
            updateTime.Enabled = true;
            updateTime.Start();

            Console.WriteLine("Start getting data by hour.");
            Console.WriteLine("Each hour will get data one time.");
            Console.WriteLine("================================");
            while (true) { };
            //Console.ReadLine();
        }

        private static bool updating = false;
        private static void updateAllCoinData(object sender, System.Timers.ElapsedEventArgs e)
        {
            DateTime updateTime = DateTime.Now;
            if (!updating)
            {
                if ((updateTime.Minute == 1) && (updateTime.Second == 0))
                {
                    updating = true;
                    Console.WriteLine("Update Time: " + updateTime.ToLongTimeString());
                    Console.WriteLine("Get data start ...");
                    getDataAllCoinEveryHour();
                    Console.WriteLine("Get data completed.");
                    TimeSpan span = DateTime.Now - updateTime;
                    Console.WriteLine("Time consuming: " + span.TotalSeconds);
                    Console.WriteLine("================================");
                    updating = false;
                }
            }
                  
        }

        static long ConvertToTimestamp(DateTime value)
        {
            TimeZoneInfo NYTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime NyTime = TimeZoneInfo.ConvertTime(value, NYTimeZone);
            TimeZone localZone = TimeZone.CurrentTimeZone;
            System.Globalization.DaylightTime dst = localZone.GetDaylightChanges(NyTime.Year);
            NyTime = NyTime.AddHours(-1);
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            TimeSpan span = (NyTime - epoch);
            return (long)Convert.ToDouble(span.TotalMilliseconds);
        }

        static void getDataAllCoinEveryHour()
        {
            List<Currency> lstBTCMarket = new List<Currency>();
            List<Currency> lstETHMarket = new List<Currency>();
            List<Currency> lstBNBMarket = new List<Currency>();
            List<Currency> lstUSDMarket = new List<Currency>();

            String json24h = api.getExchangeData24h();
            JArray json24hData = JArray.Parse(json24h);

            foreach(JObject jo in json24hData.Children<JObject>())
            {
                Currency24h coin24h = new Currency24h();
                coin24h.symbol = jo.Property("symbol").Value.ToString();
                coin24h.priceChange = Convert.ToSingle(jo.Property("priceChangePercent").Value);
                
                if(coin24h.symbol.Substring((coin24h.symbol.Length - 4)) == "USDT")
                    lstUSDMarket.Add(getCoinDataEveryHour(coin24h));
                else
                {
                    switch (coin24h.symbol.Substring((coin24h.symbol.Length - 3)))
                    {
                        case "BTC":
                            lstBTCMarket.Add(getCoinDataEveryHour(coin24h));
                            break;
                        case "ETH":
                            lstETHMarket.Add(getCoinDataEveryHour(coin24h));
                            break;
                        case "BNB":
                            lstBNBMarket.Add(getCoinDataEveryHour(coin24h));
                            break;
                    }
                }
            }

            db.insertData(DatabaseAccess.BTCTABLE, lstBTCMarket);
            db.insertData(DatabaseAccess.ETHTABLE, lstETHMarket);
            db.insertData(DatabaseAccess.BNBTABLE, lstBNBMarket);
            db.insertData(DatabaseAccess.USDTABLE, lstUSDMarket);
        }

        static Currency getCoinDataEveryHour(Currency24h coin24h)
        {
            Currency coin = new Currency();
            //lấy data của coin theo nến 1h
            coin = (Currency)getCoinDataByTime(coin24h.symbol, "1h", 2).First();
            coin.change24H = coin24h.priceChange;
            return coin;
        }

        static void getAllCoinData500h()
        {
            List<Currency> lstBTCMarket = new List<Currency>();
            List<Currency> lstETHMarket = new List<Currency>();
            List<Currency> lstBNBMarket = new List<Currency>();
            List<Currency> lstUSDMarket = new List<Currency>();

            DateTime updateTime = DateTime.Now;
            Console.WriteLine("Get initial data. Please wait...");
            Console.WriteLine("Start time: " + updateTime.ToLongTimeString());
            Console.WriteLine("Get data from api start ...");
            Console.WriteLine("Get data start time: " + DateTime.Now.ToLongTimeString());


            String json24h = api.getExchangeData24h();
            JArray json24hData = JArray.Parse(json24h);
            
            foreach (JObject jo in json24hData.Children<JObject>())
            {
                string symbol = jo.Property("symbol").Value.ToString();

                if (symbol.Substring((symbol.Length - 4)) == "USDT")
                    lstUSDMarket.AddRange(getCoinDataByTime(symbol, "1h", 500));
                else
                {
                    switch (symbol.Substring((symbol.Length - 3)))
                    {
                        case "BTC":
                            lstBTCMarket.AddRange(getCoinDataByTime(symbol, "1h", 500));
                            break;
                        case "ETH":
                            lstETHMarket.AddRange(getCoinDataByTime(symbol, "1h", 500));
                            break;
                        case "BNB":
                            lstBNBMarket.AddRange(getCoinDataByTime(symbol, "1h", 500));
                            break;
                    }
                }
            }
            Console.WriteLine("Get data from api completed.");
            Console.WriteLine("Get data end time: " + DateTime.Now.ToLongTimeString());
            Console.WriteLine("Insert data to database start ...");
            Console.WriteLine("Insert data start time: " + DateTime.Now.ToLongTimeString());

            db.insertData(DatabaseAccess.BTCTABLE, lstBTCMarket);
            db.insertData(DatabaseAccess.ETHTABLE, lstETHMarket);
            db.insertData(DatabaseAccess.BNBTABLE, lstBNBMarket);
            db.insertData(DatabaseAccess.USDTABLE, lstUSDMarket);

            Console.WriteLine("Insert data to database completed.");
            Console.WriteLine("Insert data end time: " + DateTime.Now.ToLongTimeString());
            Console.WriteLine("Get initial data completed.");
            Console.WriteLine("End time: " + DateTime.Now.ToLongTimeString());
            TimeSpan span = DateTime.Now - updateTime;
            Console.WriteLine("Time consuming: " + span.TotalSeconds);
            Console.WriteLine("================================");
        }

        static List<Currency> getCoinDataByTime(string symbol, string interval, int limit, long startTime = 0, long endTime=0)
        {
            List<Currency> lst = new List<Currency>();
            //lấy data của coin theo nến 1h
            String jsonbByTime = api.getCoinDetailCandle(symbol,interval,limit,startTime,endTime);
            JArray jsonDataByTime = JArray.Parse(jsonbByTime);

            foreach(JArray arr in jsonDataByTime.Children())
            {
                Currency coin = new Currency();
                coin.market = symbol;
                coin.openTime = Convert.ToUInt64(arr[0]);
                coin.openPrice = Convert.ToSingle(arr[1].ToString());
                coin.highPrice = Convert.ToSingle(arr[2].ToString());
                coin.lowPrice = Convert.ToSingle(arr[3].ToString());
                coin.closePrice = Convert.ToSingle(arr[4].ToString());
                coin.volume = Convert.ToSingle(arr[5].ToString());
                coin.closeTime = Convert.ToUInt64(arr[6]);
                coin.quoteAssetVolume = Convert.ToSingle(arr[7].ToString());
                coin.numberOfTrades = Convert.ToInt32(arr[8]);
                coin.takerBuyBaseAssetVolume = Convert.ToSingle(arr[9].ToString());
                coin.takerBuyQuoteAssetVolume = Convert.ToSingle(arr[10].ToString());
                lst.Add(coin);
            }
            lst.RemoveAt(lst.Count - 1);
            return lst;
        }
    }
}
