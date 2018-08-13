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
using System.Collections.ObjectModel;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.MVVM;

namespace UcbManagementInformation.Models
{
    public class Filter : NotifyPropertyChangedEnabledBase
    {
        private ObservableCollection<string> _existingValues;
        private ObservableCollection<Operand> _possibleOperands;
        private Operand _operandChosen;
        private string _value;
        public DataItem DataItemToFilter { get; set; }
        public Operand OperandChosen { 
            get { return _operandChosen; }
            set { _operandChosen = value; OnPropertyChanged("OperandChosen"); } }
        public ObservableCollection<Operand> PossibleOperands
        { get { return _possibleOperands; } set { _possibleOperands = value ;OnPropertyChanged("PossibleOperands");}  }
        public string Value { get { return _value; } set { _value = value; OnPropertyChanged("Value"); } }
        public ObservableCollection<string> ExistingValues
        {
            get { return _existingValues; }
            set { _existingValues = value; OnPropertyChanged("ExistingValues"); }
        }
        public bool TextBoxVisibility { get; set; }
        public bool DropDownVisibility { get; set; }
        public bool CalendarVisibility { get; set; }
    }
}
