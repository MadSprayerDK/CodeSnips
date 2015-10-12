using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using mmOAuth.Core.HttpServer;
using mmOAuth.Core.Model;

namespace mmOAuth.Core
{
    public class OAuth
    {
        public delegate void RecievedCode(object sender, StringEventArg args);
        public event RecievedCode OAuthCodeRecived;

        private readonly IOAuthProvider _provider;

        private readonly OAuthHttpServer _httpServer;
        private Thread _httpServerThread;

        public OAuth(IOAuthProvider provider)
        {
            _provider = provider;

            _httpServer = new OAuthHttpServer(8080, _provider.GetSuccessUri());
            _httpServer.OAuthCodeRecived += _httpServer_OAuthCodeRecived;
        }

        void _httpServer_OAuthCodeRecived(object sender, StringEventArg args)
        {
            StopOAuthRedirectServer();

            if (OAuthCodeRecived != null)
                OAuthCodeRecived(sender, args);
        }

        public void GotoAuthorization()
        {
            Process.Start(_provider.GetAuthorizationUri() + _provider.GetAuthorizationParameters());
        }

        public void StartOAuthRedirectServer()
        {
            _httpServerThread = new Thread(_httpServer.Listen);
            _httpServerThread.Start();
        }

        public void StopOAuthRedirectServer()
        {
            _httpServer.Stop();
        }

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