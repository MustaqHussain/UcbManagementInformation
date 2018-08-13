using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UcbManagementInformation.MVVM;
using System.IO;
using UcbManagementInformation.Commanding;
using UcbManagementInformation.Helpers;
using System.ComponentModel;
using System.Windows.Threading;

namespace UcbManagementInformation.ViewModels
{
    public class ParticipantCsvToXmlConverterViewModel : ViewModel,IParticipantCsvToXmlConverterViewModel
    {
        public ParticipantCsvToXmlConverterViewModel()
        {
            delimiter = ",";
            isFirstRowHeaders = true;
            SaveCsvCommand = new RelayCommand(exe => SaveCsv(exe), cmd => CanSaveCsv(cmd));
            SaveXmlCommand = new RelayCommand(exe => SaveXml(exe), cmd => CanSaveXml(cmd));
            currentDispatcher = App.Current.RootVisual.Dispatcher;
        }
        private Dispatcher currentDispatcher;

        private double percentConverted;

        public double PercentConverted
        {
            get { return percentConverted; }
            set { percentConverted = value; OnPropertyChanged("PercentConverted"); }
        }

        private bool isConverting;

        public bool IsConverting
        {
            get { return isConverting; }
            set { isConverting = value; OnPropertyChanged("IsConverting"); }
        }

        private bool CanSaveCsv(object item)
        {
            return (selectedXmlFile != null && selectedXmlFile.Exists);
        }
        public delegate void CallWrite();  
            
       
        public void SaveCsv(object commandData)
        {
            SetConverting(true);
            
            // Events used during background processing
            DoWorkEventHandler workHandler = null;
            RunWorkerCompletedEventHandler doneHandler = null;

            Stream xmlFile = selectedXmlFile.OpenRead();
            // Implementation of the BackgroundWorker
            var wrkr = new BackgroundWorker();
            wrkr.DoWork += workHandler =

                 delegate(object oDoWrk, DoWorkEventArgs eWrk)
                 {
                     // Unwire the workHandler to prevent memory leaks
                     wrkr.DoWork -= workHandler;
                     ParticipantFile fileConverter = new ParticipantFile(saveCsvFile, xmlFile, delimiter, isFirstRowHeaders);
                     fileConverter.FileProgressChanged += new ParticipantFile.FileProgressChangedEventHandler(fileConverter_FileProgressChanged);

                     fileConverter.WriteCsv();
                 };
            wrkr.RunWorkerCompleted += doneHandler =
                 delegate(object oDone, RunWorkerCompletedEventArgs eDone)
                 {
                     // Work is complete, 
                     // kill references to teh donHandler.
                     wrkr.RunWorkerCompleted -= doneHandler;
                     saveCsvFile.Close();
                     xmlFile.Close();
                     SetConverting(false);
                 };
            // This is where the actual asynchronous process will
            // start performing the work that is wired up in the
            // previous statements.
            wrkr.RunWorkerAsync();
            
        }
        

        private void SetConverting(bool isConverting)
        {
            currentDispatcher.BeginInvoke(() =>
            {
                IsConverting = isConverting;
            }
                                           );
        }

        private void fileConverter_FileProgressChanged(object sender, FileProgressChangedEventArgs a)
        {
            currentDispatcher.BeginInvoke(() =>
                                            {
                                                PercentConverted = a.PercentComplete; 
                                            }
                                            );
        }
        private bool CanSaveXml(object item)
        {
            return (selectedCsvFile != null && selectedCsvFile.Exists);
        }

        
        public void SaveXml(object commandData)
        {
            SetConverting(true);
            Stream CsvFile = selectedCsvFile.OpenRead();
            // Events used during background processing
            DoWorkEventHandler workHandler = null;
            RunWorkerCompletedEventHandler doneHandler = null;

            // Implementation of the BackgroundWorker
            var wrkr = new BackgroundWorker();
            wrkr.DoWork += workHandler =

                 delegate(object oDoWrk, DoWorkEventArgs eWrk)
                 {
                     // Unwire the workHandler to prevent memory leaks
                     wrkr.DoWork -= workHandler;
                     ParticipantFile fileConverter = new ParticipantFile(CsvFile, saveXmlFile, delimiter, isFirstRowHeaders);
                     fileConverter.FileProgressChanged += new ParticipantFile.FileProgressChangedEventHandler(fileConverter_FileProgressChanged);

                     fileConverter.WriteXml();
                 };
            wrkr.RunWorkerCompleted += doneHandler =
                 delegate(object oDone, RunWorkerCompletedEventArgs eDone)
                 {
                     // Work is complete, 
                     // kill references to teh donHandler.
                     wrkr.RunWorkerCompleted -= doneHandler;
                     saveXmlFile.Close();
                     CsvFile.Close();
                     SetConverting(false);
                 };
            // This is where the actual asynchronous process will
            // start performing the work that is wired up in the
            // previous statements.
            wrkr.RunWorkerAsync();

        }

        private string selectedXmlFileName;

        public string SelectedXmlFileName
        {
            get { return selectedXmlFileName; }
            set { selectedXmlFileName = value; OnPropertyChanged("SelectedXmlFileName"); }
        }

        private FileInfo selectedXmlFile;

        public FileInfo SelectedXmlFile
        {
            get { return selectedXmlFile; }
            set { selectedXmlFile = value; OnPropertyChanged("SelectedXmlFile"); }
        }
        private string selectedCsvFileName;

        public string SelectedCsvFileName
        {
            get { return selectedCsvFileName; }
            set { selectedCsvFileName = value; OnPropertyChanged("SelectedCsvFileName"); }
        }
        private FileInfo selectedCsvFile;

        public FileInfo SelectedCsvFile
        {
            get { return selectedCsvFile; }
            set { selectedCsvFile = value; OnPropertyChanged("SelectedCsvFile"); }
        }

        private string saveXmlFileName;

        public string SaveXmlFileName
        {
            get { return saveXmlFileName; }
            set { saveXmlFileName = value; OnPropertyChanged("SaveXmlFileName"); }
        }

        private FileStream saveXmlFile;

        public FileStream SaveXmlFile
        {
            get { return saveXmlFile; }
            set { saveXmlFile = value; OnPropertyChanged("SaveXmlFile"); }
        }
        private string saveCsvFileName;

        public string SaveCsvFileName
        {
            get { return saveCsvFileName; }
            set { saveCsvFileName = value; OnPropertyChanged("SaveCsvFileName"); }
        }
        private FileStream saveCsvFile;

        public FileStream SaveCsvFile
        {
            get { return saveCsvFile; }
            set { saveCsvFile = value; OnPropertyChanged("SaveCsvFile"); }
        }

        private string delimiter;

        public string Delimiter
        {
            get { return delimiter; }
            set { delimiter = value; OnPropertyChanged("Delimiter"); }
        }

        private bool isFirstRowHeaders;

        public bool IsFirstRowHeaders
        {
            get { return isFirstRowHeaders; }
            set { isFirstRowHeaders = value; OnPropertyChanged("IsFirstRowHeaders"); }
        }
        public RelayCommand SaveCsvCommand { get; set; }
        public RelayCommand SaveXmlCommand { get; set; }


    }
}
