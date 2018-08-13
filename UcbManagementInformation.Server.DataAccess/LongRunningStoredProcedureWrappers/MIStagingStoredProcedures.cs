using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace UcbManagementInformation.Server.DataAccess
{
    public class MIStagingStoredProcedures : IDisposable
    {
        public MIStagingStoredProcedures()
        {
                _cn = new SqlConnection(ConfigurationManager.ConnectionStrings["MIStagingConnectionString"].ConnectionString);
        }

        private SqlConnection _cn;
        private bool isOpen;
        private int _defaultCommandTimeout = 30;
        public bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }
        private bool hasTransaction;

        public bool HasTransaction
        {
            get { return hasTransaction; }
            set { hasTransaction = value; }
        }
        public SqlConnection Cn
        {
            get { return _cn; }
            set { _cn = value; }
        }
        private SqlCommand _cmd;

        public int DeleteParticipantData(string keyValue,int timeout)
        {
            return RunSqlCommandWithKeyValue("dbo.DeleteParticipantData", keyValue, timeout);
        }
        public int CopyParticipantBulkLoadedFlat(string keyValue,int timeout)
        {
            return RunSqlCommandWithKeyValue("dbo.CopyParticipantBulkLoadedFlat", keyValue, timeout);
        }
        public int ValidateAgreement(string keyValue, int timeout)
        {
            return RunSqlCommandWithKeyValue("dbo.ValidateAgreement", keyValue, timeout);
        }
        public int ValidateParticipantDuplicates(string keyValue, int timeout)
        {
            return RunSqlCommandWithKeyValue("dbo.ValidateParticipantDuplicates", keyValue, timeout);
        }
        public int ValidateAgreementRegion(string keyValue, int timeout)
        {
            return RunSqlCommandWithKeyValue("dbo.ValidateAgreementRegion", keyValue, timeout);
        }
        public int ValidateParticipantPostCode(string keyValue, int timeout)
        {
            return RunSqlCommandWithKeyValue("dbo.ValidateParticipantPostCode", keyValue, timeout);
        }
        public int ValidateLeaver(string keyValue, int timeout)
        {
            return RunSqlCommandWithKeyValue("dbo.ValidateLeaver", keyValue, timeout);
        }
        public int ValidateLeavingDate(string keyValue, int timeout)
        {
            return RunSqlCommandWithKeyValue("dbo.ValidateLeavingDate", keyValue, timeout);
        }
        public int ValidateStartDate(string keyValue, int timeout)
        {
            return RunSqlCommandWithKeyValue("dbo.ValidateStartDate", keyValue, timeout);
        }
        public int ParticipantSelectValidCount(string keyValue, int timeout)
        {
            return RunSqlCommandWithKeyValue("dbo.ParticipantSelectValidCount", keyValue, true, timeout);
        }
        public int ParticipantSelectErrorCount(string keyValue, int timeout)
        {
            return RunSqlCommandWithKeyValue("dbo.ParticipantSelectErrorCount", keyValue, true, timeout);
        }
        public int ParticipantSelectWarningCount(string keyValue, int timeout)
        {
            return RunSqlCommandWithKeyValue("dbo.ParticipantSelectWarningCount", keyValue, true, timeout);
        }
        public int ParticipantSelectInformationalCount(string keyValue, int timeout)
        {
            return RunSqlCommandWithKeyValue("dbo.ParticipantSelectInformationalCount", keyValue, true, timeout);
        }
        public int DeleteValidatedParticipantData(string keyValue, SqlTransaction transact, int timeout)
        {
            return RunSqlCommandWithKeyValue("dbo.DeleteValidatedParticipantData", keyValue, transact, false, timeout);
        }
        public int DeleteParticipantDataOver4HoursOld(int timeout)
        {
            InitializeSql("DeleteParticipantDataOver4HoursOld");
            _cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });

            return RunSqlCommand(timeout);
        }
        public int DeleteInputFileErrorsOver1YearOld(int timeout)
        {
            InitializeSql("DeleteInputFileErrorsOver1YearOld");
            _cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });

            return RunSqlCommand(timeout);
        }
        public int CopyValidatedParticipants(string keyValue, int maximumErrorlevel, SqlTransaction transact, int timeout)
        {
            string provider = keyValue.Substring(0, keyValue.Length - 2);
            string region = keyValue.Substring(keyValue.Length - 2);

            InitializeSql("CopyValidatedParticipants");
            _cmd.Transaction = transact;
            _cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });
            _cmd.Parameters.Add(new SqlParameter("@providingOrganisation", provider) { Direction = ParameterDirection.Input });
            _cmd.Parameters.Add(new SqlParameter("@regionCode", region) { Direction = ParameterDirection.Input });
            _cmd.Parameters.Add(new SqlParameter("@maximumErrorLevel", maximumErrorlevel) { Direction = ParameterDirection.Input });

            return RunSqlCommand(timeout);
        }
        public int TruncatePostCodeData(int timeout)
        {
            InitializeSql("TruncatePostCodeData");
            _cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });

            return RunSqlCommand(timeout);
        }

        public int UpdateConstituency(int timeout)
        {
            InitializeSql("UpdateConstituency");
            _cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });

            return RunSqlCommand(timeout);
        }
        public int UpdateLocalAuthority(int timeout)
        {
            InitializeSql("UpdateLocalAuthority");
            _cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });

            return RunSqlCommand(timeout);
        }
        public int UpdateLea(int timeout)
        {
            InitializeSql("UpdateLea");
            _cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });

            return RunSqlCommand(timeout);
        }
        public int UpdateRegion(int timeout)
        {
            InitializeSql("UpdateRegion");
            _cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });

            return RunSqlCommand(timeout);
        }
        public int UpdateWard(int timeout)
        {
            InitializeSql("UpdateWard");
            _cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });

            return RunSqlCommand(timeout);
        }
        public int UpdateCounty(int timeout)
        {
            InitializeSql("UpdateCounty");
            _cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });

            return RunSqlCommand(timeout);
        }
        public int TruncateMIPostCodeData(int timeout)
        {
            InitializeSql("TruncateMIPostCodeData");
            _cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });

            return RunSqlCommand(timeout);
        }
        public int PopulatePostCodes(int timeout)
        {
            InitializeSql("PopulatePostCodes");
            _cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });

            return RunSqlCommand(timeout);
        }

        private int RunSqlCommandWithKeyValue(string commandText, string keyValue,int timeout)
        {
            return RunSqlCommandWithKeyValue(commandText, keyValue, null, false,timeout);
        
        }

        private int RunSqlCommandWithKeyValue(string commandText, string keyValue,bool isScalar,int timeout)
        {
            return RunSqlCommandWithKeyValue(commandText, keyValue, null, isScalar,timeout);
        }
        private int RunSqlCommandWithKeyValue(string commandText, string keyValue,SqlTransaction transact,bool isScalar,int timeout)
        {
            string provider = keyValue.Substring(0, keyValue.Length - 2);
            string region = keyValue.Substring(keyValue.Length - 2);

            InitializeSql(commandText);
            _cmd.Transaction = transact;
            if (transact != null)
            {
                hasTransaction = true;
            }
            _cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", null) { Direction = ParameterDirection.ReturnValue });
            _cmd.Parameters.Add(new SqlParameter("@providingOrganisation", provider) { Direction = ParameterDirection.Input });
            _cmd.Parameters.Add(new SqlParameter("@regionCode", region) { Direction = ParameterDirection.Input });

            return RunSqlCommand(isScalar,timeout);
        }

        private void InitializeSql(string commandText)
        {
            if (_cn == null || _cn.ConnectionString ==null || _cn.ConnectionString == "")
            {
                _cn = new SqlConnection(ConfigurationManager.ConnectionStrings["MIStagingConnectionString"].ConnectionString);
            }
            _cmd = new SqlCommand();
            _cmd.Connection = _cn;
            _cmd.CommandText = commandText;
            _cmd.CommandType = System.Data.CommandType.StoredProcedure;

        }
        
        private int RunSqlCommand(int timeout)
        {
            return RunSqlCommand(false, timeout); 
        }
        private int RunSqlCommand(bool isScalar,int timeout)
        {
            if (timeout == 0)
            {
                _cmd.CommandTimeout = _defaultCommandTimeout;
            }
            else
            {
                _cmd.CommandTimeout = timeout;
            }
            int returnValue;

            //using (_cn)
            //{
                if (!isOpen)
                {
                    _cn.Open();
                    isOpen = true;
                }
                try
                {
                    if (isScalar)
                    {
                        returnValue = (int)_cmd.ExecuteScalar();
                    }
                    else
                    {
                        returnValue = _cmd.ExecuteNonQuery();
                    }
                }
                finally
                {
                    if (isOpen && !hasTransaction)
                    {
                        _cn.Close();
                        isOpen = false;
                    }
                }
            //}
            return returnValue;
        }

        public void Dispose()
        {
            if (_cn != null)
            {
                _cn.Dispose();
            }
            GC.SuppressFinalize(this);
           
        }
    }
}
