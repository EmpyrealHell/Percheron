using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Percheron.API.Utility
{
    public class NewtonsoftDeserializer : IDeserializer
    {
        public static NewtonsoftDeserializer Default { get; private set; } = new NewtonsoftDeserializer();

        T IDeserializer.Deserialize<T>(IRestResponse response)
        {
            var resolver = new DefaultContractResolver();
            resolver.NamingStrategy = new SnakeCaseNamingStrategy(true, false, true);
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = resolver;
            return JsonConvert.DeserializeObject<T>(response.Content, settings);
        }
    }
}
