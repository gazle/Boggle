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

namespace Boggle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            var box = (ListBox)sender;
            var command = box.Tag as DelegateCommand<(int, Key)>;
            int i = box.SelectedIndex;
            command?.Execute((i, e.Key));
            box.SelectedIndex = ++i;
        }
    }
}
