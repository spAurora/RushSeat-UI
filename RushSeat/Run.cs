using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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

       

         public static void Start()
        {
            RushSeat.SearchFreeSeat(buildingID, roomID, date, startTime, endTime);
            foreach (string seatID in RushSeat.freeSeats)
            {
                if (RushSeat.BookSeat(seatID, date, startTime, endTime) == "Success")
                    return;
                Thread.Sleep(5000);
                Config.config.textBox1.AppendText("座位ID " + seatID.ToString() + " 预约失败,尝试预约下一个座位");
            }
        }
    }
}
