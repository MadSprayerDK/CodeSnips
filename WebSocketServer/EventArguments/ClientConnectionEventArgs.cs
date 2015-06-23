using System.Net.Sockets;

namespace WebSocketServer.EventArguments
{
    public class ClientConnectionEventArgs
    {
        public TcpClient Client { set; get; }
        public string Message { set; get; }
    }
}
