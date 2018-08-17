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

namespace MemoryChallenge_Client.Windows
{
    /// <summary>
    /// Interaction logic for UserName.xaml
    /// </summary>
    public partial class UserName : Window
    {
        public UserName()
        {
            InitializeComponent();
            tbUserName.TextAlignment = TextAlignment.Center;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbUserName.Text))
            {
                MessageBox.Show("Please fill User Name field");
                return;
            }

            var gameWindow = new MainWindow(tbUserName.Text);
            gameWindow.Show();

            this.Close();
        }
    }
}
