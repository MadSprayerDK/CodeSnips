using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ManoSoftware.SpotifyLocalApi.Model;
using Newtonsoft.Json;

namespace ManoSoftware.SpotifyLocalApi
{
    public class SpotifyLocalApi : ISpotifyLocalApi
    {
        private readonly string _origin = "https://open.spotify.com";

        private readonly string _host;
        private string _oauthToken;
        private string _csrfToken;

        public SpotifyLocalApi()
        {
            _host = "https://" + RandomString(2) + ".spotilocal.com:4730";
        }

        public async Task<bool> Init()
        {
            await GetOauth();
            await GetCsrf();

            return true;
        }

        private async Task<bool> GetOauth()
        {
            var message = await PerformRequest("/token", new Dictionary<string, string>(), false, false, true);
            var token = JsonConvert.DeserializeObject<OauthToken>(message);

            _oauthToken = token.T;
            return true;
        }

        private async Task<bool> GetCsrf()
        {
            var message = await PerformRequest("/simplecsrf/token.json", new Dictionary<string, string>());
            var token = JsonConvert.DeserializeObject<CsrfToken>(message);

            _csrfToken = token.Token;
            return true;
        }

        public async Task<Status> Status()
        {
            var message = await PerformRequest("/remote/status.json", new Dictionary<string, string>());
            return JsonConvert.DeserializeObject<Status>(message);
        }

        public async Task<Status> Play(string uri)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"uri", uri}
            };
            var message = await PerformRequest("/remote/play.json", parameters);

            return JsonConvert.DeserializeObject<Status>(message);
        }

        public async Task<Status> Pause()
        {
            var parameters = new Dictionary<string, string>()
            {
                {"pause", true.ToString()}
            };
            var message = await PerformRequest("remote/pause.json", parameters);

            return JsonConvert.DeserializeObject<Status>(message);
        }

        public async Task<Status> Resume()
        {
            var parameters = new Dictionary<string, string>()
            {
                {"pause", false.ToString()}
            };
            var message = await PerformRequest("remote/pause.json", parameters);

            return JsonConvert.DeserializeObject<Status>(message);
        }

        private async Task<string> PerformRequest(string url, IDictionary<string, string> parameters, bool auth = true, bool origin = true, bool getToken = false)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(getToken ? _origin : _host)
            };

            if (auth)
            {
                parameters.Add("oauth", _oauthToken);
                parameters.Add("csrf", _csrfToken);
            }

            var param = string.Format("?{0}", string.Join("&",
                parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))));

            if (origin)
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Origin", _origin);
            }

            var response = await client.GetAsync(url + param);
            var message = await response.Content.ReadAsStringAsync();

            return message;
        }

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
