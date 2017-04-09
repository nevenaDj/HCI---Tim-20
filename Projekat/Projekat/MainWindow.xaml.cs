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
using System.Windows.Controls.Primitives;

namespace Projekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();


            //Meni ovo ne radi !!!! - Nevena
            
            FileStream f = new FileStream("../../Save/save.txt", FileMode.OpenOrCreate);
            string fileContents;
            using (StreamReader reader = new StreamReader(f))
            {
                fileContents = reader.ReadToEnd();
            }

            if (fileContents != "")
            {
                open(fileContents);
                FlowDocReader.GoToPage(Convert.ToInt32(Page));
            }
            else
            {
                HeightW = 500;
                WidthW = 800;
            }
            f.Close();


            this.DataContext = this;
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

            string text = File.ReadAllText(filename, Encoding.UTF8);

            text = regex.Replace(text, "-*-");
            text = text.Replace("\r\n", " ");
            text = text.Replace("-*-", "\r\n\r\n");

            int size = Convert.ToInt32(s[2]);   //radi                                          
            Margins = Convert.ToInt32( s[6]);
            Font = s[3];
            //font
            ColorB = s[5];
            ColorF = s[4];

            Page = Convert.ToInt32(s[1]);  //okej -> samo ne radi jump
            Par.Inlines.Clear();
            Par.Inlines.Add(text);
            FontSizeD = size;
            LineSpacing = Convert.ToInt32(s[8]);
            FlowDocReader.Zoom = Convert.ToInt32(s[7]);   //radi
            HeightW = Convert.ToInt32(s[9]);
            WidthW = Convert.ToInt32(s[10]);
            Margins = Convert.ToInt32(s[11]);
            //komanda za sakrivanje menija
            hideMenu.InputGestures.Add(new KeyGesture(Key.Escape));
            CommandBinding cb = new CommandBinding(hideMenu);
            cb.Executed += new ExecutedRoutedEventHandler(HideHandler);
            this.CommandBindings.Add(cb);

            PageNum = Convert.ToInt32(Page) + 1;

            CloseBook.Visibility = Visibility.Visible;
            MyMenu.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Visible;
            NightMode.Visibility = Visibility.Visible;


        }

        private static RoutedCommand hideMenu = new RoutedCommand();
        string filename = "";

        private void OpenBook_Click(object sender, RoutedEventArgs e)
        {

            FileStream f = new FileStream("../../Save/save.txt", FileMode.Create);

            //otvori dijalog za izbor knjige
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text documents (.txt)|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
              //  Page = 0;
                //prikazi knjigu
                Book.Visibility = Visibility.Visible;
                filename = openFileDialog.FileName;
                string text = File.ReadAllText(filename, Encoding.UTF8);

                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[\r\n]{3,}", options);
                text = regex.Replace(text, "-*-");
                text = text.Replace("\r\n", " ");
                text = text.Replace("-*-", "\r\n\r\n");

                Par.Inlines.Clear();
                Par.Inlines.Add(text);

                PageNum = 1;
                //Page = 1;
                
                
                FlowDocReader.GoToPage(FlowDocReader.PageCount);
                FlowDocReader.GoToPage(1);

                //
                FontSizeD = 12;
                LineSpacing = 20;
                ColorB = "LightYellow";
                ColorF = "DarkSlateGray";
                Font = "Times New Roman";
                Margins = 300;
                HeightW = 500;
                WidthW = 800;


                //komanda za sakrivanje menija
                hideMenu.InputGestures.Add(new KeyGesture(Key.Escape));
                CommandBinding cb = new CommandBinding(hideMenu);
                cb.Executed += new ExecutedRoutedEventHandler(HideHandler);
                this.CommandBindings.Add(cb);

                CloseBook.Visibility = Visibility.Visible;
                MyMenu.Visibility = Visibility.Hidden;
                Settings.Visibility = Visibility.Visible;
                NightMode.Visibility = Visibility.Visible;
                //Page = 0;
                //FlowDocReader.GoToPage(1);
            }

        }


        void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Book.Visibility != Visibility.Hidden)
            {
                StreamWriter f = new StreamWriter("../../Save/save.txt");
                f.WriteLine(filename.Split('\n')[0]);
                //page
                f.WriteLine(FlowDocReader.MasterPageNumber - 1);
                //font size
                f.WriteLine(FontSizeD);
                //font family
                f.WriteLine(Font);
                //font color
                f.WriteLine(ColorF);
                //background color
                f.WriteLine(ColorB);
                //padding
                //   MessageBox.Show(""+ Doc.PagePadding);
                // f.WriteLine(Margins);
                f.WriteLine("20");
                //zoom
                f.WriteLine(FlowDocReader.Zoom);
                //space between lines
                f.WriteLine(LineSpacing);
                //height
                f.WriteLine(HeightW);
                //width
                f.WriteLine(WidthW);
                //fullscreen     
                f.WriteLine(Margins);     

                f.Close();
            }



        }

        private void CloseBook_Click(object sender, RoutedEventArgs e)
        {
            Book.Visibility = Visibility.Hidden;
            CloseBook.Visibility = Visibility.Hidden;
            MyMenu.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Hidden;
            Page = 0;
            //isbrisi save.txt
            FileStream f = new FileStream("../../Save/save.txt", FileMode.Create);
        }

        private void GoToPage_Click(object sender, RoutedEventArgs e)
        {

            FlowDocReader.GoToPage(Convert.ToInt32(PageNum));

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
        private int _lineSpacing;
        private int _margins;
        private int _fontSize;
        private string _font;
        private int _page;
        private int _pageNum;
        private string _colorB;
        private string _colorF;
        private int _heightW;
        private int _widthW;
        private Thickness _changeMargins;

        public int LineSpacing
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
        public int Margins
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
                    ChangeMargins = new Thickness(_margins, 10, _margins, 10);
                    OnPropertyChanged("Margins");
                }
            }
        }
        public Thickness ChangeMargins
        {
            get
            {
                return _changeMargins;
            }
            set
            {
                if (value != _changeMargins)
                {
                    _changeMargins = value;
                    OnPropertyChanged("ChangeMargins");
                }
            }
        }
        public int FontSizeD
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

        public int Page
        {
            get
            {
                return _page;
            }
            set
            {
                if (value != _page)
                {
                    _page = value;
                    OnPropertyChanged("Page");
                }
            }
        }

        public string ColorB
        {
            get
            {
                return _colorB;
            }
            set
            {
                if (value != _colorB)
                {
                    _colorB = value;
                    OnPropertyChanged("ColorB");
                }
            }
        }

        public string ColorF
        {
            get
            {
                return _colorF;
            }
            set
            {
                if (value != _colorF)
                {
                    _colorF = value;
                    OnPropertyChanged("ColorF");
                }
            }
        }

        public int PageNum
        {
            get
            {
                return _pageNum;
            }
            set
            {
                if (value != _pageNum)
                {
                    _pageNum = value;
                    OnPropertyChanged("PageNum");
                }
            }
        }

        public int HeightW
        {
            get
            {
                return _heightW;
            }
            set
            {
                if (value != _heightW)
                {
                    _heightW = value;
                    OnPropertyChanged("HeightW");
                }
            }
        }

        public int WidthW
        {
            get
            {
                return _widthW;
            }
            set
            {
                if (value != _widthW)
                {
                    _widthW = value;
                    OnPropertyChanged("WidthW");
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
