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
using UcbManagementInformation.Server.DataAccess;
using System.Collections.ObjectModel;

namespace UcbManagementInformation.ViewModels
{
    public class FakeSelectReportModelDialogViewModel : ViewModel, ISelectReportModelDialogViewModel
    {
            
        static FakeSelectReportModelDialogViewModel()
        {
            _selectedDataModel = new DataModel { Name = "Managing Authority Data Model", Description = "A Model for producing MA Reports" };
            _dataModels = new ObservableCollection<DataModel>
             {
                 new DataModel {Name="Managing Authority",Description="A Data Model For managing Authority" },
                 new DataModel {Name="Finance",Description="A Data Model Forfinance" },
                 new DataModel {Name="Model 3",Description="A Data Model For managing 3" }
             };
        }

        
        private static ObservableCollection<DataModel> _dataModels = new ObservableCollection<DataModel>();
        public ObservableCollection<DataModel> DataModels
        {
            get
            {
                return _dataModels;
            }
            set
            {
                _dataModels = value;
            }
        }
        private static DataModel _selectedDataModel;
        public DataModel SelectedDataModel
        {
            get
            {
                return _selectedDataModel;
            }
            set
            {
                _selectedDataModel = value;
            }
        }

        public Commanding.RelayCommand OKCommand
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Commanding.RelayCommand CancelCommand
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool? DialogResult
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
