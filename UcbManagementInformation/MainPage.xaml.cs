namespace UcbManagementInformation
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using UcbManagementInformation.LoginUI;
    using System;

    /// <summary>
    /// <see cref="UserControl"/> class providing the main UI for the application.
    /// </summary>
    public partial class MainPage : UserControl
    {
        /// <summary>
        /// Creates a new <see cref="MainPage"/> instance.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            this.loginContainer.Child = new LoginStatus();
        }
        private bool menuShowing;
        private bool messageShowing;
        private bool messengerShowing;

        /// <summary>
        /// After the Frame navigates, ensure the <see cref="HyperlinkButton"/> representing the current page is selected
        /// </summary>
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            /*
            foreach (UIElement child in LinksStackPanel.Children)
            {
                HyperlinkButton hb = child as HyperlinkButton;
                if (hb != null && hb.NavigateUri != null)
                {
                    if (hb.NavigateUri.ToString().Equals(e.Uri.ToString()))
                    {
                        VisualStateManager.GoToState(hb, "ActiveLink", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(hb, "InactiveLink", true);
                    }
                }
            }*/
        }

        /// <summary>
        /// If an error occurs during navigation, show an error window
        /// </summary>
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            ErrorWindow.CreateNew(e.Exception);
        }

       

       

        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        

        

        private void ToggleMessages_Click(object sender, RoutedEventArgs e)
        {
            if (messageShowing)
            {
                VisualStateManager.GoToState(this, "MessageHidden", true);
                messageShowing = false;
            }
            else
            {
                VisualStateManager.GoToState(this, "MessageShowing", true);
                messageShowing = true;
            }
        }

        private void ToggleMessenger_Click(object sender, RoutedEventArgs e)
        {
            if (messengerShowing)
            {
                VisualStateManager.GoToState(this, "MessengerHidden", true);
                messengerShowing = false;
            }
            else
            {
                VisualStateManager.GoToState(this, "MessengerShowing", true);
                messengerShowing = true;
            }
        }

        private void carousel_SelectedItemClicked(object sender, Controls.Carousel.SelectedItemClickedEventArgs e)
        {
            //if (menuShowing)
            //{
            //    //VisualStateManager.GoToState(this, "Hidden", true);
            //    //menuShowing = false;
            //}
            //else
            //{
            //    VisualStateManager.GoToState(this, "Showing", true);
            //    menuShowing = true;
            //}
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border selectedBorder = (Border)sender;
            //SelectedItemBorder.DataContext = selectedBorder.DataContext;
            
        }
    }
}