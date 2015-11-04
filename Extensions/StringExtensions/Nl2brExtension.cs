namespace ManoSoftware.Extensions.StringExtensions
{
    public static class Nl2brExtension
    {
        public static string Nl2br(this string input)
        {
            return input.Nl2br(true);
        }

        public static string Nl2br(this string input, bool isXhtml)
        {
            return input.Replace("\r\n", isXhtml ? "<br />\r\n" : "<br>\r\n");
        }
    }
}
