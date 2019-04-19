using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Net;

namespace Hush
{
    public static class DBUtil
    {
        /*
         * 为数据库连接设置连接字符串：daa
            myDbIp，数据库连接的IP地址
            myUserName，数据库连接的用户名
            myPass，数据库连接的密码
            myDbName，数据库名 
         */
        private static string source = "local.db";
        private static string userId = "sa";
        private static string password = "123";
        private static string dbName = "Sheet";

        public static DbConnection GetConn()
        {
            DbConnection conn = new SQLiteConnection("Data Source=" + source + ";FailIfMissing=True;");//URI=file: ..\\..\\local.db;Version=3;
            conn.Open();
            if (conn.State == ConnectionState.Open)
                Console.WriteLine(@"数据库连接成功！");
            else
                Console.WriteLine(conn.State);
            //conn.ChangeDatabase(dbName);
            return conn;
        }
    }

    public static class PCUtil
    {
        public static string GetUserName()
        {
            return Dns.GetHostName(); 
        }

        public static string GetUserIp()
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "ipconfig.exe";//设置程序名     
            cmd.StartInfo.Arguments = "/all";  //参数     
            //重定向标准输出     
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.CreateNoWindow = true;//不显示窗口（控制台程序是黑屏）     

            cmd.Start();
            string info = cmd.StandardOutput.ReadToEnd();
            cmd.WaitForExit();
            cmd.Close();
            return info;
        }

    }
}
