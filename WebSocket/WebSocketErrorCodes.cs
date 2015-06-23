using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocket
{
    public enum WebSocketErrorCodes
    {
        CLOSE_NORMAL = 232,
        CLOSE_GOING_AWAY,
        CLOSE_PROTOCOL_ERROR,
        CLOSE_UNSUPPORTED
    }
}
