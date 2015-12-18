using System.Net.Http;
using ManoSoftware.mmOAuth.Core.Model;

namespace ManoSoftware.mmOAuth.Core
{
    /// <summary>
    /// Interface for Providor Class
    /// </summary>
    public interface IOAuthProvider
    {
        // Auth
        string GetAuthorizationUri();
        string GetAuthorizationParameters();

        // Getting Access Token
        string GetAccessTokenUri();
        FormUrlEncodedContent GetAccessTokenParameters(string oAuthCode);

        // Verification
        string GetVerificationUri();
        string GetVerificationParameters(string oAuthToken);

        // Reponse Methods
        string GetSuccessUri();

        // Extracting of Data
        string ExtractAccessCode(string json);
        string ExtractErrorMessage(string json);
        TokenInfo ExtractVerifiedMessage(string json);
    }
}
