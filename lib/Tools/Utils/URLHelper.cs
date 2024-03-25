namespace LIB.Tools.Utils
{
    public static class URLHelper
    {
        private static readonly string Url = Config.GetConfigValue("WebSiteRootURL");

        public static string GetUrl(string relativeURL) => Url + relativeURL;
    }
}