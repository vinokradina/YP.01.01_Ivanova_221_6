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

namespace YP._01._01_Ivanova_221_6
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFile_Unicode_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(Encoding.Unicode);
        }

        private void OpenFile_Win1251_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(Encoding.GetEncoding("windows-1251"));
        }

        private void OpenFile_RTF_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                var flowDocument = new FlowDocument();
                var range = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    range.Load(stream, DataFormats.Rtf);
                }
                txtContent.Text = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd).Text;
            }
        }

        private void OpenFile_Binary_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                byte[] binaryData = File.ReadAllBytes(filePath);

                string textContent = Encoding.UTF8.GetString(binaryData);
                txtContent.Text = textContent;
            }
        }

        private void OpenFile(Encoding encoding)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileContent = File.ReadAllText(filePath, encoding);
                txtContent.Text = fileContent;
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Unicode Text (*.txt)|*.txt|Win1251 Text (*.txt)|*.txt|RTF Document (*.rtf)|*.rtf|Binary File (*.bin)|*.bin";
            saveFileDialog.FilterIndex = 1;

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                string extension = System.IO.Path.GetExtension(filePath).ToLower(); // Используем System.IO.Path

                switch (extension)
                {
                    case ".txt":
                        if (saveFileDialog.FilterIndex == 1)
                        {
                            File.WriteAllText(filePath, txtContent.Text, Encoding.Unicode);
                        }
                        else if (saveFileDialog.FilterIndex == 2)
                        {
                            File.WriteAllText(filePath, txtContent.Text, Encoding.GetEncoding("windows-1251"));
                        }
                        break;
                    case ".rtf":
                        var flowDocument = new FlowDocument();
                        var range = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                        range.Text = txtContent.Text;
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            range.Save(stream, DataFormats.Rtf);
                        }
                        break;
                    case ".bin":
                        byte[] binaryData = Encoding.ASCII.GetBytes(txtContent.Text);
                        File.WriteAllBytes(filePath, binaryData);
                        break;
                }

                txtContent.Clear();
            }
        }

        private void SaveFile_Binary_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Binary File (*.bin)|*.bin";
            saveFileDialog.FilterIndex = 1;

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                byte[] binaryData = Encoding.ASCII.GetBytes(txtContent.Text);
                File.WriteAllBytes(filePath, binaryData);

                txtContent.Clear();
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = new FlowDocument(new Paragraph(new Run(txtContent.Text)));
                doc.PageHeight = printDialog.PrintableAreaHeight;
                doc.PageWidth = printDialog.PrintableAreaWidth;
                doc.PagePadding = new Thickness(50);
                doc.ColumnGap = 0;
                doc.ColumnWidth = printDialog.PrintableAreaWidth;

                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Print Document");
            }
        }
    }
}
