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
using UcbManagementInformation.Commanding;
using UcbManagementInformation.Web.Server;
using System.ServiceModel.DomainServices.Client;
using System.Linq;

namespace UcbManagementInformation.ViewModels
{
    public class EditDataItemViewModel : ViewModel, IEditDataItemViewModel
    {
        #region private fields
        private DataItem dataItemToEdit;
        private RelayCommand saveCommand;
        private RelayCommand cancelCommand;
        private bool? dialogResult;
        private ReportWizardContext myContext;// = new ReportWizardContext();
        private bool isSaveButtonVisible;
        #endregion

        #region public properties
        /// <summary>
        /// The data item that is displayed and edited
        /// </summary>
        public DataItem DataItemToEdit
        {
            get
            {
                return dataItemToEdit;
            }
            set
            {
                dataItemToEdit = value; OnPropertyChanged("DataItemToEdit");
            }
        }

        /// <summary>
        /// Property containing the relay command for the Save button
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand;
            }
            set
            {
                saveCommand = value; OnPropertyChanged("SaveCommand");
            }
        }

        /// <summary>
        /// Property containing the relay command for the Cancel button
        /// </summary>
        public RelayCommand CancelCommand
        {
            get
            {
                return cancelCommand;
            }
            set
            {
                cancelCommand = value; OnPropertyChanged("CancelCommand");
            }
        }

        /// <summary>
        /// Property containg the DialogResult
        /// </summary>
        public bool? DialogResult
        {
            get
            {
                return dialogResult;
            }
            set
            {
                dialogResult = value; OnPropertyChanged("DialogResult");
            }
        }

        /// <summary>
        /// Property to determine if the Save button is displayed or not
        /// </summary>
        public bool IsSaveButtonVisible
        {
            get { return isSaveButtonVisible; }
            set
            {
                //if (isSaveButtonVisible != value)
                //{
                    isSaveButtonVisible = value;
                    OnPropertyChanged("IsSaveButtonVisible");
                //}
            }
        }
        #endregion

        #region Relay command 'execute' and 'canexecute' implementations
        
        private void SaveClicked(object item)
        {
            myContext.SubmitChanges(SaveCLickedCallback, null);

        }

        //If Caption, Description or location are empty then cannot click save button
        private bool canClickSave(object item)
        {
            if (String.IsNullOrWhiteSpace(DataItemToEdit.LocationOnSystem)
                || String.IsNullOrWhiteSpace(DataItemToEdit.Description)
                || String.IsNullOrWhiteSpace(DataItemToEdit.Caption))
            {
                IsSaveButtonVisible = false;
                return false;
            }
            else
            {
                IsSaveButtonVisible = true;
                return true;
            }
        }

        private void CancelClicked(object item)
        {
            DialogResult = false;
        }

       
        #endregion

        #region EditDataItemViewModel constructors
        /// <summary>
        /// Constructor for the EditDataItemViewModel
        /// </summary>
        /// <param name="dataItemCode">The guid of the data item to edit</param>
        public EditDataItemViewModel(Guid dataItemCode):this(dataItemCode,null)
        {
            
        }

        /// <summary>
        /// Overloaded constructor for the EditDataItemViewModel. This allows a mock domain client to be used
        /// when unit testing.
        /// </summary>
        /// <param name="dataItemCode">The guid of the data item to edit</param>
        /// <param name="context">The domain context, which will have a mock domain client</param>
        public EditDataItemViewModel(Guid dataItemCode, ReportWizardContext context)
        {
            if (context == null)
            {
                myContext = new ReportWizardContext();
            }
            else
            {
                myContext = context;
            }
            
            //context.DataItems.Clear();
            myContext.Load<DataItem>(myContext.GetDataItemByCodeQuery(dataItemCode), LoadBehavior.MergeIntoCurrent, LoadDataItemCallback, null);
            
            //IsSaveButtonVisible = false;
        }
       
        #endregion

        #region Callbacks
        /// <summary>
        /// Callback called when the required data item has been retrieved from the database
        /// </summary>
        /// <param name="lo">A LoadOperation containing the required data item</param>
        void LoadDataItemCallback(LoadOperation<DataItem> lo)
        {
            DataItemToEdit = lo.Entities.FirstOrDefault<DataItem>();

            //if (String.IsNullOrWhiteSpace(DataItemToEdit.LocationOnSystem)
            //    || String.IsNullOrWhiteSpace(DataItemToEdit.Description)
            //    || String.IsNullOrWhiteSpace(DataItemToEdit.Caption))
            //{
            //    IsSaveButtonVisible = false;
            //}
            
            DataItemToEdit.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(DataItemToEdit_PropertyChanged);
            
            SaveCommand = new RelayCommand(cmd => SaveClicked(cmd), can => canClickSave(can));
            CancelCommand = new RelayCommand(cmd => CancelClicked(cmd));

            
        }

        /// <summary>
        /// Callback called when the save button has been clicked and the edited data item has been saved to the database
        /// </summary>
        /// <param name="lo">The data item that was updated/param>
        void SaveCLickedCallback(SubmitOperation lo)
        {
            if (!lo.HasError)
            {
                DialogResult = true;
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handler for the the property changed event on a data item
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The event arguments associated with the event</param>
        void DataItemToEdit_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Caption":
                case "Description":
                case "LocationOnSystem":
                    SaveCommand.UpdateCanExecuteCommand();
                    break;
            }
        }
        #endregion
    }
}
