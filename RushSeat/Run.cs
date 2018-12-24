using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace RushSeat
{
    class Run
    {
        
        public static string startTime = "";  //单位是分钟，12点是720
        public static string endTime = "";    //13点
        public static string buildingID = "1";
        public static string roomID = "4";  //6是三楼西，4是一楼某个区域
        public static string date = "2018-04-23";  //yyyy-mm-dd
        public static bool only_window = false;
        public static bool only_computer = false;

        //获取空座列表后的等待延迟
        public static int rankSuccessGetFreeSeat = 600;
        public static int repeatSearchInterval = 3000;

        public static int preventCount = 0;
        public static int lastFreeSeatCount = 0;

        public static int waitsecond;
        
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
             int wrong_count = 0;
             success = false;
             
             while (true)
             {
                 bool get_list = false;  //逻辑BUG 每次循环应重置
                 if (RushSeat.stop_rush == true)
                 {
                     Config.config.textBox1.AppendText("用户取消抢座\n");
                     Config.config.textBox1.AppendText("-------------------------------------------\n");
                     Config.config.button1.Text = "开始抢座";
                     RushSeat.stop_rush = false;
                     return;
                 }
                 Config.config.textBox1.AppendText("即将开始第 " + (++count).ToString() + " 次检索...\n");
                 //移除之前的空座列表
                 RushSeat.freeSeats.Clear();
                 lastFreeSeatCount = RushSeat.freeSeats.Count;
                 //单个房间检索
                 if (Config.config.comboBox4.SelectedIndex != 0 && Config.config.comboBox4.SelectedIndex != 1 && Config.config.comboBox4.SelectedIndex != 2)
                 {
                     //获取房间空座列表
                     string stat = "";
                     stat = RushSeat.SearchFreeSeat(buildingID, roomID, date, startTime, endTime);

                     //成功检索到空座以及没有符合条件座位但是勾选了改抢附近座位
                     if (stat == "Success" || (stat == "NoMatchSeat" && Config.config.checkBox5.Checked == true))
                     {
                         Config.config.textBox1.AppendText("单一房间模式下检索到符合条件空座列表，开始尝试预约...\n");
                         get_list = true;
                     }

                     //如果没有勾选可以改抢附近座位并且是指定座位情况下,直接跳出该循环
                     if (stat == "NoMatchSeat" && Config.config.checkBox5.Checked != true)
                     {
                         Config.config.textBox1.AppendText("没有勾选改抢附近座位选项，进行下一轮抢座...\n");
                     }
                 }
                 else//特殊模式,例如全馆检索
                 {
                     switch(Config.config.comboBox4.SelectedIndex)
                     {
                         case 0: {
                             foreach (string mroomID in RushSeat.roomList_b1)
                             {
                                 if (RushSeat.SearchFreeSeatMulti(buildingID, mroomID, date, startTime, endTime) == "Success")
                                 {
                                     get_list = true;
                                     break;
                                 }
                                 Thread.Sleep(repeatSearchInterval);
                             }
                             break;
                         }
                         case 1:  {
                                 foreach (string mroomID in RushSeat.roomList_f1)
                                 {
                                     if (RushSeat.SearchFreeSeatMulti(buildingID, mroomID, date, startTime, endTime) == "Success")
                                     {
                                         get_list = true;
                                         break;
                                     }
                                     Thread.Sleep(repeatSearchInterval);
                                 }
                                 break;
                             }
                         case 2:
                             {
                                 foreach (string mroomID in RushSeat.roomList_f2t4)
                                 {
                                     if (RushSeat.SearchFreeSeatMulti(buildingID, mroomID, date, startTime, endTime) == "Success")
                                     {
                                         get_list = true;
                                         break;
                                     }
                                     Thread.Sleep(repeatSearchInterval);
                                 }
                                 break;
                             }
                     }
                     
                     
                 }

                 //如果检索到空座
                 if (get_list == true)
                 {

                     //阶级等待
                     if (Config.rank != 'A')
                        Thread.Sleep(rankSuccessGetFreeSeat);

                     //先释放当前座位
                     string resInfo = RushSeat.CheckHistoryInf(false);
                     if (resInfo == "RESERVE")
                     {
                         if(RushSeat.CancelReservation(RushSeat.resID) != true)
                         {
                             Config.config.textBox1.AppendText("释放座位失败，请手动重试...");
                             Config.config.comboBox1.Enabled = true;
                             Config.config.comboBox2.Enabled = true;
                             Config.config.comboBox3.Enabled = true;
                             Config.config.comboBox4.Enabled = true;
                             Config.config.comboBox5.Enabled = true;
                             return;
                         }
                     }
                     if (resInfo == "CHECK_IN" || resInfo == "AWAY")
                     {
                         if (RushSeat.StopUsing() != true)
                         {
                             Config.config.textBox1.AppendText("释放座位失败，请手动重试...");
                             Config.config.comboBox1.Enabled = true;
                             Config.config.comboBox2.Enabled = true;
                             Config.config.comboBox3.Enabled = true;
                             Config.config.comboBox4.Enabled = true;
                             Config.config.comboBox5.Enabled = true;
                             return;
                         }
                     }

                     foreach (string seatID in RushSeat.freeSeats)
                     {
                         string status_1 = RushSeat.BookSeat(seatID, date, startTime, endTime);
                         //bool outloop = false;
                         int count_1 = 0;
                         if (status_1 == "Success")
                         {
                             success = true;
                             break;
                         }
                         Thread.Sleep(300);

                         //如果在系统开放的前后3s内没抢到座位，即使座位还在，也会自动改签
                         //所以说要注意系统的时间
                         while (status_1 == "NotAtTime" && count_1 < 20)
                         {
                             count_1 = count_1 + 1;  //真的服了
                             status_1 = RushSeat.BookSeat(seatID, date, startTime, endTime);
                             if (status_1 == "Success")
                             {
                                 success = true;
                                 break;
                             }
                             else if (status_1 == "NotAtTime")
                             {
                                 Config.config.textBox1.AppendText("系统尚未开放...\n");
                                 Thread.Sleep(300);
                             }
                             //status_1 = RushSeat.BookSeat(seatID, date, startTime, endTime);   //放在这里就GG了
                         }

                         //如果成功抢座就跳出foreach循环，如果是抢座失败就根据情况进行下一步操作
                         if (success == true)
                             break;
                         else    //这种情况是没抢过别人，但是这个循环被多重抢座、指定抢座共同使用，不能再根据是否勾选改签来改选，只能先将就一下，指定座位模式下自动改签
                         {
                             Thread.Sleep(500);
                             Config.config.textBox1.AppendText("座位ID " + seatID.ToString() + " 预约失败,尝试预约下一个座位\n");
                         }
                     }
                     //成功抢座后自动关机
                     if (success == true)
                     {
                         //静默检查预约信息，激活释放按钮
                         RushSeat.CheckHistoryInf(false);

                         //窗口弹出
                         if (Config.config.Visible != true)
                            Config.config.Visible = true;
                         Config.config.WindowState = FormWindowState.Normal;

                         //发短信
                         if (Config.config.checkBox4.Checked)
                         {
                             Config.config.textBox1.AppendText("短信已发送，返回值：\n" + RushSeat.SendMessage() + "\n");
                             Config.config.textBox1.AppendText("若返回值小于0为发送失败，请联系开发者\n");
                             Config.config.textBox1.AppendText("------------------------------------------\n");
                         } 

                         if (Config.config.checkBox3.Checked)
                         {
                             Config.config.textBox1.AppendText("2min后自动关机\n");
                             Config.config.textBox1.AppendText("如果想取消自动关机请在桌面用快捷键win + R启动控制台, 在控制台自行输入 shutdown -a\n");
                             Config.config.textBox1.AppendText("-----------------------------------------------------\n");
                             Process.Start("shutdown.exe", "-s -t " + "120");
                         }
                         else
                         {
                             //Config.config.textBox1.AppendText("订座成功");
                         }

                         Config.config.button1.Text = "开始抢座";
                         Config.config.comboBox1.Enabled = true;
                         Config.config.comboBox2.Enabled = true;
                         Config.config.comboBox3.Enabled = true;
                         Config.config.comboBox4.Enabled = true;
                         Config.config.comboBox5.Enabled = true;
                         break;
                     }
                     else
                     {
                         //有空座但是抢座失败(别人手快或者碰到更高级的了)
                         wrong_count++;
                         Config.config.textBox1.AppendText("*****预约失败，"+(((double)repeatSearchInterval)/1000).ToString() +"s后重新开始检索...*****\n");
                         get_list = false;
                         if(wrong_count == 5)
                         {
                             Config.config.textBox1.AppendText("多次抢座失败，为防止封号中止抢座\n");
                             Config.config.textBox1.AppendText("这种情况不常见，请保存工作记录并联系开发者\n");
                             Config.config.comboBox1.Enabled = true;
                             Config.config.comboBox2.Enabled = true;
                             Config.config.comboBox3.Enabled = true;
                             Config.config.comboBox4.Enabled = true;
                             Config.config.comboBox5.Enabled = true;
                             return;
                         }
                     }
                 }
                 Thread.Sleep(repeatSearchInterval);
                 preventCount++;
                 //if (preventCount == 30)
                 //{
                 //    Config.config.textBox1.AppendText("防止被封，睡眠10s..........\n");
                 //    Thread.Sleep(10000);
                 //    preventCount = 0;
                 //}
             }
         }
    }
}
