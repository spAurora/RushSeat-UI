using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RushSeat
{
    public partial class Login : Form
    {

        public static Login login;

        public Login()
        {
            InitializeComponent();
            login = this;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RushSeat.studentID = textBox1.Text;
            RushSeat.password = textBox2.Text;
            string response = RushSeat.GetToken(true);
            if (response == "Success")
            {
                    Hide();
                    Config config = new Config();
                    config.Show();
                    config.textBox1.AppendText("登录成功!\n");
                    if (RushSeat.CheckHistoryInf() == "NO")
                    {
                        config.textBox1.AppendText("当前无有效预约\n");
                    }
                    else  //已经有有效预约
                    {
                        
                    }
                    RushSeat.GetUserInfo();
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
