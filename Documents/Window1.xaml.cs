namespace Documents
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Security;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using QuantumBitDesigns.Core;
    using Db4objects.Db4o;
    
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private ObservableList<Document> _documentList;
        private BackgroundWorker backgroundWorker;

        public Window1()
        {
            InitializeComponent();

            Env.db = Db4oFactory.OpenFile("D:\\documents.db");
            backgroundWorker =
                ((BackgroundWorker)this.FindResource("addItemsWorker"));

            this._documentList = new ObservableList<Document>(
                System.Windows.Threading.Dispatcher.CurrentDispatcher);

            foreach (Document doc in Document.GetAllDocuments())
                this._documentList.Add(doc);

            lbox.ItemsSource = DocumentList;
            //LB_Tags.ItemsSource = TagDocumentRelation.TagRelations.Keys;
        }

        public ObservableCollection<Document> DocumentList
        {
            get 
            {
                return (this._documentList == null) ? null : this._documentList.ObservableCollection;
            }
        }

        private void MenuItem_Add(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openPDF = new Microsoft.Win32.OpenFileDialog();
            openPDF.Filter = "PDF Files|*.pdf|All Files|*.*";
            openPDF.CheckFileExists = true;
            openPDF.Multiselect = true;

            if (openPDF.ShowDialog() == true)
            {
                StatusText.Text = "Adding documents...";
                backgroundWorker.RunWorkerAsync(openPDF.FileNames);
            }
        }

        #region BackgroundWorker functions
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            foreach (Document doc in getFilesIn((string[])e.Argument))
                this._documentList.Add(doc);
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressbar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressbar.Value = 0;
            StatusText.Text = "";
        }
        #endregion

        private IEnumerable getFilesIn(string[] filenames)
        {
            ObservableCollection<Document> docList = new ObservableCollection<Document>();
            int arrayLength = filenames.Length;

            for (int i = 0; i < arrayLength; i++)
            {
                if ((backgroundWorker != null) && backgroundWorker.WorkerReportsProgress)
                {
                    double percentage = ((double)i / (double)arrayLength) * 100;
                    backgroundWorker.ReportProgress(Convert.ToInt32(percentage));
                }

                yield return (new Document(filenames[i])).Put();
            }
        }

        private void Context_AddTag(object sender, RoutedEventArgs e) {
            Console.WriteLine("ASDASD");
            Console.WriteLine(sender.ToString());
        }
    }
}
