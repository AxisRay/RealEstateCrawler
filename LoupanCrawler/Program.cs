using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace LoupanCrawler
{
    static class Program
    {
        private static bool _finish = false;
        private static string DBPath = @"C:\Users\Ray\Documents\realEstate.accdb";
        private static string RootUrl = "http://newhouse.fang.com/house/s/list/";
        private static string _currentCity;
        private static string _currentUrl;
        private static Dictionary<string, string> GetCitiesList(string root)
        {
            var cities = new Dictionary<string, string>();
            var request = new HttpHelper(root);
            var response = request.GetResponseStream();
            var doc = GetHtmlDoc(response);
            var citiesNodes=doc.SelectNodes("/html[1]/body[1]/div[1]/div[1]/div[1]/div[2]/div[2]/div[2]/a");
            foreach (var cityNode in citiesNodes)
            {
                var href = cityNode.Attributes["href"].Value;
                if (href.StartsWith("http://newhouse"))
                {
                    string city = cityNode.InnerText;
                    cities.Add(city,href);
                    Console.WriteLine(city+"    "+href);
                }
            }
            return cities;
        }

        static void Main(string[] args)
        {
            var helper = new DBHelper(DBPath);
            var citiesList =  GetCitiesList(RootUrl);
            helper.OpenConnection();
            foreach (var item in citiesList)
            {
                if (item.Key != "西安")
                {//东莞
                    CityLoupanSummary(item, helper);
                    CatchLoupanPriceHistory(item.Key, helper);
                }

            }
            //CatchLoupanPriceHistory("西安",helper);
            //CatchLoupanDetail("西安", helper);
            helper.CloseConnection();
        }

        private static void CatchLoupanDetail(string city, DBHelper helper)
        {
            var reader = helper.GetLoupanSummaryReader(city);
            while (reader.Read())
            {
                string url = reader["Url"].ToString();
                string LoupanID = reader["ID"].ToString();
                HtmlNode detailNode = LoupanDetailPage(url);
            }
            reader.Close();
        }

        private static void RecordLoupanDetail(string url,DBHelper helper)
        {
            
        }

        private static void CatchLoupanPriceHistory(string city,DBHelper helper)
        {
            var reader = helper.GetLoupanSummaryReader(city);
            while (reader.Read())
            {   
                string url = reader["Url"].ToString();
                string LoupanID = reader["ID"].ToString();
                HtmlNode detailNode = LoupanDetailPage(url);
                RecordHistoryPrice(LoupanID,detailNode,helper);
            }
            reader.Close();
        }

        private static HtmlNode LoupanDetailPage(string loupanPage)
        {
            //loupanPage = "http://hailanchenghy.fang.com/";
            var request = new HttpHelper(loupanPage);

            var responseStream = request.GetResponseStream();
            var doc = GetHtmlDoc(responseStream);
            responseStream.Close();
            HtmlNode loupanDetailNode;
            try
            {
                loupanDetailNode = doc
                    .OwnerDocument
                    .GetElementbyId("orginalNaviBox")
                    .SelectNodes("./a")[2];
            }
            catch (Exception e)
            {
                return null;
            }
            if (loupanDetailNode.InnerText != "楼盘详情")
                return null;

            string loupanDetailUrl = loupanDetailNode.Attributes["href"].Value;

            request = new HttpHelper(loupanDetailUrl);

            responseStream = request.GetResponseStream();
            doc = GetHtmlDoc(responseStream);
            responseStream.Close();
            return doc;

        }

        private static void RecordHistoryPrice(string loupanID,HtmlNode loupanDetialNode,DBHelper helper)
        {
            if (loupanDetialNode == null)
                return;
            HtmlNodeCollection historyPriceList = loupanDetialNode
                .OwnerDocument
                .GetElementbyId("priceListOpen")
                .SelectNodes("./table[1]/tr");
            historyPriceList.RemoveAt(0);

            foreach (var node in historyPriceList)
            {
                string date = node
                    .SelectSingleNode("./td[1]")
                    .InnerText
                    .Replace("-","/");
                int MaxPrice = node
                    .SelectSingleNode("./td[2]")
                    .InnerText
                    .GetPrice();
                int AvgPrice = node
                    .SelectSingleNode("./td[3]")
                    .InnerText
                    .GetPrice();
                int MinPrice = node
                    .SelectSingleNode("./td[4]")
                    .InnerText
                    .GetPrice();
                string Description = node
                    .SelectSingleNode("./td[5]")
                    .InnerText
                    .Replace("&nbsp;",String.Empty);
                int count = helper.InsertHistoryPrice(loupanID, date, MaxPrice, AvgPrice, MinPrice, Description);
                Console.WriteLine($"{loupanID},{date},{AvgPrice},{MinPrice},{MaxPrice}");
                Console.WriteLine(count > 0 ? "Success!" : "Failed!");
            }
        }

        private static int GetPrice(this String priceStr)
        {
            int price;
            string replaced = Regex.Replace(priceStr, @"[^\d]*", String.Empty);
            bool success = int.TryParse(replaced, out price);
            if (!success)
                return -1;
            return price;
        }

        private static void CityLoupanSummary(KeyValuePair<string, string> item, DBHelper helper)
        {
            _currentCity = item.Key;
            _currentUrl = item.Value;
            _finish = false;
            for (int i = 1; i < 100; i++)
            {
                string currentPage = _currentUrl + "b9" + i.ToString();
                Console.WriteLine(currentPage);

                var request = new HttpHelper(currentPage);

                var responseStream = request.GetResponseStream();
                var doc = GetHtmlDoc(responseStream);
                responseStream.Close();

                var loupanList = GetLoupanList(doc);
                LoupanListParse(loupanList, helper);
                if (_finish)
                    break;
            }
        }

        private static void LoupanListParse(HtmlNodeCollection loupan,DBHelper helper)
        {
            foreach (var node in loupan)
            {
                if(!node.Id.StartsWith("loupan"))
                    continue;
                try
                {
                    Record(node, helper);
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine("Finish!");
                    _finish = true;
                    break;
                }
                
            }
        }

        private static void Record(HtmlNode loupanNode,DBHelper helper)
        {
            string name = GetLoupanName(loupanNode);
            int price = GetLoupanPrice(loupanNode);
            if(price < 1000)
                return;
            string address = GetAddress(loupanNode);
            string region = GetRegion(loupanNode);
            string url = GetLoupanUrl(loupanNode);
            Console.WriteLine("{0},{1},{2},{3}",name,price,address,region);

            int count=helper.InsertLoupanSummary(name, price, address, region, _currentCity,url);
            Console.WriteLine(count > 0 ? "Success!" : "Fail!");
        }

        private static HtmlNode GetHtmlDoc(Stream responseStream)
        {
            HtmlDocument doc =new HtmlDocument();
            doc.Load(responseStream,Encoding.GetEncoding("GB2312"));
            return doc.DocumentNode;
        }

        private static HtmlNodeCollection GetLoupanList(HtmlNode loupanNode)
        {
            HtmlNodeCollection list = loupanNode.SelectNodes("/html[1]/body[1]/div[3]/div[2]/div[1]/div");
            return list;
        }

        private static string GetLoupanName(HtmlNode loupanNode)
        {
            var nameNode = loupanNode.SelectSingleNode("./dl[1]/dt[1]/dd[1]/div[1]/h4[1]/a[1]");
            return nameNode.InnerText;
        }

        private static string GetLoupanUrl(HtmlNode loupanNode)
        {
            var nameNode = loupanNode.SelectSingleNode("./dl[1]/dt[1]/dd[1]/div[1]/h4[1]/a[1]");
            return nameNode.Attributes["href"].Value;
        }

        private static int GetLoupanPrice(HtmlNode loupanNode)
        {
            bool flag = false;
            int price = 0;
            var priceNodeCollections = loupanNode.SelectNodes("./dl[1]/dt[1]/dd[1]/div");
            foreach (var node in priceNodeCollections)
            {
                if (node.Attributes["class"] == null)
                    continue;
                string nodeValue = node.Attributes["class"].Value;
                if (nodeValue == "fr")
                {
                    var priceNode = node.SelectSingleNode("./h5[1]/span[1]");
                    flag = int.TryParse(priceNode.InnerText, out price);
                    break;
                }
            }
            if (!flag)
                return -1;
            return price;
        }

        private static string GetAddress(HtmlNode loupanNode)
        {
            var addressNode = loupanNode.SelectSingleNode("./dl[1]/dt[1]/dd[3]/div[1]/a[1]");
            return addressNode.Attributes["title"].Value;
        }

        private static string GetRegion(HtmlNode loupanNode)
        {
            string address = GetAddress(loupanNode);
            string[] splited = address.Split(new[] {'[', ']', ' '}, StringSplitOptions.RemoveEmptyEntries);
            if (splited.Length < 2)
            {
                return String.Empty;
            }
            return splited.First();
        }



    }
}
