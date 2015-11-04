using System.Net.Sockets;

namespace ManoSoftware.WebSocketServer.EventArguments
{
    public class NewMessageEventArgs
    {
        public string Message { set; get; }
        public TcpClient Client { set; get; }
    }
}
