using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateCrawler
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

        public int InsertHistoryPrice(string loupanID, string date, int maxPrice, int avgPrice, int minPrice,
            string description)
        {
            var cmd = new OleDbCommand(
                $"INSERT INTO HistoryPrice(LoupanID,RecordDate,MaxPrice,AveragePrice,MinPrice,Description) VALUES('{loupanID}','{date}','{maxPrice}','{avgPrice}','{minPrice}','{description}')",
                _conn);   
            return cmd.ExecuteNonQuery();
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
