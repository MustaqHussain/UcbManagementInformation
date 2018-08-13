using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UcbManagementInformation.Server.IoC.ServiceLocation;
using UcbManagementInformation.Server.DataAccess.Repositories;

namespace UcbManagementInformation.Server.DataAccess
{
    public class DataAccessUtilities
    {
        private DataAccessUtilities()
        { }
        public static T RepositoryLocator<T>(IObjectContext context) 
        {
            Dictionary<string, object> contextHolder = new Dictionary<string, object>();
            
            contextHolder.Add("context", context);
            
            return SimpleServiceLocator.Instance.Get<T>(contextHolder);
        }
        public static T RepositoryLocator<T>()
        {
            return RepositoryLocator<T>(new NullObjectContext());
        }

        public static string GetSystemParameterByName(string Name)
        {
            IUcbManagementInformationRepository<MCSystemParameter> systemParameterRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<MCSystemParameter>>();
            MCSystemParameter parameter = systemParameterRepository.Find(x => x.Name == Name).FirstOrDefault();
            if (parameter != null)
            {
                return parameter.ParameterValue;
            }
            else
            {
                return null;
            }
        }    
        /// <summary>
        /// A comparer to allow sorting / grouping by period when considering a date value
        /// </summary>
        public class DatePeriodComparer : IEqualityComparer<DateTime>
        {

            public bool Equals(DateTime x, DateTime y)
            {
                return ConvertDateToPeriod(x) == ConvertDateToPeriod(y);
            }

            public int GetHashCode(DateTime obj)
            {
                return ConvertDateToPeriod(obj).GetHashCode();
            }

        }
        /// <summary>
        /// Takes a date and returns the corresponding Period value
        /// </summary>
        /// <param name="dateToConvert">DateTime to find the period of</param>
        /// <returns>string representing Period value</returns>
        public static string ConvertDateToPeriod(DateTime dateToConvert)
        {
            int Year = dateToConvert.Year;
            int Month = dateToConvert.Month;
            if (Month == 12) Month = 0;
            Month += 2 - (Month % 3);
            string FormattedDate = Year.ToString() + Month.ToString().PadLeft(2, '0');
            return FormattedDate;
        }
    }
}
