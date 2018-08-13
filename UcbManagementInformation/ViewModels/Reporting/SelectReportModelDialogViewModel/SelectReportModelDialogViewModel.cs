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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Commanding;

namespace UcbManagementInformation.ViewModels
{
    /// <summary>
    /// A View Model to support the SelectReportModel Dialog Window
    /// </summary>
    public class SelectReportModelDialogViewModel : ViewModel, ISelectReportModelDialogViewModel
    {
        /// <summary>
        /// Constructor for the SelectReportModelDialogViewModel
        /// </summary>
        /// <param name="modelList">A list of Report Models used to allow user selection</param>
        public SelectReportModelDialogViewModel(IEnumerable<DataModel> modelList)
        {
            //Set listener for property changed events
            PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SelectReportModelDialogViewModel_PropertyChanged);
            
            dataModels = new ObservableCollection<DataModel>(modelList);
            
            //Wire Up the commands
            OKCommand = new RelayCommand(cmd => OKChosen(cmd), can => canChooseOK(can));
            CancelCommand = new RelayCommand(cmd => CancelChosen(cmd));
        }

        /// <summary>
        /// Handler for the PropertyChanged event
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The event arguments associated with the event</param>
        void SelectReportModelDialogViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //Hand
            switch (e.PropertyName)
            {
                case "SelectedDataModel":
                    OKCommand.UpdateCanExecuteCommand();
                    break;
            }
        }

        #region private fields
        private ObservableCollection<DataModel> dataModels;
        private DataModel selectedDataModel;
        private bool? dialogResult;
        private RelayCommand okCommand;
        private RelayCommand cancelCommand;
        #endregion

        #region relay command execute and CanExecute implementations
        private void OKChosen(object item)
        {
            DialogResult = true;
        }
        private void CancelChosen(object item)
        {
            DialogResult = false;
        }

        private bool canChooseOK(object item)
        {
            return selectedDataModel != null;
        }
        #endregion

        /// <summary>
        /// A list of data models from which to choose.
        /// </summary>
        public ObservableCollection<DataModel> DataModels
        {
            get
            {
                return dataModels;
            }
            set
            {
                dataModels = value; OnPropertyChanged("DataModels");
            }
        }

        /// <summary>
        /// The currently selected Data Model
        /// </summary>
        public DataModel SelectedDataModel
        {
            get { return selectedDataModel; }
            set { selectedDataModel = value; OnPropertyChanged("SelectedDataModel"); }
        }

        /// <summary>
        /// Property containing the relay command for the OK
        /// </summary>
        public RelayCommand OKCommand
        { get { return okCommand; } set { okCommand = value; OnPropertyChanged("OKCommand"); } }

        /// <summary>
        /// Property containing the relay command for the cancel
        /// </summary>
        public RelayCommand CancelCommand
        { get { return cancelCommand; } set { cancelCommand = value; OnPropertyChanged("CancelCommand"); } }
        
        /// <summary>
        /// Property containg the DialogResult
        /// </summary>
        public bool? DialogResult
        {
            get { return dialogResult; }
            set { dialogResult = value; OnPropertyChanged("DialogResult"); }
        }

    }
}
