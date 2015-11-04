using System.Collections.Generic;
using System.Net.Http;
using ManoSoftware.mmOAuth.Core;
using ManoSoftware.mmOAuth.Core.Model;
using ManoSoftware.mmOAuth.TwitchAlerts.Model;
using Newtonsoft.Json;

namespace ManoSoftware.mmOAuth.TwitchAlerts
{
    public class Provider : IOAuthProvider
    {
        private readonly string[] _scopes;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;
        private readonly string _successUri;
        private string _apiBaseUri = "https://www.twitchalerts.com/api/v1.0/";

        public Provider(string[] scopes, string clientId, string clientSecret, string redirectUri, string successUri)
        {
            _scopes = scopes;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _redirectUri = redirectUri;
            _successUri = successUri;
        }

        // Authorization
        public string GetAuthorizationUri()
        {
            return _apiBaseUri + "authorize";
        }

        public string GetAuthorizationParameters()
        {
            var scopes = string.Join("+", _scopes);
            var uri = "?response_type=code" +
                      "&client_id=" + _clientId +
                      "&redirect_uri=" + _redirectUri +
                      "&scope=" + scopes;

            return uri;
        }

        // Access Token
        public string GetAccessTokenUri()
        {
            return _apiBaseUri + "token";
        }

        public FormUrlEncodedContent GetAccessTokenParameters(string oAuthCode)
        {
            return new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret), 
                new KeyValuePair<string, string>("redirect_uri", _redirectUri),
                new KeyValuePair<string, string>("code", oAuthCode) 
            });
        }

        // Verification
        public string GetVerificationUri()
        {
            return "";
        }

        public string GetVerificationParameters(string oAuthToken)
        {
            return "";
        }

        // Response
        public string GetSuccessUri()
        {
            return _successUri;
        }

        // Extraction
        public string ExtractAccessCode(string json)
        {
            var token = JsonConvert.DeserializeObject<AccessToken>(json);
            return token.Access_Token;
        }

        public string ExtractErrorMessage(string json)
        {
            var error = JsonConvert.DeserializeObject<ErrorMessage>(json);
            return error.Error + ": " + error.Message;
        }

        public TokenInfo ExtractVerifiedMessage(string json)
        {
            return null;
        }
    }
}
