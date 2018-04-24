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
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string response = RushSeat.GetToken(true);
            if (response == "Success")
            {
                //是否已预约检查
                if (RushSeat.CheckHistoryInf() == true)
                {
                    Config config = new Config();
                    config.Show();
                    config.textBox1.AppendText("登录成功!\n");
                    RushSeat.GetUserInfo();
                }
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
