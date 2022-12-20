using Microsoft.Win32;
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
using IronPdf;

namespace Text_Redactor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string fileName = "text";
        private bool saved = false;
        private Encoding encoding = Encoding.UTF8;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btNew_Click(object sender, RoutedEventArgs e)
        {
            if (!saved)
            {
                MessageBoxResult result= MessageBox.Show("Do you want to save changes", "Text Redactor", MessageBoxButton.YesNoCancel);
                {
                    if (result == MessageBoxResult.Yes)
                    {
                        if (File.Exists(fileName))
                        {
                            File.WriteAllText(fileName, new TextRange(rtbText.Document.ContentStart, rtbText.Document.ContentEnd).Text, encoding);
                            rtbText.Document.Blocks.Clear(); fileName = "text";
                        }
                        else
                        {
                            SaveFileDialog dlg = new SaveFileDialog();

                            dlg.Filter = "All Files (*.*)|*.*|Text Files (*.txt)|*.txt|Pdf Files (*.pdf)|*.pdf|Rtf Files (*.rtf)|*.rtf";
                            dlg.FilterIndex = 2;
                            dlg.FileName = fileName;

                            bool? res = dlg.ShowDialog();
                            if (res.HasValue && res.Value)
                            {
                                File.WriteAllText(dlg.FileName, new TextRange(rtbText.Document.ContentStart, rtbText.Document.ContentEnd).Text, encoding);
                                rtbText.Document.Blocks.Clear();fileName = "text";
                            }

                        }
                        
                    }else if(result == MessageBoxResult.No)
                    {
                        rtbText.Document.Blocks.Clear();
                    }
                }

            }
            else
            {
                rtbText.Document.Blocks.Clear();
                saved = false;
            }
            
        }
        void SaveToRtf(string _fileName)
        {
            TextRange range;
            FileStream fStream;
            range = new TextRange(rtbText.Document.ContentStart, rtbText.Document.ContentEnd);
            fStream = new FileStream(_fileName, FileMode.Create);
            range.Save(fStream, DataFormats.Rtf);
            fStream.Close();
        }

        private void btOpen_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //bool? res = openFileDialog.ShowDialog();
            //if (res.HasValue && res.Value)
            //{
            //    using (FileStream fstream = new FileStream(openFileDialog.FileName, FileMode.Open))
            //    {
            //        TextRange range = new TextRange(rtbText.Document.ContentStart, rtbText.Document.ContentEnd);
            //        range.Load(fstream, DataFormats.XamlPackage);

            //    }
            //}
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files (*.*)|*.*|Text Files (*.txt)|*.txt|Pdf Files (*.pdf)|*.pdf|Rtf Files (*.rtf)|*.rtf";
            dlg.FilterIndex = 2;
            bool? res = dlg.ShowDialog();
            if (res.HasValue && res.Value)
            {
                rtbText.Document.Blocks.Clear();
                using (StreamReader reader = File.OpenText(dlg.FileName))
                {
                    rtbText.Document.Blocks.Add(new Paragraph(new Run(reader.ReadToEnd())));
                }
                fileName = dlg.FileName;
                saved = true;
            }
            
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(fileName))
            {
                File.WriteAllText(fileName, new TextRange(rtbText.Document.ContentStart, rtbText.Document.ContentEnd).Text, encoding);
                saved = true;
            }
            else
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "All Files (*.*)|*.*|Text Files (*.txt)|*.txt|Pdf Files (*.pdf)|*.pdf|Rtf Files (*.rtf)|*.rtf";
                dlg.FilterIndex = 2;
                dlg.FileName = fileName;
                bool? res = dlg.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    File.WriteAllText(dlg.FileName, new TextRange(rtbText.Document.ContentStart, rtbText.Document.ContentEnd).Text, encoding);
                    saved = true;
                }

            }

        }

        private void btSaveas_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "All Files (*.*)|*.*|Text Files (*.txt)|*.txt|Pdf Files (*.pdf)|*.pdf|Rtf Files (*.rtf)|*.rtf";
            dlg.FilterIndex = 2;
            //dlg.FileName = fileName;
            
           
            bool? res = dlg.ShowDialog();
            if (res.HasValue && res.Value)
            {
                FileInfo fileInfo = new FileInfo(dlg.FileName);
                string ext = fileInfo.Extension;
                switch (ext)
                {
                    case ".rtf":
                        {
                            SaveToRtf(dlg.FileName);
                            break;
                        }
                    case ".pdf":
                        {
                            PrintDialog pd = new PrintDialog();
                            if (pd.ShowDialog() == true)
                            {
                                IDocumentPaginatorSource idp = rtbText.Document;
                                pd.PrintDocument(idp.DocumentPaginator, "Document");
                            }
                            break;
                        }
                    default:
                        {
                            File.WriteAllText(dlg.FileName, new TextRange(rtbText.Document.ContentStart, rtbText.Document.ContentEnd).Text);
                            break;
                        }
                }
                saved = true;

            }
            
        }

        private void rtbText_TextChanged(object sender, TextChangedEventArgs e)
        {
            saved = false;
        }

        private void btFont_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
