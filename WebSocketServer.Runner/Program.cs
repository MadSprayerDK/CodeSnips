using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebSocket.Runner
{
    class Program
    {
        private static WebSocketServer wsServer;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Websocket Server");
            wsServer = new WebSocketServer(81);
            wsServer.ClientConnected += wsServer_ClientConnected;
            wsServer.ClientDisconnected += wsServer_ClientDisconnected;
            wsServer.ExceptionOccurred += wsServer_ExceptionOccurred;
            wsServer.NewMessage += wsServer_NewMessage;
            wsServer.Start();
            Console.WriteLine("Awaiting connections");
        }

        static void wsServer_NewMessage(object sender, EventArguments.NewMessageEventArgs args)
        {
            Console.WriteLine("New Message: " + args.Message);
            wsServer.SendMessage(args.Client, args.Message);
        }

        static void wsServer_ExceptionOccurred(object sender, EventArguments.ExceptionOccurredEventArgs args)
        {
            Console.WriteLine("An exception occurred: " + args.Message + "\n Exception Message: " + args.Exception.Message);
        }

        static void wsServer_ClientDisconnected(object sender, EventArguments.ClientConnectionEventArgs args)
        {
            Console.WriteLine("Client Disconnected: " + ((IPEndPoint)args.Client.Client.RemoteEndPoint).Address + "\nReason: " + args.Message);
        }

        static void wsServer_ClientConnected(object sender, EventArguments.ClientConnectionEventArgs args)
        {
            Console.WriteLine("Client Connected: " + ((IPEndPoint)args.Client.Client.RemoteEndPoint).Address);
        }
    }
}
