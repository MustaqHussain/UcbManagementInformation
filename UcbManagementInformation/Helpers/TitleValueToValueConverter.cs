namespace UcbManagementInformation
{
    using System;
    using System.Windows.Data;
    using UcbManagementInformation.Models;

    /// <summary>
    /// Two way <see cref="IValueConverter"/> that does nothing
    /// </summary>
    public class TitleValueToValueConverter : IValueConverter
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
            TitleValue TitleValueItem = value as TitleValue;
            if (TitleValueItem != null)
            {
                object ReturnValue = TitleValueItem.Value;
                string ConversionParameter = (parameter == null ?  "" : parameter.ToString());
                if (ReturnValue is int)
                {
                    return ((int)ReturnValue).ToString(ConversionParameter);
                }
                else if (ReturnValue is decimal)
                {
                    return ((decimal)ReturnValue).ToString(ConversionParameter);
                }
                else if (ReturnValue is double)
                {
                    return ((double)ReturnValue).ToString(ConversionParameter);
                }
            }
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
            throw new NotImplementedException("Convert back not implemented.");
        }
    }
}
