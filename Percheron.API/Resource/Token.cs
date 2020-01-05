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
    public static class Token
    {
        public static TokenResponse Get(string clientId, string clientSecret, string code, string redirectUri)
        {
            var client = new RestClient("https://id.twitch.tv");
            client.AddHandler("application/json", NewtonsoftDeserializer.Default);
            var request = new RestRequest("oauth2/token", Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddQueryParameter("client_id", clientId);
            request.AddQueryParameter("client_secret", clientSecret);
            request.AddQueryParameter("code", code);
            request.AddQueryParameter("grant_type", "authorization_code");
            request.AddQueryParameter("redirect_uri", redirectUri);
            var response = client.Execute<TokenResponse>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }
            return null;
        }
    }
}
