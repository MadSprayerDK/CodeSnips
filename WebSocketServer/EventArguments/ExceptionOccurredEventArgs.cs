using System;

namespace ManoSoftware.WebSocketServer.EventArguments
{
    public class ExceptionOccurredEventArgs
    {
        public string Message { set; get; }
        public Exception Exception { set; get; }
    }
}
