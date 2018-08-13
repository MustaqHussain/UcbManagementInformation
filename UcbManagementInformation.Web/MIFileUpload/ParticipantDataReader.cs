using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Web.Models;

namespace UcbManagementInformation.Server.DataAccess
{
    public class ParticipantDataReader : IDataReader
    {
        private Participant _currentParticipantRecord;
        private Participant _currentParticipantRecordMaskedByType;
        private Guid _inputFileCode;
        private Guid _providingOrganisationCode;
        private int _recordCount = 0;
        private string _filePath;
        private XmlReader _xmlReader;
        private FileStream _xmlStream;
        public ParticipantDataReader(string participantFilePath, Guid inputFileCode, Guid providingOrganisationCode)
        {
            _inputFileCode = inputFileCode;
            _providingOrganisationCode = providingOrganisationCode;

            _filePath = participantFilePath;
            _xmlStream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlReaderSettings Settings = new XmlReaderSettings();
            Settings.ValidationType = ValidationType.None;
            _xmlReader = XmlReader.Create(_xmlStream, Settings);
            _currentParticipantRecord = new Participant();
        }

        #region IDataReader Members

        public bool Read()
        {


            while (_xmlReader.Read())
            {
                    if (_xmlReader.IsStartElement())
                    {
                        switch (_xmlReader.LocalName)
                        {
                            case "ProvidingOrganisation":
                                _currentParticipantRecord.ProvidingOrganisation = _xmlReader.ReadString();
                                break;
                            case "FileRunDate":
                                _currentParticipantRecord.FileRunDate = _xmlReader.ReadString();
                                break;
                            case "RegionCode":
                                _currentParticipantRecord.RegionCode = _xmlReader.ReadString();
                                break;
                            case "RegionName":
                                _currentParticipantRecord.RegionName = _xmlReader.ReadString();
                                break;
                            case "AgreementID":
                                _currentParticipantRecord.AgreementID = _xmlReader.ReadString();
                                break;
                            case "ProviderProjectID":
                                _currentParticipantRecord.ProviderProjectID = _xmlReader.ReadString();
                                break;
                            case "FundingType":
                                _currentParticipantRecord.FundingType = _xmlReader.ReadString();
                                break;
                            case "ParticipantReferenceID":
                                _recordCount++;
                                _currentParticipantRecord.ParticipantCode = _recordCount;//Guid.NewGuid();
                                _currentParticipantRecord.ParticipantReferenceID = _xmlReader.ReadString();
                                break;
                            case "StartDate":
                                _currentParticipantRecord.StartDate = _xmlReader.ReadString();
                                break;
                            case "Gender":
                                _currentParticipantRecord.Gender = _xmlReader.ReadString();
                                break;
                            case "DOB":
                                _currentParticipantRecord.DOB = _xmlReader.ReadString();
                                break;
                            case "Postcode":
                                _currentParticipantRecord.Postcode = _xmlReader.ReadString();
                                break;
                            case "EmploymentStatus":
                                _currentParticipantRecord.EmploymentStatus = _xmlReader.ReadString();
                                break;
                            case "UnemployedMonths":
                                _currentParticipantRecord.UnemployedMonths = _xmlReader.ReadString();
                                break;
                            case "Ethnicity":
                                _currentParticipantRecord.Ethnicity = _xmlReader.ReadString();
                                break;
                            case "Disability":
                                _currentParticipantRecord.Disability = _xmlReader.ReadString();
                                break;
                            case "QualificationHeld":
                                _currentParticipantRecord.QualificationHeld = _xmlReader.ReadString();
                                break;
                            case "InPostGradResearch":
                                _currentParticipantRecord.InPostGradResearch = _xmlReader.ReadString();
                                break;
                            case "GraduatePlacedWithSME":
                                _currentParticipantRecord.GraduatePlacedWithSME = _xmlReader.ReadString();
                                break;
                            case "LeavingDate":
                                _currentParticipantRecord.LeavingDate = _xmlReader.ReadString();
                                break;
                            case "P1P4LeavingStatus":
                                _currentParticipantRecord.P1P4LeavingStatus = _xmlReader.ReadString();
                                break;
                            case "P1P4NoQualificationGained":
                                _currentParticipantRecord.P1P4NoQualificationGained = _xmlReader.ReadString();
                                break;
                            case "P1P4QualificationGained":
                                _currentParticipantRecord.P1P4QualificationGained = _xmlReader.ReadString();
                                break;
                            case "P2P5IntoEducationOrTraining":
                                _currentParticipantRecord.P2P5IntoEducationOrTraining = _xmlReader.ReadString();
                                break;
                            case "P5GraduatePlacedWithinSMEWhoGainedEmployment":
                                _currentParticipantRecord.P5GraduatePlacedWithinSMEWhoGainedEmployment = _xmlReader.ReadString();
                                break;
                            case "P2P5NoQualificationGained":
                                _currentParticipantRecord.P2P5NoQualificationGained = _xmlReader.ReadString();
                                break;
                            case "P2P5QualificationGained":
                                _currentParticipantRecord.P2P5QualificationGained = _xmlReader.ReadString();
                                break;
                        }

                    }
                    else
                    {
                        switch (_xmlReader.LocalName)
                        {
                            case "ProvidingOrganisation":
                            case "FileRunDate":
                                break;
                            case "Region":
                                _currentParticipantRecord.RegionCode = "";
                                _currentParticipantRecord.RegionName = "";
                                _currentParticipantRecord.AgreementID = "";
                                _currentParticipantRecord.ProviderProjectID = "";
                                _currentParticipantRecord.FundingType = "";
                                _currentParticipantRecord.ParticipantReferenceID = "";
                                _currentParticipantRecord.StartDate = "";
                                _currentParticipantRecord.Gender = "";
                                _currentParticipantRecord.DOB = "";
                                _currentParticipantRecord.Postcode = "";
                                _currentParticipantRecord.EmploymentStatus = "";
                                _currentParticipantRecord.UnemployedMonths = "";
                                _currentParticipantRecord.Ethnicity = "";
                                _currentParticipantRecord.Disability = "";
                                _currentParticipantRecord.QualificationHeld = "";
                                _currentParticipantRecord.InPostGradResearch = "";
                                _currentParticipantRecord.GraduatePlacedWithSME = "";
                                _currentParticipantRecord.LeavingDate = "";
                                _currentParticipantRecord.P1P4LeavingStatus = "";
                                _currentParticipantRecord.P1P4NoQualificationGained = "";
                                _currentParticipantRecord.P1P4QualificationGained = "";
                                _currentParticipantRecord.P2P5IntoEducationOrTraining = "";
                                _currentParticipantRecord.P5GraduatePlacedWithinSMEWhoGainedEmployment = "";
                                _currentParticipantRecord.P2P5NoQualificationGained = "";
                                _currentParticipantRecord.P2P5QualificationGained = "";
                                break;
                            case "Agreement":
                                _currentParticipantRecord.AgreementID = "";
                                _currentParticipantRecord.ProviderProjectID = "";
                                _currentParticipantRecord.FundingType = "";
                                _currentParticipantRecord.ParticipantReferenceID = "";
                                _currentParticipantRecord.StartDate = "";
                                _currentParticipantRecord.Gender = "";
                                _currentParticipantRecord.DOB = "";
                                _currentParticipantRecord.Postcode = "";
                                _currentParticipantRecord.EmploymentStatus = "";
                                _currentParticipantRecord.UnemployedMonths = "";
                                _currentParticipantRecord.Ethnicity = "";
                                _currentParticipantRecord.Disability = "";
                                _currentParticipantRecord.QualificationHeld = "";
                                _currentParticipantRecord.InPostGradResearch = "";
                                _currentParticipantRecord.GraduatePlacedWithSME = "";
                                _currentParticipantRecord.LeavingDate = "";
                                _currentParticipantRecord.P1P4LeavingStatus = "";
                                _currentParticipantRecord.P1P4NoQualificationGained = "";
                                _currentParticipantRecord.P1P4QualificationGained = "";
                                _currentParticipantRecord.P2P5IntoEducationOrTraining = "";
                                _currentParticipantRecord.P5GraduatePlacedWithinSMEWhoGainedEmployment = "";
                                _currentParticipantRecord.P2P5NoQualificationGained = "";
                                _currentParticipantRecord.P2P5QualificationGained = "";
                                break;
                            case "ProviderProjectReference":
                                _currentParticipantRecord.ProviderProjectID = "";
                                _currentParticipantRecord.FundingType = "";
                                _currentParticipantRecord.ParticipantReferenceID = "";
                                _currentParticipantRecord.StartDate = "";
                                _currentParticipantRecord.Gender = "";
                                _currentParticipantRecord.DOB = "";
                                _currentParticipantRecord.Postcode = "";
                                _currentParticipantRecord.EmploymentStatus = "";
                                _currentParticipantRecord.UnemployedMonths = "";
                                _currentParticipantRecord.Ethnicity = "";
                                _currentParticipantRecord.Disability = "";
                                _currentParticipantRecord.QualificationHeld = "";
                                _currentParticipantRecord.InPostGradResearch = "";
                                _currentParticipantRecord.GraduatePlacedWithSME = "";
                                _currentParticipantRecord.LeavingDate = "";
                                _currentParticipantRecord.P1P4LeavingStatus = "";
                                _currentParticipantRecord.P1P4NoQualificationGained = "";
                                _currentParticipantRecord.P1P4QualificationGained = "";
                                _currentParticipantRecord.P2P5IntoEducationOrTraining = "";
                                _currentParticipantRecord.P5GraduatePlacedWithinSMEWhoGainedEmployment = "";
                                _currentParticipantRecord.P2P5NoQualificationGained = "";
                                _currentParticipantRecord.P2P5QualificationGained = "";
                                break;
                            case "Participant":
                                MaskByType(1);
                                //reset flag for next participant
                                _currentParticipantRecord.ParticipantReferenceID = "";
                                _currentParticipantRecord.StartDate = "";
                                _currentParticipantRecord.Gender = "";
                                _currentParticipantRecord.DOB = "";
                                _currentParticipantRecord.Postcode = "";
                                _currentParticipantRecord.EmploymentStatus = "";
                                _currentParticipantRecord.UnemployedMonths = "";
                                _currentParticipantRecord.Ethnicity = "";
                                _currentParticipantRecord.Disability = "";
                                _currentParticipantRecord.QualificationHeld = "";
                                _currentParticipantRecord.InPostGradResearch = "";
                                _currentParticipantRecord.GraduatePlacedWithSME = "";
                                _currentParticipantRecord.LeavingDate = "";
                                _currentParticipantRecord.P1P4LeavingStatus = "";
                                _currentParticipantRecord.P1P4NoQualificationGained = "";
                                _currentParticipantRecord.P1P4QualificationGained = "";
                                _currentParticipantRecord.P2P5IntoEducationOrTraining = "";
                                _currentParticipantRecord.P5GraduatePlacedWithinSMEWhoGainedEmployment = "";
                                _currentParticipantRecord.P2P5NoQualificationGained = "";
                                _currentParticipantRecord.P2P5QualificationGained = "";
                                return true;
                                
                                break;
                            case "ESFP1P4Qualification":
                                MaskByType(2);
                                _currentParticipantRecord.P1P4QualificationGained = "";
                                return true;
                                
                                break;
                            case "ESFP2P5Qualification":
                                MaskByType(3);
                                _currentParticipantRecord.P2P5QualificationGained = "";
                                return true;
                                break;
                        }
                    }
              }
              return false;
        }

        private void MaskByType(int recordType)
        {
            _currentParticipantRecordMaskedByType = new Participant(_currentParticipantRecord);
            switch (recordType)
            {
                case 1:
                    _currentParticipantRecordMaskedByType.RecordType = "1";
                    _currentParticipantRecordMaskedByType.P1P4QualificationGained = "";
                    _currentParticipantRecordMaskedByType.P2P5QualificationGained = "";
                    break;
                case 2:
                    _currentParticipantRecordMaskedByType.RecordType = "2";
                    _currentParticipantRecordMaskedByType.Gender = "";
                    _currentParticipantRecordMaskedByType.DOB = "";
                    _currentParticipantRecordMaskedByType.Postcode = "";
                    _currentParticipantRecordMaskedByType.EmploymentStatus = "";
                    _currentParticipantRecordMaskedByType.UnemployedMonths = "";
                    _currentParticipantRecordMaskedByType.Ethnicity = "";
                    _currentParticipantRecordMaskedByType.Disability = "";
                    _currentParticipantRecordMaskedByType.QualificationHeld = "";
                    _currentParticipantRecordMaskedByType.InPostGradResearch = "";
                    _currentParticipantRecordMaskedByType.GraduatePlacedWithSME = "";
                    _currentParticipantRecordMaskedByType.LeavingDate = "";
                    _currentParticipantRecordMaskedByType.P1P4LeavingStatus = "";
                    _currentParticipantRecordMaskedByType.P1P4NoQualificationGained = "";
                    _currentParticipantRecordMaskedByType.P2P5IntoEducationOrTraining = "";
                    _currentParticipantRecordMaskedByType.P5GraduatePlacedWithinSMEWhoGainedEmployment = "";
                    _currentParticipantRecordMaskedByType.P2P5NoQualificationGained = "";
                    _currentParticipantRecordMaskedByType.P2P5QualificationGained = "";
                    break;
                case 3:
                    _currentParticipantRecordMaskedByType.RecordType = "3";
                    _currentParticipantRecordMaskedByType.Gender = "";
                    _currentParticipantRecordMaskedByType.DOB = "";
                    _currentParticipantRecordMaskedByType.Postcode = "";
                    _currentParticipantRecordMaskedByType.EmploymentStatus = "";
                    _currentParticipantRecordMaskedByType.UnemployedMonths = "";
                    _currentParticipantRecordMaskedByType.Ethnicity = "";
                    _currentParticipantRecordMaskedByType.Disability = "";
                    _currentParticipantRecordMaskedByType.QualificationHeld = "";
                    _currentParticipantRecordMaskedByType.InPostGradResearch = "";
                    _currentParticipantRecordMaskedByType.GraduatePlacedWithSME = "";
                    _currentParticipantRecordMaskedByType.LeavingDate = "";
                    _currentParticipantRecordMaskedByType.P1P4LeavingStatus = "";
                    _currentParticipantRecordMaskedByType.P1P4NoQualificationGained = "";
                    _currentParticipantRecordMaskedByType.P1P4QualificationGained = "";
                    _currentParticipantRecordMaskedByType.P2P5IntoEducationOrTraining = "";
                    _currentParticipantRecordMaskedByType.P5GraduatePlacedWithinSMEWhoGainedEmployment = "";
                    _currentParticipantRecordMaskedByType.P2P5NoQualificationGained = "";
                    break;
            }
        }

        public void Close()
        {
            _xmlReader.Close();
            _xmlStream.Close();
        }

        
        #region NotImplemented
        
        public int Depth
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public System.Data.DataTable GetSchemaTable()
        {   
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsClosed
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool NextResult()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        
        public int RecordsAffected
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }
        #endregion

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _xmlStream.Dispose();
        }

        #endregion

        #region IDataRecord Members
        
        public int FieldCount
        {
            get { return 32; }
        }

        public object GetValue(int i)
        {
            switch (i)
            {
                case 0:
                    return DBNullIfEmptyInt(_currentParticipantRecordMaskedByType.RecordType);
                    
                case 1:
                    return DBNullIfEmptyInt(_currentParticipantRecordMaskedByType.ParticipantCode.ToString());
                    
                case 2:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.ProvidingOrganisation);
                    
                case 3:
                    return DBNullIfEmptyDate(_currentParticipantRecordMaskedByType.FileRunDate);
                    
                case 4:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.RegionCode);
                 
                case 5:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.RegionName);
                  
                case 6:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.AgreementID);
                  
                case 7:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.ProviderProjectID);
                  
                case 8:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.FundingType);
                  
                case 9:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.ParticipantReferenceID);
                  
                case 10:
                    return DBNullIfEmptyDate(_currentParticipantRecordMaskedByType.StartDate);
                  
                case 11:
                    return DBNullIfEmptySmallInt(_currentParticipantRecordMaskedByType.Gender);
                  
                case 12:
                    return DBNullIfEmptyDate(_currentParticipantRecordMaskedByType.DOB);
  
                case 13:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.Postcode);

                case 14:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.Postcode.Replace(" ",""));
                
                case 15:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.EmploymentStatus);
                  
                case 16:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.UnemployedMonths);
                  
                case 17:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.Ethnicity);
                  
                case 18:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.Disability);
                  
                case 19:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.QualificationHeld);
                  
                case 20:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.InPostGradResearch);
                  
                case 21:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.GraduatePlacedWithSME);
                  
                case 22:
                    return DBNullIfEmptyDate(_currentParticipantRecordMaskedByType.LeavingDate);
                
                case 23:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.P1P4LeavingStatus);
                    
                case 24:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.P1P4NoQualificationGained);
                case 25:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.P1P4QualificationGained);
                
                case 26:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.P2P5IntoEducationOrTraining);
                
                case 27:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.P5GraduatePlacedWithinSMEWhoGainedEmployment);
                
                case 28:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.P2P5NoQualificationGained);
                
                case 29:
                    return DBNullIfEmptyString(_currentParticipantRecordMaskedByType.P2P5QualificationGained);
                
                case 30:
                    return _inputFileCode;
                
                case 31:
                    return _providingOrganisationCode;
                //32 is sequential
                
                default:
                    return null;
            }
        }
        private static object DBNullIfEmptyString(string fieldToCheck)
        {
            return (fieldToCheck != "" ? (object)(fieldToCheck) : (object)DBNull.Value);

        }
        private static object DBNullIfEmptyInt(string fieldToCheck)
        {
            return (fieldToCheck != "" ? (object)Convert.ToInt32(fieldToCheck) : (object)DBNull.Value);
        }
        private static object DBNullIfEmptySmallInt(string fieldToCheck)
        {
            return (fieldToCheck != "" ? (object)Convert.ToInt16(fieldToCheck) : (object)DBNull.Value);
        }
        private static object DBNullIfEmptyDate(string fieldToCheck)
        {
            return (fieldToCheck != "" ? (object)DateTime.Parse(fieldToCheck) : (object)DBNull.Value);
        }
        public int GetOrdinal(string name)
        {
            switch (name)
            {
                case "RecordType":
                    return 0;

                case "ParticipantCode":
                    return 1;

                case "ProvidingOrganisation":
                    return 2;

                case "FileRunDate":
                    return 3;

                case "RegionCode":
                    return 4;
                case "RegionName":
                    return 5;

                case "AgreementID":
                    return 6;

                case "ProviderProjectID":
                    return 7;

                case "FundingType":
                    return 8;

                case "ParticipantReferenceID":
                    return 9;

                case "StartDate":
                    return 10;

                case "Gender":
                    return 11;

                case "DOB":
                    return 12;

                case "Postcode":
                    return 13;
                case "PostcodeCompressed":
                    return 14;

                case "EmploymentStatus":
                    return 15;
                case "UnemployedMonths":
                    return 16;

                case "Ethnicity":
                    return 17;

                case "Disability":
                    return 18;

                case "QualificationHeld":
                    return 19;

                case "InPostGradResearch":
                    return 20;

                case "GraduatePlacedWithSME":
                    return 21;

                case "LeavingDate":
                    return 22;
                case "P1P4LeavingStatus":
                    return 23;

                case "P1P4NoQualificationGained":
                    return 24;
                case "P1P4QualificationGained":
                    return 25;
                case "P2P5IntoEducationOrTraining":
                    return 26;
                case "P5GraduatePlacedWithinSMEWhoGainedEmployment":
                    return 27;

                case "P2P5NoQualificationGained":
                    return 28;

                case "P2P5QualificationGained":
                    return 29;

                case "InputFileCode":
                    return 30;
                
                case "ProvidingOrganisationCode":
                    return 31;
                case "SequentialCode":
                    return 32;
                default:
                    return -1;
            }
        }

        public object this[string name]
        {
            get { return this[GetOrdinal(name)]; }
        }

        public object this[int i]
        {
            get { return this.GetValue(i); }
        }

        #region Not Implemented

        

        public bool GetBoolean(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public byte GetByte(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public char GetChar(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IDataReader GetData(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetDataTypeName(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DateTime GetDateTime(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public decimal GetDecimal(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public double GetDouble(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Type GetFieldType(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public float GetFloat(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Guid GetGuid(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public short GetInt16(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetInt32(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public long GetInt64(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetName(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        

        public string GetString(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        

        public int GetValues(object[] values)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsDBNull(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        

        #endregion

        #endregion
    }
}
