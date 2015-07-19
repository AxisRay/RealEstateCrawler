using System;
using System.Data.OleDb;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace HouseCrawler
{
    class DBHelper
    {
        private readonly OleDbConnection _conn;
        private readonly OleDbCommand _cmd;

        public DBHelper(string filePath)
        {
            string connstr = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath}";
            _conn = new OleDbConnection(connstr);
            _cmd = new OleDbCommand {Connection = _conn};
        }

        public int InsertLoupanSummary(string name,int price,string address,string region,string city,string url)
        {
            _cmd.CommandText =
                $"INSERT INTO loupan(LoupanName, Price, Address,Region,City,Url)VALUES('{name}', {price}, '{address}','{region}','{city}','{url}')";

            return _cmd.ExecuteNonQuery();
        }

        public OleDbDataReader GetLoupanSummaryReader(string city)
        {
            _cmd.CommandText =
                $"SELECT * FROM loupan WHERE City = '{city}'";
            return _cmd.ExecuteReader();
        }

        public int InsertHistoryPrice(
            string loupanID, 
            string date, 
            int maxPrice, 
            int avgPrice, 
            int minPrice,
            string description)
        {
            var cmd = new OleDbCommand(
                $"INSERT INTO HistoryPrice(LoupanID,RecordDate,MaxPrice,AveragePrice,MinPrice,Description) VALUES('{loupanID}','{date}','{maxPrice}','{avgPrice}','{minPrice}','{description.Replace('\'', ' ')}')",
                _conn);
            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return -1;
            }
            
        }

        public int InsertHouseDetail(JToken house,string loupanid)
        {
            var cmd = new OleDbCommand(
                "INSERT INTO HouseDetail(" +
                "loupanid," +
                "houseid," +
                "dongid," +
                "newcode," +
                "projname," +
                "dongname," +
                "address," +
                "room," +
                "ting," +
                "wei," +
                "chu," +
                "jianzhumianji," +
                "houselocation," +
                "district," +
                "tehui_price," +
                "price_s," +
                "price_s_type," +
                "price_t," +
                "price_t_type) " +
                "VALUES("+
                $"{loupanid}," +
                $"'{house["houseid"].Value<string>()}'," +
                $"'{house["dongid"].Value<string>()}'," +
                $"'{house["newCode"].Value<string>()}'," +
                $"'{house["projname"].Value<string>()}'," +
                $"'{house["dongname"].Value<string>()}'," +
                $"'{house["address"].Value<string>()}'," +
                $"{GetNumbers(house["room"].Value<string>())}," +
                $"{GetNumbers(house["ting"].Value<string>())}," +
                $"{GetNumbers(house["wei"].Value<string>())}," +
                $"{GetNumbers(house["chu"].Value<string>())}," +
                $"{house["jianzhumianji"].Value<string>()}," +
                $"'{house["houselocation"].Value<string>()}'," +
                $"'{house["district"].Value<string>()}'," +
                $"{GetNumbers(house["tehui_price"].Value<string>())}," +
                $"{GetNumbers(house["price_s"].Value<string>())}," +
                $"'{house["Price_s_type"].Value<string>()}'," +
                $"{GetNumbers(house["price_t"].Value<string>())}," +
                $"'{house["Price_t_type"].Value<string>()}')",
                _conn);
            //float price = GetNumbers(house["tehui_price"].Value<string>());

            return cmd.ExecuteNonQuery();
        }
        private float GetNumbers(string priceStr)
        {
            float numbers;
            string replaced = Regex.Replace(priceStr, @"[^\d.]*", String.Empty);
            bool success = float.TryParse(replaced, out numbers);
            if (!success)
                return -1;
            return numbers;
        }
        public int InsertLoupanDetail()
        {
            return -1;
        }

        public void OpenConnection()
        {
            _conn.Open();
        }

        public void CloseConnection()
        {
            _conn.Close();
        }
    }
}
