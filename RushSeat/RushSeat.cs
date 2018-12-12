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
using System.Drawing;
using System.ComponentModel;

namespace RushSeat
{
    class RushSeat
    {
        private static string longinUrl = "https://seat.lib.whu.edu.cn:8443/rest/auth";  //登录API
        private static string stats_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/room/stats2/";  // +信息分馆区域信息API  信息馆ID1
        private static string layout_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/room/layoutByDate/";  // 某区域座位信息 +6:三楼西区域 后面还有yyyy-mm-dd时间
        private static string startTime_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/startTimesForSeat/";  // 座位开始时间API 后面还有yyyy-mm-dd时间
        private static string endTime_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/endTimesForSeat/";  // 座位结束时间API 后面还有yyyy-mm-dd时间
        private static string book_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/freeBook";  // 座位预约API
        private static string search_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/searchSeats/";  // 空位检索API date+startTime+endTime
        private static string history_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/history/1/10";  //预约记录 最后一位数为记录数目，自习助手默认为10  
        private static string usr_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/user";  // 用户信息API
        private static string cancel_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/cancel/";  // 取消预约API + 预约ID
        private static string stop_url = "https://seat.lib.whu.edu.cn:8443/rest/v2/stop";  // 座位释放API  不需要其它ID
        private static string snum_url = "http://www.smschinese.cn/web_api/SMS/?Action=SMS_Num&Uid=银翼随风&Key=d41d8cd98f00b204e980";

        private static string msg_url = "http://utf8.sms.webchinese.cn/?";//发送短信平台网址SMS  
        private static string strUid = "Uid=银翼随风";//注册的SMS平台的账号ID  
        private static string strKey = "&key=d41d8cd98f00b204e980";//注册的SMS平台的接口密匙  
        private static string strMob = "&smsMob=";//手机号码  
        private static string strContent = "&smsText=";// 发送的内容 

        public static string[] rankAList = { "2015302590047", "2015302590096", "2015302590043", "2015302590145", "2015302590143", "2016302590189", "2016302590152", "2017302590158", "2015301610184", "2014302590064", "2016302590082", "2017302590210", "2015302590068", "2015302590105" };
        public static List<string> rankBList = new List<string>();
        public static string[] rankDList = { "2015302590102"};

        public static string studentID = "";
        public static string password = "";
        private static string token = "";
        public static string resID = "";

        public static string now_state = "free"; // free空闲 wait等待 supervise监测抢座

        //信部全馆房间编号
        public static string[] roomList_b1 = {"4", "5", "6", "7", "8", "9", "10", "11", "12", "14", "15" ,"16"};
        public static string[] roomList_f1 = {"4", "5", "14" , "15", "16"};
        public static string[] roomList_f2t4 = {"6", "7", "8", "9", "10", "11", "12"};

        public static bool stop_waiting = false;
        public static bool stop_rush = false;

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
            //SetHeaderValue(request.Headers, "User-Agent", "doSingle/11 CFNetwork/893.14.2 Darwin/17.3.0");
            SetHeaderValue(request.Headers, "User-Agent", "doSingle/11 CFNetwork/976 Darwin/18.2.0");
            SetHeaderValue(request.Headers, "Accept-Language", "zh-cn");
            SetHeaderValue(request.Headers, "token", token);
            SetHeaderValue(request.Headers, "Accept-Encoding", "gzip, deflate");
        }

        //倒计时
        public static void Wait(string hour, string minute, string second, bool enter = true)
        {
            TimeSpan delta2;
            time = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + hour + ":" + minute + ":" + second);
            //Config.config.textBox1.AppendText("等待系统开放:\n");
            //后台倒计时线程启动
            
            //Config.config.backgroundWorker1 = new BackgroundWorker(); //不加这句会报不能启动多个backgroundworker的错误
            
            if (Config.config.backgroundWorker1.IsBusy)
            {
                //结束前一进程
                Config.config.backgroundWorker1.WorkerSupportsCancellation = true;
                Config.config.backgroundWorker1.CancelAsync();
                Config.config.backgroundWorker1 = new BackgroundWorker(); //不加这句会报不能启动多个backgroundworker的错误
                Config.config.textBox1.AppendText("等待前一倒计时进程结束...\n");
                Thread.Sleep(500);
            }
            Config.config.backgroundWorker1.RunWorkerAsync(enter);
            while(true)
            {               
                delta2 = RushSeat.time.Subtract(DateTime.Now);
                if (delta2.TotalSeconds < 0 || stop_waiting == true)
                {
                    Config.config.backgroundWorker1.WorkerSupportsCancellation = true;
                    Config.config.backgroundWorker1.CancelAsync();
                    break;
                }
                //防止控件假死
                Application.DoEvents();
            }
            return;
        }
        public static void WaitNew(string hour, string minute, string second, bool enter = true)
        {
            TimeSpan delta2;
            time = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + hour + ":" + minute + ":" + second);
            //Config.config.textBox1.AppendText("等待系统开放:\n");
            //后台倒计时线程启动

            //Config.config.backgroundWorker1 = new BackgroundWorker(); //不加这句会报不能启动多个backgroundworker的错误

            if (Config.config.backgroundWorker3.IsBusy)
            {
                //结束前一进程
                Config.config.backgroundWorker3.WorkerSupportsCancellation = true;
                Config.config.backgroundWorker3.CancelAsync();
                Config.config.backgroundWorker3 = new BackgroundWorker(); //不加这句会报不能启动多个backgroundworker的错误
                Config.config.textBox1.AppendText("等待前一倒计时进程结束...\n");
                Thread.Sleep(500);
            }
            Config.config.backgroundWorker3.RunWorkerAsync(enter);
            while (true)
            {
                delta2 = RushSeat.time.Subtract(DateTime.Now);
                if (delta2.TotalSeconds < 0 || stop_waiting == true)
                {
                    Config.config.backgroundWorker3.WorkerSupportsCancellation = true;
                    Config.config.backgroundWorker3.CancelAsync();
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
            request.Timeout = 3000;

            try
            {
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
                    //MessageBox.Show(jObject["message"].ToString());
                    return jObject["message"].ToString();
                }
            }
            catch
            {
                //MessageBox.Show("登录失败，请检查网络连接");
                return "Fail";
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

            try
            {
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
                                Config.config.textBox1.AppendText("ID: " + res["id"] + "\r\n时间: " + res["date"] + " " + res["begin"] + "~" + res["end"] + "\n");
                                Config.config.textBox1.AppendText("若释放座位可点击释放座位，若不释放座位可以自动改签\n");
                                Config.config.textBox1.AppendText("---------------------------------------\n");
                            }
                            //激活释放按钮
                            Config.config.button2.Enabled = true;
                            if (res["stat"].ToString() == "RESERVE")
                                Config.config.button2.Text = "取消预约";
                            if (res["stat"].ToString() == "CHECK_IN" || res["stat"].ToString() == "AWAY")
                                Config.config.button2.Text = "结束使用";
                            //存一下预约ID，释放时用
                            RushSeat.resID = res["id"].ToString();
                            return res["stat"].ToString();
                        }
                }
                return "NO";
            }
            catch
            {
                Config.config.textBox1.AppendText("Connection lost...");
                return "WRONG";
            }

        }

        //获取用户信息并设置权限
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

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                StreamReader streamReader = new StreamReader(stream, encoding);
                string json = streamReader.ReadToEnd();
                JObject jObject = JObject.Parse(json);
           

            //检查阶级
            //根据等级设置具体参数
            foreach (string i in RushSeat.rankAList)
            {
                if (jObject["data"]["username"].ToString() == i)
                {
                    Config.rank = 'A';
                    Run.rankSuccessGetFreeSeat = 0;
                    Run.repeatSearchInterval = 1800;
                    
                    Config.config.checkBox4.Enabled = true;
                    //Config.config.checkBox4.Checked = true;
                    Config.config.textBox3.Enabled = true;
                    if (File.Exists(@"telnumber.txt"))
                    {
                        string[] strs2 = File.ReadAllLines(@"telnumber.txt");
                        Config.config.textBox3.Text = strs2[0];
                    }
                    break;
                }
            }

            //在固定时间之前短信提示可用
            if (DateTime.Compare(DateTime.Now, Convert.ToDateTime("2019-1-1" + " 00:00:00")) < 0)
            {
                Config.config.checkBox4.Enabled = true;
                //Config.config.checkBox4.Checked = true;
                Config.config.textBox3.Enabled = true;
                if (File.Exists(@"telnumber.txt"))
                {
                    string[] strs2 = File.ReadAllLines(@"telnumber.txt");
                    Config.config.textBox3.Text = strs2[0];
                }
            }

            foreach (string i in RushSeat.rankBList)
            {
                if (jObject["data"]["username"].ToString() == i)
                {
                    Config.rank = 'B';
                    Run.rankSuccessGetFreeSeat = 750;
                    Run.repeatSearchInterval = 2400;
                    break;
                }
            }
            foreach (string i in RushSeat.rankDList)
            {
                if (jObject["data"]["username"].ToString() == i)
                {
                    Config.rank = 'D';
                    Run.rankSuccessGetFreeSeat = 3600000;
                    Run.repeatSearchInterval = 3600000;
                    break;
                }
            }

            


            if (jObject["status"].ToString() == "success")
            {
                Config.config.textBox1.AppendText("姓名：" + jObject["data"]["name"].ToString() + "\n");
                Config.config.textBox1.AppendText("学号：" + jObject["data"]["username"].ToString() + "\n");
                string caste = "Wrong，请联系开发者";
                switch (Config.rank)
                {
                    case 'A': { caste = "A  (0, 1800, 短信提示可用)"; break; }
                    case 'B': { caste = "B  (750, 2400，短信提示暂时可用)"; break; }
                    case 'C': { caste = "C  (1500, 3000， 短信提示暂时可用)"; break; }
                    case 'D': { caste = "D  (3.6*10^6, 3.6*10^6)"; break; }
                }
                Config.config.richTextBox1.Text = "Your Rank：" + caste;
                //阶级部分字体变红
                Config.config.richTextBox1.Select(10, 3);
                Config.config.richTextBox1.SelectionColor = Color.Red;
                Config.config.textBox1.AppendText("违约次数：" + jObject["data"]["violationCount"].ToString() +"\n");
                Config.config.textBox1.AppendText("Lastlogin:" + jObject["data"]["lastLogin"].ToString() + "\n");
                Config.config.textBox1.AppendText("---------------------------------------\n");
                
                return true;
            }

            }
            catch
            {
                Config.config.textBox1.AppendText("获取信息失败，估计是被关小黑屋了...第二天会解封\n");
                Config.config.textBox1.AppendText("---------------------------------------\n");
                return false;
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
            Config.config.textBox1.AppendText("正在取消当前预约... : \n");
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

            //try
            //{
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
                //Config.config.textBox1.AppendText("正在获取空座信息...\n");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                StreamReader streamReader = new StreamReader(stream, encoding);
                string json = streamReader.ReadToEnd();
                JObject jObject = JObject.Parse(json);

                if (Config.config.comboBox5.SelectedIndex == 0) //对座位没有要求
                {
                    //查到空座就将座位ID放入freeseat中
                    if (jObject["data"]["seats"].ToString() != "{}")
                    {
                        //Config.config.textBox1.AppendText("success\n");
                        JToken seats = jObject["data"]["seats"];

                        //18/5/5 此处存在BUG
                        //若检索到的座位都不满足座位类型需求，仍会返回Success
                        foreach (var num in seats)
                        {
                            if (Run.only_window == false)
                            {
                                if (Run.only_computer == false)
                                    freeSeats.Add(num.First["id"].ToString());
                                else
                                    if (num.First["computer"].ToString() == "True")
                                        freeSeats.Add(num.First["id"].ToString());
                            }
                            if (Run.only_window == true)
                            {
                                if (Run.only_computer == false)
                                {
                                    if (num.First["window"].ToString() == "True")  //JSON转换bool首字母大写
                                        freeSeats.Add(num.First["id"].ToString());
                                }
                                else
                                    if (num.First["window"].ToString() == "True" && num.First["computer"].ToString() != "True")
                                        freeSeats.Add(num.First["id"].ToString());
                            }
                        }

                        //add 检查是否真正检索到符合条件座位
                        if (freeSeats.Count != 0)
                        {
                            Config.config.textBox1.AppendText("成功检索到符合条件座位\n");
                            return "Success";
                        }
                        else
                        {
                            Config.config.textBox1.AppendText("没有符合条件的空座了" + (((double)Run.repeatSearchInterval) / 1000).ToString() + "s后会再次检索\n");
                            return "NoFreeSeat";
                        }
                    }
                    else
                    {
                        Config.config.textBox1.AppendText("没有符合条件的空座了" + (((double)Run.repeatSearchInterval) / 1000).ToString() + "s后会再次检索\n");
                        return "NoFreeSeat";
                    }
                }
                //对座位有要求
                else
                {
                    //首先不为空
                    //忽视座位属性要求
                    if (jObject["data"]["seats"].ToString() != "{}")
                    {
                        JToken seats = jObject["data"]["seats"];
                        //foreach (var num in seats)
                        //{
                        //    if (num.First["name"].ToString() == Config.config.comboBox5.SelectedValue.ToString())
                        //    {
                        //        freeSeats.Add(num.First["id"].ToString());
                        //        Config.config.textBox1.AppendText("检索到空闲的倾向座位, 尝试预约...\n");
                        //        return "Success";
                        //    }
                        //}
                        //Config.config.textBox1.AppendText("倾向座位已被预约但房间内有其它空座\n");
                        //return "NoMatchSeat";
                        ArrayList freeSeatsName = new ArrayList();
                        bool one = true;
                        bool get = false;
                        foreach (var num in seats)
                        {
                            get = false;
                            if (one)
                            {
                                //Config.config.textBox1.AppendText(Int32.Parse("056").ToString() + "\n");
                                //Config.config.textBox1.AppendText(Config.config.comboBox5.SelectedIndex.ToString() + "\n");
                                freeSeats.Add(num.First["id"].ToString());
                                freeSeatsName.Add(num.First["name"].ToString());
                                one = false;
                                continue;
                            }
                            for (int i = 0; i < freeSeats.Count; i++ )
                            {
                                //逐一比对空座和倾向座位号码的差值绝对值，按照从小到大排序
                                //经过排序后将倾向座位附近的座位将被安排在freeseats中倾向座位之后
                                int a = Math.Abs(int.Parse(num.First["name"].ToString()) - int.Parse(Config.config.comboBox5.SelectedIndex.ToString()));
                                int b = Math.Abs(int.Parse(freeSeatsName[i].ToString()) - int.Parse(Config.config.comboBox5.SelectedIndex.ToString()));
                                if (a < b)  //从freeseats列表里检察可以插入的位置（绝对值从小到大）
                                {
                                    //Config.config.textBox1.AppendText("ok!\n");
                                    //Config.config.textBox1.AppendText(a.ToString()+"    "+b.ToString()+"\n");
                                    freeSeats.Insert(i, num.First["id"].ToString());
                                    freeSeatsName.Insert(i, num.First["name"].ToString());
                                    get = true;
                                    break;
                                }
                            }
                            if (!get)
                            {
                                
                                freeSeats.Add(num.First["id"].ToString());
                                freeSeatsName.Add(num.First["name"].ToString());
                            }
                        }
                        //Config.config.textBox1.AppendText(freeSeatsName.Count.ToString() + "\n");
                        //foreach (var num in freeSeatsName)
                            //Config.config.textBox1.AppendText(num.ToString() + "\n");
                        if (Int32.Parse(freeSeatsName[0].ToString()).ToString() == Int32.Parse(Config.config.comboBox5.SelectedIndex.ToString()).ToString()) //有倾向座位
                        {
                            Config.config.textBox1.AppendText("检索到空闲的倾向座位, 尝试预约...\n");
                            return "Success";
                        }
                        else
                        {
                            //Config.config.textBox1.AppendText(Int32.Parse(freeSeatsName[0].ToString()).ToString() + "    这是freeseat第一个\n");
                            //Config.config.textBox1.AppendText(Config.config.comboBox5.SelectedIndex.ToString() + "    这是c5\n");
                            Config.config.textBox1.AppendText("倾向座位已被预约，房间内有其它空座...\n");
                            return "NoMatchSeat";
                        }
                    }
                    else
                    {
                        Config.config.textBox1.AppendText("倾向座位已被预约，房间内无其它空座...\n");
                        return "NoFreeSeat";
                    }
                }
            //}
            //catch
            //{
            //    Config.config.textBox1.AppendText("获取空座信息出错...可能是连接丢失，也可能是其它问题，请重试\n");
            //    return "SomethingWrong";
            //}
        }

        public static string SearchFreeSeatMulti(string buildingId, string roomId, string date, string startTime, string endTime)
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
                //Config.config.textBox1.AppendText("正在获取空座信息...\n");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                StreamReader streamReader = new StreamReader(stream, encoding);
                string json = streamReader.ReadToEnd();
                JObject jObject = JObject.Parse(json);

                //获取房间名
                string roomname = "";
                foreach (DictionaryEntry item in Config.xt_list)
                {
                    if (item.Key.ToString() == roomId)
                    {
                        roomname = item.Value.ToString();
                    }
                }


                if (jObject["data"]["seats"].ToString() != "{}")
                {
                    //Config.config.textBox1.AppendText("success\n");
                    JToken seats = jObject["data"]["seats"];

                    //18/5/5 此处存在BUG
                    //若检索到的座位都不满足座位类型需求，仍会返回Success
                    foreach (var num in seats)
                    {
                        if (Run.only_window == false)
                        {
                            if (Run.only_computer == false)
                                freeSeats.Add(num.First["id"].ToString());
                            else
                                if (num.First["computer"].ToString() == "True")
                                    freeSeats.Add(num.First["id"].ToString());
                        }
                        if (Run.only_window == true)
                        {
                            if (Run.only_computer == false)
                            {
                                if (num.First["window"].ToString() == "True")  //JSON转换bool首字母大写
                                    freeSeats.Add(num.First["id"].ToString());
                            }
                            else
                                if (num.First["window"].ToString() == "True" && num.First["computer"].ToString() != "True")
                                    freeSeats.Add(num.First["id"].ToString());
                        }
                    }

                   
                    //add 检查是否真正检索到符合条件座位
                    if (freeSeats.Count > Run.lastFreeSeatCount)
                    {
                        Run.lastFreeSeatCount = freeSeats.Count;
                        Config.config.textBox1.AppendText(roomname + " 成功检索到符合条件座位!\n");
                        return "Success";
                    }
                    else
                    {
                        Run.lastFreeSeatCount = freeSeats.Count;
                        Config.config.textBox1.AppendText(roomname + " 无符合条件座位...\n");
                        return "NoFreeSeat";
                    }
                }
                else
                {
                    Run.lastFreeSeatCount = freeSeats.Count;
                    Config.config.textBox1.AppendText(roomname + " 无符合条件座位...\n");
                    return "NoFreeSeat";
                }
           }
            catch
            {
               Config.config.textBox1.AppendText("获取空座信息出错...可能是连接丢失，也可能是其它问题，请重试\n");
                return "SomethingWrong";
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
                Config.config.textBox1.AppendText("抢座成功\n");
                Config.config.textBox1.AppendText("时间：" + jObject["data"]["onDate"].ToString() + ", " + jObject["data"]["begin"].ToString() + "~" + jObject["data"]["end"].ToString() + "\n");
                Config.config.textBox1.AppendText("地点：" + jObject["data"]["location"].ToString() + "\n");

                string msg_time = jObject["data"]["onDate"].ToString().Replace(" ", "").Remove(0, 5);
                //编辑短信内容
                if (Config.config.checkBox4.Checked)
                {
                    Config.config.textBox1.AppendText("正在编辑短信内容...\n");
                    strContent += "订座成功\n";
                    strContent += ("时间：" +  msg_time + jObject["data"]["begin"].ToString().Replace(" ", "") + "~" + jObject["data"]["end"].ToString().Replace(" ", "") + " \n");
                    strContent += ("地点：" + jObject["data"]["location"].ToString()).Replace("信息科学分馆", "") + "【RS】";
                    strMob += Config.config.textBox3.Text.ToString();
                }
                //debug
                Config.config.textBox1.AppendText("(debug)订座成功返回信息" + "\n");
                Config.config.textBox1.AppendText(jObject.ToString() + "\n");
                //Config.config.textBox1.AppendText("短信内容：" + strContent);

                return "Success";
            }
            else if (jObject["code"].ToString() == "1")  //message中文判断不准确，先用着code，不过感觉有点问题
            {
                //Config.config.textBox1.AppendText("系统尚未开放...\n");
                return "NotAtTime";
            }
            else
            {
                //debug
                Config.config.textBox1.AppendText("(debug)订座失败返回信息" + "\n");
                Config.config.textBox1.AppendText(jObject.ToString() + "\n");
                return "Failed";
            }            
        }

        public static string GetSMSNum()
        {
            string url = snum_url;
            string strRet = null;
            if (url == null || url.Trim().ToString() == "")
            {
                return strRet;
            }
            string targeturl = url.Trim().ToString();
            try
            {
                HttpWebRequest hr = (HttpWebRequest)WebRequest.Create(targeturl);
                hr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                hr.Method = "GET";
                hr.Timeout = 30 * 60 * 1000;
                WebResponse hs = hr.GetResponse();
                Stream sr = hs.GetResponseStream();
                StreamReader ser = new StreamReader(sr, Encoding.Default);
                strRet = ser.ReadToEnd();
            }
            catch (Exception ex)
            {
                strRet = null;
            }
            return strRet;
        }

        public static string SendMessage()
        {
            msg_url = msg_url + strUid  + strKey + strMob + strContent;
            string strRet = null;
            if (msg_url == null || msg_url.Trim().ToString() == "")
            {
                return strRet;
            }
            string targeturl = msg_url.Trim().ToString();
            try
            {
                HttpWebRequest hr = (HttpWebRequest)WebRequest.Create(targeturl);
                hr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                hr.Method = "GET";
                hr.Timeout = 30 * 60 * 1000;
                WebResponse hs = hr.GetResponse();
                Stream sr = hs.GetResponseStream();
                StreamReader ser = new StreamReader(sr, Encoding.Default);
                strRet = ser.ReadToEnd();
            }
            catch (Exception ex)
            {
                strRet = null;
            }
            return strRet;
        }

        public static bool GetSeats(string roomId, ArrayList seats)
        {
            string url = layout_url + roomId + "/" + DateTime.Now.ToString("yyyy-MM-dd");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            SetHeaderValues(request);
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => { return true; });
            request.Timeout = 5000;

            Config.config.textBox1.AppendText("正在获取指定房间座位信息...\n");
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                StreamReader streamReader = new StreamReader(stream, encoding);
                string json = streamReader.ReadToEnd();
                JObject jObject = JObject.Parse(json);
                Config.config.textBox1.AppendText(jObject["status"].ToString() + "\n");
                if (jObject["status"].ToString() == "success")
                {
                    JToken layout = jObject["data"]["layout"];
                    foreach (var num in layout)
                    {
                        if (num.First["type"].ToString() == "seat")
                        {
                            seats.Add(new DictionaryEntry(num.First["id"].ToString(), num.First["name"].ToString()));
                        }
                    }
                    NewComparer newComparer = new NewComparer();
                    seats.Sort(newComparer);
                    return true;
                }
                else
                {
                    Config.config.textBox1.AppendText("\r\n" + jObject.ToString());
                    return false;
                }
            }
            catch
            {
                Config.config.textBox1.AppendText("Connection lost");
                return false;
            }
        }
        
    }
}
