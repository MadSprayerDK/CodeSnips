using System.Net.Sockets;

namespace ManoSoftware.TcpServer.EventArguments
{
    public class NewMessageEventArgs
    {
        public string Message { set; get; }
        public TcpClient Client { set; get; }
    }
}
