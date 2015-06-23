using System.Net.Sockets;

namespace TcpServer.EventArguments
{
    public class NewMessageEventArgs
    {
        public string Message { set; get; }
        public TcpClient Client { set; get; }
    }
}
