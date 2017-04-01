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

        private void OpenBook_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text documents (.txt)|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                Book.Visibility = Visibility.Visible;
                string filename = openFileDialog.FileName;
                ParagraphT.Inlines.Clear();
                ParagraphT.Inlines.Add(System.IO.File.ReadAllText(filename));
                CloseBook.Visibility = Visibility.Visible;
                
            }
        }

        private void CloseBook_Click(object sender, RoutedEventArgs e)
        {
            Book.Visibility = Visibility.Hidden;
            CloseBook.Visibility = Visibility.Hidden;
        }
    }
}
