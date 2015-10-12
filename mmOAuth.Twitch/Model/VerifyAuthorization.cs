using System;

namespace mmOAuth.Twitch.Model
{
    internal class VerifyAuthorization
    {
        public string[] Scopes { set; get; }
        public DateTime Created_At { set; get; }
        public DateTime Updated_At { set; get; }
    }
}