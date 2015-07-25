using System;
using System.Data.OleDb;

namespace LoupanCrawler
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

        public int InsertLoupanSummary(string loupanid,string name,int price,string address,string region,string city,string url)
        {
            _cmd.CommandText =
                $"INSERT INTO loupan(ID,LoupanName, Price, Address,Region,City,Url)VALUES({loupanid},'{name}', {price}, '{address}','{region}','{city}','{url}')";

            return _cmd.ExecuteNonQuery();
        }

        public OleDbDataReader GetLoupanSummaryReader(string city)
        {
            _cmd.CommandText =
                $"SELECT * FROM loupan WHERE City = '{city}'";
            return _cmd.ExecuteReader();
        }

        public int InsertHistoryPrice(string loupanID, string date, int maxPrice, int avgPrice, int minPrice,
            string description)
        {
            var cmd = new OleDbCommand(
                $"INSERT INTO HistoryPrice(LoupanID,RecordDate,MaxPrice,AveragePrice,MinPrice,Description) VALUES('{loupanID}','{date}','{maxPrice}','{avgPrice}','{minPrice}','{description.Replace('\'', ' ')}')",
                _conn);
            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch (Exception )
            {
                return -1;
            }
            
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
