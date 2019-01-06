using Percheron.API.Utility;
using Percheron.Client.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Percheron.Client
{
    /// <summary>
    /// Interaction logic for AuthenticationModal.xaml
    /// </summary>
    public partial class AuthenticationModal : Window
    {
        private const string client_id = "9rpwecj5h5juxzc7ln7w6iqcoiumgo";
        private const string redirect_uri = "http://localhost";

        private string state;

        public string Token { get; set; }

        public AuthenticationModal()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.state = AuthState.Generate();
            var builder = new UriBuilder("https", "id.twitch.tv");
            builder.Path = "oauth2/authorize";
            builder.AddQuery("client_id", client_id);
            builder.AddQuery("redirect_uri", redirect_uri);
            builder.AddQuery("response_type", "token");
            builder.AddQuery("scope", "chat:read chat:edit whispers:read whispers:edit");
            builder.AddQuery("force_verify", "true");
            builder.AddQuery("state", this.state);
            Console.WriteLine(builder.Uri);
            this.Browser.Navigate(builder.Uri);
        }

        private void Browser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Uri.ToString().StartsWith(redirect_uri))
            {
                var returnValues = e.Uri.Fragment.Substring(1).Split('&').Select(x => x.Split('=')).ToDictionary(key => key[0], value => value[1]);
                if (returnValues["state"] == this.state)
                {
                    this.Token = returnValues["access_token"];
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    throw new SecurityException("CSRF attack detecting, exiting application");
                }
            }
        }
    }
}
