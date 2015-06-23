using System.Net.Sockets;

namespace TcpServer.EventArguments
{
    public class ClientConnectionEventArgs
    {
        public TcpClient Client { set; get; }
    }
}
