using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManoSoftware.mmOAuth.Core.Model;

namespace ManoSoftware.mmOAuth.Core
{
    /// <summary>
    /// Interface for OAuth Core class
    /// </summary>
    public interface IOauth
    {
        void GotoAuthorization();
        void StartOAuthRedirectServer();
        void StopOAuthRedirectServer();
        Task<string> GetAccessToken(string oauthCode);
        Task<TokenInfo> VerifyToken(string oAuthToken);
    }
}
