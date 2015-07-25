using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace LoupanCrawler
{
    class HttpHelper
    {
        private readonly HttpWebRequest _request;
        private HttpWebResponse _response;

        public HttpHelper(string Url)
        {
            _request = WebRequest.Create(Url) as HttpWebRequest;
            if (_request != null)
            {
                _request.Method = "GET";
                _request.ContentType = "application/x-www-form-urlencoded;charset=GB2312";
            }
        }

        public Stream GetResponseStream()
        {
            try
            {
                _response = _request.GetResponse() as HttpWebResponse;
                Stream myStream = _response.GetResponseStream();
                if (_response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    if (myStream != null)
                        myStream = new GZipStream(myStream, CompressionMode.Decompress);
                }
                return myStream;
            }
            catch (Exception)
            {
                return Stream.Null;
            }
        }
    }
}
