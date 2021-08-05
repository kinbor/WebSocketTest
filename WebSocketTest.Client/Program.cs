using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket4Net;

namespace WebSocketTest.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("正在连接服务器。。。");
            var ws = new WebSocket("ws://127.0.0.1:9090");
            /*
             * 服务器若使用证书，协议ws应改为wss，且证书应该是有效证书与域名匹配
             * 使用ws协议可以是IP地址，使用wss就要使用域名了，因为颁发证书时确定了使用证书的域名。
             * ws://127.0.0.1:9090
             * wss://www.test.com:9090
             */
            ws.Opened += Ws_Opened;
            ws.Closed += Ws_Closed;
            ws.MessageReceived += Ws_MessageReceived;
            ws.Error += Ws_Error;
            ws.Open();

            Console.WriteLine("输入STOP后,按Enter键结束。。。");
            while (true)
            {
                var info = Console.ReadLine();
                if (info == "STOP")
                {
                    break;
                }
                else
                {
                    ws.Send(info);
                }
            }

            ws.Close();
            ws.Dispose();
        }

        private static void Ws_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine($"服务器：{e.Message}");
        }

        private static void Ws_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Console.WriteLine($"错误信息：{e.Exception.Message} | {e.Exception.StackTrace}");
        }

        private static void Ws_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("服务器连接断开！");
        }
        private static void Ws_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("服务器连接成功！");
        }
    }
}
