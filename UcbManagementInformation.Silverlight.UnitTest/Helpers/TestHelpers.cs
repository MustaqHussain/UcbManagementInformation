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
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.Reflection;

namespace UcbManagementInformation.Silverlight.UnitTest.Helpers
{
   public static class INotifyPropertyChangedExtensions 
   {
       public static void AssertRaisesPropertyChangedFor(this INotifyPropertyChanged obj, string propertyName,object propertyValue) 
       {
           

           using (PropertyChangedEventListener listener = new PropertyChangedEventListener(obj, propertyName))
           {
               Type MainType = obj.GetType();
               PropertyInfo info = MainType.GetProperty(propertyName);
               info.SetValue(obj, propertyValue, null);
           }
           
       }
       public static void AssertRaisesPropertyChangedFor(this INotifyPropertyChanged obj, string propertyName)
       {
            AssertRaisesPropertyChangedFor(obj, propertyName, null);
       }
   }
   /// <summary> 
    /// Helper class for asserting a PropertyChanged event gets raised for a particular 
    /// property. If it hasn't been called by the time this object is disposed, an 
    /// assertion will fail.</summary> 
    public class PropertyChangedEventListener : IDisposable 
    {     
        bool wasRaised = false;     
        readonly string expectedPropertyName;     
        bool IsDisposed = false;    
        readonly INotifyPropertyChanged obj;      
        public PropertyChangedEventListener(INotifyPropertyChanged obj, string propertyName)    
        {        
            if (obj == null)        
                throw new ArgumentNullException("obj");   
            if (propertyName == null)          
                throw new ArgumentNullException("propertyName");        
            this.obj = obj;      
            this.expectedPropertyName = propertyName;     
            obj.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);  
        }       
        
        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)    
        {
            if (this.expectedPropertyName.Equals(e.PropertyName))
            { this.wasRaised = true; }   
        }      
        public void Dispose()    
            {   
            Dispose(true); 
            GC.SuppressFinalize(this);
        }      
        protected void Dispose(bool Disposing)  
        {        
            if (!IsDisposed)     
            {           
                if (Disposing)       
                {                 
                    // Cleanup references...       
                    this.obj.PropertyChanged -= new PropertyChangedEventHandler(OnPropertyChanged);  
                    // Assert we got called          
                    Assert.IsTrue(this.wasRaised,         
                        String.Format("PropertyChanged was not raised for property '{0}'",         
                        this.expectedPropertyName));          
                }    
            }       
            IsDisposed = true;  
        }      
        ~PropertyChangedEventListener()  
        {      
            Dispose(false);    
        } 
    }
    
}
