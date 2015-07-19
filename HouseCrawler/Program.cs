using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HouseCrawler
{
    class Program
    {
        private static string RootUrl = "http://newhouse.xian.fang.com/house/s/list/";//+"b810-b9{n}-c9y/";
        private static string DBFilePath = @"C:\Users\Ray\Documents\realEstate.accdb";
        private static string City = "西安";
        static void Main(string[] args)
        {
            ManualResetEvent eventX=new ManualResetEvent(false);
            ThreadPool.SetMaxThreads(10,10);
            DBHelper helper =new DBHelper(DBFilePath);
            Crawler crawler = new Crawler(eventX);
            helper.OpenConnection();
            var reader = helper.GetLoupanSummaryReader(City);
            while (reader.Read())
            {
                HouseParser parser = new HouseParser(
                    reader["ID"].ToString(),
                    reader["Url"].ToString(), 
                    City,
                    helper);
                Crawler.iMaxCount++;
                ThreadPool.QueueUserWorkItem(crawler.CrawlerStart, parser);
            }
            reader.Close();
            Console.WriteLine("主线程等待中……");
            eventX.WaitOne(Timeout.Infinite, true);
            Console.WriteLine("任务完成！");
            Console.ReadLine();
        }

        private static void RegexTest()
        {
            string test = "http://hanhuacheng.fang.com/2/house/3611048298/housedetail.htm";
            Console.WriteLine(Regex.Match(test, @"((?<=/)|(?<=/))[\d]{10}"));
        }

        private static void testPost()
        {
            Random rand = new Random();
            string url = "http://huazhoucheng.fang.com/house/ajaxrequest/dongList2015.php?t="+ rand.NextDouble();
            Dictionary<string,string> parameters = new Dictionary<string, string>();
            parameters.Add("newcode", "3611048298");
            parameters.Add("pageindex", "1");
            parameters.Add("ju", "");
            parameters.Add("dong","");
            parameters.Add("louc","");
            parameters.Add("saling","");
            parameters.Add("city","西安");
            HttpWebResponse response = HttpHelper.CreatePostHttpResponse(url, parameters);
            Stream responseStream = response.GetDecompressedStream();
            StreamReader reader = new StreamReader(responseStream,Encoding.UTF8);
            string responseStr = reader.ReadToEnd();

            DBHelper helper = new DBHelper(DBFilePath);
            helper.OpenConnection();

            JObject responseJson = JObject.Parse(responseStr);
            foreach (var house in responseJson["list"])
            {
                helper.InsertHouseDetail(house, "123123");
            }
        }

    }

    class Crawler
    {
        private ManualResetEvent _manualEvent;

        private DBHelper _helper;

        private static int iCount;
        public static int iMaxCount = 0;

        public Crawler(ManualResetEvent manualEvent)
        {
            _manualEvent = manualEvent;
            iCount = 0;
        }
        public void CrawlerStart(object p)
        {
            var parser = p as HouseParser;
            _manualEvent.Reset();

            Console.WriteLine("URL:{0}",parser.LoupanUrl);

            while(parser.RecordHouseDetail())
            {
                Console.WriteLine("Page：{0}/{1}",parser.CurrentPage,parser.TotalPage);
            }

            Interlocked.Increment(ref iCount);
            if (iCount == iMaxCount)
            {
                Console.WriteLine("发出结束信号!");
                //将事件状态设置为终止状态，允许一个或多个等待线程继续。
                _manualEvent.Set();
            }
        }

    }
}

