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
using System.Collections.ObjectModel;
using UcbManagementInformation.Models;
using UcbManagementInformation.Commanding;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Linq;
using UcbManagementInformation.Helpers;
using ICSharpCode.SharpZipLib.Core;
using System.ComponentModel;
using System.Windows.Threading;
using UcbManagementInformation.Web.Services;
using UcbManagementInformation.Web.MIFileUpload.JobQueue;
using System.Collections.Generic;
using UcbManagementInformation.Web.MIFileUpload.JobQueue.Jobs;
namespace UcbManagementInformation.ViewModels
{
    public class TransferPostCodeFilesViewModel : ViewModel, ITransferPostCodeFilesViewModel, INavigable
    {
        private IModalDialogService modalDialogService;

        public TransferPostCodeFilesViewModel()
            : this(new MIUploadContext())
        {
            
        }
        MIUploadContext myContext;
        public TransferPostCodeFilesViewModel(MIUploadContext context)
        {
            myContext = context;
            currentDispatcher = App.Current.RootVisual.Dispatcher;
            UploadFileCommand = new RelayCommand(cmd => UploadFile(cmd),exe => CanUploadFile(exe));
            JobMonitorCommand = new RelayCommand(cmd => JobMonitorNavigate(cmd), exe => CanJobMonitorNavigate(exe));
            
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(UploadFileViewModel_PropertyChanged);
            
        }

        void UploadFileViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedFile":
                    
                    break;
            }
        }

        private bool CanJobMonitorNavigate(object item)
        {
            return canUploadJob;
        }

        private void JobMonitorNavigate(object item)
        {
            //Start the upload process on the server passing the file name
            myContext.UploadPostCodeFiles(uniqueFileNameList[0],uniqueFileNameList[1],uniqueFileNameList[2],uniqueFileNameList[3]
                ,uniqueFileNameList[4],uniqueFileNameList[5],uniqueFileNameList[6],
                (io) =>
                {
                    jobGuid = io.Value;
                    CurrentJob = null;
                    CurrentJobSteps = null;

                    //NavigateToJobMonitor
                    App.Session[SessionKey.JobCode] = jobGuid;
                    NavigationService.Navigate("/ParticipantMI/JobQueueMonitor");
                }, null);

            
        }

        private bool CanUploadFile(object item)
        {
            return selectedWPCFile != null && selectedWPCFile.Exists
                && selectedLAFile != null && selectedLAFile.Exists
                && selectedCtyFile != null && selectedCtyFile.Exists
                && selectedLEAFile != null && selectedLEAFile.Exists
                && selectedNUTS1File != null && selectedNUTS1File.Exists
                && selectedWDFile != null && selectedWDFile.Exists
                && selectedPCFile != null && selectedPCFile.Exists
                ;
        }
        
        private void UploadFile(object item)
        {
            canUploadJob = false;
            JobMonitorCommand.UpdateCanExecuteCommand();

            Uri SourceUri = new Uri(App.Current.Host.Source, "../UploadFileHandler.ashx");
            System.UriBuilder ub = new System.UriBuilder(SourceUri);
            Uri docUri = new System.UriBuilder(SourceUri).Uri;
            
            //Uri docUri = new System.UriBuilder(
            //        System.Windows.Browser.HtmlPage.Document.DocumentUri.Scheme
            //        , System.Windows.Browser.HtmlPage.Document.DocumentUri.Host
            //        , System.Windows.Browser.HtmlPage.Document.DocumentUri.Port
            //        , "UploadFileHandler.ashx").Uri;
            // Initiate a background worker so we don't hold up the UI of the Client App 
            BackgroundWorker bw = new BackgroundWorker();      
            bw.DoWork += (bw_sender, bw_e) =>      
            {
                ProcessSingleFile(selectedWPCFileName, selectedWPCFile, bw_sender, bw_e, docUri);
                ProcessSingleFile(selectedLAFileName, selectedLAFile, bw_sender, bw_e, docUri);
                ProcessSingleFile(selectedCtyFileName, selectedCtyFile, bw_sender, bw_e, docUri);
                ProcessSingleFile(selectedLEAFileName, selectedLEAFile, bw_sender, bw_e, docUri);
                ProcessSingleFile(selectedNUTS1FileName, selectedNUTS1File, bw_sender, bw_e, docUri);
                ProcessSingleFile(selectedWDFileName, selectedWDFile, bw_sender, bw_e, docUri);
                ProcessSingleFile(selectedPCFileName, selectedPCFile, bw_sender, bw_e, docUri);
            };
            bw.RunWorkerAsync();
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        void ProcessSingleFile(string selectedFileName, FileInfo selectedFile,object bw_sender, DoWorkEventArgs bw_e,Uri docUri)
        {
            System.UriBuilder ub = null;
            uniqueFileNameNonUI = System.DateTime.Now.ToString("yyyyMMdd_HHmmss_")
                     + selectedFile.Name;
            uniqueFileNameList.Add(uniqueFileNameNonUI);
            currentDispatcher.BeginInvoke(() =>
            {
                Chunks = new ObservableCollection<Chunk>();
                UniqueFileName = uniqueFileNameNonUI;
                FilePosition = 0;
                FileSize = selectedFile.Length;
                 // Construct URI to the File upload Handler ASHX file
            
            });
            ub = new UriBuilder(docUri.AbsoluteUri);
            // Set any parameters the Handler will require.
            // Here we pass in the original + unique file name      
            ub.Query = "filename=" + selectedFile.Name
                + "&uniquefilename=" + uniqueFileNameNonUI;
           
           
            // The function that does the background work
            ProcessFileUpload(ub.Uri
                , (System.IO.FileInfo)selectedFile
                , (System.ComponentModel.BackgroundWorker)bw_sender
                , bw_e);      
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            canUploadJob = true;
            JobMonitorCommand.UpdateCanExecuteCommand();
            
        }

        private void ProcessFileUpload(System.Uri uri
            , System.IO.FileInfo fileInfo
            , System.ComponentModel.BackgroundWorker worker 
            , System.ComponentModel.DoWorkEventArgs e) 
        {
            using (FileStream stream = fileInfo.OpenRead())
            {
                int i = 0;
                bool finished = false;
                while (!finished)
                {
                    MemoryStream chunk = new MemoryStream();
                    Chunk chunkToAdd = new Chunk() {FileName=uniqueFileNameNonUI,Number=i,Size=1};
                    currentDispatcher.BeginInvoke(() =>
                      {
                          Chunks.Add(chunkToAdd);
                      });

                    //Compress to get a 3MB Chunk of file
                    finished = CreateZippedChunk(stream, chunk, 4096, 3000000, "zip" + i.ToString(),chunkToAdd);
                    //Upload Chunk
                    UploadChunk(uri, chunk, worker, e,i,finished,chunkToAdd);
                    i++;
                }
            }
        }
        private void UploadChunk(System.Uri uri
            , System.IO.Stream fs
            , System.ComponentModel.BackgroundWorker worker 
            , System.ComponentModel.DoWorkEventArgs e,int chunkNumber,bool finished,Chunk addedChunk)
        {
            try    
            {
                long length = fs.Length; 
                byte[] buffer; 
                int position = 0;   
                int packetSize = 500 * 1024;
                
                System.UriBuilder ub = new System.UriBuilder(uri);
                if (ub.Query == string.Empty)
                {
                    ub.Query = "chunk="+chunkNumber;
                }
                else
                {
                    ub.Query = ub.Query.TrimStart('?') + "&chunk=" +chunkNumber;
                }
                if (ub.Query == string.Empty)
                {
                    ub.Query = "finished=" + (finished?1:0);
                }
                else
                {
                    ub.Query = ub.Query.TrimStart('?') + "&finished=" + (finished?1:0);
                }         
                while (position < length)  
                {
                    ub.Query = ub.Query.Replace("completed=1","").Replace("completed=0","");
                    if (position + packetSize > length)   
                    {
                        buffer = new byte[length - position]; 
                        // If this is the last buffer for this file..                
                        // .. set the completed flag for the handler               
                        //System.UriBuilder ub = new System.UriBuilder(uri);               
                        if (ub.Query == string.Empty)
                        {
                            ub.Query = "completed=1";
                        }              
                        else 
                        {
                            ub.Query = ub.Query.TrimStart('?') + "&completed=1";
                        }            
                        uri = ub.Uri;         
                    }         
                    else       
                    {          
                        buffer = new byte[packetSize];
                        if (ub.Query == string.Empty)
                        {
                            ub.Query = "completed=0";
                        }
                        else
                        {
                            ub.Query = ub.Query.TrimStart('?') + "&completed=0";
                        }
                        uri = ub.Uri;      
                    }            
                    fs.Read(buffer, 0, buffer.Length);  
                    position += buffer.Length;       
                    
                    // Mutex to ensure packets are sent in order       
                    System.Threading.AutoResetEvent mutex =                         
                        new System.Threading.AutoResetEvent(false);            
                    System.Net.WebClient wc = new System.Net.WebClient();       
                    
                    wc.OpenWriteCompleted += (wc_s, wc_e) =>            
                    {            
                        UploadPacketToHandler(buffer, wc_e.Result);
                        currentDispatcher.BeginInvoke(() =>
                        {
                            if (ub.Query.Contains("completed=1"))
                            {
                                addedChunk.IsDecompressing = true;
                            }
                        });
                    };          
                    
                    wc.WriteStreamClosed += (ws_s, ws_e) =>    
                    {
                        currentDispatcher.BeginInvoke(() =>
                      {
                          addedChunk.SizeTransferred = position;
                          if (ub.Query.Contains("completed=1"))
                          {
                                addedChunk.IsDecompressed = true;
                                addedChunk.IsDecompressing = false;
                          }
                      });
                        mutex.Set();     
                    };       
                    
                    wc.OpenWriteAsync(uri);        
                    
                    mutex.WaitOne();      
                }          
                fs.Close();  
            }   
            catch (Exception ex)  
            {     
                //Handle errors here   
            } 
        }
        private void UploadPacketToHandler(byte[] fileData, System.IO.Stream outputStream)
        {
            // Pipe the packet into the Steam to the Server  
            outputStream.Write(fileData, 0, fileData.Length); 
            outputStream.Close();
        }

        
        
        public bool CreateZippedChunk(Stream input, Stream output, int bufferSize, long maxChunkSize,string zipEntryName,Chunk addedChunk)
        {
            byte[] buffer = new byte[bufferSize];
            int read=0;

            ZipOutputStream zipStream = new ZipOutputStream(output);
            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

            ZipEntry newEntry = new ZipEntry(zipEntryName);
            newEntry.DateTime = DateTime.Now;

            zipStream.PutNextEntry(newEntry);
            
            while ((output.Length < maxChunkSize && (read = input.Read(buffer, 0, buffer.Length)) > 0))
            {
                zipStream.Write(buffer, 0, read);
                currentDispatcher.BeginInvoke(() =>
                      {
                          addedChunk.Size = output.Length;
                          FilePosition = input.Position;
                      });
            }
            zipStream.Flush();
            output.Flush();
            zipStream.CloseEntry();

            zipStream.IsStreamOwner = false;	// False stops the Close also Closing the underlying stream.
            zipStream.Close();			// Must finish the ZipOutputStream before using outputMemStream.
            output.Position = 0;//position set to 0 ready for re-using.
            if (read <= 0)
            {
                return true;
            }
            return false;
        }
        private INavigationService navigationService;

        public INavigationService NavigationService
        {
            get { return navigationService; }
            set { navigationService = value; OnPropertyChanged("NavigationService"); }
        }
        private DispatcherTimer timer;

        private FileInfo currentFile;

            
        private FileInfo selectedWPCFile;

        public FileInfo SelectedWPCFile
        {
            get { return selectedWPCFile; }
            set
            {
                selectedWPCFile = value;
                UploadFileCommand.UpdateCanExecuteCommand();
 
                OnPropertyChanged("SelectedWPCFile");

               
            }
        }

        private FileInfo selectedLAFile;

        public FileInfo SelectedLAFile
        {
            get { return selectedLAFile; }
            set
            {
                selectedLAFile = value;
                UploadFileCommand.UpdateCanExecuteCommand();

                OnPropertyChanged("SelectedLAFile");


            }
        }
        private FileInfo selectedCtyFile;

        public FileInfo SelectedCtyFile
        {
            get { return selectedCtyFile; }
            set
            {
                selectedCtyFile = value;
                UploadFileCommand.UpdateCanExecuteCommand();

                OnPropertyChanged("SelectedCtyFile");


            }
        }

        private FileInfo selectedLEAFile;

        public FileInfo SelectedLEAFile
        {
            get { return selectedLEAFile; }
            set
            {
                selectedLEAFile = value;
                UploadFileCommand.UpdateCanExecuteCommand();
                OnPropertyChanged("SelectedLEAFile");
            }
        }

        private FileInfo selectedNUTS1File;

        public FileInfo SelectedNUTS1File
        {
            get { return selectedNUTS1File; }
            set
            {
                selectedNUTS1File = value;
                UploadFileCommand.UpdateCanExecuteCommand();
                OnPropertyChanged("SelectedNUTS1File");
            }
        }

        private FileInfo selectedWDFile;

        public FileInfo SelectedWDFile
        {
            get { return selectedWDFile; }
            set
            {
                selectedWDFile = value;
                UploadFileCommand.UpdateCanExecuteCommand();
                OnPropertyChanged("SelectedWDFile");
            }
        }
        private FileInfo selectedPCFile;

        public FileInfo SelectedPCFile
        {
            get { return selectedPCFile; }
            set
            {
                selectedPCFile = value;
                UploadFileCommand.UpdateCanExecuteCommand();
                OnPropertyChanged("SelectedPCFile");
            }
        }
        private Dispatcher currentDispatcher;

        private bool canUploadJob;

        private string currentFileName;


        private string selectedWPCFileName;

        public string SelectedWPCFileName
        {
            get { return selectedWPCFileName; }
            set { selectedWPCFileName = value; OnPropertyChanged("SelectedWPCFileName"); }
        }
        private string selectedLAFileName;

        public string SelectedLAFileName
        {
            get { return selectedLAFileName; }
            set { selectedLAFileName = value; OnPropertyChanged("SelectedLAFileName"); }
        }
        private string selectedCtyFileName;

        public string SelectedCtyFileName
        {
            get { return selectedCtyFileName; }
            set { selectedCtyFileName = value; OnPropertyChanged("SelectedCtyFileName"); }
        }
        private string selectedLEAFileName;

        public string SelectedLEAFileName
        {
            get { return selectedLEAFileName; }
            set { selectedLEAFileName = value; OnPropertyChanged("SelectedLEAFileName"); }
        }
        private string selectedNUTS1FileName;

        public string SelectedNUTS1FileName
        {
            get { return selectedNUTS1FileName; }
            set { selectedNUTS1FileName = value; OnPropertyChanged("SelectedNUTS1FileName"); }
        }
        private string selectedWDFileName;

        public string SelectedWDFileName
        {
            get { return selectedWDFileName; }
            set { selectedWDFileName = value; OnPropertyChanged("SelectedWDFileName"); }
        }
        private string selectedPCFileName;

        public string SelectedPCFileName
        {
            get { return selectedPCFileName; }
            set { selectedPCFileName = value; OnPropertyChanged("SelectedPCFileName"); }
        }
        private long fileSize;

        public long FileSize
        {
            get { return fileSize; }
            set { fileSize = value; OnPropertyChanged("FileSize"); }
        }

        private long filePosition;

        public long FilePosition
        {
            get { return filePosition; }
            set { filePosition = value; OnPropertyChanged("FilePosition"); }
        }
        private string uniqueFileNameNonUI;
        private List<string> uniqueFileNameList = new List<string>();
        private string uniqueFileName;

        public string UniqueFileName
        {
            get { return uniqueFileName; }
            set { uniqueFileName = value; OnPropertyChanged("UniqueFileName"); }
        }

        private ObservableCollection<Chunk> chunks = new ObservableCollection<Chunk>();

        public ObservableCollection<Chunk> Chunks
        {
            get { return chunks; }
            set { chunks = value; OnPropertyChanged("Chunks"); }
        }

        private ObservableCollection<Stage> stageList;
        public ObservableCollection<Stage> StageList
        {
            get { return stageList; }
            set { stageList = value; OnPropertyChanged("StageList"); }
        }
       
        private Guid jobGuid;

        private JobBase currentJob;

        public JobBase CurrentJob
        {
            get { return currentJob; }
            set { currentJob = value; OnPropertyChanged("CurrentJob"); }
        }
        private ObservableCollection<JobStep> currentJobSteps;

        public ObservableCollection<JobStep> CurrentJobSteps
        {
            get { return currentJobSteps; }
            set { currentJobSteps = value; OnPropertyChanged("CurrentJobSteps"); }
        }

        public RelayCommand JobMonitorCommand { get; set; }
        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand UploadFileCommand { get; set; }
    }
}
