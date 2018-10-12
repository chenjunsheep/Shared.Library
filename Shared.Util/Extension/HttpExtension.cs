using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Shared.Util.Extension
{
    public static class HttpExtension
    {
        public static Uri AddQuery(this Uri uri, string name, string value)
        {
            try
            {
                var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

                httpValueCollection.Remove(name);
                httpValueCollection.Add(name, value);

                var ub = new UriBuilder(uri);
                ub.Query = httpValueCollection.ToString();

                return ub.Uri;
            }
            catch (Exception)
            {
                return uri;
            }
        }

        public static Uri AddPath(this Uri uri, params string[] paths)
        {
            try
            {
                return new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) => string.Format("{0}/{1}", current.TrimEnd('/'), path.TrimStart('/'))));
            }
            catch (Exception)
            {
                return uri;
            }
        }
    }
}
