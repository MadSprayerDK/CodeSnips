using System.Collections.Generic;
using System.Net.Http;
using mmOAuth.Core;
using mmOAuth.Core.Model;
using mmOAuth.Twitch.Model;
using Newtonsoft.Json;

namespace mmOAuth.Twitch
{
    public class Provider : IOAuthProvider
    {
        private readonly string[] _scopes;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;
        private readonly string _successUri;
        private string _apiBaseUri = "https://api.twitch.tv/kraken/";

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
            return _apiBaseUri + "oauth2/authorize";
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
            return _apiBaseUri + "oauth2/token";
        }

        public FormUrlEncodedContent GetAccessTokenParameters(string oAuthCode)
        {
            return new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret), 
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                
                new KeyValuePair<string, string>("redirect_uri", _redirectUri),
                new KeyValuePair<string, string>("code", oAuthCode) 
            });
        }

        // Verification
        public string GetVerificationUri()
        {
            return _apiBaseUri;
        }

        public string GetVerificationParameters(string oAuthToken)
        {
            return "?oauth_token=" + oAuthToken;
        }

        // Response
        public string GetSuccessUri()
        {
            return _successUri;
        }

        // Extraction
        public string ExtractAccessCode(string json)
        {
            var token = JsonConvert.DeserializeObject<AccessCode>(json);
            return token.Access_Token;
        }

        public string ExtractErrorMessage(string json)
        {
            var error = JsonConvert.DeserializeObject<ErrorQuery>(json);
            return error.Error + ": " + error.Message;
        }

        public TokenInfo ExtractVerifiedMessage(string json)
        {
            var info = new TokenInfo();
            var verifiedToken = JsonConvert.DeserializeObject<Verify>(json);

            info.Valid = verifiedToken.Token.Valid;

            if (!info.Valid)
                return info;

            info.UserName = verifiedToken.Token.User_Name;
            info.Scope = verifiedToken.Token.Authorization.Scopes;

            return info;
        }
    }
}