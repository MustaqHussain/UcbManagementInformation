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

namespace UcbManagementInformation.Models
{
    public class Chunk : NotifyPropertyChangedEnabledBase
    {
        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; OnPropertyChanged("FileName"); }
        }
        private int number;

        public int Number
        {
            get { return number; }
            set { number = value; OnPropertyChanged("Number"); }
        }

        private long size;

        public long Size
        {
            get { return size; }
            set { size = value; OnPropertyChanged("Size"); }
        }
        private long sizeTransferred;

        public long SizeTransferred
        {
            get { return sizeTransferred; }
            set { sizeTransferred = value; OnPropertyChanged("SizeTransferred"); }
        }

        private bool isDecompressed;

        public bool IsDecompressed
        {
            get { return isDecompressed; }
            set { isDecompressed = value; OnPropertyChanged("IsDecompressed"); }
        }
        private bool isDecompressing;

        public bool IsDecompressing
        {
            get { return isDecompressing; }
            set { isDecompressing = value; OnPropertyChanged("IsDecompressing"); }
        }
    }
}
