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
        private static string City = "西安";
        static void Main(string[] args)
        {
            HouseDBDataContext db = new HouseDBDataContext();
            var loupanlist =
                from loupan in db.LoupanSummary
                where loupan.City == City
                select new
                {
                    loupan.ID,
                    loupan.Url
                };

            ManualResetEvent eventX = new ManualResetEvent(false);
            ThreadPool.SetMaxThreads(10, 10);
            Crawler crawler = new Crawler(eventX);
            foreach (var loupan in loupanlist)
            {                
                HouseParser parser = new HouseParser(
                    loupan.ID,
                    loupan.Url, 
                    City);
                Crawler.iMaxCount++;
                ThreadPool.QueueUserWorkItem(crawler.CrawlerStart, parser);
            }
            Console.WriteLine("主线程等待中……");
            eventX.WaitOne(Timeout.Infinite, true);
            Console.WriteLine("任务完成！");
            Console.ReadLine();
        }
    }

    class Crawler
    {
        private ManualResetEvent _manualEvent;

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

            Console.WriteLine("URL:{0}", parser.LoupanUrl);

            while (parser.RecordHouseDetail())
            {
                Console.WriteLine("Page：{0}/{1}", parser.CurrentPage, parser.TotalPage);
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

