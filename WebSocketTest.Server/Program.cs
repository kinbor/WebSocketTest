using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketTest.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //客户端url以及其对应的Socket对象字典
            var dic_Sockets = new Dictionary<string, IWebSocketConnection>();
            //创建
            //本地测试的配置IP和端口，自行设置
            WebSocketServer server = new WebSocketServer("ws://0.0.0.0:9090");
            /*
             * 使用证书，则协议应改为wss
             * wss://0.0.0.0:9090
             * server.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2("", "");
             */
            //出错后进行重启
            server.RestartAfterListenError = true;

            //开始监听
            server.Start(socket =>
            {
                socket.OnOpen = () =>   //连接建立事件
                {
                    //获取客户端网页的url
                    string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                    dic_Sockets.Add(clientUrl, socket);
                    Console.WriteLine(DateTime.Now.ToString() + "|服务器:和客户端网页:" + clientUrl + " 建立WebSock连接！");
                };
                socket.OnClose = () =>  //连接关闭事件
                {
                    string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                    //如果存在这个客户端,那么对这个socket进行移除
                    if (dic_Sockets.ContainsKey(clientUrl))
                    {
                        //注:Fleck中有释放
                        //关闭对象连接
                        //if (dic_Sockets[clientUrl] != null)
                        //{
                        //dic_Sockets[clientUrl].Close();
                        //}
                        dic_Sockets.Remove(clientUrl);
                    }
                    Console.WriteLine(DateTime.Now.ToString() + "|服务器:和客户端网页:" + clientUrl + " 断开WebSock连接！");
                };
                socket.OnMessage = message =>  //接受客户端网页消息事件
                {
                    string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                    //socket.ConnectionInfo.Headers["UserId"] = message;
                    string key = message.Replace("UserId=", "");
                    dic_Sockets.Add(key, socket);
                    Console.WriteLine(DateTime.Now.ToString() + "|服务器:【收到】来客户端网页:" + clientUrl + "的信息：\n" + message);
                };
            });

            Console.ReadKey();
            foreach (var item in dic_Sockets.Values)
            {
                if (item.IsAvailable == true)
                {
                    item.Send("服务器消息：" + DateTime.Now.ToString());
                }
            }

            //关闭与客户端的所有的连接
            Console.ReadKey();
            foreach (var item in dic_Sockets.Values)
            {
                if (item != null)
                {
                    item.Close();
                }
            }

            Console.ReadKey();
        }
    }
}
