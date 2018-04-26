﻿using System;
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

        private bool first = true;
        private int index;
        public Config()
        {
            InitializeComponent();
            config = this;
            CheckForIllegalCrossThreadCalls = false;  //不检查其它线程对控件的非法访问
        }

        private void Config_Load(object sender, EventArgs e)
        {

            if (config.checkBox1.Checked)
                Run.only_window = "true";
            if (config.checkBox2.Checked)
                Run.only_window = "true";
            //Run.date = comboBox1.SelectedValue.ToString();
            Run.roomID = textBox3.Text;
            Run.startTime = textBox4.Text;
            Run.endTime = textBox5.Text;

            

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
            //在22:15之前预约明天的
            if (Config.config.comboBox1.SelectedIndex == 1 && DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 22:15:00")) < 0)  
            {
                Run.date = comboBox1.SelectedValue.ToString();
                RushSeat.Wait("22", "15", "10");

                //重新登录
                string response = RushSeat.GetToken(true);
                if (response == "Success")
                {
                    //是否已预约检查
                    if (RushSeat.CheckHistoryInf() == true)
                    {
                        textBox1.AppendText("再次登录成功!\n");
                        Run.Start();
                    }
                }
                else
                {
                    textBox1.AppendText(response);
                }      
            }
            //在22:00之前预约今天的
            else if (DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 22:00:00")) < 0 && Config.config.comboBox1.SelectedIndex == 0) 
            {
                Run.date = comboBox1.SelectedValue.ToString();
                Run.Start();
            }
            //在22：15之后预约明天的
            else if (DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 22:15:00")) > 0 && Config.config.comboBox1.SelectedIndex == 1 && DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 23:45:00")) < 0)
            {
                Run.date = comboBox1.SelectedValue.ToString();
                Run.Start();
            }
            //在23:45后预约明天的
            else if (DateTime.Compare(DateTime.Now, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 23:45:00")) > 0)
            {
                textBox1.AppendText("预约时间已过");
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
           textBox1.AppendText("剩余时间: " + delta.ToString()+ "\n");
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           textBox1.AppendText("等待完成...");
        }

        private void Config_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
