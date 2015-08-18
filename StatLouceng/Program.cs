using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;
using System.Reflection;

namespace StatLouceng
{
    class Dong
    {
        public string dongid;
        public string newcode;
        public string houseid;
        public string houseurl;
    }
    class Program
    {
        
        static void Main(string[] args)
        {
            HouseDBDataContext db = new HouseDBDataContext();
            var dongList = from house in db.HouseDetail
                           group house by house.dongid into dong
                           select new Dong
                           {
                               dongid = dong.Key,
                               newcode = dong.First().newcode,
                               houseid = dong.First().houseid,
                               houseurl = dong.First().tehui_url
                           };

            //ManualResetEvent eventX = new ManualResetEvent(false);
            //ThreadPool.SetMaxThreads(10, 10);
            //Crawler crawler = new Crawler(eventX);
            //Crawler.iMaxCount = dongList.Count();

            foreach (var dong in dongList)
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(dong.houseurl);
                request.Timeout=10000;
                Console.WriteLine(dong.houseurl);
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    if (response.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                    }
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("GB2312"));
                    string html = reader.ReadToEnd();
                    Match match = Regex.Match(html, @"(?<=共)[0-9]+(?=层)");
                    if (match.Success)
                    {
                        Console.WriteLine("{0},{1}", dong.dongid, match.Value);
                        db.louceng.InsertOnSubmit(new louceng()
                        {
                            dongid = dong.dongid,
                            louceng1 = match.Value
                        });
                    }
                }
                catch(WebException e)
                {
                    Console.WriteLine(e.Message);
                }


            }
            db.SubmitChanges();

            //Console.WriteLine("主线程等待中……");
            //eventX.WaitOne(Timeout.Infinite, true);
            //Console.WriteLine("任务完成！");
            //Console.ReadLine();
        }
    }
    class Crawler
    {
        private ManualResetEvent _manualEvent;
        public HouseDBDataContext db = new HouseDBDataContext();
        private static int iCount;
        public static int iMaxCount = 0;

        public Crawler(ManualResetEvent manualEvent)
        {
            _manualEvent = manualEvent;
            iCount = 0;
        }
        public void CrawlerStart(object p)
        {
            var dong = (Dong)p;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(dong.houseurl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            if (response.ContentEncoding.ToLower().Contains("gzip"))
            {
                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
            }
            StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("GB2312"));
            string html = reader.ReadToEnd();
            Match match = Regex.Match(html, @"(?<=共)[0-9]+(?=层)");
            if(match.Success)
            {
                Console.WriteLine("{0},{1}", dong.dongid, match.Value);
                db.louceng.InsertOnSubmit(new louceng() {
                    dongid = dong.dongid,
                    louceng1 = match.Value
                });
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
