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

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Run.Start();
        }
    }
}
