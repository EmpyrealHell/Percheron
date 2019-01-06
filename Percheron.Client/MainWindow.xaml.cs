using Percheron.API.Resource;
using Percheron.Core.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Percheron.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChatClient chat;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var auth = new AuthenticationModal();
            var success = auth.ShowDialog();
            if (success.HasValue && success.Value)
            {
                var token = auth.Token;
                var username = Users.Get(token);
                Console.WriteLine("sucessfully got login info for " + username);
                this.chat = new ChatClient();
                this.chat.Connect(username, token, true);
            }
        }
    }
}
