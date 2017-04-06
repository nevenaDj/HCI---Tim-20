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
using System.Collections.ObjectModel;



namespace Projekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            

            //Meni ovo ne radi !!!! - Nevena
            
            FileStream f = new FileStream("../../Save/save.txt", FileMode.Open);
            string fileContents;
            using (StreamReader reader = new StreamReader(f))
            {
                fileContents = reader.ReadToEnd();
            }

            if (fileContents == "")
                MessageBox.Show("empty");
            else
            {
                //MessageBox.Show(fileContents);
                open(fileContents);
                FlowDocReader.GoToPage(page);
            }
            f.Close();
            
            this.DataContext = this;
        }
        int page = 1;
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

            string text = File.ReadAllText(filename, Encoding.UTF8);

            text = regex.Replace(text, "-*-");
            text = text.Replace("\r\n", " ");
            text = text.Replace("-*-", "\r\n\r\n");

            Paragraph paragrah = new Paragraph(new Run(text));
            //  paragrah.Inlines.Add();
            int size = Convert.ToInt32(s[2]);   //radi

            Doc = new FlowDocument(paragrah);

            string[] pad = s[6].Split(',');   //ne dobijem dobru padding
            Doc.ColumnWidth = 1000;
            Doc.PagePadding = new Thickness(Convert.ToInt32(pad[0]), Convert.ToInt32(pad[1]), Convert.ToInt32(pad[2]), Convert.ToInt32(pad[3]));
            Doc.TextAlignment = TextAlignment.Justify;
            Doc.FontStretch = FontStretches.UltraExpanded;
            Doc.FontSize = size;       //radi
            Doc.FontFamily = new FontFamily(s[3]);  //radi
            Color color = (Color)ColorConverter.ConvertFromString(s[4]);
            SolidColorBrush brush = new SolidColorBrush(color);
            Doc.Foreground = brush;  //ne radi
            Color color2 = (Color)ColorConverter.ConvertFromString(s[5]);
            SolidColorBrush brush2 = new SolidColorBrush(color2);
            Doc.Background = brush2;  //ne radi
            page = Convert.ToInt32(s[1]);  //okej -> samo ne radi jump
            Doc.LineHeight = Convert.ToInt32(s[8]);

            FlowDocReader.Document = Doc;
            FlowDocReader.Zoom = Convert.ToInt32(s[7]);   //radi
            //komanda za sakrivanje menija
            hideMenu.InputGestures.Add(new KeyGesture(Key.Escape));
            CommandBinding cb = new CommandBinding(hideMenu);
            cb.Executed += new ExecutedRoutedEventHandler(HideHandler);
            this.CommandBindings.Add(cb);

            CloseBook.Visibility = Visibility.Visible;
            MyMenu.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Visible;

            MessageBox.Show("" + FlowDocReader.CanGoToPage(2));
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


                //Paragraph paragrah = new Paragraph();
                Par.Inlines.Clear();
                Par.Inlines.Add(text);

                //Doc = new FlowDocument(paragrah);
                //Doc.Background = Brushes.LightYellow;
                //Doc.ColumnWidth = 1000;
                //Doc.PagePadding = new Thickness(150, 50, 50, 50);
                //Doc.TextAlignment = TextAlignment.Justify;
                //Doc.FontStretch = FontStretches.UltraExpanded;
                // Doc.LineHeight = 30;

                //FlowDocReader.Document = Doc;


                //komanda za sakrivanje menija
                hideMenu.InputGestures.Add(new KeyGesture(Key.Escape));
                CommandBinding cb = new CommandBinding(hideMenu);
                cb.Executed += new ExecutedRoutedEventHandler(HideHandler);
                this.CommandBindings.Add(cb);

                CloseBook.Visibility = Visibility.Visible;
                MyMenu.Visibility = Visibility.Hidden;
                Settings.Visibility = Visibility.Visible;
                NightMode.Visibility = Visibility.Visible;

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
                f.WriteLine(Doc.FontSize);
                //font family
                f.WriteLine(Doc.FontFamily);
                //font color
                f.WriteLine(Doc.Foreground);
                //background color
                f.WriteLine(FlowDocReader.Background);
                //padding
             //   MessageBox.Show(""+ Doc.PagePadding);
                f.WriteLine(Doc.PagePadding);
                //zoom
                f.WriteLine(FlowDocReader.Zoom);
                //space between lines
                //MessageBox.Show(""+FlowDocument.LineHeightProperty.DefaultMetadata);
                f.WriteLine(Doc.LineHeight);
             
                
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
            this.DataContext = this;
            Theme = new ObservableCollection<string>();
            Theme.Add("Black on white");
            Theme.Add("Sepia");
            Theme.Add("Night");

            Fonts = new ObservableCollection<string>();

            foreach (System.Drawing.FontFamily font in System.Drawing.FontFamily.Families)
            {
                Fonts.Add(font.Name);
            }

            FontSettings newWindow = new FontSettings()
            {
                DataContext = this
            };
            newWindow.ShowDialog();
        }

        private void NightMode_Click(object sender, RoutedEventArgs e)
        {
            
          
            
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

        #region NotifyProperties
        private string _lineSpacing;
        private string _margins;
        private string _fontSize;
        private string _font;
        public string LineSpacing
        {
            get
            {
                return _lineSpacing;
            }
            set
            {
                if (value != _lineSpacing)
                {
                    _lineSpacing = value;
                    OnPropertyChanged("LineSpacing");
                }
            }
        }
        public string Margins
        {
            get
            {
                return _margins;
            }
            set
            {
                if (value != _margins)
                {
                    _margins = value;
                    OnPropertyChanged("Margins");
                }
            }
        }
        public string FontSizeD
        {
            get
            {
                return _fontSize;
            }
            set
            {
                if (value != _fontSize)
                {
                    _fontSize = value;
                    OnPropertyChanged("FontSizeD");
                }
            }
        }

        public string Font
        {
            get
            {
                return _font;
            }
            set
            {
                if (value != _font)
                {
                    _font = value;
                    OnPropertyChanged("Font");
                }
            }
        }

        public ObservableCollection<string> Theme
        {
            get;
            set;
        }

        public ObservableCollection<string> Fonts
        {
            get;
            set;
        }

        #endregion
        #region PropertyChangedNotifier
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
