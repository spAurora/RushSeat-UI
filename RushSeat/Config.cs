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
        public Config()
        {
            InitializeComponent();
            config = this;
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
            //预约明天的
            if (Config.config.comboBox1.SelectedIndex == 1)
            {
                Run.date = comboBox1.SelectedValue.ToString();
                RushSeat.Wait("22", "15", "00");
                Run.Start();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int index = textBox2.GetFirstCharIndexOfCurrentLine();
            bool first = true;
            while (true)
            {
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                TimeSpan delta = RushSeat.time.Subtract(DateTime.Now);
                if ((bool)e.Argument)
                {
                    if (first)
                    {
                        textBox2.AppendText("\r\n\r\n正在等待系统开放，剩余" + ((int)delta.TotalSeconds).ToString() + "秒\r\n");
                        first = false;
                    }
                    else
                    {
                        textBox2.Select(index, textBox2.TextLength - index - 1);
                        textBox2.SelectedText = "\r\n\r\n正在等待系统开放，剩余" + ((int)delta.TotalSeconds).ToString() + "秒\r\n";
                    }
                }
                else
                {
                    if (first)
                    {
                        textBox2.AppendText("\r\n正在等待系统开放，剩余" + ((int)delta.TotalSeconds).ToString() + "秒\r\n");
                        first = false;
                    }
                    else
                    {
                        textBox2.Select(index, textBox2.TextLength - index - 1);
                        textBox2.SelectedText = "\r\n正在等待系统开放，剩余" + ((int)delta.TotalSeconds).ToString() + "秒\r\n";
                    }
                }
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                Thread.Sleep(1000);
            }
        }
    }
}
