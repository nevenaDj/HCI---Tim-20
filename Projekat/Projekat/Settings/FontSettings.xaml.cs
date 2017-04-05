using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Projekat.Settings
{
    /// <summary>
    /// Interaction logic for FontSettings.xaml
    /// </summary>
    public partial class FontSettings : Window
    {
        public FontSettings()
        {
            InitializeComponent();
        }

       // public FontStyle FontStyle { get; set; }
        //public FontFamily FontFamily { get; set; }
        [TypeConverterAttribute(typeof(FontSizeConverter))]
        [LocalizabilityAttribute(LocalizationCategory.None)]
        public double FontSizeAtrr { get; set; }

        private void SmallerButton_Click(object sender, RoutedEventArgs e)
        {
            FontSizeAtrr = -2;

        }

        private void BiggerButton_Click(object sender, RoutedEventArgs e)
        {
            FontSizeAtrr = +2;

        }
    }
}
