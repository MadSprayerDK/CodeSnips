namespace mmOAuth.Core.Model
{
    public class TokenInfo
    {
        public bool Valid { set; get; }
        public string UserName { set; get; }
        public string[] Scope { set; get; }
    }
}
