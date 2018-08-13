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
using System.Windows.Interactivity;
namespace UcbManagementInformation.Controls
{
    public class OpenFileDialogBehavior : TargetedTriggerAction<Button>
    {
        #region FileDialogResultCommandProperty
        public static readonly DependencyProperty FileDialogResultCommandProperty =
            DependencyProperty.Register("FileDialogResultCommandProperty",
            typeof(object), typeof(OpenFileDialogBehavior), null);
        public object FileDialogResultCommand
        {
            get
            {
                return (object)base.GetValue(FileDialogResultCommandProperty);
            }
            set
            {
                base.SetValue(FileDialogResultCommandProperty, value);
            }
        }
        #endregion
        #region FileDialogResultFullNameProperty
        public static readonly DependencyProperty FileDialogResultFullNameProperty =
            DependencyProperty.Register("FileDialogResultFullNameProperty",
            typeof(string), typeof(OpenFileDialogBehavior), null);
        public string FileDialogResultFullName
        {
            get
            {
                return (string)base.GetValue(FileDialogResultFullNameProperty);
            }
            set
            {
                base.SetValue(FileDialogResultFullNameProperty, value);
            }
        }
        #endregion
        #region DialogFilterProperty
        public static readonly DependencyProperty DialogFilterProperty =
           DependencyProperty.Register("DialogFilter", typeof(string),
           typeof(OpenFileDialogBehavior), null);
        public string DialogFilter
        {
            get
            {
                if (base.GetValue(DialogFilterProperty) == null)
                {
                    return "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
                }
                else
                {
                    return (string)base.GetValue(DialogFilterProperty);
                }
            }
            set
            {
                base.SetValue(DialogFilterProperty, value);
            }
        }
        #endregion
        #region Invoke
        //Shows OpenFileDialog on event trigger you set in designer
        protected override void Invoke(object parameter)
        {
            OpenFileDialog objOpenFileDialog = new OpenFileDialog();
            objOpenFileDialog.Filter = DialogFilter;
            objOpenFileDialog.Multiselect = false;
            if (objOpenFileDialog.ShowDialog() ?? false)
            {
                FileDialogResultFullName = objOpenFileDialog.File.Name;
                FileDialogResultCommand = objOpenFileDialog.File;
            }
        }
        #endregion
    }
}