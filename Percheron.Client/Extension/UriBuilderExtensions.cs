using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percheron.Client.Extension
{
    public static class UriBuilderExtensions
    {
        public static void AddQuery(this UriBuilder builder, string rawKey, string rawValue)
        {
            var key = Uri.EscapeUriString(rawKey);
            var value = Uri.EscapeUriString(rawValue);
            if (!string.IsNullOrWhiteSpace(builder.Query))
            {
                builder.Query = builder.Query.Substring(1) + "&" + key + "=" + value;
            }
            else
            {
                builder.Query = key + "=" + value;
            }
        }
    }
}
