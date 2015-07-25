using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace HouseCrawler
{
    static class HttpHelper
    {
        public static HttpWebResponse CreateGetHttpResponse(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded;charset=GB2312";
            return request.GetResponse() as HttpWebResponse;
        }

        public static Stream GetDecompressedStream(this HttpWebResponse response)
        {
            if (response != null)
            {
                Stream myStream = response.GetResponseStream();
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    if (myStream != null)
                        myStream = new GZipStream(myStream, CompressionMode.Decompress);
                }
                return myStream;
            }
            return Stream.Null;
        }

        public static string GetResponseContent(this HttpWebResponse response)
        {
            Stream responseStream = response.GetDecompressedStream();
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            StreamReader reader = new StreamReader(responseStream,encoding);
            string content = reader.ReadToEnd();
            reader.Close();
            responseStream.Close();
            return content;
        }

        public static HttpWebResponse CreatePostHttpResponse(string url,Dictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url is null");
            }

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Referer = url;
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; Touch; rv:11.0) like Gecko";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF8";
            
            //如果需要POST数据    
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    buffer.AppendFormat(i > 0 ? "&{0}={1}" : "{0}={1}", key, parameters[key]);
                    i++;
                }
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                return request.GetResponse() as HttpWebResponse;
            }
            catch (Exception)
            {
                return null;
            }
            

        }
    }
}
