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

namespace UcbManagementInformation.ViewModels
{
    public class FakeEditDataItemViewModel : ViewModel, IEditDataItemViewModel
    {
        #region private fields
        private static DataItem dataItemToEdit;
        private static RelayCommand saveCommand;
        private static RelayCommand cancelCommand;
        private static bool? dialogResult;
        private static bool isSaveButtonVisible;
        #endregion


        static FakeEditDataItemViewModel()
        {
            dataItemToEdit = new DataItem 
            { 
                Name = "EthnicMinorityLeaver",
                Caption = "Ethnic Minority Leaver",
                Description = "Populated field for counting leavers who are identified in a ethnic monority group on starting (relating to target references 1.25, 2.29 to 2.31, 4.25 and 5.35 to 5.37)",
                LocationOnSystem ="Test Test Test Location On System"
            };
            saveCommand = new RelayCommand(cmd => { });
            cancelCommand = new RelayCommand(cmd => { });
            dialogResult = true;
            isSaveButtonVisible = true;

        }

        #region IEditDataItemViewModel public proerties
        public DataItem DataItemToEdit
        {
            get { return dataItemToEdit; }
            set { dataItemToEdit = value; }
        }

        public  RelayCommand SaveCommand
        {
            get { return saveCommand; }
            set { saveCommand = value; }
        }

        public RelayCommand CancelCommand
        {
            get { return cancelCommand; }
            set { cancelCommand = value; }
        }

        public bool? DialogResult
        {
            get {return dialogResult; }
            set {dialogResult = value; }
        }

        public bool IsSaveButtonVisible
        {
            get { return isSaveButtonVisible; }
            set { isSaveButtonVisible = value; }
        }
        #endregion
    }
}
