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
using System.Threading;
using HtmlAgilityPack;

namespace LoupanCrawler
{

    static class Program
    {
        private static RealEstateDBDataContext db;
        private static bool _finish = false;
        private static string RootUrl = "http://newhouse.fang.com/";
        private static string _currentCity;
        private static string _currentUrl;
        

        static void Main(string[] args)
        {

            var citiesList =  GetCitiesList(RootUrl);
            Crawler.iMaxCount = citiesList.Count - 2;
            ManualResetEvent eventX=new ManualResetEvent(false);
            ThreadPool.SetMaxThreads(5, 5);
            foreach (var item in citiesList)
            {
                if (item.Key=="北京"||item.Key=="上海")
                {
                    continue;
                }
                Crawler crawler=new Crawler(eventX,item);
                ThreadPool.QueueUserWorkItem(crawler.CrawlerStart);
            }
            Console.WriteLine("主线程等待中……");
            eventX.WaitOne(Timeout.Infinite, true);
            Console.WriteLine("任务完成！");
            Console.ReadLine();
        }

        /// <summary>
        /// 获取热门城市列表
        /// </summary>
        /// <param name="root">楼盘主页</param>
        /// <returns></returns>
        private static Dictionary<string, string> GetCitiesList(string root)
        {
            var cities = new Dictionary<string, string>();
            var request = new HttpHelper(root);
            var response = request.GetResponseStream();
            var doc = new HtmlDocument();
            doc.Load(response,Encoding.GetEncoding("GB2312"));
            var rootNode = doc.DocumentNode;
            var citiesNodes= rootNode.SelectNodes("/html[1]/body[1]/div[1]/div[1]/div[1]/div[2]/div[2]/div[2]/a");
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


    }
}
