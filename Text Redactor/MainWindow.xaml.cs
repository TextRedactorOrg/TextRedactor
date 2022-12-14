﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Text_Redactor
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

        private void btNew_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? res = openFileDialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                using(FileStream fstream = new FileStream(openFileDialog.FileName, FileMode.Open))
                {
                    TextRange range = new TextRange(rtbText.Document.ContentStart, rtbText.Document.ContentEnd);
                    range.Load(fstream, DataFormats.XamlPackage);
                    
                }
            }
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btSaveas_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}