using System.Net.Sockets;

namespace WebSocket.EventArguments
{
    public class NewMessageEventArgs
    {
        public string Message { set; get; }
        public TcpClient Client { set; get; }
    }
}
