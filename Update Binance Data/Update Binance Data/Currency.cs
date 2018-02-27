using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Update_Binance_Data
{
    public class Currency
    {
        public int ID { get; set; }
        public string market { get; set; }
        public UInt64 openTime { get; set; }
        public float openPrice { get; set; }
        public float highPrice { get; set; }
        public float lowPrice { get; set; }
        public float closePrice { get; set; }
        public float volume { get; set; }
        public UInt64 closeTime { get; set; }
        public float quoteAssetVolume { get; set; }
        public int numberOfTrades { get; set; }
        public float takerBuyBaseAssetVolume { get; set; }
        public float takerBuyQuoteAssetVolume { get; set; }       
        public float ignoreData { get; set; }
        public float change24H { get; set; }
    }
}
