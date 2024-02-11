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

namespace CobraCrawl
{
    /// <summary>
    /// Logika interakcji dla klasy PlayerNameDialog.xaml
    /// </summary>
    public partial class PlayerNameDialog : Window
    {
        public string PlayerName { get; private set; }

        public PlayerNameDialog()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerName = PlayerNameTextBox.Text;
            this.DialogResult = true; // Set result on true
            this.Close(); // Cose the window
        }
    }
}
