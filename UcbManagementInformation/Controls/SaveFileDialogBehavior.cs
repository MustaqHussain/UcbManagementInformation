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
    public class SaveFileDialogBehavior : TargetedTriggerAction<Button>
    {
        #region FileStreamInputdProperty
        public static readonly DependencyProperty FileStreamInputProperty =
            DependencyProperty.Register("FileStreamInputProperty",
            typeof(object), typeof(SaveFileDialogBehavior), null);
        public object FileStreamInput
        {
            get
            {
                return (object)base.GetValue(FileStreamInputProperty);
            }
            set
            {
                base.SetValue(FileStreamInputProperty, value);
            }
        }
        #endregion
        #region FileDialogResultSafeFullNameProperty
        public static readonly DependencyProperty FileDialogResultSafeFullNameProperty =
            DependencyProperty.Register("FileDialogResultSafeFullNameProperty",
            typeof(string), typeof(SaveFileDialogBehavior), null);
        public string FileDialogResultSafeFullName
        {
            get
            {
                return (string)base.GetValue(FileDialogResultSafeFullNameProperty);
            }
            set
            {
                base.SetValue(FileDialogResultSafeFullNameProperty, value);
            }
        }
        #endregion
        #region DialogFilterProperty
        public static readonly DependencyProperty DialogFilterProperty =
           DependencyProperty.Register("DialogFilter", typeof(string),
           typeof(SaveFileDialogBehavior), null);
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
            SaveFileDialog objSaveFileDialog = new SaveFileDialog();
            objSaveFileDialog.Filter = DialogFilter;
            if (objSaveFileDialog.ShowDialog() ?? false)
            {
                FileDialogResultSafeFullName = objSaveFileDialog.SafeFileName;
                FileStreamInput = objSaveFileDialog.OpenFile();
            }
        }
        #endregion
    }
}