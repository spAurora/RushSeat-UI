using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace RushSeat
{
    public partial class Login : Form
    {

        public static Login login;

        public static string[] strs1;

        public Login()
        {
            InitializeComponent();
            login = this;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            RushSeat.studentID = textBox1.Text.ToString();
            RushSeat.password = textBox2.Text.ToString();

            if (checkBox1.Checked)
            {
                string[] strs = {textBox1.Text.ToString(), textBox2.Text.ToString()};
                File.WriteAllLines(@"saveInfo.txt", strs);

            }

            string response = RushSeat.GetToken(true);
            if (response == "Success")
            {
                    Hide();
                    Config config = new Config();
                    config.Show();
                    config.textBox1.AppendText("登录成功!\n");
                    RushSeat.GetUserInfo();
                    if (RushSeat.CheckHistoryInf(true) == "NO")
                    {
                        config.textBox1.AppendText("当前无有效预约\n");
                    }
                    else  //已经有有效预约
                    {
                        
                    }
                    Config.config.textBox1.AppendText("剩余可发送短信数目：" + RushSeat.GetSMSNum() + "\n");
                    Config.config.textBox1.AppendText("---------------------\n");
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            if (File.Exists(@"saveInfo.txt"))
            {
                strs1 = File.ReadAllLines(@"saveInfo.txt");
                textBox1.Text = strs1[0];
                textBox2.Text = strs1[1];
            }
            else
            {
                
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
