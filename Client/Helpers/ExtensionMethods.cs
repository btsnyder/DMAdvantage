using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;
using System.Web;

namespace DMAdvantage.Client.Helpers
{
    public static class ExtensionMethods
    {
        public static NameValueCollection QueryString(this NavigationManager navigationManager)
        {
            return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
        }

        public static string? QueryString(this NavigationManager navigationManager, string key)
        {
            return navigationManager.QueryString()[key];
        }

        public static object? GetPropertyByName(this object obj, string name)
        {
            var info = obj.GetType().GetProperty(name);
            return info?.GetValue(obj);
        }

        public static void SetPropertyByName(this object obj, string name, object value)
        {
            var info = obj.GetType().GetProperty(name);
            info?.SetValue(obj, value);
        }
    }
}
