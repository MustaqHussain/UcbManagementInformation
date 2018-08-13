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
using System.Xml.Schema;
using UcbManagementInformation.MVVM;
using System.Collections.ObjectModel;
using UcbManagementInformation.Models;
using UcbManagementInformation.Commanding;

namespace UcbManagementInformation.ViewModels
{
    public class FakeUploadFileViewModel : ViewModel,IUploadFileViewModel
    {
        public FakeUploadFileViewModel()
        {
            provider = "TestYH";
            fileType = "Participant";
            selectedFileName=@"C:\Temp\ParticipantFile1.xml";

            fileSize = 10000;
            filePosition = 5000;
            uniqueFileName = DateTime.Now.ToString("yyyyMMdd_HHmmss_") + selectedFileName;
            chunks = new ObservableCollection<Chunk>()
            {
                new Chunk(){Number=0,Size=3020000,SizeTransferred=3020000,IsDecompressed=true},
                new Chunk(){Number=1,Size=3020000,SizeTransferred=1000000,IsDecompressed=false}
            };
            stageList = new ObservableCollection<Stage>
            {
                new Stage {Code = Guid.NewGuid(),Name="Validate Schema",Status=StageStatus.Completed},
                new Stage {Code = Guid.NewGuid(),Name="Compressing",Status=StageStatus.Completed},
                new Stage {Code = Guid.NewGuid(),Name="Transferring to Server",Status=StageStatus.Running},
                new Stage {Code = Guid.NewGuid(),Name="Inflating",Status=StageStatus.NotStarted},
                new Stage {Code = Guid.NewGuid(),Name="Bulk Upload",Status=StageStatus.NotStarted},
                new Stage {Code = Guid.NewGuid(),Name="Business Validation",Status=StageStatus.NotStarted},
                new Stage {Code = Guid.NewGuid(),Name="Copy To Validated",Status=StageStatus.NotStarted}
            };
        }
        private string selectedFileName;

        public string SelectedFileName
        {
            get { return selectedFileName; }
            set { selectedFileName = value; OnPropertyChanged("SelectedFileName"); }
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
        private string fileType;

        public string FileType
        {
            get { return fileType; }
            set { fileType = value; OnPropertyChanged("FileType"); }
        }
        private string provider;

        public string Provider
        {
            get { return provider; }
            set { provider = value; OnPropertyChanged("Provider"); }
        }
        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand UploadFileCommand { get; set; }

    }
}
