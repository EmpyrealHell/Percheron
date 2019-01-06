using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Percheron.API.Utility;
using Percheron.API.Model;

namespace Percheron.API.Resource
{
    public static class Users
    {
        public static string Get(string token)
        {
            var client = new RestClient("https://api.twitch.tv");
            client.AddHandler("application/json", NewtonsoftDeserializer.Default);
            var request = new RestRequest("helix/users", Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Accept", "application/json");
            var response = client.Execute<UserResponse>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data.data.First().Login;
            }
            return null;
        }
    }
}
