using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using WebSocket.EventArguments;
using WebSocket.Extensions;

namespace WebSocket
{
    public class WebSocketServer
    {
        // Private variables used in library
        private readonly TcpListener _tcpListener;
        private readonly Thread _listenThread;
        private bool _keepAlive = true;

        // Event for OnNewMessage
        public delegate void NewMessageHandler(object sender, NewMessageEventArgs args);
        public event NewMessageHandler NewMessage;

        // Event for OnExceptionOccurred
        public delegate void ServerErrorHandler(object sender, ExceptionOccurredEventArgs args);
        public event ServerErrorHandler ExceptionOccurred;

        // Event for Client Connected / Disconnected
        public delegate void ClientConnectionHandler(object sender, ClientConnectionEventArgs args);
        public event ClientConnectionHandler ClientConnected;
        public event ClientConnectionHandler ClientDisconnected;

        private readonly int _maxReadBytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketServer"/>.
        /// </summary>
        /// <param name="listenPort">The port the server will listen on</param>
        /// /// <param name="maxReadBytes">Maximum number of bytes to read at the time</param>
        public WebSocketServer(int listenPort, int maxReadBytes = 4096)
        {
            // Create new TcpListener
            _tcpListener = new TcpListener(IPAddress.Any, listenPort);

            // Create seperate thread for listener, so it doesn't block main thread
            _listenThread = new Thread(ListenForClients);

            // Set maximum of read bytes at a time
            _maxReadBytes = maxReadBytes;
        }

        /// <summary>
        /// Starts the <see cref="WebSocketServer"/>.
        /// </summary>
        public void Start()
        {
            _listenThread.Start();
        }

        /// <summary>
        /// Stops the <see cref="WebSocketServer"/>.
        /// </summary>
        public void Stop()
        {
            // Stop TcpListener and Threads
            _keepAlive = false;
            _tcpListener.Stop();
            _listenThread.Abort();
        }

        /// <summary>
        /// Threaded function listening for new connections.
        /// </summary>
        private void ListenForClients()
        {
            // Tries to open the TcpListener.
            // Otherwise rais exception event, and return the function
            try
            {
                _tcpListener.Start();
            }
            catch (Exception e)
            {
                OnExceptionOccured(new ExceptionOccurredEventArgs
                {
                    Exception = e,
                    Message = "Error when starting the TcpListener"
                });

                return;
            }

            // Keep accepting new clients as long as the server is active
            while (_keepAlive)
            {
                var client = _tcpListener.AcceptTcpClient();

                var clientThread = new Thread(HandleClientCommunication);
                clientThread.Start(client);
            }
        }

        /// <summary>
        /// Handles the client communication.
        /// </summary>
        /// <param name="client">Object containing the client connection</param>
        private void HandleClientCommunication(object client)
        {
            // Cast client as TcpClient and check for null.
            var tcpClient = client as TcpClient;

            if (tcpClient == null)
            {
                OnExceptionOccured(new ExceptionOccurredEventArgs
                {
                    Exception = null,
                    Message = "Unable to cast client as TcpClient"
                });

                return;
            }

            OnClientConnect(new ClientConnectionEventArgs { Client = tcpClient });

            // Get stream from the client.
            var clientSteam = tcpClient.GetStream();

            // Create message buffer.
            var message = new byte[_maxReadBytes];

            while (_keepAlive)
            {
                // Create counter for number of bytes read from stream.
                int bytesRead;

                try
                {
                    bytesRead = clientSteam.Read(message, 0, _maxReadBytes);
                }
                catch (Exception e)
                {
                    // Raise event and break loop if exception occured
                    OnExceptionOccured(new ExceptionOccurredEventArgs
                    {
                        Exception = e,
                        Message = "Error when listening to the stream"
                    });

                    break;
                }

                // Check if the client has disconnected from the server
                if (bytesRead == 0)
                    break;

                // Get message and raise event
                var encoder = new UTF8Encoding();
                var output = encoder.GetString(message, 0, bytesRead);

                var headerLength = output.IndexOf("\r\n\r\n", StringComparison.Ordinal);

                if (headerLength != -1)
                    PerformHandshake(output, headerLength, tcpClient);

                else
                {
                    string errorMessage;
                    var success = GetMessage(message, bytesRead, tcpClient, out errorMessage);

                    if (success)
                        continue;

                    OnClientDisconnect(new ClientConnectionEventArgs
                    {
                        Client = tcpClient,
                        Message = errorMessage
                    });

                    tcpClient.Close();
                    return;
                }
            }

            OnClientDisconnect(new ClientConnectionEventArgs { Client = tcpClient, Message = "Client closed connection" });

            // Close TcpClient after disconnect
            tcpClient.Close();
        }

        private void PerformHandshake(string output, int headerLength, TcpClient tcpClient)
        {
            var rawHeader = output.Substring(0, headerLength);

            var headerField = rawHeader.Replace("\r", "").Split('\n');
            var httpHeader = new Dictionary<string, string>();

            for (int i = 0; i < headerField.Length; i++)
            {
                if (i == 0)
                {
                    var elements = headerField[i].Split(' ');
                    httpHeader.Add("RequestType", elements[0]);
                    httpHeader.Add("RequestTarget", elements[1]);
                    httpHeader.Add("HttpVersion", elements[2]);
                }
                else
                {
                    var elements = headerField[i].Split(':');
                    httpHeader.Add(elements[0], elements[1].Trim());
                }
            }

            if (httpHeader["Connection"] != "Upgrade")
                return;

            Console.WriteLine("Handshake: " + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address);
            var webSocketKey = httpHeader["Sec-WebSocket-Key"];

            var sha1 = SHA1.Create();
            var sha1Hash =
                sha1.ComputeHash(Encoding.Default.GetBytes(webSocketKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));

            var base64 = Convert.ToBase64String(sha1Hash);

            var handshakeReply = "HTTP/1.1 101 Switching Protocols\r\n" +
                                    "Upgrade: websocket\r\n" +
                                    "Connection: Upgrade\r\n" +
                                    "Sec-WebSocket-Accept: " + base64 + "\r\n\r\n";

            SendRawMessage(tcpClient, handshakeReply);
        }

        private bool GetMessage(byte[] message, int messageLength, TcpClient tcpClient, out string reason)
        {
            reason = "Connection clsoed";

            var secondByte = message[1];
            var encoded = Convert.ToBoolean(secondByte & 128);

            if (!encoded)
            {
                reason = "Message is not encoded";
                return false;
            }

            var length = secondByte & 127;
            var indexFirstMask = 2;

            if (length == 126)
                indexFirstMask = 4;
            else if (length == 127)
                indexFirstMask = 10;

            var mask = message.SubArray(indexFirstMask, 4);
            var indexFirstData = indexFirstMask + 4;

            var decoded = new byte[messageLength - indexFirstData];

            for (int i = indexFirstData, j = 0; i < messageLength; i++, j++)
            {
                decoded[j] = Convert.ToByte(message[i] ^ mask[j % 4]);
            }

            if (decoded[0] == 3)
            {
                switch ((WebSocketErrorCodes)decoded[1])
                {
                    case WebSocketErrorCodes.CLOSE_GOING_AWAY:
                        reason = "Endpoint is going away";
                        break;
                    case WebSocketErrorCodes.CLOSE_PROTOCOL_ERROR:
                        reason = "Protocol Error";
                        break;
                    case WebSocketErrorCodes.CLOSE_UNSUPPORTED:
                        reason = "Unsupported";
                        break;
                }

                return false;
            }

            var encoder = new UTF8Encoding();
            var output = encoder.GetString(decoded, 0, decoded.Length);

            OnNewMessage(new NewMessageEventArgs
            {
                Client = tcpClient,
                Message = output
            });

            return true;
        }

        /// <summary>
        /// Raises the <see cref="E:NewMessage" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="NewMessageEventArgs"/> instance containing the event data.</param>
        private void OnNewMessage(NewMessageEventArgs eventArgs)
        {
            if (NewMessage != null)
                NewMessage(this, eventArgs);
        }

        /// <summary>
        /// Raises the <see cref="E:ExceptionOccured" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="ExceptionOccurredEventArgs"/> instance containing the event data.</param>
        private void OnExceptionOccured(ExceptionOccurredEventArgs eventArgs)
        {
            if (ExceptionOccurred != null)
                ExceptionOccurred(this, eventArgs);
        }

        private void OnClientConnect(ClientConnectionEventArgs eventArgs)
        {
            if (ClientConnected != null)
                ClientConnected(this, eventArgs);
        }

        private void OnClientDisconnect(ClientConnectionEventArgs eventArgs)
        {
            if (ClientDisconnected != null)
                ClientDisconnected(this, eventArgs);
        }

        /// <summary>
        /// Send a messsage to the client via the provided client
        /// </summary>
        /// <param name="client">The client to recive the message.</param>
        /// <param name="message">The message to send.</param>
        public void SendMessage(TcpClient client, string message)
        {
            var header = new byte[10];
            int headerLength;

            header[0] = 129;

            if (message.Length <= 125)
            {
                header[1] = Convert.ToByte(message.Length);

                headerLength = 2;
            }
            else if (message.Length >= 126 && message.Length <= 65535)
            {
                header[1] = 126;
                header[2] = Convert.ToByte((message.Length >> 8) & 255);
                header[3] = Convert.ToByte(message.Length & 255);

                headerLength = 4;
            }
            else
            {
                header[1] = 127;
                header[2] = Convert.ToByte((message.Length >> 56) & 255);
                header[3] = Convert.ToByte((message.Length >> 48) & 255);
                header[4] = Convert.ToByte((message.Length >> 40) & 255);
                header[5] = Convert.ToByte((message.Length >> 32) & 255);
                header[6] = Convert.ToByte((message.Length >> 24) & 255);
                header[7] = Convert.ToByte((message.Length >> 16) & 255);
                header[8] = Convert.ToByte((message.Length >> 8) & 255);
                header[9] = Convert.ToByte(message.Length & 255);

                headerLength = 10;
            }

            var encoder = new UTF8Encoding();
            var outputMessage = encoder.GetBytes(message);

            var buffer = new byte[headerLength + message.Length];

            header.SubArray(0, headerLength).CopyTo(buffer, 0);
            outputMessage.CopyTo(buffer, headerLength);

            var clientStream = client.GetStream();

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
        }

        public void SendRawMessage(TcpClient client, string message)
        {
            var encoder = new UTF8Encoding();
            var buffer = encoder.GetBytes(message);

            var clientStream = client.GetStream();

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
        }
    }
}
