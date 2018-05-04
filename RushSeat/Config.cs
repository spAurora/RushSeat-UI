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

namespace RushSeat
{
    public partial class Config : Form
    {
        public static Config config;
        public static TimeSpan delta;

        public ArrayList startTime = new ArrayList();

        public static char rank = 'C';

        public Config()
        {
            InitializeComponent();
            config = this;
            CheckForIllegalCrossThreadCalls = false;  //不检查其它线程对控件的非法访问
        }

        private void Config_Load(object sender, EventArgs e)
        {
           


            Config.config.textBox1.AppendText("1. 软件作者本意只是为了方便学习，无意对预约系统造成任何不良影响\n");
            Config.config.textBox1.AppendText("2. 程序中涉及的个人信息已经过DES加密处理\n");
            Config.config.textBox1.AppendText("3. 程序试用期至本学期结束，未来会视情况而定\n");
            Config.config.textBox1.AppendText("4. 普通用户等级为第三级，等级会影响到可以使用的功能以及关键功能延迟(等级后面的2个数字单位为毫秒)，如想提升等级请\"收集程序BUG或者提出其它改进建议\"，并联系QQ：751984964∩ω∩\n");
            Config.config.textBox1.AppendText("5. 程序完全免费，祝大家用的开心●ω●\n");

            //Config.config.textBox1.AppendText("程序代码已经开源，详情见https://github.com/spAurora/RushSeat-UI.git\n");
            Config.config.textBox1.AppendText("---------------------------------------\n");

            
            //Run.date = comboBox1.SelectedValue.ToString();

            ArrayList xt_list = new ArrayList();
            xt_list.Add(new DictionaryEntry("1", "只包含2~4楼和1楼云桌面"));
            xt_list.Add(new DictionaryEntry("5", "一楼创新学习讨论区"));
            xt_list.Add(new DictionaryEntry("4", "一楼3C创客空间"));
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
            comboBox4.SelectedIndex = 8;


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
           // comboBox2.SelectedIndex = 4;  //默认10点开始


            ArrayList date = new ArrayList();
            //字典形式对应
            date.Add(new DictionaryEntry(DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd") + " (今天)"));
            date.Add(new DictionaryEntry(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " (明天)"));
            comboBox1.DataSource = date;
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.SelectedIndex = 1;

            

            
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
                backgroundWorker2.RunWorkerAsync();
                RushSeat.stop_waiting = false;
                Run.roomID = comboBox4.SelectedValue.ToString();
                Run.startTime = comboBox2.SelectedValue.ToString();
                Run.endTime = comboBox3.SelectedValue.ToString();
                //在22:45之前预约明天的
                if (Config.config.comboBox1.SelectedIndex == 1 && DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 22:45:00")) < 0)
                {
                    button1.Text = "结束等待";
                    Run.date = comboBox1.SelectedValue.ToString();
                    //防止系统拥堵，不同等级加入不同延迟
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
                    RushSeat.Wait("22", "45", Run.waitsecond.ToString());
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
            while (true)
           {
               delta = RushSeat.time.Subtract(DateTime.Now);
               backgroundWorker1.ReportProgress(0, "");
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
           textBox1.AppendText("剩余时间: " + (delta.ToString()).Substring(0, 8)+ "\n");
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (RushSeat.stop_waiting == true)
                textBox1.AppendText("用户中止等待...\n");
            else
                textBox1.AppendText("等待完成...\n");
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
            for (i = comboBox2.SelectedIndex + 1; i <= 27; i++)
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
    }
}
