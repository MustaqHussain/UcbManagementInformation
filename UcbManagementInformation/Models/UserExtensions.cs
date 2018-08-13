namespace UcbManagementInformation.Server.DataAccess
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Extensions to the <see cref="User"/> class.
    /// </summary>
     public partial class MCUser
    {
       #region Make DisplayName Bindable

         public string DisplayName{get {return this.Forename + " " + this.Surname;}}
        /// <summary>
        /// Override of the <c>OnPropertyChanged</c> method that generates
        /// property change notifications when <see cref="User.DisplayName"/> changes.
        /// </summary>
        /// <param name="e">The property change event args.</param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            base.OnPropertyChanged(e);

            if (e.PropertyName == "Name" || e.PropertyName == "FriendlyName")
            {
                this.RaisePropertyChanged("DisplayName");
            }
        }
        #endregion
    }
}
