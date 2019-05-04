using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System;
using System.Threading.Tasks;

namespace IDG
{


    public static class HttpClient
    {
        /// <summary>
        /// 异步发送Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="send"></param>
        /// <returns></returns>
        public static async Task<string> PostAsync(string url, string send)
        {
            HttpWebRequest request = null;

            request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            var requestSteam = await Task.Run(() =>
            {
                return request.GetRequestStream();
            });
            using (StreamWriter stream = new StreamWriter(requestSteam, Encoding.UTF8))
            {
                stream.Write(send);
                stream.Close();
            }
            var response = await Task.Run(() =>
            {
                return request.GetResponse();
            });
            string receive = "";
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                receive = reader.ReadToEnd();
            }
            return receive;

        }
    }
    public static class DataHttpClient
    {
        public static async Task<KeyValueProtocol> PostAsync(string url, KeyValueProtocol send)
        {
            try
            {
                return new KeyValueProtocol(await HttpClient.PostAsync(url, send.GetString()));
            }
            catch (Exception)
            {
                var receive = new KeyValueProtocol();
                receive["status"] = "网络错误";
                receive["info"] = "连接不到服务器 请检查网络";
                return receive;
            }
        }
    }
}

