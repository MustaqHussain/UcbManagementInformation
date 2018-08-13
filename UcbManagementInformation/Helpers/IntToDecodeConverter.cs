namespace UcbManagementInformation
{
    using System;
    using System.Windows.Data;
    using System.Collections.Generic;

    /// <summary>
    /// Two way <see cref="IValueConverter"/> that does nothing
    /// </summary>
    public class IntToDecodeConverter : IValueConverter
    {
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
            string ConvertToListRaw = parameter as string;
            int[] Index = value as int[];
            if (ConvertToListRaw == null)
            {
                throw new ArgumentException("Parameter must be pipe delimited string");
            }
            //if (Index == null)
            //{
            //    throw new ArgumentException("Value must be integer array");
            //}
            //if (Index.GetUpperBound(0) != ConvertToList.Count - 1)
            //{
            //    throw new ArgumentOutOfRangeException("Parameter must be pipe delimited string  " + System.Convert.ToString(Index.GetUpperBound(0) + 1) + " options");
            //}

            List<string> ConvertToList = new List<string>(ConvertToListRaw.Split('|'));
            return ConvertToList;
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
