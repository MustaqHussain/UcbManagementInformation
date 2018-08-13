namespace UcbManagementInformation
{
    using System;
    using System.Windows.Data;
    using System.Collections.Generic;

    /// <summary>
    /// Two way <see cref="IValueConverter"/> that does nothing
    /// </summary>
    public class ChartTypeToMaximumConverter : IValueConverter
    {
        private enum ChartType
        {
            Percentage,
            NonPercentage
        }
        
        /// <summary>
        /// Decodes the given <paramref name="value"/> to the string according to the pipe delimited parameter
        /// </summary>
        /// <param name="value">The zero based index within the list.</param>
        /// <param name="targetType">The type to convert to (ignored).</param>
        /// <param name="parameter">pipe delimited string containing look up list.</param>
        /// <param name="culture">The culture of the conversion (ignored).</param>
        /// <returns>The input <paramref name="value"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string ReturnValue = null;
            ChartType ThisChartType = (ChartType)Enum.Parse(typeof(ChartType), (string)value, true);

            switch (ThisChartType)
            {
                case ChartType.Percentage:
                    ReturnValue = "100";
                    break;
                case ChartType.NonPercentage:
                    ReturnValue = "";
                    break;
            }

            return ReturnValue;
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
            throw new NotSupportedException(); 
        }
    }
}
