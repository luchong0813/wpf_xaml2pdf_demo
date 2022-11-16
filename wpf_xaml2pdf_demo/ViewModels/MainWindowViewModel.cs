using Microsoft.Win32;

using Prism.Commands;
using Prism.Mvvm;

using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace wpf_xaml2pdf_demo.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private FlowDocument studentTemplate = null;

        public MainWindowViewModel()
        {
            ExportCommand = new DelegateCommand(ExportPdfMethod);
        }

        private delegate void DoExportPdfMethod();
        public DelegateCommand ExportCommand { get; set; }

        private void ExportPdfMethod()
        {
            Application.Current.Dispatcher.BeginInvoke(
                new DoExportPdfMethod(() => SavePDF()),
                DispatcherPriority.ApplicationIdle
                );
        }
        private void SavePDF()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "(*.pdf)|*.pdf";
                saveFileDialog.FileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                //填充模板
                if (studentTemplate == null)
                {
                    studentTemplate = FillFlowDocument();
                }
                if (saveFileDialog.ShowDialog() == true)
                {
                    string xpsPath = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), $"Xps_{DateTime.Now:yyyyMMddHHmmssfff}.xps");
                    using (XpsDocument xpsDocument = new XpsDocument(xpsPath, FileAccess.Write))
                    {
                        XpsDocumentWriter documentwriter = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                        documentwriter.Write(((IDocumentPaginatorSource)studentTemplate).DocumentPaginator);
                    }
                    PdfSharp.Xps.XpsConverter.Convert(xpsPath, saveFileDialog.FileName, 0);
                    DeletedInvalidFile(xpsPath);
                    MessageBox.Show($"PDF文件保存成功，路径：{saveFileDialog.FileName}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存PDF时发生异常：{ex}");
            }
        }

        private FlowDocument FillFlowDocument()
        {
            FlowDocument doc = (FlowDocument)Application.LoadComponent(new Uri("/wpf_xaml2pdf_demo;component/Resources/StudentTemplate.xaml", UriKind.RelativeOrAbsolute));
            doc.DataContext = new Models.Student
            {
                Name = "上海-wpf-老色批",
                Class = "三年一班",
                Chinese = 99,
                Math = 59,
                English = 66,
                Comments = "此乃难得一见的奇才，未来可期！！！（Tips:这文字是绑定的，不是静态数据）"
            };
            return doc;
        }

        public bool DeletedInvalidFile(string str)
        {
            try
            {
                if (File.Exists(str))
                {
                    File.Delete(str);
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}
