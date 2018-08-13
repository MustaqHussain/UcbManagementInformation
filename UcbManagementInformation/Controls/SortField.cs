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

namespace UcbManagementInformation.Controls
{
    public class SortField
    {
        public char SortIndicator { get; set; }
        public bool IsSorted { get; set; }
        public bool IsAscending { get; set; }
        public string SortIndicatorString
        {
            get
            {
                return SortIndicator.ToString();
            }
        }
        public SortField()
        {
            Reset();
        }

        public void Reset()
        {
            SortIndicator = ' ';
            IsSorted = false;
            IsAscending = false;
        }

        public void ToggleState()
        {
            switch (SortIndicator)
            {
                case 'v':
                    SortIndicator = '^';
                    IsAscending = true;
                    IsSorted = true;
                    break;
                case '^':
                    SortIndicator = 'v';
                    IsAscending = false;
                    IsSorted = true;
                    break;
                case ' ':
                    SortIndicator = '^';
                    IsAscending = true;
                    IsSorted = true;
                    break;
            }
        }
    }
}
