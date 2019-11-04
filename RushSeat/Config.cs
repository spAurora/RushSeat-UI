using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace RushSeat
{
    public partial class Config : Form
    {
        public static Config config;
        public static TimeSpan delta;

        public ArrayList startTime = new ArrayList();

        public static ArrayList xt_list = new ArrayList();

        public static char rank = 'C';

        public static bool first = true;

        public static string[] strC;

        public Config()
        {
            InitializeComponent();
            config = this;
            CheckForIllegalCrossThreadCalls = false;  //不检查其它线程对控件的非法访问
        }

        private void Config_Load(object sender, EventArgs e)
        {
           


            Config.config.textBox1.AppendText("1. 绿色、免费小软件\n");
            Config.config.textBox1.AppendText("2. 合理使用，宽以待人\n");
            Config.config.textBox1.AppendText("3. 程序使用期限至2099-8-1\n");
            Config.config.textBox1.AppendText("4. 普通用户等级为第三级，等级会影响到可以使用的功能以及关键功能延迟，如想提升等级请在源码中A级列表中加入自己学号并重新编译\n");
            Config.config.textBox1.AppendText("5. 山水一程，三生有幸，值此一别，天涯路远，流年笑掷，未来可期，我在中科院祝各位学业、工作一帆风顺！\n");
            Config.config.textBox1.AppendText("                               ——by wHy 2019-7-22\n");
            Config.config.textBox1.AppendText("\n");
            Config.config.textBox1.AppendText("注：发布群117805521，开源地址见https://github.com/spAurora/RushSeat-UI.git\n，联系本人QQ：751984964\n");
            Config.config.textBox1.AppendText("---------------------------------------\n");

            
            //Run.date = comboBox1.SelectedValue.ToString();

            
            //xt_list.Add(new DictionaryEntry("1", "只包含2~4楼和1楼云桌面"));
            xt_list.Add(new DictionaryEntry("-1", "全馆检索"));
            xt_list.Add(new DictionaryEntry("-3", "1楼所有房间"));
            xt_list.Add(new DictionaryEntry("-2", "2~4楼所有房间"));
            xt_list.Add(new DictionaryEntry("5", "一楼创新学习讨论区"));
            xt_list.Add(new DictionaryEntry("4", "一楼3C创客空间"));
            xt_list.Add(new DictionaryEntry("13", "一楼3C创客电子阅读"));
            xt_list.Add(new DictionaryEntry("14", "3C创客-双屏电脑(20台)"));
            xt_list.Add(new DictionaryEntry("15", "创新学习-MAC电脑（12台）"));
            xt_list.Add(new DictionaryEntry("16", "创新学习-云桌面（42台）"));
            xt_list.Add(new DictionaryEntry("6", "二楼西自然科学图书借阅区"));
            xt_list.Add(new DictionaryEntry("7", "二楼东自然科学图书借阅区"));
            xt_list.Add(new DictionaryEntry("8", "三楼西社会科学图书借阅区"));
            xt_list.Add(new DictionaryEntry("10", "三楼东社会科学图书借阅区"));
            xt_list.Add(new DictionaryEntry("12", "三楼自主学习区"));
            xt_list.Add(new DictionaryEntry("9", "四楼西图书阅览区"));
            xt_list.Add(new DictionaryEntry("11", "四楼东图书阅览区"));
            comboBox4.DataSource = xt_list;
            comboBox4.DisplayMember = "Value";
            comboBox4.ValueMember = "Key";
            //comboBox4.SelectedIndex = 0;


            startTime.Add(new DictionaryEntry("480", "08:00"));
            startTime.Add(new DictionaryEntry("510", "08:30"));
            startTime.Add(new DictionaryEntry("540", "09:00"));
            startTime.Add(new DictionaryEntry("570", "09:30"));
            startTime.Add(new DictionaryEntry("600", "10:00"));
            startTime.Add(new DictionaryEntry("630", "10:30"));
            startTime.Add(new DictionaryEntry("660", "11:00"));
            startTime.Add(new DictionaryEntry("690", "11:30"));
            startTime.Add(new DictionaryEntry("720", "12:00"));
            startTime.Add(new DictionaryEntry("750", "12:30"));
            startTime.Add(new DictionaryEntry("780", "13:00"));
            startTime.Add(new DictionaryEntry("810", "13:30"));
            startTime.Add(new DictionaryEntry("840", "14:00"));
            startTime.Add(new DictionaryEntry("870", "14:30"));
            startTime.Add(new DictionaryEntry("900", "15:00"));
            startTime.Add(new DictionaryEntry("930", "15:30"));
            startTime.Add(new DictionaryEntry("960", "16:00"));
            startTime.Add(new DictionaryEntry("990", "16:30"));
            startTime.Add(new DictionaryEntry("1020", "17:00"));
            startTime.Add(new DictionaryEntry("1050", "17:30"));
            startTime.Add(new DictionaryEntry("1080", "18:00"));
            startTime.Add(new DictionaryEntry("1110", "18:30"));
            startTime.Add(new DictionaryEntry("1140", "19:00"));
            startTime.Add(new DictionaryEntry("1170", "19:30"));
            startTime.Add(new DictionaryEntry("1200", "20:00"));
            startTime.Add(new DictionaryEntry("1230", "20:30"));
            startTime.Add(new DictionaryEntry("1260", "21:00"));
            startTime.Add(new DictionaryEntry("1290", "21:30"));
            startTime.Add(new DictionaryEntry("1320", "22:00"));
            comboBox2.DataSource = startTime;
            comboBox2.DisplayMember = "Value";
            comboBox2.ValueMember = "Key";
            //comboBox2.SelectedIndex = 4;  //默认10点开始


            ArrayList date = new ArrayList();
            //字典形式对应
            date.Add(new DictionaryEntry(DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd") + " (今天)"));
            date.Add(new DictionaryEntry(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " (明天)"));
            comboBox1.DataSource = date;
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            //comboBox1.SelectedIndex = 1;

            //if (File.Exists(@"configuration.txt"))
            //{
            //    strC = File.ReadAllLines(@"configuration.txt");
            //    config.comboBox4.SelectedIndex = Convert.ToInt32(strC[0]);
            //    config.comboBox5.SelectedIndex = Convert.ToInt32(strC[1]);
            //    config.comboBox1.SelectedIndex = Convert.ToInt32(strC[2]);
            //    config.comboBox2.SelectedIndex = Convert.ToInt32(strC[3]);
            //    config.comboBox3.SelectedIndex = Convert.ToInt32(strC[4]);
            //}

            
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (checkBox4.Checked)
            {
                    string[] strs = { textBox3.Text.ToString()};
                    File.WriteAllLines(@"telnumber.txt", strs);
            }

            if (config.checkBox1.Checked)
                Run.only_window = true;
            else
                Run.only_window = false;
            if (config.checkBox2.Checked)
                Run.only_computer = true;
            else
                Run.only_computer = false;

            if (button1.Text == "开始抢座")
            {
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                comboBox5.Enabled = false;
                button1.Enabled = false;

                //保存配置信息
                string[] strsC = { config.comboBox4.SelectedIndex.ToString(), config.comboBox5.SelectedIndex.ToString(), config.comboBox1.SelectedIndex.ToString(), config.comboBox2.SelectedIndex.ToString(), config.comboBox3.SelectedIndex.ToString() };
                File.WriteAllLines(@"configuration.txt", strsC);

                backgroundWorker2.RunWorkerAsync();
                RushSeat.stop_waiting = false;
                Run.roomID = comboBox4.SelectedValue.ToString();
                Run.startTime = comboBox2.SelectedValue.ToString();
                Run.endTime = comboBox3.SelectedValue.ToString();
                //在22:45之前预约明天的
                if (Config.config.comboBox1.SelectedIndex == 1 && DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 22:44:50")) < 0)
                {
                    button1.Text = "结束等待";
                    Run.date = comboBox1.SelectedValue.ToString();
                    //防止系统拥堵，不同等级加入不同延迟
                    Random rd = new Random();

                    /****这里保留了对D级用户的尊重****/
                    //if (rank == 'C')
                    //    Run.waitsecond = rd.Next(7, 10); //
                    //else if (rank == 'A')
                    //    Run.waitsecond = 0;
                    //else if (rank == 'B')
                    //    Run.waitsecond = rd.Next(3, 5); //
                    //else
                    //{
                    //    Run.waitsecond = 3600;
                    //}

                    //RushSeat.Wait("22", "45", Run.waitsecond.ToString());
                    textBox1.AppendText("将于22:44:50重新获取登录令牌\n");
                    RushSeat.Wait("22", "44", "50");
                    Thread.Sleep(1000);
                    //如果是用户停止等待
                    if (RushSeat.stop_waiting)
                    {
                        //RushSeat.stop_waiting = false;
                        return;
                    }
                    //正常等待结束,重新登录
                    string response = RushSeat.GetToken(true);
                    if (response == "Success")
                    {
                        textBox1.AppendText("重新获取登录令牌成功!\n");
                        if (rank == 'A')
                            RushSeat.WaitNew("22", "44", "59");
                        else
                        //18-12-24 现在所有用户处于同一起跑线上
                        RushSeat.WaitNew("22", "44", "59");

                        //极速模式,此模式下不作任何其它判断，直接抢想要的座位
                        if (comboBox4.SelectedIndex != 0 && comboBox4.SelectedIndex != 1 && comboBox4.SelectedIndex != 2 && comboBox5.SelectedIndex != 0 && checkBox6.Checked == true)
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                if (RushSeat.BookSeat(comboBox5.SelectedValue.ToString(), Run.date, Run.startTime, Run.endTime) == "Success")
                                {
                                    //textBox1.AppendText("急速模式下抢座成功！\n");
                                    //静默检查预约信息，激活释放按钮
                                    RushSeat.CheckHistoryInf(false);

                                    //窗口弹出
                                    if (Config.config.Visible != true)
                                        Config.config.Visible = true;
                                    Config.config.WindowState = FormWindowState.Normal;

                                    //发短信
                                    if (Config.config.checkBox4.Checked)
                                    {
                                        Config.config.textBox1.AppendText("短信已发送，返回值：\n" + RushSeat.SendMessage() + "\n");
                                        Config.config.textBox1.AppendText("若返回值小于0为发送失败，请联系开发者\n");
                                        Config.config.textBox1.AppendText("------------------------------------------\n");
                                    }

                                    if (Config.config.checkBox3.Checked)
                                    {
                                        Config.config.textBox1.AppendText("2min后自动关机\n");
                                        Config.config.textBox1.AppendText("如果想取消自动关机请在桌面用快捷键win + R启动控制台, 在控制台自行输入 shutdown -a\n");
                                        Config.config.textBox1.AppendText("-----------------------------------------------------\n");
                                        Process.Start("shutdown.exe", "-s -t " + "120");
                                    }
                                    else
                                    {
                                        //Config.config.textBox1.AppendText("订座成功");
                                    }

                                    Config.config.button1.Text = "开始抢座";
                                    Config.config.comboBox1.Enabled = true;
                                    Config.config.comboBox2.Enabled = true;
                                    Config.config.comboBox3.Enabled = true;
                                    Config.config.comboBox4.Enabled = true;
                                    Config.config.comboBox5.Enabled = true;

                                    return;
                                }
                                //0.5s后再次尝试抢倾向座位
                                textBox1.AppendText("急速抢座失败，0.5s后再次尝试...");
                                Thread.Sleep(500);
                            }
                            textBox1.AppendText("急速抢座失败，转入普通模式");
                        }

                        //如果是用户停止等待
                        if (RushSeat.stop_waiting)
                        {
                            //RushSeat.stop_waiting = false;
                            return;
                        }
                        //启动抢座进程
                        Run.Start();
                    }
                    else
                    {
                        textBox1.AppendText("重新获取登录令牌失败，相应信息如下：\n");
                        textBox1.AppendText(response);
                    }
                }
                //在1点之前预约今天的
                else if(DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 01:00:00")) < 0 && Config.config.comboBox1.SelectedIndex == 0)
                {
                    button1.Text = "结束等待";
                    Run.date = comboBox1.SelectedValue.ToString();
                    Random rd = new Random();
                    if (rank == 'C')
                        Run.waitsecond = rd.Next(7, 10);
                    else if (rank == 'A')
                        Run.waitsecond = rd.Next(0, 2);
                    else if (rank == 'B')
                        Run.waitsecond = rd.Next(3, 5);
                    else
                    {
                        Run.waitsecond = 3600;
                    }
                    RushSeat.Wait("01", "00", Run.waitsecond.ToString());
                    //如果是用户停止等待
                    if (RushSeat.stop_waiting)
                    {
                        //RushSeat.stop_waiting = false;
                        return;
                    }
                    //正常等待结束,重新登录
                    string response = RushSeat.GetToken(true);
                    if (response == "Success")
                    {
                        textBox1.AppendText("再次登录成功!\n");
                        Run.Start();
                    }
                    else
                    {
                        textBox1.AppendText(response);
                    }
                }

                //在1:00 之后 22:00之前预约今天的
                else if (DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 01:00:00")) > 0 && DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 22:00:00")) < 0 && Config.config.comboBox1.SelectedIndex == 0)
                {
                    button1.Text = "结束抢座";
                    Run.date = comboBox1.SelectedValue.ToString();
                    Run.Start();
                }
                //在22：45之后 23:50之前预约明天的
                else if (DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 22:45:00")) > 0 && Config.config.comboBox1.SelectedIndex == 1 && DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 23:50:00")) < 0)
                {
                    button1.Text = "结束抢座";
                    Run.date = comboBox1.SelectedValue.ToString();
                    Run.Start();
                }
                //在23:50后预约明天的
                else if (DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 23:50:00")) > 0)
                {
                    textBox1.AppendText("预约时间已过");
                }
                return;
            }
            if (button1.Text == "结束等待")
            {
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
                button1.Enabled = false;
                backgroundWorker2.RunWorkerAsync();
                RushSeat.CheckHistoryInf(false);
                RushSeat.stop_waiting = true;
                button1.Text = "开始抢座";
                return;
            }
            if (button1.Text == "结束抢座")
            {
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
                button1.Enabled = false;
                backgroundWorker2.RunWorkerAsync();
                RushSeat.CheckHistoryInf(false);
                RushSeat.stop_rush = true;
                button1.Text = "开始抢座";
                return;
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            first = true;
            while (true)
           {
               delta = RushSeat.time.Subtract(DateTime.Now);
               backgroundWorker1.ReportProgress(0, "");
                if(e.Cancel)
                {
                    return;
                }
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                Thread.Sleep(1000);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //MessageBox.Show((string)e.UserState);
            
            if (first)
            {
                //int index = textBox1.GetFirstCharIndexOfCurrentLine();
                textBox1.AppendText("\r\n倒计时器启动(次日预约开放时间 22:45 当日预约开放时间 1:00)：\r\n");
                first = false;
            }
            else
            {
                //int start = textBox1.GetFirstCharIndexFromLine(0);//第一行第一个字符的索引
                //int end = textBox1.GetFirstCharIndexFromLine(1);//第二行第一个字符的索引
                //textBox1.Select(start, end);//选中第一行
                //textBox1.SelectedText = "";//设置第一行的内容为空
                //textBox1.AppendText("剩余时间: " + (delta.ToString()).Substring(0, 8) + "\n");
                int index = textBox1.GetFirstCharIndexOfCurrentLine();
                textBox1.Select(index-1, textBox1.TextLength - index);
                textBox1.SelectedText = "\r\n剩余时间: " + (delta.ToString()).Substring(0, 8) + "\r\n";
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (RushSeat.stop_waiting == true)
                textBox1.AppendText("用户中止等待...\n");
            else
                textBox1.AppendText("令牌获取等待完成...\n");
        }

        private void Config_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void comboBox2_StyleChanged(object sender, EventArgs e)
        {
           
        }

        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            ArrayList endTime = new ArrayList();
            int i;
            for (i = comboBox2.SelectedIndex + 1; i <= 28; i++)
            {
                endTime.Add(startTime[i]);
            }
            endTime.Add(new DictionaryEntry("1350", "22:30"));
            comboBox3.DataSource = endTime;
            comboBox3.DisplayMember = "Value";
            comboBox3.ValueMember = "Key";
            int length = endTime.Count;
            comboBox3.SelectedIndex = length - 1;  //默认晚上10:30点结束
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "取消预约")
            {
                if(RushSeat.CancelReservation(RushSeat.resID))
                    button2.Enabled = false;
            }
            if (button2.Text == "结束使用")
            {
                if(RushSeat.StopUsing())
                    button2.Enabled = false;
            }
            if (button2.Text != "取消预约" && button2.Text != "结束使用")
                config.textBox1.AppendText("程序出现逻辑BUG，请联系开发者");
        }

        private void Config_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();   //隐藏窗体
                notifyIcon1.Visible = true; //使托盘图标可见
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox4.SelectedIndex != 0 && comboBox4.SelectedIndex != 1 && comboBox4.SelectedIndex != 2)
            {
                ArrayList seats = new ArrayList();
                if (comboBox4.SelectedValue.GetType() == typeof(string))
                {
                    if (int.Parse(comboBox4.SelectedValue.ToString()) > 1)
                    {
                        RushSeat.GetSeats(comboBox4.SelectedValue.ToString(), seats);
                    }
                }
                seats.Insert(0, new DictionaryEntry("0", "(从0开始顺序检索)"));
                comboBox5.DataSource = seats;
                comboBox5.DisplayMember = "Value";
                comboBox5.ValueMember = "Key";
            }
            else
            {
                comboBox5.DataSource = null;
                comboBox5.Items.Clear();
                comboBox5.Items.Add("特殊模式下该项不可用");
                comboBox5.SelectedIndex = 0;
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            DateTime now = DateTime.Now;
            while (true)
            {
                delta = DateTime.Now.Subtract(now);
                if (Convert.ToInt32(delta.ToString().Substring(7, 1)) > 1)
                {
                    button1.Enabled = true;
                    return;
                }
                Thread.Sleep(200);
            }
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //预约今天
            if (comboBox1.SelectedIndex == 0)
            {
                int index_shift = 0;
                bool mark = false;
                foreach (DictionaryEntry item in startTime)
                {
                    
                    //如果key值和当前时间比较起来偏小，偏移值+1
                    if (DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + item.Value.ToString() + ":00"), DateTime.Now) < 0)
                        index_shift++;
                    else
                    {
                        mark = true;
                        break;
                    }
                }
                if (mark)
                    comboBox2.SelectedIndex = index_shift;
                else
                    comboBox2.SelectedIndex = index_shift - 1;
            }
            else  //预约明天
            {
                comboBox2.SelectedIndex = 0;
            }

        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            first = true;
            while (true)
            {
                delta = RushSeat.time.Subtract(DateTime.Now);
                backgroundWorker3.ReportProgress(0, "");
                if (e.Cancel)
                {
                    return;
                }
                if (backgroundWorker3.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                if (backgroundWorker3.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                Thread.Sleep(1000);
            }
        }

        private void backgroundWorker3_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //MessageBox.Show((string)e.UserState);

            if (first)
            {
                //int index = textBox1.GetFirstCharIndexOfCurrentLine();
                //textBox1.AppendText("\r\n倒计时器启动(次日预约开放时间 22:45 当日预约开放时间 1:00)：\r\n");
                textBox1.AppendText("\n10s后开始抢座...\n");
                first = false;
            }
            else
            {
                //int start = textBox1.GetFirstCharIndexFromLine(0);//第一行第一个字符的索引
                //int end = textBox1.GetFirstCharIndexFromLine(1);//第二行第一个字符的索引
                //textBox1.Select(start, end);//选中第一行
                //textBox1.SelectedText = "";//设置第一行的内容为空
                //textBox1.AppendText("剩余时间: " + (delta.ToString()).Substring(0, 8) + "\n");
                //int index = textBox1.GetFirstCharIndexOfCurrentLine();
                //textBox1.Select(index - 1, textBox1.TextLength - index);
                //textBox1.SelectedText = "\r\n剩余时间: " + (delta.ToString()).Substring(0, 8) + "\r\n";
            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (RushSeat.stop_waiting == true)
                textBox1.AppendText("用户中止抢座...\n");
            else
                textBox1.AppendText("抢座等待完成...\n");
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
