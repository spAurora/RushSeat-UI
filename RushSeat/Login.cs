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
using System.Threading;

namespace RushSeat
{
    public partial class Login : Form
    {

        public static Login login;

        public static string[] strs1;
        public static string[] strs2;

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
                string[] strs = {DES.EncryptDES(textBox1.Text.ToString(), "shAurora"), DES.EncryptDES(textBox2.Text.ToString(), "shAurora")};
                File.WriteAllLines(@"userInfo.txt", strs);

            }

            Hide();
            Config config = new Config();
            config.Show();

            string response = "empty";
            int tryNum = 1, col = 0;
            while (response != "Success" && tryNum != 5)
            {
                
                response = RushSeat.GetToken(true);
                if (response == "Success")
                {
                    //Hide();
                    //Config config = new Config();
                    //config.Show();
                    col = 1;
                  

                    config.textBox1.AppendText("登录成功!\n");
                    Config.config.textBox1.AppendText("---------------------------------------\n");
                    //获取各个房间的座位列表
                    config.comboBox4.SelectedIndex = 0;
                    config.comboBox2.SelectedIndex = 4;  //默认10点开始
                    config.comboBox1.SelectedIndex = 1;
                    //加载
                    if (File.Exists(@"configuration.txt"))
                    {
                        Config.strC = File.ReadAllLines(@"configuration.txt");
                        config.comboBox4.SelectedIndex = Convert.ToInt32(Config.strC[0]);
                        config.comboBox5.SelectedIndex = Convert.ToInt32(Config.strC[1]);
                        config.comboBox1.SelectedIndex = Convert.ToInt32(Config.strC[2]);
                        config.comboBox2.SelectedIndex = Convert.ToInt32(Config.strC[3]);
                        config.comboBox3.SelectedIndex = Convert.ToInt32(Config.strC[4]);
                    }
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
                else
                {
                    config.textBox1.AppendText("第"+ tryNum +"次登录失败，3s后重试\n");
                    config.textBox1.AppendText("[info]"+ response + "\n");
                    for (int i = 0; i < 10; i++)
                    {
                        Application.DoEvents();
                        //防止控件假死
                        Thread.Sleep(300);
                    }                   
                    tryNum++;
                }
            }
            if (col == 0)
            {
                config.textBox1.AppendText("尝试登录失败次数过多，请检查网络连接或服务器状态");
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            //检查程序是否过期
            if (DateTime.Compare(DateTime.Now, Convert.ToDateTime("2019-8-1" + " 00:00:00")) > 0)
            {
                MessageBox.Show("使用期已过！请进入发布群获取最新版");
                System.Environment.Exit(0);
            }

            if (File.Exists(@"userInfo.txt"))
            {
                strs1 = File.ReadAllLines(@"userInfo.txt");
                textBox1.Text = DES.DecryptDES(strs1[0], "shAurora");
                textBox2.Text = DES.DecryptDES(strs1[1], "shAurora");     
            }
            else
            {
                
            }

            //读取B级列表，加入B级权限组
            if (File.Exists(@"rankBList.txt"))
            {
                strs2 = File.ReadAllLines(@"rankBList.txt");
                foreach(string i in strs2)
                {
                    RushSeat.rankBList.Add(DES.DecryptDES(i, "wTsunami"));   
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
