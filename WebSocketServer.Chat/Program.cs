using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using WebSocketServer.EventArguments;

namespace WebSocketServer.Chat
{
    class Program
    {
        private static WebSocketServer _wsServer;
        private static List<TcpClient> _clients;

        static void Main()
        {
            _clients = new List<TcpClient>();

            Console.WriteLine("Starting Websocket Server");
            _wsServer = new WebSocketServer(81);
            _wsServer.ClientConnected += wsServer_ClientConnected;
            _wsServer.ClientDisconnected += wsServer_ClientDisconnected;
            _wsServer.ExceptionOccurred += wsServer_ExceptionOccurred;
            _wsServer.NewMessage += wsServer_NewMessage;
            _wsServer.Start();
            Console.WriteLine("Awaiting connections");
        }

        static void wsServer_NewMessage(object sender, NewMessageEventArgs args)
        {
            Console.WriteLine("New Message: " + args.Message);

            foreach (var client in _clients)
            {
                _wsServer.SendMessage(client, args.Message);
            }
        }

        static void wsServer_ExceptionOccurred(object sender, ExceptionOccurredEventArgs args)
        {
            Console.WriteLine("An exception occurred: " + args.Message + "\n Exception Message: " + args.Exception.Message);
        }

        static void wsServer_ClientDisconnected(object sender, ClientConnectionEventArgs args)
        {
            _clients.Remove(args.Client);
            Console.WriteLine("Client Disconnected: " + ((IPEndPoint)args.Client.Client.RemoteEndPoint).Address + "\nReason: " + args.Message);
        }

        static void wsServer_ClientConnected(object sender, ClientConnectionEventArgs args)
        {
            _clients.Add(args.Client);
            Console.WriteLine("Client Connected: " + ((IPEndPoint)args.Client.Client.RemoteEndPoint).Address);
        }
    }
}
