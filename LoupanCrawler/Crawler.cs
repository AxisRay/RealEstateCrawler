using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace LoupanCrawler
{
    class Crawler
    {
        private ManualResetEvent _manualEvent;
        private RealEstateDBDataContext db;

        private static int iCount;
        public static int iMaxCount = 0;

        private string currentCity;
        private string currentUrl;
        private bool finish = false;

        public Crawler(ManualResetEvent manualEvent,KeyValuePair<string,string> city)
        {
            currentCity = city.Key;
            currentUrl = city.Value;
            db =new RealEstateDBDataContext();
            _manualEvent = manualEvent;
            iCount = 0;
        }
        public void CrawlerStart(object state)
        {
            _manualEvent.Reset();

            CatchCityLoupanSummary();
            CatchLoupanPriceHistory(currentCity);

            Interlocked.Increment(ref iCount);
            if (iCount == iMaxCount)
            {
                Console.WriteLine("发出结束信号!");
                //将事件状态设置为终止状态，允许一个或多个等待线程继续。
                _manualEvent.Set();
            }
        }

        /// <summary>
        /// 抓取制定城市楼盘历史价格信息
        /// </summary>
        /// <param name="city">城市名</param>
        private  void CatchLoupanPriceHistory(string city)
        {
            var loupanList =
                from loupan in db.LoupanSummary
                where loupan.City == city
                select new
                {
                    loupan.ID,
                    loupan.Url
                };

            foreach (var loupan in loupanList)
            {
                string url = loupan.Url;
                string LoupanID = loupan.ID.ToString();
                HtmlNode detailNode = LoupanDetailPage(url);
                RecordHistoryPrice(LoupanID, detailNode);
                db.SubmitChanges();
            }
        }

        /// <summary>
        /// 获取“楼盘详情”页面
        /// </summary>
        /// <param name="loupanPage">楼盘主页地址</param>
        /// <returns></returns>
        private  HtmlNode LoupanDetailPage(string loupanPage)
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
            catch (Exception)
            {
                return null;
            }
            if (loupanDetailNode.InnerText != "楼盘详情")
                return null;

            string loupanDetailUrl = loupanDetailNode.Attributes["href"].Value;

            request = new HttpHelper(loupanDetailUrl);

            responseStream = request.GetResponseStream();
            if (responseStream == null)
            {
                return null;
            }
            doc = GetHtmlDoc(responseStream);
            responseStream.Close();
            return doc;

        }

        /// <summary>
        /// 记录指定楼盘的历史价格
        /// </summary>
        /// <param name="loupanID">楼盘ID</param>
        /// <param name="loupanDetialNode">“楼盘详情”页面</param>
        private void RecordHistoryPrice(string loupanID, HtmlNode loupanDetialNode)
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
                string date = node.SelectSingleNode("./td[1]").InnerText;
                if (date.StartsWith("0000"))
                    return;
                int MaxPrice = GetPrice(node.SelectSingleNode("./td[2]").InnerText);
                int AvgPrice = GetPrice(node.SelectSingleNode("./td[3]").InnerText);
                int MinPrice = GetPrice(node.SelectSingleNode("./td[4]").InnerText);
                string Description = node.SelectSingleNode("./td[5]").InnerText.Replace("&nbsp;", String.Empty);

                HistoryPrice loupanPrice = new HistoryPrice()
                {
                    LoupanID = loupanID,
                    RecordDate = DateTime.ParseExact(date, "yyyy-MM-dd", DateTimeFormatInfo.CurrentInfo),
                    MaxPrice = MaxPrice,
                    AveragePrice = AvgPrice,
                    MinPrice = MinPrice,
                    Description = Description
                };
                db.HistoryPrice.InsertOnSubmit(loupanPrice);
                Console.WriteLine($"{loupanID},{date},{AvgPrice},{MinPrice},{MaxPrice}");
            }
        }

        /// <summary>
        /// 从字符串总提取价格
        /// </summary>
        /// <param name="priceStr">含有价格的字符串</param>
        /// <returns></returns>
        private int GetPrice(string priceStr)
        {
            int price;
            string replaced = Regex.Replace(priceStr, @"[^\d]*", String.Empty);
            bool success = int.TryParse(replaced, out price);
            if (!success)
                return -1;
            return price;
        }

        /// <summary>
        /// 抓取楼指定城市楼盘列表
        /// </summary>
        /// <param name="item"></param>
        private void CatchCityLoupanSummary()
        {
            finish = false;
            for (int i = 1; i < 100; i++)
            {
                string currentPage = currentUrl + "house/s/b9" + i.ToString();
                Console.WriteLine(currentPage);

                var request = new HttpHelper(currentPage);

                var responseStream = request.GetResponseStream();
                var doc = GetHtmlDoc(responseStream);
                responseStream.Close();

                var loupanList = GetLoupanList(doc);
                LoupanListParse(loupanList);
                db.SubmitChanges();
                if (finish)
                    break;
            }

        }

        /// <summary>
        /// 抓取页面中楼盘列表
        /// </summary>
        /// <param name="loupan">楼盘列表节点</param>
        private void LoupanListParse(HtmlNodeCollection loupan)
        {
            foreach (var node in loupan)
            {
                if (!node.Id.StartsWith("loupan"))
                    continue;
                try
                {
                    LoupanRecord(node);
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("Finish!");
                    finish = true;
                    break;
                }

            }

        }

        /// <summary>
        /// 记录楼盘信息
        /// </summary>
        /// <param name="loupanNode">楼盘节点</param>
        private void LoupanRecord(HtmlNode loupanNode)
        {
            string loupanid = GetLoupanID(loupanNode);
            string name = GetLoupanName(loupanNode);
            int price = GetLoupanPrice(loupanNode);
            if (price < 1000)
                return;
            string address = GetAddress(loupanNode);
            string region = GetRegion(loupanNode);
            string url = GetLoupanUrl(loupanNode);
            Console.WriteLine("{0},{1},{2},{3}", name, price, address, region);

            int? rtn=-1;
            db.sp_Insert_Loupan(loupanid, name, price, address, region, currentCity, url,ref rtn);
            if (rtn == 0)
            {
                Console.WriteLine("success!");
            }
        }

        /// <summary>
        /// 获取楼盘ID（newcode）
        /// </summary>
        /// <param name="loupanNode">楼盘节点</param>
        /// <returns></returns>
        private static string GetLoupanID(HtmlNode loupanNode)
        {
            return loupanNode.Id.Split('_').Last();
        }

        /// <summary>
        /// 获取页面根节点
        /// </summary>
        /// <param name="responseStream">响应流</param>
        /// <returns></returns>
        private static HtmlNode GetHtmlDoc(Stream responseStream)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(responseStream, Encoding.GetEncoding("GB2312"));
            return doc.DocumentNode;
        }

        /// <summary>
        /// 获取页面中楼盘列表区域
        /// </summary>
        /// <param name="loupanNode">楼盘页面</param>
        /// <returns></returns>
        private static HtmlNodeCollection GetLoupanList(HtmlNode loupanNode)
        {
            var node = loupanNode.OwnerDocument.GetElementbyId("bx1");
            HtmlNodeCollection list = node.SelectNodes("./div[1]/div[1]/div");
            return list;
        }

        /// <summary>
        /// 获取楼盘名称
        /// </summary>
        /// <param name="loupanNode">楼盘页面</param>
        /// <returns></returns>
        private static string GetLoupanName(HtmlNode loupanNode)
        {
            var nameNode = loupanNode.SelectSingleNode("./dl[1]/dd[1]/div[1]/h4[1]/a[1]");
            return nameNode.InnerText.Trim();
        }

        /// <summary>
        /// 获取楼盘详情页面地址
        /// </summary>
        /// <param name="loupanNode">楼盘节点</param>
        /// <returns></returns>
        private static string GetLoupanUrl(HtmlNode loupanNode)
        {
            var nameNode = loupanNode.SelectSingleNode("./dl[1]/dd[1]/div[1]/h4[1]/a[1]");
            return nameNode.Attributes["href"].Value;
        }

        /// <summary>
        /// 获取楼盘价格
        /// </summary>
        /// <param name="loupanNode">楼盘节点</param>
        /// <returns></returns>
        private static int GetLoupanPrice(HtmlNode loupanNode)
        {
            bool flag = false;
            int price = 0;
            var priceNodeCollections = loupanNode.SelectNodes("./dl[1]/dd[1]/div");
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

        /// <summary>
        /// 获取楼盘地址
        /// </summary>
        /// <param name="loupanNode">楼盘节点</param>
        /// <returns></returns>
        private static string GetAddress(HtmlNode loupanNode)
        {
            var addressNode = loupanNode.SelectSingleNode("./dl[1]/dd[3]/div[1]/a[1]");
            return addressNode.Attributes["title"].Value;
        }

        /// <summary>
        /// 获取楼盘所在地区
        /// </summary>
        /// <param name="loupanNode">楼盘节点</param>
        /// <returns></returns>
        private static string GetRegion(HtmlNode loupanNode)
        {
            string address = GetAddress(loupanNode);
            string region = Regex.Match(address, @"(?<=\[).*(?=\])").Value;
            return region;
        }

    }
}
