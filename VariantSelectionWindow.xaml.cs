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

namespace DocGOST
{
    /// <summary>
    /// Interaction logic for VariantSelectionWindow.xaml
    /// </summary>
    public partial class VariantSelectionWindow : Window
    {
        public VariantSelectionWindow(List<String> variantNamesList)
        {
            InitializeComponent();
            variantSelectionComboBox.ItemsSource = variantNamesList;
            variantSelectionComboBox.SelectedIndex = 0;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
