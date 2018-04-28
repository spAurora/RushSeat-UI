using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace RushSeat
{
    class Run
    {
        
        public static string startTime = "";  //单位是分钟，12点是720
        public static string endTime = "";    //13点
        public static string buildingID = "1";
        public static string roomID = "4";  //6是三楼西，4是一楼某个区域
        public static string date = "2018-04-23";  //yyyy-mm-dd
        public static string only_window = "false";
        public static string only_conputer = "false";

        private static Thread thread;

        private static bool success = false;

         public static void Start()
        {

            thread = new Thread(run);
            thread.IsBackground = true;
            thread.Start();
        }

        public static void run()
         {
             Config.config.button2.Enabled = false;
             int count = 0;
             bool get_list = false;
             while (true)
             {
                 if (RushSeat.stop_rush == true)
                 {
                     Config.config.textBox1.AppendText("用户取消抢座\n");
                     Config.config.button1.Text = "开始抢座";
                     RushSeat.stop_rush = false;
                     return;
                 }
                 Config.config.textBox1.AppendText("即将开始第 " + (++count).ToString() + " 次检索...\n");
                 if (RushSeat.SearchFreeSeat(buildingID, roomID, date, startTime, endTime) == "Success")
                 {
                     Config.config.textBox1.AppendText("检索到符合条件空座列表，开始尝试预约...\n");
                     get_list = true;
                 }

                 //如果检索到空座
                 if (get_list == true)
                 {
                     //先释放当前座位
                     string resInfo = RushSeat.CheckHistoryInf(false);
                     if (resInfo == "RESERVE")
                     {
                         //Config.config.textBox1.AppendText("正在释放当前座位...\n");

                         if(RushSeat.CancelReservation(RushSeat.resID) != true)
                         {
                             Config.config.textBox1.AppendText("请手动重试...");
                             return;
                         }
                     }
                     if (resInfo == "CHECK_IN" || resInfo == "AWAY")
                     {
                         if (RushSeat.StopUsing() != true)
                         {
                             Config.config.textBox1.AppendText("请手动重试...");
                             return;
                         }
                     }

                     foreach (string seatID in RushSeat.freeSeats)
                     {
                         if (RushSeat.BookSeat(seatID, date, startTime, endTime) == "Success")
                         {
                             success = true;
                             break;
                         }
                         Thread.Sleep(500);
                         Config.config.textBox1.AppendText("座位ID " + seatID.ToString() + " 预约失败,尝试预约下一个座位");
                     }
                     //成功抢座后自动关机
                     if (success == true)
                     {
                         //静默检查预约信息，激活释放按钮
                         RushSeat.CheckHistoryInf(false);

                         if (Config.config.checkBox3.Checked)
                         {
                             Config.config.textBox1.AppendText("2min后自动关机");
                             Config.config.textBox1.AppendText("如果想取消自动关机请在控制台自行输入 shutdown -a");
                             Process.Start("shutdown.exe", "-s -t " + "120");
                         }
                         else
                         {
                             //Config.config.textBox1.AppendText("订座成功");
                         }
                         Config.config.button1.Text = "开始抢座";
                         break;
                     }
                     else
                     {
                         //有空座但是抢座失败(别人手快)
                         Config.config.textBox1.AppendText("*****预约失败，即将重新开始检索...*****\n");
                         get_list = false;
                     }
                 }
                 Thread.Sleep(1000);
             }
         }
    }
}
