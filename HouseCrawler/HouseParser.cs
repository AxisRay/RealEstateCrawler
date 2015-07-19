using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace HouseCrawler
{
    class HouseParser
    {
        public string LoupanId { get; }
        public string LoupanUrl { get; }
        private Dictionary<string, string> _parameters;
        private DBHelper _helper;
        public int CurrentPage { get; private set; }
        public int TotalPage { get; private set; }

        public HouseParser(string loupanId,string loupanUrl,string city,DBHelper helper)
        {
            _helper = helper;
            LoupanId = loupanId;
            LoupanUrl = loupanUrl;
            InitPostParameters(GetNewCode(loupanUrl),city);
        }

        private string GetNewCode(string loupanUrl)
        {
            HttpWebResponse response = HttpHelper.CreateGetHttpResponse(loupanUrl);
            Stream responseStream = response.GetDecompressedStream();
            HtmlDocument doc = new HtmlDocument();
            doc.Load(responseStream,Encoding.GetEncoding("GB2312"));
            responseStream.Close();
            try
            {
                HtmlNodeCollection naviBox = doc.GetElementbyId("orginalNaviBox").ChildNodes;
                string houseListUrl = naviBox[5].Attributes["href"].Value;
                string houseid = Regex.Match(houseListUrl, @"((?<=/)|(?<=/))[\d]{10}").Value;
                return houseid;
            }
            catch (Exception)
            {
                Console.WriteLine("非匹配页面");
                return String.Empty;
            }

        }

        private void InitPostParameters(string newcode, string city)
        {
            _parameters = new Dictionary<string, string>
            {
                {"newcode", newcode},
                {"pageindex", "1"},
                {"ju", ""},
                {"dong", ""},
                {"louc", ""},
                {"saling", ""},
                {"city", city}
            };
        }

        public bool RecordHouseDetail()
        {
            Random rand = new Random();
            string queryUrl = LoupanUrl + "house/ajaxrequest/dongList2015.php?t=" + rand.NextDouble();
            HttpWebResponse response = HttpHelper.CreatePostHttpResponse(queryUrl,_parameters);
            JObject houseListJson = JObject.Parse(response.GetResponseContent());

            if (houseListJson["resCode"].Value<string>() != "100")
            {
                Console.WriteLine("Error:{0},{1}", houseListJson["resCode"].Value<string>(), houseListJson["msg"].Value<string>());
                return false;
            }
                

            CurrentPage = houseListJson["thispage"].Value<int>();
            TotalPage = houseListJson["totalpage"].Value<int>();

            foreach (var house in houseListJson["list"])
            {
                _helper.InsertHouseDetail(house, LoupanId);
                Console.WriteLine("House:{0},{1}",house["projname"].Value<string>(),house["houselocation"].Value<string>());
            }

            if (CurrentPage >= TotalPage)
                return false;
            _parameters["pageindex"] = (CurrentPage + 1).ToString();
            return true;
        }
    }
}
