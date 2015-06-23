using System;

namespace WebSocket.EventArguments
{
    public class ExceptionOccurredEventArgs
    {
        public string Message { set; get; }
        public Exception Exception { set; get; }
    }
}
