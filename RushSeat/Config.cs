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

        private bool first = true;
        private int index;
        public Config()
        {
            InitializeComponent();
            config = this;
            //CheckForIllegalCrossThreadCalls = false;  //不检查其它线程对控件的非法访问
        }

        private void Config_Load(object sender, EventArgs e)
        {
            backgroundWorker2.RunWorkerAsync();

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
                RushSeat.Wait("23", "15", "00");
                Run.Start();
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
            // while (true)
           {
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.ReportProgress(0, "");
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
            MessageBox.Show((string)e.UserState);
           // textBox1.AppendText("12312");
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBox1.AppendText("12312");
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            int i = 0;
            backgroundWorker2.ReportProgress(0, "");
            i++;
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("666");
            textBox1.AppendText("1");
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            textBox1.AppendText("2");
        }
    }
}
