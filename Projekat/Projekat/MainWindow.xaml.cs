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
using System.ComponentModel;
using Projekat.Settings;

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

            FileStream f = new FileStream("../../Save/save.txt", FileMode.Open);
            string fileContents;
            using (StreamReader reader = new StreamReader(f))
            {
                fileContents = reader.ReadToEnd();
            }

            if (fileContents=="")
                MessageBox.Show("empty");
            else
            {
                //MessageBox.Show(fileContents);
                open(fileContents);

            }
            f.Close();

        }

        private void open(string fileContents)
        {
            string[] s = fileContents.Split('\n');
            Book.Visibility = Visibility.Visible;
          //  filename = s[0].Replace('\\','/');
            filename = s[0];

            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[\r\n]{3,}", options);
            filename = regex.Replace(filename, "-*-");
            filename = filename.Replace("\r\n", "");
            filename = filename.Replace("\r", "");
            filename = filename.Replace("-*-", "");

            string text = File.ReadAllText(@filename, Encoding.UTF8);

            text = regex.Replace(text, "-*-");
            text = text.Replace("\r\n", " ");
            text = text.Replace("-*-", "\r\n\r\n");

            Paragraph paragrah = new Paragraph();
            paragrah.Inlines.Add(text);
            FlowDocument document = new FlowDocument(paragrah);
            document.Background = Brushes.LightYellow;
            document.ColumnWidth = 1000;
            document.PagePadding = new Thickness(150, 50, 50, 50);
            document.TextAlignment = TextAlignment.Justify;
            document.FontStretch = FontStretches.UltraExpanded;
            document.LineHeight = 30;

            int page = Convert.ToInt32(s[1]);
            MessageBox.Show(""+page);
            /*     //page
                 f.WriteLine(FlowDocReader.MasterPageNumber);
                 //font size
                 f.WriteLine(FlowDocReader.FontSize);
                 //font family
                 f.WriteLine(FlowDocReader.FontFamily);
                 //font color
                 f.WriteLine(FlowDocReader.Foreground);
                 //background color
                 f.WriteLine(FlowDocReader.Background);
                 //padding
                 f.WriteLine(FlowDocReader.Padding);
                 //zoom
                 f.WriteLine(FlowDocReader.Zoom);
                 */

            Doc = document;
            FlowDocReader.Document = Doc;
           
            //komanda za sakrivanje menija
            hideMenu.InputGestures.Add(new KeyGesture(Key.Escape));
            CommandBinding cb = new CommandBinding(hideMenu);
            cb.Executed += new ExecutedRoutedEventHandler(HideHandler);
            this.CommandBindings.Add(cb);
       
            CloseBook.Visibility = Visibility.Visible;
            MyMenu.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Visible;

            //this.FlowDocReader.GoToPage(page);
        }

        private static RoutedCommand hideMenu = new RoutedCommand();
        string filename ="";
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
                filename = openFileDialog.FileName;
                MessageBox.Show(filename);
                string text = File.ReadAllText(filename, Encoding.UTF8);

                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[\r\n]{3,}", options);
                text = regex.Replace(text, "-*-");
                text = text.Replace("\r\n", " ");
                text = text.Replace("-*-", "\r\n\r\n");

                Paragraph paragrah = new Paragraph();
                paragrah.Inlines.Add(text);
                FlowDocument document = new FlowDocument(paragrah);
                document.Background = Brushes.LightYellow;
                document.ColumnWidth = 1000;
                document.PagePadding = new Thickness(150,50,50,50);
                document.TextAlignment = TextAlignment.Justify;
                document.FontStretch = FontStretches.UltraExpanded;
                FlowDocReader.Document = document;

                //komanda za sakrivanje menija
                hideMenu.InputGestures.Add(new KeyGesture(Key.Escape));
                CommandBinding cb = new CommandBinding(hideMenu);
                cb.Executed += new ExecutedRoutedEventHandler(HideHandler);
                this.CommandBindings.Add(cb);

                CloseBook.Visibility = Visibility.Visible;
                MyMenu.Visibility = Visibility.Hidden;
              
                this.FlowDocReader.GoToPage(1);
            }
            
        }


        void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Book.Visibility != Visibility.Hidden)
            {
                
                StreamWriter f = new StreamWriter("../../Save/save.txt");
                f.WriteLine(filename.Split('\n')[0]);
                //page
                f.WriteLine(FlowDocReader.MasterPageNumber);
                //font size
                f.WriteLine(FlowDocReader.FontSize);
                //font family
                f.WriteLine(FlowDocReader.FontFamily);
                //font color
                f.WriteLine(FlowDocReader.Foreground);
                //background color
                f.WriteLine(FlowDocReader.Background);
                //padding
                f.WriteLine(FlowDocReader.Padding);
                //zoom
                f.WriteLine(FlowDocReader.Zoom);
                //space between lines
                //MessageBox.Show(""+FlowDocument.LineHeightProperty.DefaultMetadata);
               // f.WriteLine(.LineHeight);
             
                
                f.Close();
            }
                

            // If data is dirty, notify user and ask for a response

        }

        private void CloseBook_Click(object sender, RoutedEventArgs e)
        {
            Book.Visibility = Visibility.Hidden;
            CloseBook.Visibility = Visibility.Hidden;
            MyMenu.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Hidden;
            //isbrisi save.txt
            FileStream f = new FileStream("../../Save/save.txt", FileMode.Create);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var s = new FontSettings();
            s.Show();
        }

        private void HideHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (MyMenu.Visibility == Visibility.Visible)
            {
                MyMenu.Visibility = Visibility.Hidden;
            }
            else if (MyMenu.Visibility == Visibility.Hidden)
            {
                MyMenu.Visibility = Visibility.Visible;
            }
        }

        private void FullScreen_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FullScreen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized && WindowStyle == WindowStyle.None)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
                ResizeMode = ResizeMode.CanResize;
            }
            else
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                ResizeMode = ResizeMode.NoResize;
            }
        }
    }
}
