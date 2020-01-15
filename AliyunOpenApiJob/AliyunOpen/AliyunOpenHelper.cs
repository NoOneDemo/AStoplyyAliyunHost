using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
//using System.Web;
using Aliyun.Acs.Core.Http;
using System.Web;
using Newtonsoft.Json;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core;
using Newtonsoft.Json.Linq;
using Common.Logging;

namespace AliyunOpenApiJob.AliyunOpen
{
    public static class AliyunOpenHelper
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AliyunOpenHelper));
        private static readonly string accessKeyId = ConfigurationManager.AppSettings["AccessKeyId"];
        private static readonly string accessSecret = ConfigurationManager.AppSettings["AccessSecret"];
        private static readonly string domain = ConfigurationManager.AppSettings["domain"];


        public static JObject GetDomainList(string domain)
        {
            var selectRecord = new Dictionary<string, string>();
            selectRecord.Add("Action", "DescribeDomainRecords");
            selectRecord.Add("DomainName", domain);
            var result = HttpSend("", "application/json", GetSendRequest(selectRecord), false, 10, "GET", null);
            return JsonConvert.DeserializeObject<JObject>(result);
        }

        public static string UpdateDomain(Dictionary<string, string> dic)
        {
            dic.Add("Action", "UpdateDomainRecord");
            return HttpSend("", "application/json", GetSendRequest(dic), false, 10, "GET", null);
        }

        public static string GetCurIp()
        {
            return HttpSend("", "application/json", "http://45.32.164.128/ip.php", false, 10, "GET", null);
            //var result = HttpSend("", "application/json", "http://45.32.164.128/ip.php", false, 10, "GET", null);
            //var start = result.ToString().IndexOf('[');
            //var end = result.ToString().IndexOf(']');
            //return result.Substring(start).Substring(1, end - start-1);
        }

        public static string UpdateDomainList(string ip)
        {
            var selectRecord = new Dictionary<string, string>();
            selectRecord.Add("Action", "DescribeDomainRecords");
            selectRecord.Add("DomainName", "codersun.cn");
            var result = HttpSend("", "", GetSendRequest(selectRecord), false, 10, "GET", null);

            return result;
        }

        #region 基础方法

        public static string GetSendRequest(Dictionary<string, string> body)
        {
            var HTTPMethod = "GET";
            var sd = new SortedDictionary<string, string>(StringComparer.Ordinal);
            //公共参数
            sd.Add("Format", "JSON");
            sd.Add("Version", "2015-01-09");
            sd.Add("AccessKeyId", accessKeyId);
            sd.Add("SignatureMethod", "HMAC-SHA1");
            sd.Add("Timestamp", DateTime.Now.AddHours(-8).ToString("yyyy-MM-ddTHH:mm:ssZ", DateTimeFormatInfo.InvariantInfo));
            sd.Add("SignatureVersion", "1.0");
            sd.Add("SignatureNonce", Guid.NewGuid().ToString("N"));
            //请求参数  
            body.ToList().ForEach(p => sd.Add(p.Key, p.Value));

            var StringToSign = HTTPMethod + "&%2F&" + UrlEncode(GetUrlStr(sd));
            HMACSHA1 sha1 = new HMACSHA1(new UTF8Encoding().GetBytes(accessSecret + "&"));
            var encodedBytes = sha1.ComputeHash(new UTF8Encoding().GetBytes(StringToSign));
            string decodedString = Convert.ToBase64String(encodedBytes);
            //var a= get_uft8()
            sd.Add("Signature", decodedString);
            return "http://alidns.aliyuncs.com/?" + GetUrlStr(sd);
        }

        public static string UrlEncode(string str)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in str)
            {
                if (HttpUtility.UrlEncode(c.ToString()).Length > 1)
                {
                    builder.Append(HttpUtility.UrlEncode(c.ToString()).ToUpper());
                }
                else
                {
                    builder.Append(c);
                }
            }
            return builder.Replace("+", "%20").Replace("*", "%2A").Replace("%7E", "~").ToString();
        }
        private static string GetUrlStr(SortedDictionary<string, string> sd)
        {
            StringBuilder CanonicalizedQueryString = new StringBuilder();
            var a = sd.ToList();
            for (int i = 0; i < a.Count; i++)
            {
                CanonicalizedQueryString.Append(UrlEncode(a[i].Key) + "=" + UrlEncode(a[i].Value) + "&");
            }
            int nLen = CanonicalizedQueryString.Length;
            CanonicalizedQueryString.Remove(nLen - 1, 1); //去掉最后一个字符
            return CanonicalizedQueryString.ToString();
        }

        public static string HttpSend(string postStr, string contextFormart, string url, bool isUseCert, int timeout, string method, HttpWebRequest request)
        {
            string result = ""; //返回结果

            HttpWebResponse response = null;
            Stream reqStream = null;

            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = request ?? (HttpWebRequest)WebRequest.Create(url);

                request.Method = method;
                request.Timeout = timeout * 1000;

                if (!string.IsNullOrEmpty(contextFormart))
                {
                    request.ContentType = contextFormart;
                }
                else
                {
                    request.ContentType = "text/xml";
                }
                if (!string.IsNullOrEmpty(postStr))
                {
                    //设置POST的数据类型和长度
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(postStr);
                    request.ContentLength = data.Length;

                    //往服务器写入数据
                    reqStream = request.GetRequestStream();
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();

                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Exception e)
            {
                Logger.Error($"http异常=>{url}",e);
            }
            finally
            {
                //关闭连接和流
                reqStream?.Close();
                response?.Close();
                request?.Abort();
            }

            return result;
        }
        #endregion
    }
}
