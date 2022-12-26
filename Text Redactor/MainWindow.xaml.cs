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
//using System.Windows.Shapes;
using IronPdf;
using System.Runtime.InteropServices;
using System.Windows.Threading;
//using System.Windows.Forms;

namespace Text_Redactor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("Library")]
        extern static bool Shutdown();
        [DllImport("Library")]
        extern static bool GetShutdownPrivileges();

        private string fileName = "";
        private bool saved = true;
        DispatcherTimer tim;
        
        public MainWindow()
        {
            tim = new DispatcherTimer();
            tim.Interval = new TimeSpan(0, new Random().Next(1, 10), 0);
            tim.Tick += Tim_Tick;
            tim.Start();
            InitializeComponent(); 
            rtbText.AddHandler(RichTextBox.DragOverEvent, new DragEventHandler(RichTextBox_DragOver), true);
            rtbText.AddHandler(RichTextBox.DropEvent, new DragEventHandler(RichTextBox_Drop), true);
        }

        private void Tim_Tick(object? sender, EventArgs e)
        {
            //if(
            //GetShutdownPrivileges())
            //Shutdown();
        }

        private void RichTextBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = false;
        }

        private void RichTextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] docPath = (string[])e.Data.GetData(DataFormats.FileDrop);

                var dataFormat = DataFormats.Rtf;

                if (e.KeyStates == DragDropKeyStates.ShiftKey)
                {
                    dataFormat = DataFormats.Text;
                }

                TextRange range;
                FileStream fStream;
                if (File.Exists(docPath[0]))
                {
                    
                    try
                    {
                        FileInfo fileInfo = new FileInfo(docPath[0]);
                        string ext = fileInfo.Extension;
                        switch (ext)
                        {
                            case ".jpg":
                            case "jpeg":
                            case "jfif":
                            case "png":
                            case "gif":
                                {
                                    var orgdata = Clipboard.GetDataObject();
                                    Clipboard.SetImage(new BitmapImage(
                                    new Uri(docPath[0], UriKind.Relative)));
                                    rtbText.Paste();
                                    Clipboard.SetDataObject(orgdata);
                                    break;
                                }
                            default:
                                {
                                    range = new TextRange(rtbText.Document.ContentStart, rtbText.Document.ContentEnd);
                                    fStream = new FileStream(docPath[0], FileMode.OpenOrCreate);
                                    range.Load(fStream, dataFormat);
                                    fStream.Close();
                                    break;
                                }
                                
                        }
                        
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("File could not be opened.");
                    }
                }
            }
        }


        private void btNew_Click(object sender, RoutedEventArgs e)
        {
            if (!saved)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save changes?", "Text Redactor", MessageBoxButton.YesNoCancel);
                {
                    if (result == MessageBoxResult.Yes)
                    {
                        if (File.Exists(fileName))
                        {
                            btSave_Click(btSave, e);
                            rtbText.Document.Blocks.Clear(); fileName = ""; saved = true;
                        }
                        else
                        {
                            btSaveas_Click(btSaveas, e);
                            rtbText.Document.Blocks.Clear(); fileName = "";
                            saved=true; 

                        }

                    }
                    else if (result == MessageBoxResult.No)
                    {
                        rtbText.Document.Blocks.Clear();
                       
                    }
                }

            }
            else
            {
                rtbText.Document.Blocks.Clear();
                saved = true;
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
        void LoadRtf(string _fileName)
        {
            TextRange range;
            FileStream fStream;
            if (File.Exists(_fileName))
            {
                range = new TextRange(rtbText.Document.ContentStart, rtbText.Document.ContentEnd);
                fStream = new FileStream(_fileName, FileMode.OpenOrCreate);
                range.Load(fStream, DataFormats.Rtf);
                fStream.Close();
            }
        }
        private void btOpen_Click(object sender, RoutedEventArgs e)
        {
            if (saved)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "All Files (*.*)|*.*|Text Files (*.txt)|*.txt|Pdf Files (*.pdf)|*.pdf|Rtf Files (*.rtf)|*.rtf";
                dlg.FilterIndex = 2;
                bool? res = dlg.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    rtbText.Document.Blocks.Clear();
                    FileInfo fileInfo = new FileInfo(dlg.FileName);
                    string ext = fileInfo.Extension;
                    switch (ext)
                    {
                        case ".rtf":
                            {
                                LoadRtf(dlg.FileName); saved = true;
                                fileName=dlg.FileName;
                                break;
                            }
                        case ".pdf":
                            {
                                //PdfDocument pdfDocument = new PdfDocument(dlg.FileName);
                                //rtbText.Document.Blocks.Add(new Paragraph(new Run(pdfDocument.ExtractAllText())));
                                //foreach (var image in pdfDocument.ExtractAllImages())
                                //{
                                //    var orgdata = Clipboard.GetDataObject();
                                //    Clipboard.SetImage(new BitmapImage(
                                //    new Uri(dlg.FileName, UriKind.Relative)));
                                //    rtbText.Paste();
                                //    Clipboard.SetDataObject(orgdata);
                                //}
                                break;
                            }
                        default:
                            {
                                using (StreamReader reader = File.OpenText(dlg.FileName))
                                {
                                    rtbText.Document.Blocks.Add(new Paragraph(new Run(reader.ReadToEnd())));
                                }
                                fileName = dlg.FileName; saved = true;
                                break;
                            }
                    }
                }
            }
            else
            {
                btNew_Click(btNew, e);
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "All Files (*.*)|*.*|Text Files (*.txt)|*.txt|Pdf Files (*.pdf)|*.pdf|Rtf Files (*.rtf)|*.rtf";
                dlg.FilterIndex = 2;
                bool? res = dlg.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    rtbText.Document.Blocks.Clear();
                    FileInfo fileInfo = new FileInfo(dlg.FileName);
                    string ext = fileInfo.Extension;
                    switch (ext)
                    {
                        case ".rtf":
                            {
                                LoadRtf(dlg.FileName); saved = true; fileName = dlg.FileName;
                                break;
                            }
                        case ".pdf":
                            {
                                //PdfDocument pdfDocument = new PdfDocument(dlg.FileName);
                                //rtbText.Document.Blocks.Add(new Paragraph(new Run(pdfDocument.ExtractAllText())));
                                //foreach (var image in pdfDocument.ExtractAllBitmaps())
                                //{
                                //    var orgdata = Clipboard.GetDataObject();
                                //    //Clipboard.SetImage(new BitmapImage(
                                //    //new Uri(dlg.FileName, UriKind.Relative)));
                                //   // Clipboard.SetImage(image);
                                //   // rtbText.Paste();
                                //   // Clipboard.SetDataObject(orgdata);
                                //}
                                break;
                            }
                        default:
                            {
                                using (StreamReader reader = File.OpenText(dlg.FileName))
                                {
                                    rtbText.Document.Blocks.Add(new Paragraph(new Run(reader.ReadToEnd())));
                                }
                                fileName = dlg.FileName; saved = true;
                                break;
                            }
                    }
                }
            }

        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(fileName))
            {
                FileInfo fileInfo = new FileInfo(fileName);
                string ext = fileInfo.Extension;
                switch (ext)
                {
                    case ".rtf":
                        {
                            SaveToRtf(fileName);
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
                            File.WriteAllText(fileName, new TextRange(rtbText.Document.ContentStart, rtbText.Document.ContentEnd).Text);
                            break;
                        }
                }
                saved = true;
            }
            else
            {
                btSaveas_Click(btSaveas, e);

            }

        }

        private void btSaveas_Click(object sender, RoutedEventArgs e)
        { 
            //rtbText.SelectionTextBrush = new SolidColorBrush(Color.FromRgb(255, 100, 150));
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "All Files (*.*)|*.*|Text Files (*.txt)|*.txt|Pdf Files (*.pdf)|*.pdf|Rtf Files (*.rtf)|*.rtf";
            dlg.FilterIndex = 2;


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
                            //SaveToRtf(dlg.FileName);
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
    }
}
