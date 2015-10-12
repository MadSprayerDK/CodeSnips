using System.Net.Http;
using mmOAuth.Core.Model;

namespace mmOAuth.Core
{
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
