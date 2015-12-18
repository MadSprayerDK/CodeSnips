using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ManoSoftware.mmOAuth.Core.HttpServer;
using ManoSoftware.mmOAuth.Core.Model;

namespace ManoSoftware.mmOAuth.Core
{
    /// <summary>
    /// Main Oauth class for mmOAuth
    /// </summary>
    /// <seealso cref="ManoSoftware.mmOAuth.Core.IOauth" />
    public class OAuth : IOauth
    {
        public delegate void RecievedCode(object sender, StringEventArg args);
        public event RecievedCode OAuthCodeRecived;

        private readonly IOAuthProvider _provider;

        private readonly OAuthHttpServer _httpServer;
        private Thread _httpServerThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth"/> class.
        /// </summary>
        /// <param name="provider">Oauth Provider</param>
        /// <param name="port">Port for Web server to listen. Must be the port you direct back to!</param>
        public OAuth(IOAuthProvider provider, int port)
        {
            _provider = provider;

            _httpServer = new OAuthHttpServer(port, _provider.GetSuccessUri());
            _httpServer.OAuthCodeRecived += _httpServer_OAuthCodeRecived;
        }

        /// <summary>
        /// Fired when a authentication code is recived from the web server
        /// </summary>
        void _httpServer_OAuthCodeRecived(object sender, StringEventArg args)
        {
            StopOAuthRedirectServer();
            OAuthCodeRecived?.Invoke(sender, args);
        }

        /// <summary>
        /// Starts a browser, and redirects user to login page.
        /// </summary>
        public void GotoAuthorization()
        {
            Process.Start(_provider.GetAuthorizationUri() + _provider.GetAuthorizationParameters());
        }

        /// <summary>
        /// Start the Web server, to catch the user when returning from the authentication page.
        /// </summary>
        public void StartOAuthRedirectServer()
        {
            _httpServerThread = new Thread(_httpServer.Listen);
            _httpServerThread.Start();
        }

        /// <summary>
        /// Stops the Web server
        /// </summary>
        public void StopOAuthRedirectServer()
        {
            _httpServer.Stop();
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <param name="oauthCode">The oauth code.</param>
        /// <returns>Valid access token.</returns>
        /// <exception cref="Exception">Exception containing the errormessage if one occoured</exception>
        public async Task<string> GetAccessToken(string oauthCode)
        {
            var client = new HttpClient { BaseAddress = new Uri(_provider.GetAccessTokenUri()) };
            var content = _provider.GetAccessTokenParameters(oauthCode);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PostAsync("", content);
            var message = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
                return _provider.ExtractAccessCode(message);

            throw new Exception(_provider.ExtractErrorMessage(message));
        }

        /// <summary>
        /// Verifies a given oAuth Token
        /// </summary>
        /// <param name="oAuthToken">The o authentication token.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<TokenInfo> VerifyToken(string oAuthToken)
        {
            string uri = _provider.GetVerificationUri();

            if (string.IsNullOrEmpty(uri))
                return null;

            var client = new HttpClient { BaseAddress = new Uri(uri) };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync(_provider.GetVerificationParameters(oAuthToken));
            var message = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return _provider.ExtractVerifiedMessage(message);
            }

            throw new Exception(_provider.ExtractErrorMessage(message));
        }
    }
}