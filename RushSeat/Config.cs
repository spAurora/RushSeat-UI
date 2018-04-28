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

namespace RushSeat
{
    public partial class Config : Form
    {
        public static Config config;
        public static TimeSpan delta;

        public ArrayList startTime = new ArrayList();

        public Config()
        {
            InitializeComponent();
            config = this;
            CheckForIllegalCrossThreadCalls = false;  //不检查其它线程对控件的非法访问
        }

        private void Config_Load(object sender, EventArgs e)
        {

            Config.config.textBox1.AppendText("软件作者本意只是为了方便学习，无意对预约系统造成任何不良影响\n");
            Config.config.textBox1.AppendText("请用户不要尝试其它对预约系统的破坏行为\n");
            Config.config.textBox1.AppendText("程序代码已经开源，详情见https://github.com/spAurora/RushSeat-UI.git\n");
            Config.config.textBox1.AppendText("---------------------------------------\n");


            if (config.checkBox1.Checked)
                Run.only_window = "true";
            if (config.checkBox2.Checked)
                Run.only_window = "true";
            //Run.date = comboBox1.SelectedValue.ToString();

            ArrayList xt_list = new ArrayList();
            xt_list.Add(new DictionaryEntry("1", "只包含2~4楼和1楼云桌面"));
            xt_list.Add(new DictionaryEntry("5", "一楼创新学习讨论区"));
            xt_list.Add(new DictionaryEntry("4", "一楼3C创客空间"));
            xt_list.Add(new DictionaryEntry("14", "3C创客-双屏电脑"));
            xt_list.Add(new DictionaryEntry("15", "创新学习-MAC电脑"));
            xt_list.Add(new DictionaryEntry("16", "创新学习-云桌面"));
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


            ArrayList date = new ArrayList();
            //字典形式对应
            date.Add(new DictionaryEntry(DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd") + " (今天)"));
            date.Add(new DictionaryEntry(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " (明天)"));
            comboBox1.DataSource = date;
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.SelectedIndex = 1;

            startTime.Add(new DictionaryEntry("480", "8:00"));
            startTime.Add(new DictionaryEntry("510", "8:30"));
            startTime.Add(new DictionaryEntry("540", "9:00"));
            startTime.Add(new DictionaryEntry("570", "9:30"));
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
            comboBox2.DataSource = startTime;
            comboBox2.DisplayMember = "Value";
            comboBox2.ValueMember = "Key";
            comboBox2.SelectedIndex = 4;  //默认10点开始

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "开始抢座")
            {
                Run.roomID = comboBox4.SelectedValue.ToString();
                Run.startTime = comboBox2.SelectedValue.ToString();
                Run.endTime = comboBox3.SelectedValue.ToString();
                //在22:15之前预约明天的
                if (Config.config.comboBox1.SelectedIndex == 1 && DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 22:15:00")) < 0)
                {
                    button1.Text = "结束等待";
                    Run.date = comboBox1.SelectedValue.ToString();
                    RushSeat.Wait("22", "15", "10");
                    //如果是用户停止等待
                    if (RushSeat.stop_waiting)
                    {
                        RushSeat.stop_waiting = false;
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
                
                //在22:00之前预约今天的
                else if (DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 22:00:00")) < 0 && Config.config.comboBox1.SelectedIndex == 0)
                {
                    button1.Text = "结束抢座";
                    Run.date = comboBox1.SelectedValue.ToString();
                    Run.Start();
                }
                //在22：15之后预约明天的
                else if (DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 22:15:00")) > 0 && Config.config.comboBox1.SelectedIndex == 1 && DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 23:45:00")) < 0)
                {
                    button1.Text = "结束抢座";
                    Run.date = comboBox1.SelectedValue.ToString();
                    Run.Start();
                }
                //在23:45后预约明天的
                else if (DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 23:45:00")) > 0)
                {
                    textBox1.AppendText("预约时间已过");
                }
                return;
            }
            if (button1.Text == "结束等待")
            {
                RushSeat.stop_waiting = true;
                button1.Text = "开始抢座";
                return;
            }
            if (button1.Text == "结束抢座")
            {
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
                textBox1.AppendText("等待完成...");
            else
                textBox1.AppendText("用户中止等待...");
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
            endTime.Add(new DictionaryEntry("1320", "22:00"));
            comboBox3.DataSource = endTime;
            comboBox3.DisplayMember = "Value";
            comboBox3.ValueMember = "Key";
            int length = endTime.Count;
            comboBox3.SelectedIndex = length - 1;  //默认晚上10点结束
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
    }
}
