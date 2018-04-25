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
            Run.date = comboBox1.SelectedValue.ToString();
            Run.Start();
        }
    }
}
