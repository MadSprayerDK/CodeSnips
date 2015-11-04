using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ManoSoftware.TcpServer.EventArguments;

namespace ManoSoftware.TcpServer
{
    /// <summary>
    /// A threaded TcpServer.
    /// Main listen is  on a seperate thread from the main thread (Doesn't block UI)
    /// Each connection spawns a different thread for execution
    /// </summary>
    public class TcpServer
    {
        // Private variables used in library
        private readonly TcpListener _tcpListener;
        private readonly Thread _listenThread;
        private bool _keepAlive = true;

        // Event for OnNewMessage
        public delegate void NewMessageHandler(Object sender, NewMessageEventArgs args);
        public event NewMessageHandler NewMessage;

        // Event for OnExceptionOccurred
        public delegate void ServerErrorHandler(Object sender, ExceptionOccurredEventArgs args);
        public event ServerErrorHandler ExceptionOccurred;

        // Event for Client Connected / Disconnected
        public delegate void ClientConnectionHandler(Object sender, ClientConnectionEventArgs args);
        public event ClientConnectionHandler ClientConnected;
        public event ClientConnectionHandler ClientDisconnected;

        private readonly int _maxReadBytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServer"/>.
        /// </summary>
        /// <param name="listenPort">The port the server will listen on</param>
        /// /// <param name="maxReadBytes">Maximum number of bytes to read at the time</param>
        public TcpServer(int listenPort, int maxReadBytes = 4096)
        {
            // Create new TcpListener
            _tcpListener = new TcpListener(IPAddress.Any, listenPort);

            // Create seperate thread for listener, so it doesn't block main thread
            _listenThread = new Thread(ListenForClients);

            // Set maximum of read bytes at a time
            _maxReadBytes = maxReadBytes;
        }

        /// <summary>
        /// Starts the <see cref="TcpServer"/>.
        /// </summary>
        public void Start()
        {
            _listenThread.Start();
        }

        /// <summary>
        /// Stops the <see cref="TcpServer"/>.
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
                        Message = "Error when starting the TcpListener"
                    });

                    break;
                }

                // Check if the client has disconnected from the server
                if (bytesRead == 0)
                    break;

                // Get message and raise event
                var encoder = new ASCIIEncoding();
                OnNewMessage(new NewMessageEventArgs
                {
                    Client = tcpClient,
                    Message = encoder.GetString(message, 0, bytesRead)
                });
            }

            OnClientDisconnect(new ClientConnectionEventArgs { Client = tcpClient });

            // Close TcpClient after disconnect
            tcpClient.Close();
        }

        /// <summary>
        /// Raises the <see cref="E:NewMessage" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="NewMessageEventArgs"/> instance containing the event data.</param>
        protected virtual void OnNewMessage(NewMessageEventArgs eventArgs)
        {
            if (NewMessage != null)
                NewMessage(this, eventArgs);
        }

        /// <summary>
        /// Raises the <see cref="E:ExceptionOccured" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="ExceptionOccurredEventArgs"/> instance containing the event data.</param>
        protected virtual void OnExceptionOccured(ExceptionOccurredEventArgs eventArgs)
        {
            if (ExceptionOccurred != null)
                ExceptionOccurred(this, eventArgs);
        }

        protected virtual void OnClientConnect(ClientConnectionEventArgs eventArgs)
        {
            if (ClientConnected != null)
                ClientConnected(this, eventArgs);
        }

        protected virtual void OnClientDisconnect(ClientConnectionEventArgs eventArgs)
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
            var clientStream = client.GetStream();
            var encoder = new ASCIIEncoding();
            var buffer = encoder.GetBytes(message);

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
        }
    }
}
