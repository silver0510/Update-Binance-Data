using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySql.Data;

namespace Update_Binance_Data
{
    public class DatabaseAccess
    {
        //dữ liệu hiện tại là server của thèng Thái, của mình là phần bị comment
        public static ServerInfo onlineServer = new ServerInfo
        {
            SERVER = "128.199.233.99",
            DATABASE = "binance",
            UID = "binance",
            PASSWORD = "binance##",
        };
        public static ServerInfo localServer = new ServerInfo
        {
            SERVER = "localhost",
            DATABASE = "binance_data",
            UID = "root",
            PASSWORD = "",
        };

        public static string BTCTABLE = "BTCMARKET";
        public static string ETHTABLE = "ETHMARKET";
        public static string BNBTABLE = "BNBMARKET";
        public static string USDTABLE = "USDMARKET";

        string connetionString;
        MySqlConnection con = null;
        private ServerInfo server;

        public DatabaseAccess(ServerInfo sv)
        {
            server = sv;
            connetionString = "server=" + server.SERVER + ";database=" + server.DATABASE + ";uid=" + server.UID + ";pwd=" + server.PASSWORD + ";";
        }


        public void DBConnect()
        {
            try
            {
                if (con == null)
                    con = new MySqlConnection(connetionString);
                if (con.State == System.Data.ConnectionState.Closed)
                    con.Open();
                //Console.WriteLine("Connected");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }   
        }

        public void DBClose()
        {
            try
            {
                if (con == null) return;
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void insertData(string tableName ,List<Currency> lst)
        {
            StringBuilder sCommand = new StringBuilder("INSERT INTO " + tableName +
             "(MARKET,OPENTIME,OPENPRICE,HIGHPRICE,LOWPRICE,CLOSEPRICE,VOLUME,CLOSETIME,QUOTE_ASSET_VOLUME,NUMBER_OF_TRADES,TAKER_BUY_BASE_ASSET_VOLUME,TAKER_BUY_QUOTE_ASSET_VOLUME,IGNORE_DATA,CHANGE24H) VALUES ");
            List<string> Rows = new List<string>();
            foreach (Currency coin in lst)
            {
                Rows.Add(string.Format("('{0}',{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13})", 
                                        coin.market,
                                        coin.openTime,
                                        coin.openPrice,
                                        coin.highPrice,
                                        coin.lowPrice,
                                        coin.closePrice,
                                        coin.volume,
                                        coin.closeTime,
                                        coin.quoteAssetVolume,
                                        coin.numberOfTrades,
                                        coin.takerBuyBaseAssetVolume,
                                        coin.takerBuyQuoteAssetVolume,
                                        coin.ignoreData,
                                        coin.change24H));
            }
            sCommand.Append(string.Join(",", Rows));
            sCommand.Append(";");
            DBConnect();
            using (MySqlCommand mySqlCmd = new MySqlCommand(sCommand.ToString(), con))
            {
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                mySqlCmd.ExecuteNonQuery();
            }
            DBClose();

        }

        //kiểm tra đây có phải lần đầu tiên chạy chương trình không
        //nếu đúng thì trả về true
        public bool checkNewDBOrNot()
        {
            DBConnect();
            MySqlCommand mySqlCmd = new MySqlCommand();
            mySqlCmd.Connection = con;
            mySqlCmd.CommandType = System.Data.CommandType.Text;
            string cmd = "SELECT COUNT(*) FROM " + BTCTABLE;
            mySqlCmd.CommandText = cmd;
            var obj = mySqlCmd.ExecuteScalar();
            int rowcount = Convert.ToInt32(obj);

            if (rowcount == 0) return true;
            DBClose();
            return false;
        }

        //lấy thời gian cuối cùng update
        //nếu đúng thì trả về true
        public long getLastTimeUpdate()
        {
            long ret = 0;

            DBConnect();
            MySqlCommand mySqlCmd = new MySqlCommand();
            mySqlCmd.Connection = con;
            mySqlCmd.CommandType = System.Data.CommandType.Text;
            string cmd = "SELECT * FROM " + BTCTABLE + " ORDER BY ID DESC LIMIT 1 ";
            mySqlCmd.CommandText = cmd;

            MySqlDataReader mySqlReader = mySqlCmd.ExecuteReader();
            if (mySqlReader.Read())
            {
                ret = mySqlReader.GetInt64("CLOSETIME");
            }

            DBClose();
            return ret;
        }
    }
}
