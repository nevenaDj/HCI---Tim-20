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
                f = new FileStream("../../Save/defalut.txt", FileMode.OpenOrCreate);

                using (StreamReader reader = new StreamReader(f))
                {
                    fileContents = reader.ReadToEnd();
                }

                if (fileContents != "")
                {
                    HeightW = 500;
                    WidthW = 800;
                    creating_Recent_Files();
                    previewOpen(fileContents);
               
                }

            }
            f.Close();


            this.DataContext = this;

        }

        private string[] recentFiles;
        private void open(string fileContents)
        {
            string[] s = fileContents.Split('\n');
          

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
            if (Convert.ToInt32(s[11]) == 0){
                WindowState = WindowState.Maximized;
            }
            Margins = Convert.ToInt32(s[6]);
            highlight();
            //komanda za sakrivanje menija
            hideMenu.InputGestures.Add(new KeyGesture(Key.Escape));
            CommandBinding cb = new CommandBinding(hideMenu);
            cb.Executed += new ExecutedRoutedEventHandler(HideHandler);
            this.CommandBindings.Add(cb);

            PageNum = Convert.ToInt32(Page) + 1;

            Book.Visibility = Visibility.Visible;
            CloseBook.Visibility = Visibility.Visible;
            MyMenu.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Visible;
            NightMode.Visibility = Visibility.Visible;
            Highlight.Visibility = Visibility.Visible;
            Not_Book.Visibility = Visibility.Hidden;


        }

        
        private void previewOpen(string fileContents)
        {
            
            string[] s = fileContents.Split('\n');
            

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
            Margins = Convert.ToInt32(s[6]);
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

        }

        private static RoutedCommand hideMenu = new RoutedCommand();
        string filename = "";

        private void OpenBook_Click(object sender, RoutedEventArgs e)
        {
            if (Book.Visibility != Visibility.Collapsed)
            {
                save_To_Recent_Files();
            }


            //otvori dijalog za izbor knjige
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text documents (.txt)|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
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
                Page = 0;

                FlowDocReader.GoToPage(FlowDocReader.PageCount);
                FlowDocReader.GoToPage(1);

                FileStream f = new FileStream("../../Save/recentFiles.txt", FileMode.OpenOrCreate);
                f.Close();

                string recentText = File.ReadAllText("../../Save/recentFiles.txt");
                recentText = regex.Replace(recentText, "-*-");
                recentText = recentText.Replace("\r\n", "\n");
                recentText = recentText.Replace("\r", "");
                recentText = recentText.Replace("-*-", "\n");

                recentFiles = recentText.Split('$');

                int x = 0;
                foreach (string book in recentFiles)
                {
                    string[] data = book.Split('\n');
                    if (data[0] == filename)
                    {
                        x++;
                        int size = Convert.ToInt32(data[2]);   //radi                                          
                        Font = data[3];
                        //font
                        ColorB = data[5];
                        ColorF = data[4];

                        Page = Convert.ToInt32(data[1]);  //okej -> samo ne radi jump


                        FontSizeD = size;
                        LineSpacing = Convert.ToInt32(data[8]);
                        FlowDocReader.Zoom = Convert.ToInt32(data[7]);   //radi
                        HeightW = Convert.ToInt32(data[9]);
                        WidthW = Convert.ToInt32(data[10]);
                        if (Convert.ToInt32(data[11]) == 0)
                        {
                            WindowState = WindowState.Maximized;
                        }
                        Margins = Convert.ToInt32(data[6]);
                        //FlowDocReader.GoToPage(Convert.ToInt32(Page));
                    }
                }

                if (x == 0)
                {
                    PageNum = 1;
                    FlowDocReader.GoToPage(1);

                    FontSizeD = 12;
                    LineSpacing = 20;
                    ColorB = "LightYellow";
                    ColorF = "DarkSlateGray";
                    Font = "Times New Roman";
                    Margins = 30;
                    HeightW = 500;
                    WidthW = 800;
                }

                highlight();

                //komanda za sakrivanje menija
                hideMenu.InputGestures.Add(new KeyGesture(Key.Escape));
                CommandBinding cb = new CommandBinding(hideMenu);
                cb.Executed += new ExecutedRoutedEventHandler(HideHandler);
                this.CommandBindings.Add(cb);

                Book.Visibility = Visibility.Visible;
                CloseBook.Visibility = Visibility.Visible;
                MyMenu.Visibility = Visibility.Hidden;
                Settings.Visibility = Visibility.Visible;
                NightMode.Visibility = Visibility.Visible;
                Highlight.Visibility = Visibility.Visible;
                Not_Book.Visibility = Visibility.Hidden;
            }

        }


        void save_To_Recent_Files()
        {
            FileStream f1 = new FileStream("../../Save/recentFiles.txt", FileMode.OpenOrCreate);
            f1.Close();

            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[\r\n]{3,}", options);
            string recentText = File.ReadAllText("../../Save/recentFiles.txt");
            /*recentText = regex.Replace(recentText, "-*-");
            recentText = recentText.Replace("\r\n", "\n");
            recentText = recentText.Replace("-*-", "\r\n\r\n");*/

            recentFiles = recentText.Split('$');
            string text = "";
            foreach (string book in recentFiles)
            {
                string[] data = book.Split('\n');
                if (data[0].Split('\r')[0] != filename && data[0]!="")
                {
                    text += book + "\n$";
                   // MessageBox.Show(book);
                }
            }


            StreamWriter f = new StreamWriter("../../Save/recentFiles.txt");
            f.Write(text);
            f.WriteLine(filename.Split('\n')[0]);
            //page
            f.WriteLine(FlowDocReader.MasterPageNumber-1);
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
            f.WriteLine(Margins);
            //zoom
            f.WriteLine(FlowDocReader.Zoom);
            //space between lines
            f.WriteLine(LineSpacing);
            //height
            f.WriteLine(HeightW);
            //width
            f.WriteLine(WidthW);

            if (WindowState == WindowState.Maximized)
            {
                f.WriteLine(0);
            }
            else
            {
                f.WriteLine(1);
            }
            f.WriteLine(FlowDocReader.PageCount);
            f.Write("$");
            f.Close();
        }

        void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Book.Visibility != Visibility.Collapsed)
            {
                StreamWriter f = new StreamWriter("../../Save/save.txt");
                f.WriteLine(filename.Split('\n')[0]);
                //page
                f.WriteLine(FlowDocReader.MasterPageNumber-1);
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
                f.WriteLine(Margins);
                //zoom
                f.WriteLine(FlowDocReader.Zoom);
                //space between lines
                f.WriteLine(LineSpacing);
                //height
                f.WriteLine(HeightW);
                //width
                f.WriteLine(WidthW); 

                if (WindowState == WindowState.Maximized)
                {
                    f.WriteLine(0);
                }else
                {
                    f.WriteLine(1);
                }

                f.Close();
            }

        }

        private void CloseBook_Click(object sender, RoutedEventArgs e)
        {
            save_To_Recent_Files();
            creating_Recent_Files();
            Book.Visibility = Visibility.Collapsed;
            Not_Book.Visibility = Visibility.Visible;
            CloseBook.Visibility = Visibility.Hidden;
            MyMenu.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Hidden;
            Highlight.Visibility = Visibility.Hidden;
            NightMode.Visibility = Visibility.Hidden;
            Page = 0;
            //isbrisi save.txt
            FileStream f = new FileStream("../../Save/save.txt", FileMode.Create);
        }

        private void GoToPage_Click(object sender, RoutedEventArgs e)
        {
            FlowDocReader.GoToPage(Convert.ToInt32(PageNum));
        }

        private void B1_Click(object sender, RoutedEventArgs e)
        {
            open(recentFiles[recentFiles.Length - 2]);
            int p = Page;
            FlowDocReader.GoToPage(p);
            FlowDocReader.NextPage();
        }

        private void B2_Click(object sender, RoutedEventArgs e)
        {
            open(recentFiles[recentFiles.Length - 3]);
            FlowDocReader.GoToPage(Page);
            FlowDocReader.NextPage();
            this.DataContext = this;
        }

        private void B3_Click(object sender, RoutedEventArgs e)
        {
            open(recentFiles[recentFiles.Length - 4]);
            FlowDocReader.GoToPage(Page);
            FlowDocReader.NextPage();
            this.DataContext = this;
        }

        private void B4_Click(object sender, RoutedEventArgs e)
        {
            open(recentFiles[recentFiles.Length - 5]);
            FlowDocReader.GoToPage(Convert.ToInt32(Page));
            FlowDocReader.NextPage();
            this.DataContext = this;
        }

        private void B5_Click(object sender, RoutedEventArgs e)
        {
            open(recentFiles[recentFiles.Length - 6]);
            FlowDocReader.GoToPage(Convert.ToInt32(Page));
            FlowDocReader.NextPage();
            this.DataContext = this;
        }

        private void B6_Click(object sender, RoutedEventArgs e)
        {
            open(recentFiles[recentFiles.Length - 7]);
            FlowDocReader.GoToPage(Convert.ToInt32(Page));
            FlowDocReader.NextPage();
            this.DataContext = this;
        }

        private void B7_Click(object sender, RoutedEventArgs e)
        {
            open(recentFiles[recentFiles.Length - 8]);
            FlowDocReader.GoToPage(Convert.ToInt32(Page));
            FlowDocReader.NextPage();
            this.DataContext = this;
        }

        private void B8_Click(object sender, RoutedEventArgs e)
        {
            open(recentFiles[recentFiles.Length - 9]);
            FlowDocReader.GoToPage(Convert.ToInt32(Page));
            FlowDocReader.NextPage();
            this.DataContext = this;
        }
        private void B9_Click(object sender, RoutedEventArgs e)
        {
            Page = 0;
            open(recentFiles[recentFiles.Length - 10]);
            FlowDocReader.GoToPage(Convert.ToInt32(Page));
            FlowDocReader.NextPage();
            this.DataContext = this;
        }

        private void B10_Click(object sender, RoutedEventArgs e)
        {
            open(recentFiles[recentFiles.Length - 11]);
            FlowDocReader.GoToPage(Convert.ToInt32(Page ));
            FlowDocReader.NextPage();
            this.DataContext = this;
        }

        
       

        private void creating_Recent_Files()
        {
            FileStream f = new FileStream("../../Save/recentFiles.txt", FileMode.OpenOrCreate);
            f.Close();
            string text = File.ReadAllText("../../Save/recentFiles.txt");

            recentFiles = text.Split('$');
            int size = recentFiles.Length;

            if (recentFiles.Length != 0)
            {
                if (recentFiles.Length > 1)
                {
                    button1.Visibility = Visibility.Visible;
                    string[] data = recentFiles[size-2].Split('\n');
                    int percentage = Convert.ToInt32((Convert.ToDouble(data[1])+1) / Convert.ToDouble(data[12]) * 100);
                    B1 = data[0] + "\n" + percentage + " %\nPage: "+ (Convert.ToInt32(data[1]) + 1) +"/" + data[12];
                }
                if (recentFiles.Length > 2)
                {
                    button2.Visibility = Visibility.Visible;
                    string[] data = recentFiles[size - 3].Split('\n');
                    double percentage = Convert.ToInt32((Convert.ToDouble(data[1]) + 1) / Convert.ToDouble(data[12]) * 100);
                    B2 = data[0] + "\n" + percentage + " %\nPage: " + (Convert.ToInt32(data[1]) + 1) + "/" + data[12];
                }
                if (recentFiles.Length > 3)
                {
                    button3.Visibility = Visibility.Visible;
                    string[] data = recentFiles[size - 4].Split('\n');
                    double percentage = Convert.ToInt32((Convert.ToDouble(data[1])+1 ) / Convert.ToDouble(data[12]) * 100);
                    B3 = data[0] + "\n" + percentage + " %\nPage: " + (Convert.ToInt32(data[1]) + 1) + "/" + data[12];
                }
                if (recentFiles.Length > 4)
                {
                    button4.Visibility = Visibility.Visible;
                    string[] data = recentFiles[size - 5].Split('\n');
                    double percentage = Convert.ToInt32((Convert.ToDouble(data[1]) + 1) / Convert.ToDouble(data[12]) * 100);
                    B4 = data[0] + "\n" + percentage + " %\nPage: " + (Convert.ToInt32(data[1]) + 1) + "/" + data[12];
                }
                if (recentFiles.Length > 5)
                {
                    button5.Visibility = Visibility.Visible;
                    string[] data = recentFiles[size - 6].Split('\n');
                    double percentage = Convert.ToInt32((Convert.ToDouble(data[1]) + 1) / Convert.ToDouble(data[12]) * 100);
                    B5 = data[0] + "\n" + percentage + " %\nPage: " + (Convert.ToInt32(data[1]) + 1) + "/" + data[12];
                }
                if (recentFiles.Length > 6)
                {
                    button6.Visibility = Visibility.Visible;
                    string[] data = recentFiles[size - 7].Split('\n');
                    double percentage = Convert.ToInt32((Convert.ToDouble(data[1]) + 1) / Convert.ToDouble(data[12]) * 100);
                    B6 = data[0] + "\n" + percentage + " %\nPage: " + (Convert.ToInt32(data[1]) + 1) + "/" + data[12];
                }
                if (recentFiles.Length > 7)
                {
                    button7.Visibility = Visibility.Visible;
                    string[] data = recentFiles[size - 8].Split('\n');
                    double percentage = Convert.ToInt32((Convert.ToDouble(data[1]) + 1) / Convert.ToDouble(data[12]) * 100);
                    B7 = data[0] + "\n" + percentage + " %\nPage: " + (Convert.ToInt32(data[1]) + 1) + "/" + data[12];
                }
                if (recentFiles.Length > 8)
                {
                    button8.Visibility = Visibility.Visible;
                    string[] data = recentFiles[size - 9].Split('\n');
                    double percentage = Convert.ToInt32((Convert.ToDouble(data[1]) + 1) / Convert.ToDouble(data[12]) * 100);
                    B8 = data[0] + "\n" + percentage + " %\nPage: " + (Convert.ToInt32(data[1]) + 1) + "/" + data[12];
                }
                if (recentFiles.Length > 9)
                {
                    button9.Visibility = Visibility.Visible;
                    string[] data = recentFiles[size - 10].Split('\n');
                    double percentage = Convert.ToInt32((Convert.ToDouble(data[1]) + 1) / Convert.ToDouble(data[12]) * 100);
                    B9 = data[0] + "\n" + percentage + " %\nPage: " + (Convert.ToInt32(data[1]) + 1) + "/" + data[12];
                }
                if (recentFiles.Length > 10)
                {
                    button10.Visibility = Visibility.Visible;
                    string[] data = recentFiles[size-11].Split('\n');
                    double percentage = Convert.ToInt32((Convert.ToDouble(data[1]) + 1) / Convert.ToDouble(data[12]) * 100);
                    B10 = data[0] + "\n" + percentage + " %\nPage: " + (Convert.ToInt32(data[1]) + 1) + "/" + data[12];
                }
            }
            

        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;
            Themes = new ObservableCollection<string>();
            Themes.Add("Black on white");
            Themes.Add("Sepia");
            Themes.Add("Night");

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

            if(ColorB == "#1d1d1d")
            {
                ColorB = "LightYellow";
                ColorF = "DarkSlateGray";
            }
            else
            {
                ColorB = "#1d1d1d";
                ColorF = "Azure";
            }


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
        private string _theme;
        private string _b1;
        private string _b2;
        private string _b3;
        private string _b4;
        private string _b5;
        private string _b6;
        private string _b7;
        private string _b8;
        private string _b9;
        private string _b10;


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
        public string Theme
        {
            get
            {
                return _theme;
            }
            set
            {
                if (value == "Sepia")
                {
                    ColorB = "LightYellow";
                    ColorF = "DarkSlateGray";
                    _theme = value;
                    OnPropertyChanged("Theme");
                }
                else if(value == "Black on white")
                {
                    ColorB = "White";
                    ColorF = "Black";
                    _theme = value;
                    OnPropertyChanged("Theme");
                }
                else if(value == "Night")
                {
                    ColorB = "#1d1d1d";
                    ColorF = "Azure";
                    _theme = value;
                    OnPropertyChanged("Theme");
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

        public string B1
        {
            get
            {
                return _b1;
            }
            set
            {
                if (value != _b1)
                {
                    _b1 = value;
                    OnPropertyChanged("B1");
                }
            }
        }

        public string B2
        {
            get
            {
                return _b2;
            }
            set
            {
                if (value != _b2)
                {
                    _b2 = value;
                    OnPropertyChanged("B2");
                }
            }
        }

        public string B3
        {
            get
            {
                return _b3;
            }
            set
            {
                if (value != _b3)
                {
                    _b3 = value;
                    OnPropertyChanged("B3");
                }
            }
        }

        public string B4
        {
            get
            {
                return _b4;
            }
            set
            {
                if (value != _b4)
                {
                    _b4 = value;
                    OnPropertyChanged("B4");
                }
            }
        }

        public string B5
        {
            get
            {
                return _b5;
            }
            set
            {
                if (value != _b5)
                {
                    _b5 = value;
                    OnPropertyChanged("B5");
                }
            }
        }

        public string B6
        {
            get
            {
                return _b6;
            }
            set
            {
                if (value != _b6)
                {
                    _b6 = value;
                    OnPropertyChanged("B6");
                }
            }
        }

        public string B7
        {
            get
            {
                return _b7;
            }
            set
            {
                if (value != _b7)
                {
                    _b7 = value;
                    OnPropertyChanged("B7");
                }
            }
        }

        public string B8
        {
            get
            {
                return _b8;
            }
            set
            {
                if (value != _b8)
                {
                    _b8 = value;
                    OnPropertyChanged("B8");
                }
            }
        }

        public string B9
        {
            get
            {
                return _b9;
            }
            set
            {
                if (value != _b9)
                {
                    _b9 = value;
                    OnPropertyChanged("B9");
                }
            }
        }

        public string B10
        {
            get
            {
                return _b10;
            }
            set
            {
                if (value != _b10)
                {
                    _b10 = value;
                    OnPropertyChanged("B10");
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

        public ObservableCollection<string> Themes
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

        private void highlight()
        {
            FileStream f = new FileStream("../../Save/highlight.txt", FileMode.OpenOrCreate);
            f.Close();
            string text = File.ReadAllText(@"../../Save/highlight.txt");

            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[\r\n]{3,}", options);
            text = regex.Replace(text, "-*-");
            text = text.Replace("\r\n", "\n");
            text = text.Replace("-*-", "\r\n\r\n");

            string[] highlights = text.Split('$');


            foreach (string h in highlights)
            {
                string[] texts = h.Split('\n');

                if (texts[0] == filename)
                {
                    var start = Doc.ContentStart;
                    var startPos = start.GetPositionAtOffset(Convert.ToInt32(texts[1]));
                    var endPos = start.GetPositionAtOffset(Convert.ToInt32(texts[2]));
                    var textRange = new TextRange(startPos, endPos);
                    textRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Green));
                    textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.White));

                }
            }


        }

        private void Highlight_Click(object sender, RoutedEventArgs e)
        {
            var textRange = FlowDocReader.Selection;

            TextPointer tp = FlowDocReader.Selection.Start;
            TextPointer tp2 = FlowDocReader.Selection.End;

            StreamWriter f = new StreamWriter("../../Save/highlight.txt", true);
            f.WriteLine(filename.Split('\n')[0]);
            f.WriteLine(Doc.ContentStart.GetOffsetToPosition(tp));
            f.WriteLine(Doc.ContentStart.GetOffsetToPosition(tp2));
            f.Write("$");
            //fullscreen          

            f.Close();

            if (ColorB == "#1d1d1d")
            {

                textRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.White));
                textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Gray));
            }
            else
            {
                textRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Green));
                textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.White));
            }

            //MessageBox.Show(""+textRange.Text);

        }
}
}
