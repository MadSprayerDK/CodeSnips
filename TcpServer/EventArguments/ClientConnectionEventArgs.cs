﻿using System.Net.Sockets;

namespace ManoSoftware.TcpServer.EventArguments
{
    public class ClientConnectionEventArgs
    {
        public TcpClient Client { set; get; }
    }
}
