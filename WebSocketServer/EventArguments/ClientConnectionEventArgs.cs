using System.Net.Sockets;

namespace ManoSoftware.WebSocketServer.EventArguments
{
    public class ClientConnectionEventArgs
    {
        public TcpClient Client { set; get; }
        public string Message { set; get; }
    }
}
