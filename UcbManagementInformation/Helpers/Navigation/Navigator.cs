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

namespace UcbManagementInformation.Helpers
{
    public class Navigator : DependencyObject
    {
        public static INavigable GetSource(DependencyObject obj)
        {
            return (INavigable)obj.GetValue(SourceProperty);
        }

        public static void SetSource(DependencyObject obj, INavigable value)
        {
            obj.SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
                "Source",
                typeof(INavigable),
                typeof(Navigator), 
                new PropertyMetadata(OnSourceChanged));


        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	    {
	        Page page = d as Page;
	 
	        page.Loaded += PageLoaded;
	    }        

	    private static void PageLoaded(object sender, RoutedEventArgs e)
	    {
	        Page page = sender as Page;
	 
	        INavigable navSource = GetSource(page);
	 
	        if (navSource != null)
	        {
	            navSource.NavigationService = new PageNavigationService(page.NavigationService);
	        }
	    }
}
}
