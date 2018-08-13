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
using System.Reflection;
using Microsoft.Expression.Interactivity.Core;

namespace UcbManagementInformation.MVVM
{
    public class InvokeViewModelMethod : ChangePropertyAction
    {
        #region MethodName
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register(
                "MethodName",
                typeof(string),
                typeof(InvokeViewModelMethod),
                new PropertyMetadata(new PropertyChangedCallback(OnMethodNamePropertyChanged))
                );
        public string MethodName
        {
            get { return GetValue(MethodNameProperty) as string; }
            set { SetValue(MethodNameProperty, value); }
        }
        private static void OnMethodNamePropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
        }
        #endregion

        #region PageRootParameter
        public static readonly DependencyProperty PageRootProperty =
            DependencyProperty.Register(
                "PageRoot",
                typeof(FrameworkElement),
                typeof(InvokeViewModelMethod),
                new PropertyMetadata(new PropertyChangedCallback(OnPageRootPropertyChanged))
                );
        public FrameworkElement PageRoot
        {
            get { return GetValue(PageRootProperty) as FrameworkElement; }
            set { SetValue(PageRootProperty, value); }
        }
        private static void OnPageRootPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
        }
        #endregion

        protected override void Invoke(object parameter)
        {
            base.Invoke(parameter);

            // Obtain a reference to the DataContext object
            object dataContext = PageRoot.DataContext;

            // Grab the DataContext object's type
            Type viewModelType = dataContext.GetType();
            // Make sure we have the method specified by our "MethodName" dependency property
            MethodInfo targetMethod = viewModelType.GetMethod(MethodName);

            object[] parameters = new object[] { this.Value };
            // Invoke the method on the ViewModel
            targetMethod.Invoke(dataContext, parameters);
        }
    }
}
