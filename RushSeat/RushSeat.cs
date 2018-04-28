using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Security;
using System.Net.Security;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Threading;

namespace RushSeat
{
    class RushSeat
    {
        private static string longinUrl = "https://seat.lib.whu.edu.cn:8443/rest/auth";  //登录API
        private static string stats_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/room/stats2/";  // +信息分馆区域信息API  信息馆ID1
        private static string layout_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/room/layoutByDate/";  //+6:三楼西区域 后面还有yyyy-mm-dd时间
        private static string startTime_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/startTimesForSeat/";  // 座位开始时间API 后面还有yyyy-mm-dd时间
        private static string endTime_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/endTimesForSeat/";  // 座位结束时间API 后面还有yyyy-mm-dd时间
        private static string book_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/freeBook";  // 座位预约API
        private static string search_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/searchSeats/";  // 空位检索API date+startTime+endTime
        private static string history_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/history/1/10";  //预约记录 最后一位数为记录数目，自习助手默认为10  
        private static string usr_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/user";  // 用户信息API
        private static string cancel_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/cancel/";  // 取消预约API + 预约ID
        private static string stop_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/stop";  // 座位释放API  不需要其它ID


        public static string studentID = "";
        public static string password = "";
        private static string token = "";
        public static string resID = ""; 

        public static DateTime time;

        public static ArrayList freeSeats = new ArrayList();

        private static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            ////参见https://blog.csdn.net/chordwang/article/details/54311033
            //BindingFlags.Instance可在搜索中包含对象实例 .NonPubilc可在搜索中包含非公共成员（即私有成员和受保护的成员）
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection", BindingFlags.Instance | BindingFlags.NonPublic);  
            if (property != null)
            {
                ////参见http://www.cnblogs.com/mrray/archive/2011/09/28/2193831.html
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }

        public static void SetHeaderValues(HttpWebRequest request)
        {
            SetHeaderValue(request.Headers, "Host", "seat.lib.whu.edu.cn:8443");
            SetHeaderValue(request.Headers, "Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            SetHeaderValue(request.Headers, "Connection", "keep-alive");
            SetHeaderValue(request.Headers, "Accept", "*/*");
            SetHeaderValue(request.Headers, "User-Agent", "doSingle/11 CFNetwork/893.14.2 Darwin/17.3.0");
            SetHeaderValue(request.Headers, "Accept-Language", "zh-cn");
            SetHeaderValue(request.Headers, "token", token);
            SetHeaderValue(request.Headers, "Accept-Encoding", "gzip, deflate");
        }

        //倒计时
        public static void Wait(string hour, string minute, string second, bool enter = true)
        {
            TimeSpan delta2;
            time = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + hour + ":" + minute + ":" + second);
            Config.config.textBox1.AppendText("等待系统开放:\n");
            //后台倒计时线程启动
            Config.config.backgroundWorker1.RunWorkerAsync(enter);
            while(true)
            {               
                delta2 = RushSeat.time.Subtract(DateTime.Now);
                if (delta2.TotalSeconds < 0)
                {
                    Config.config.backgroundWorker1.CancelAsync();
                    break;
                }
                //防止控件假死
                Application.DoEvents();
            }
            return;
        }

        public static string GetToken(bool test = false)
        {
            //request
            string url = longinUrl + "?username=" + studentID + "&password=" + password;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            SetHeaderValues(request);
            //不对服务端证书进行有效性校验（非第三方权威机构颁发的证书，如自己生成的）
            ServicePointManager.ServerCertificateValidationCallback
                = new RemoteCertificateValidationCallback((a, b, c, d) => { return true; });
            //超时
            request.Timeout = 10000;

            //response
            //获取reponse流
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            //JSON格式化
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            StreamReader streamReader = new StreamReader(stream, encoding);
            string json = streamReader.ReadToEnd();
            JObject jObject = JObject.Parse(json);
                //登录成功
            if (jObject["status"].ToString() == "success")
            {
                token = jObject["data"]["token"].ToString();
                //MessageBox.Show("登录成功");
                return "Success";
            }
            else
            {
                MessageBox.Show(jObject["message"].ToString());
                return jObject["message"].ToString();
            }
        }

        //检查是否有预约或者正在使用，alert开关提示
        public static string CheckHistoryInf(bool alert = true)
        {
            string url = history_url;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            SetHeaderValues(request);
            ServicePointManager.ServerCertificateValidationCallback
                = new RemoteCertificateValidationCallback((a, b, c, d) => { return true; });
            request.Timeout = 5000;
            
            //response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            StreamReader streamReader = new StreamReader(stream, encoding);
            string json = streamReader.ReadToEnd();
            JObject jObject = JObject.Parse(json);

            if (jObject["status"].ToString() == "success")
            {
                foreach (JToken res in jObject["data"]["reservations"])
                    if (res["stat"].ToString() == "RESERVE" || res["stat"].ToString() == "CHECK_IN" || res["stat"].ToString() == "AWAY")
                    {
                        if (alert)
                        {
                            Config.config.textBox1.AppendText("检测到已有有效预约:\n");
                            Config.config.textBox1.AppendText("ID: " + res["id"] + "\r\n时间: " + res["date"] + " " + res["begin"] + "~" + res["end"]);
                            Config.config.textBox1.AppendText("若释放座位可点击释放座位，若不释放座位可以自动改签\n");
                        }
                        //激活释放按钮
                        Config.config.button2.Enabled = true;
                        if (res["stat"].ToString() == "RESERVE")
                            Config.config.button2.Text = "取消预约";
                        if (res["stat"].ToString() == "CHECK_IN" || res["stat"].ToString() == "AWAY")
                            Config.config.button2.Text = "结束使用";
                        RushSeat.resID = res["id"].ToString();
                        return res["stat"].ToString();
                    }
            }
            return "NO";
        }

        public static bool GetUserInfo()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(usr_url);
            request.Method = "GET";
            SetHeaderValues(request);
            ServicePointManager.ServerCertificateValidationCallback
                = new RemoteCertificateValidationCallback((a, b, c, d) => { return true; });
            request.Timeout = 5000;

            Config.config.textBox1.AppendText("\n--------------------------\n");
            Config.config.textBox1.AppendText("正在获取你的信息:\n");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            StreamReader streamReader = new StreamReader(stream, encoding);
            string json = streamReader.ReadToEnd();
            JObject jObject = JObject.Parse(json);

            if (jObject["status"].ToString() == "success")
            {
                Config.config.textBox1.AppendText("姓名：" + jObject["data"]["name"].ToString() + "\n");
                Config.config.textBox1.AppendText("学号：" + jObject["data"]["username"].ToString() + "\n");
                Config.config.textBox1.AppendText("Lastlogin:" + jObject["data"]["lastLogin"].ToString() + "\n");
                Config.config.textBox1.AppendText("---------------------------------------\n");
                
                return true;
            }
            return false;
        }


        //取消预约
        public static bool CancelReservation(string id)
        {
            string url = cancel_url + id;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            SetHeaderValues(request);
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => { return true; });
            request.Timeout = 5000;
            Config.config.textBox1.AppendText("正在取消预约... : \n");
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                StreamReader streamReader = new StreamReader(stream, encoding);
                string json = streamReader.ReadToEnd();
                JObject jObject = JObject.Parse(json);
                if (jObject["status"].ToString() == "success")
                {
                    Config.config.textBox1.AppendText("取消当前预约成功\n");
                    return true;
                }
                else
                {
                    Config.config.textBox1.AppendText("取消当前预约失败\r\n" + jObject.ToString());
                    return false;
                }
            }
            catch
            {
               Config.config.textBox1.AppendText("Connection lost\n");
               return false;
            }
        }

        //结束使用
        public static bool StopUsing()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(stop_url);
            request.Method = "GET";
            SetHeaderValues(request);
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => { return true; });
            request.Timeout = 5000;
            Config.config.textBox1.AppendText("\r\n正在结束使用当前座位...\n");

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                StreamReader streamReader = new StreamReader(stream, encoding);
                string json = streamReader.ReadToEnd();
                JObject jObject = JObject.Parse(json);
                if (jObject["status"].ToString() == "success")
                {
                    Config.config.textBox1.AppendText("结束当前使用成功\n");
                    return true;
                }
                else
                {
                    Config.config.textBox1.AppendText("结束当前使用失败\r\n" + jObject.ToString());
                    return false;
                }
            }
            catch
            {
                Config.config.textBox1.AppendText("Connection lost\n");
                return false;
            }
        }
        public static string SearchFreeSeat(string buildingId, string roomId, string date, string startTime, string endTime)
        {
            string url = search_url + date + "/" + startTime + "/" + endTime;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            SetHeaderValues(request);
            ServicePointManager.ServerCertificateValidationCallback
                = new RemoteCertificateValidationCallback((a, b, c, d) => { return true; });
            request.Timeout = 5000;

            try
            {
                StringBuilder buffer = new StringBuilder();
                buffer.AppendFormat("{0}={1}", "t", "1");
                buffer.AppendFormat("&{0}={1}", "roomId", roomId);
                buffer.AppendFormat("&{0}={1}", "buildingId", buildingId);
                buffer.AppendFormat("&{0}={1}", "batch", "9999");
                buffer.AppendFormat("&{0}={1}", "page", "1");
                buffer.AppendFormat("&{0}={1}", "t2", "2");
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                request.ContentLength = data.Length;
                request.GetRequestStream().Write(data, 0, data.Length);
                Config.config.textBox1.AppendText("正在获取空座信息...\n");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                StreamReader streamReader = new StreamReader(stream, encoding);
                string json = streamReader.ReadToEnd();
                JObject jObject = JObject.Parse(json);

                //查到空座就将座位ID放入freeseat中
                if (jObject["data"]["seats"].ToString() != "{}")
                {
                    Config.config.textBox1.AppendText("success\n");
                    JToken seats = jObject["data"]["seats"];
                    //靠窗模式
                    // if (Config.checkBox3.Checked == true)


                    foreach (var num in seats)
                    {
                        if (Run.only_window == "false")
                        {
                            if (Run.only_conputer == "false")
                                freeSeats.Add(num.First["id"].ToString());
                            else
                                if (num.First["computer"].ToString() != "False")
                                    freeSeats.Add(num.First["id"].ToString());
                        }
                        if (Run.only_window != "false")
                        {
                            if (Run.only_conputer == "false")
                            {
                                if (num.First["window"].ToString() != "False")
                                    freeSeats.Add(num.First["id"].ToString());
                            }
                            else
                                if (num.First["window"].ToString() != "False" && num.First["computer"].ToString() != "False")
                                    freeSeats.Add(num.First["id"].ToString());
                        }
                    }
                    return "Success";
                }
                else
                {
                    Config.config.textBox1.AppendText("No free seat!\n");
                    return "NoFreeSeat";
                }
            }
            catch
            {
                Config.config.textBox1.AppendText("获取空座信息出错...\n");
                return "Something wrong";
            }
        }

        //订座
        public static string BookSeat(string seatId, string date, string startTime, string endTime)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(book_url);
            request.Method = "POST";
            SetHeaderValues(request);
            ServicePointManager.ServerCertificateValidationCallback
                = new RemoteCertificateValidationCallback((a, b, c, d) => { return true; });
            request.Timeout = 5000;

            StringBuilder buffer = new StringBuilder();
            buffer.AppendFormat("{0}={1}", "t", "1");
            buffer.AppendFormat("&{0}={1}", "startTime", startTime);
            buffer.AppendFormat("&{0}={1}", "endTime", endTime);
            buffer.AppendFormat("&{0}={1}", "seat", seatId);
            buffer.AppendFormat("&{0}={1}", "date", date);
            buffer.AppendFormat("&{0}={1}", "t2", "2");
            byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
            request.ContentLength = data.Length;
            request.GetRequestStream().Write(data, 0, data.Length);

            Config.config.textBox1.AppendText("正在尝试订座:\n");
          
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            StreamReader streamReader = new StreamReader(stream, encoding);
            string json = streamReader.ReadToEnd();
            JObject jObject = JObject.Parse(json);
            if (jObject["status"].ToString() == "success")
            {
                Config.config.textBox1.AppendText("订座成功\n");
                Config.config.textBox1.AppendText("时间：" + jObject["data"]["onDate"].ToString() + ", " + jObject["data"]["begin"].ToString() + "~" + jObject["data"]["end"].ToString() + "\n");
                Config.config.textBox1.AppendText("地点：" + jObject["data"]["location"].ToString() + "\n");
                return "Success";
            }
            else
            {
                Config.config.textBox1.AppendText(jObject.ToString() + "\n");
                return "Failed";
            }            
        }


        
    }
}
