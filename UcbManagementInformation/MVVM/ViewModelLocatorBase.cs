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
using System.ComponentModel;
using UcbManagementInformation.ServiceLocation;
namespace UcbManagementInformation.MVVM
{
    public abstract class ViewModelLocatorBase<TViewModel> : NotifyPropertyChangedEnabledBase where TViewModel : class
    {

        private static bool? isInDesignMode;
        private TViewModel runtimeViewModel;
        private TViewModel designtimeViewModel;

        /// <summary>  
        /// Gets a value indicating whether the control is in design mode  
        /// (running in Blend or Visual Studio).  
        /// </summary>  
        public static bool IsInDesignMode
        {
            get
            {
                if (!isInDesignMode.HasValue)
                {
                    isInDesignMode = DesignerProperties.IsInDesignTool;
                }

                return isInDesignMode.Value;
            }
        }

        /// <summary>  
        /// Holds the intance of the runtime version of the ViewModel that is instantiated only when application is really running by retrieving the instance from IOC container  
        /// </summary>  
        protected TViewModel RuntimeViewModel
        {
            get
            {
                if (this.runtimeViewModel == null)
                {
                    this.RuntimeViewModel = SimpleServiceLocator.Instance.Get<TViewModel>();
                }
                return runtimeViewModel;
            }

            set
            {
                runtimeViewModel = value;
                this.OnPropertyChanged("ViewModel");
            }
        }

        /// <summary>  
        /// Gets current ViewModel instance so if we are in designer its <see cref="DesigntimeViewModel"/> and if its runtime then its <see cref="RuntimeViewModel"/>.  
        /// </summary>  
        public TViewModel ViewModel
        {
            get
            {
                return IsInDesignMode ? this.DesigntimeViewModel : this.RuntimeViewModel;
            }
        }

        /// <summary>  
        /// Holds the intance of the designtime version of the ViewModel that is instantiated only when application is opened in IDE designer (VisualStudio, Blend etc).  
        /// </summary>  
        public TViewModel DesigntimeViewModel
        {
            get
            {
                return designtimeViewModel;
            }

            set
            {
                designtimeViewModel = value;
                this.OnPropertyChanged("ViewModel");
            }
        }
    }
}
