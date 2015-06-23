using System;
using System.Net;
using TcpServer.EventArguments;

namespace TcpServer.Runner
{
    class Program
    {
        private static TcpServer _tcpServer;

        static void Main(string[] args)
        {
            _tcpServer = new TcpServer(2000);
            _tcpServer.NewMessage += TcpServerNewMessage;
            _tcpServer.ExceptionOccurred += TcpServerExceptionOccurred;
            _tcpServer.ClientConnected += TcpserverClientConnected;
            _tcpServer.ClientDisconnected += TcpServerClientDisconnected;

            _tcpServer.Start();

            Console.WriteLine("Press Any Key to close the application.");
            Console.ReadKey();

            _tcpServer.Stop();
            Environment.Exit(0);
        }

        static void TcpserverClientConnected(object sender, ClientConnectionEventArgs args)
        {
            Console.WriteLine("New Connection from:" + ((IPEndPoint)args.Client.Client.RemoteEndPoint).Address);
        }

        static void TcpServerExceptionOccurred(object sender, ExceptionOccurredEventArgs args)
        {
            Console.WriteLine("Exception: " + args.Message);
        }

        static void TcpServerClientDisconnected(object sender, ClientConnectionEventArgs args)
        {
            Console.WriteLine("Lost Connection from:" + ((IPEndPoint)args.Client.Client.RemoteEndPoint).Address);
        }

        static void TcpServerNewMessage(object sender, NewMessageEventArgs args)
        {
            Console.WriteLine(args.Message);
            _tcpServer.SendMessage(args.Client, args.Message);
        }
    }
}
