using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace UcbManagementInformation.Server.DataAccess
{
    public class UcbManagementInformationStoredProcedures
    {
        public UcbManagementInformationStoredProcedures()
        { }

        private SqlConnection _cn;
        private SqlCommand _cmd;


        public int LoadParticipantMessages(Guid inputFileHistoryCode, int timeout)
        {
           InitializeSql("dbo.LoadParticipantMessages");

            _cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });
            _cmd.Parameters.Add(new SqlParameter("@InputFileHistoryCode", inputFileHistoryCode) { Direction = ParameterDirection.Input });

            return RunSqlCommand(timeout);
        }
        public int InputFileErrorsSummary(Guid inputFileHistoryCode,out int FileLevelInfo, out int FileLevelWarning,
            out int FileLevelError, out int RecordLevelInfo, out int RecordLevelWarning, out int RecordLevelError)
        {
            InitializeSql("dbo.InputFileErrorsSummary");

            //_cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });
            _cmd.Parameters.Add(new SqlParameter("@InputFileHistoryCode", inputFileHistoryCode) { Direction = ParameterDirection.Input });
            using (_cn)
            {
                _cn.Open();
                try
                {
                    var ResultsReader = _cmd.ExecuteReader();
                    FileLevelInfo = 0;
                    FileLevelWarning = 0;
                    FileLevelError = 0;
                    RecordLevelInfo = 0;
                    RecordLevelWarning = 0;
                    RecordLevelError = 0;

                    while (ResultsReader.Read())
                    {
                        FileLevelInfo = ResultsReader.GetInt32(0);
                        FileLevelWarning = ResultsReader.GetInt32(1);
                        FileLevelError = ResultsReader.GetInt32(2);
                        RecordLevelInfo = ResultsReader.GetInt32(3);
                        RecordLevelWarning = ResultsReader.GetInt32(4);
                        RecordLevelError = ResultsReader.GetInt32(5);
                    }
                    ResultsReader.Close();
                }
                finally
                {
                    _cn.Close();
                }
            }
            return 0;
        }
        private int RunSqlCommandWithKeyValue(string commandText, string keyValue, int timeout)
        {
            string provider = keyValue.Substring(0, keyValue.Length - 2);
            string region = keyValue.Substring(keyValue.Length - 2);

            InitializeSql(commandText);

            _cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });
            _cmd.Parameters.Add(new SqlParameter("@providingOrganisation", provider) { Direction = ParameterDirection.Input });
            _cmd.Parameters.Add(new SqlParameter("@regionCode", region) { Direction = ParameterDirection.Input });

            return RunSqlCommand(timeout);
        }
        public int PublishError(string category, int? errorLevel, string errorMessage, Guid inputFileHistoryCode, DateTime? recordDateTime, string recordKey)
        {
            InitializeSql("PublishError");
            
            _cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });
            _cmd.Parameters.Add(new SqlParameter("@errorMessage", (errorMessage==null)?DBNull.Value:(object)errorMessage ) { Direction = ParameterDirection.Input });
            _cmd.Parameters.Add(new SqlParameter("@inputFileHistoryCode", (inputFileHistoryCode == null) ? DBNull.Value : (object)inputFileHistoryCode) { Direction = ParameterDirection.Input });
            _cmd.Parameters.Add(new SqlParameter("@recordKey", recordKey == null ? DBNull.Value : (object)recordKey) { Direction = ParameterDirection.Input });
            _cmd.Parameters.Add(new SqlParameter("@category", category == null ? DBNull.Value : (object)category) { Direction = ParameterDirection.Input });
            _cmd.Parameters.Add(new SqlParameter("@errorLevel", errorLevel == null ? DBNull.Value : (object)errorLevel) { Direction = ParameterDirection.Input });
            _cmd.Parameters.Add(new SqlParameter("@recordDateTime", recordDateTime == null ? DBNull.Value : (object)recordDateTime) { Direction = ParameterDirection.Input });

            return RunSqlCommand(0);
        }
        private void InitializeSql(string commandText)
        {
            _cn = new SqlConnection(ConfigurationManager.ConnectionStrings["UcbManagementInformationDatabase"].ConnectionString);
            _cmd = new SqlCommand();
            _cmd.Connection = _cn;
            _cmd.CommandText = commandText;
            _cmd.CommandType = System.Data.CommandType.StoredProcedure;

        }

        private int RunSqlCommand(int timeout)
        {
            int returnValue;
            if (timeout == 0)
            {
                _cmd.CommandTimeout = 30;
            }
            else
            {
                _cmd.CommandTimeout = timeout;
            }
            using (_cn)
            {
                _cn.Open();
                try
                {
                    returnValue = _cmd.ExecuteNonQuery();
                }
                finally
                {
                    _cn.Close();
                }
            }
            return returnValue;
        }
 
    }
}
