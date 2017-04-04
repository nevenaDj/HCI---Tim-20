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
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;

namespace Projekat
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

        private static RoutedCommand hideMenu = new RoutedCommand();
        private void OpenBook_Click(object sender, RoutedEventArgs e)
        {
            //otvori dijalog za izbor knjige
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text documents (.txt)|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                //prikazi knjigu
                Book.Visibility = Visibility.Visible;
                string filename = openFileDialog.FileName;
                ParagraphT.Inlines.Clear();
                string text = File.ReadAllText(filename);

                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[\r\n]{3,}", options);
                text = regex.Replace(text, "-*-");
                text = text.Replace("\r\n", " ");
                text = text.Replace("-*-", "\r\n\r\n");

                ParagraphT.Inlines.Add(text);
                FlowDocument document = new FlowDocument(ParagraphT);

                FlowDocReader.Document = document;



                //komanda za sakrivanje menija
                hideMenu.InputGestures.Add(new KeyGesture(Key.Escape));
                CommandBinding cb = new CommandBinding(hideMenu);
                cb.Executed += new ExecutedRoutedEventHandler(HideHandler);
                this.CommandBindings.Add(cb);

                CloseBook.Visibility = Visibility.Visible;
                MyMenu.Visibility = Visibility.Hidden;

              
                //ovo ne radi 
                this.FlowDocReader.GoToPage(1);
            }
            
        }


        private void CloseBook_Click(object sender, RoutedEventArgs e)
        {
            Book.Visibility = Visibility.Hidden;
            CloseBook.Visibility = Visibility.Hidden;
            MyMenu.Visibility = Visibility.Visible;
        }

        
        private void HideHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (MyMenu.Visibility == Visibility.Visible)
            {
                MyMenu.Visibility = Visibility.Hidden;
                FlowDocReader.FirstPage();  //ovde radi xD
            }
            else if (MyMenu.Visibility == Visibility.Hidden)
            {
                MyMenu.Visibility = Visibility.Visible;
            }
        }
    }
}
