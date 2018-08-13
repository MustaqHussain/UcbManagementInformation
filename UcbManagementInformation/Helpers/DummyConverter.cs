namespace UcbManagementInformation
{
    using System;
    using System.Windows.Data;
    using UcbManagementInformation.Commanding;

    /// <summary>
    /// Two way <see cref="IValueConverter"/> that does nothing
    /// </summary>
    public class DummyConverter : IValueConverter
    {
        /// <summary>
        /// Converts the given <paramref name="value"/> to be its inverse
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The type to convert to (ignored).</param>
        /// <param name="parameter">Optional parameter (ignored).</param>
        /// <param name="culture">The culture of the conversion (ignored).</param>
        /// <returns>The input <paramref name="value"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        /// <summary>
        /// The inverse of the <see cref="Convert"/>.
        /// </summary>
        /// <param name="value">The value to convert back.</param>
        /// <param name="targetType">The type to convert to (ignored).</param>
        /// <param name="parameter">Optional parameter (ignored).</param>
        /// <param name="culture">The culture of the conversion (ignored).</param>
        /// <returns>The input <paramref name="value"/>.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
