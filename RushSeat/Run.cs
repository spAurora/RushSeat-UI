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
             RushSeat.SearchFreeSeat(buildingID, roomID, date, startTime, endTime);
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
            if(success)
            {
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

            }
         }
    }
}
