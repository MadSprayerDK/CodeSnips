namespace ManoSoftware.mmOAuth.Twitch.Model
{
    internal class AccessCode
    {
        public string Access_Token { set; get; }
        public string Refresh_Token { set; get; }
        public string[] Scope { set; get; }
    }
}