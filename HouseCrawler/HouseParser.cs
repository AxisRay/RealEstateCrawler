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
        private HouseDBDataContext db;
        public string LoupanId { get; }
        public string LoupanUrl { get; }
        private Dictionary<string, string> _parameters;
        public int CurrentPage { get; private set; }
        public int TotalPage { get; private set; }

        public HouseParser(string loupanId,string loupanUrl,string city)
        {
            db=new HouseDBDataContext();
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
            string responseStr = response.GetResponseContent();
            if (string.IsNullOrEmpty(responseStr))
            {
                return false;
            }
            JObject houseListJson = JObject.Parse(responseStr);

            if (houseListJson["resCode"].Value<string>() != "100")
            {
                Console.WriteLine("Error:{0},{1}", houseListJson["resCode"].Value<string>(), houseListJson["msg"].Value<string>());
                return false;
            }
                

            CurrentPage = houseListJson["thispage"].Value<int>();
            TotalPage = houseListJson["totalpage"].Value<int>();

            foreach (var house in houseListJson["list"])
            {
                InsertIntoDB(house);
                Console.WriteLine("House:{0},{1}",house["projname"].Value<string>(),house["houselocation"].Value<string>());
            }
            db.SubmitChanges();

            if (CurrentPage >= TotalPage)
                return false;
            _parameters["pageindex"] = (CurrentPage + 1).ToString();
            return true;
        }

        public void InsertIntoDB(JToken house)
        {
            HouseDetail hd = new HouseDetail()
            {
                houseid = house["houseid"].Value<string>(),
                dongid = house["dongid"].Value<string>(),
                newcode = house["newCode"].Value<string>(),
                projname = house["projname"].Value<string>(),
                dongname = house["dongname"].Value<string>(),
                address = house["address"].Value<string>(),
                room = house["room"].Value<int>(),
                ting = house["ting"].Value<int>(),
                wei = house["wei"].Value<int>(),
                chu = house["chu"].Value<int>(),
                jianzhumianji = house["jianzhumianji"].Value<float>(),
                houselocation = house["houselocation"].Value<string>(),
                district = house["district"].Value<string>(),
                tehui_price = PriceParse(house["tehui_price"].Value<string>()),
                price_s = PriceParse(house["price_s"].Value<string>()),
                price_s_type = house["Price_s_type"].Value<string>(),
                price_t = PriceParse(house["price_t"].Value<string>()),
                price_t_type = house["Price_t_type"].Value<string>()
            };
            db.HouseDetail.InsertOnSubmit(hd);

        }

        private float PriceParse(string priceStr)
        {
            float price;
            string replaced = Regex.Replace(priceStr, @"[^\d.]*", String.Empty);
            bool success = float.TryParse(replaced, out price);
            if (!success)
                return -1;
            return price>100000?price/10000:price;
        }
    }
}
