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
        private TwitchChatClient chat;
        private string channel;

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
                this.chat = new TwitchChatClient();
                this.chat.OnConnect += Chat_OnConnect;
                this.chat.OnPrivMsg += Chat_OnMessageReceived;
                this.chat.Connect(username, token, true);
            }
        }

        private void Chat_OnMessageReceived(ChatMessage message)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var paragraph = new Paragraph();
                var color = this.Messages.Foreground;
                if (message.Tags.ContainsKey("color"))
                {
                    var colorTag = message.Tags["color"];
                    if (!string.IsNullOrEmpty(colorTag))
                    {
                        color = (SolidColorBrush)new BrushConverter().ConvertFrom(colorTag);
                    }
                }
                paragraph.Inlines.Add(new Bold(new Run(DateTime.Now.ToString("HH:mm:ss") + " " + message.Sender + ": ") { Foreground = color }));
                paragraph.Inlines.Add(new Run(message.Message));
                this.Messages.Document.Blocks.Add(paragraph);
                if (this.Messages.Document.Blocks.Count > 4096)
                {
                    this.Messages.Document.Blocks.Remove(this.Messages.Document.Blocks.FirstBlock);
                }
                this.Messages.ScrollToEnd();
            }));
        }

        private void Chat_OnConnect()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var joinChannel = new GetChannelName("Channel", "Enter the name of a channel to join", "");
                var success = joinChannel.ShowDialog();
                if (success.HasValue && success.Value)
                {
                    this.Input.Text = "";
                    this.Messages.Document.Blocks.Clear();
                    this.channel = joinChannel.Value;
                    this.chat.JoinChannel(this.channel);
                }
            }));
        }

        private void Input_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(this.channel))
                {
                    this.chat.SendMessage(this.channel, this.Input.Text);
                }
                this.Input.Text = "";
                e.Handled = true;
            }
        }
    }
}
