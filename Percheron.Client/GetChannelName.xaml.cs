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
using System.Windows.Shapes;

namespace Percheron.Client
{
    /// <summary>
    /// Interaction logic for GetChannelName.xaml
    /// </summary>
    public partial class GetChannelName : Window
    {
        public string Value { get; set; }

        public GetChannelName(string caption, string message, string initialText)
        {
            InitializeComponent();
            this.Title = caption;
            this.Message.Text = message;
            this.Input.Text = initialText;
            this.Input.Focus();
        }

        private void SendData()
        {
            this.DialogResult = true;
            this.Value = this.Input.Text;
            this.Close();
        }

        private void Input_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.SendData();
            }
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            this.SendData();
        }
    }
}
