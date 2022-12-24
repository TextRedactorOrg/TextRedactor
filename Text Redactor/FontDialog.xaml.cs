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
using System.Windows.Shapes;

namespace Text_Redactor
{
    /*
        An actual WPF font dialog.
            [X] Scrapped.
     */

    /// <summary>
    /// Interaction logic for FontDialog.xaml
    /// </summary>
    public partial class FontDialog : Window
    {
        
        public FontFamily fontFamily { get => cbFontFamilies.FontFamily; }
        public FontStretch fontStretch { get => tbSample.FontStretch; }
        public FontStyle fontStyle { get => tbSample.FontStyle; }
        public FontWeight fontWeight { get => tbSample.FontWeight; }
        public double fontSize { get => tbSample.FontSize; }
        public FontDialog()
        {
            InitializeComponent();
        }
    }
}
