﻿using System;
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
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();


            //Meni ovo ne radi !!!! - Nevena
            Page = "0";
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
            string[] pad = s[6].Split(',');   //ne dobijem dobru padding
           /* Doc.ColumnWidth = 1000;
            Doc.PagePadding = new Thickness(Convert.ToInt32(pad[0]), Convert.ToInt32(pad[1]), Convert.ToInt32(pad[2]), Convert.ToInt32(pad[3]));
            Doc.TextAlignment = TextAlignment.Justify;
            Doc.FontStretch = FontStretches.UltraExpanded;
            Doc.FontFamily = new FontFamily(s[3]);  //radi
            Color color = (Color)ColorConverter.ConvertFromString(s[4]);
            SolidColorBrush brush = new SolidColorBrush(color);
            Doc.Foreground = brush;  //ne radi
            Color color2 = (Color)ColorConverter.ConvertFromString(s[5]);
            SolidColorBrush brush2 = new SolidColorBrush(color2);
            Doc.Background = brush2;  //ne radi*/
            Page = s[1];  //okej -> samo ne radi jump
            Par.Inlines.Clear();
            Par.Inlines.Add(text);
            FontSizeD = size;
            LineSpacing = Convert.ToInt32(s[8]); 
            FlowDocReader.Zoom = Convert.ToInt32(s[7]);   //radi

            //komanda za sakrivanje menija
            hideMenu.InputGestures.Add(new KeyGesture(Key.Escape));
            CommandBinding cb = new CommandBinding(hideMenu);
            cb.Executed += new ExecutedRoutedEventHandler(HideHandler);
            this.CommandBindings.Add(cb);

            PageNum = Convert.ToInt32(Page)+1;

            CloseBook.Visibility = Visibility.Visible;
            MyMenu.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Visible;
            //GoToPage.Visibility = Visibility.Visible;


            

        }

        private static RoutedCommand hideMenu = new RoutedCommand();
        string filename ="";

        private void OpenBook_Click(object sender, RoutedEventArgs e)
        {
            FileStream f = new FileStream("../../Save/save.txt", FileMode.Create);

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

                Par.Inlines.Clear();
                Par.Inlines.Add(text);


                //Doc.Background = Brushes.LightYellow;
                //Doc.ColumnWidth = 1000;
                //Doc.PagePadding = new Thickness(150, 50, 50, 50);
                //Doc.TextAlignment = TextAlignment.Justify;
                //Doc.FontStretch = FontStretches.UltraExpanded;
                // Doc.LineHeight = 30;



                //komanda za sakrivanje menija
                hideMenu.InputGestures.Add(new KeyGesture(Key.Escape));
                CommandBinding cb = new CommandBinding(hideMenu);
                cb.Executed += new ExecutedRoutedEventHandler(HideHandler);
                this.CommandBindings.Add(cb);

                CloseBook.Visibility = Visibility.Visible;
                MyMenu.Visibility = Visibility.Hidden;
                Settings.Visibility = Visibility.Visible;
                NightMode.Visibility = Visibility.Visible;
       
                Page = "0";
                this.FlowDocReader.GoToPage(1);
                PageNum = 1;
          
                FontSizeD = 12;
                LineSpacing = 20;
                
            }
            
        }


        void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Book.Visibility != Visibility.Hidden)
            {
                
                StreamWriter f = new StreamWriter("../../Save/save.txt");
                f.WriteLine(filename.Split('\n')[0]);
                //page
                f.WriteLine(FlowDocReader.MasterPageNumber-1);
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
        private double _lineSpacing;
        private double _margins;
        private double _fontSize;
        private string _font;
        private string _page;
        private int _pageNum;

        public double LineSpacing
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
        public double Margins
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
        public double FontSizeD
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

        public string Page
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
