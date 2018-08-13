using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Xml;

namespace UcbManagementInformation.Web.MIFileUpload
{
    public class LookUpDataReader : IEnhancedDataReader
    {
        private LookUp _currentLookUpRecord;
        private int _recordCount = 0;
        private string _filePath;
        private string _category;
        private StreamReader _textReader;
        private int _keyColumn;
        private int _nameColumn;
        private long _fileLength;

        public long FileLength
        {
            get { return _fileLength; }
            set { _fileLength = value; }
        }
        private long currentPosition;

        public long CurrentPosition
        {
            get { return currentPosition; }
            set { currentPosition = value; }
        }

        public LookUpDataReader(string category,string lookUpFilePath,int keyColumn,int nameColumn)
        {
            _filePath = lookUpFilePath;
            _textReader = File.OpenText(_filePath);
            _category = category;
            _currentLookUpRecord = new LookUp();
            _currentLookUpRecord.RecordType = category;
            _keyColumn = keyColumn;
            _nameColumn = nameColumn;
            _fileLength = _textReader.BaseStream.Length;
            currentPosition = 0;
        }

        #region IDataReader Members

        public bool Read()
        {

            string currentLine = null;
            if (_recordCount == 0)
            {
                //read the header row and ignore
                currentLine = _textReader.ReadLine();
                currentPosition = _textReader.BaseStream.Position;
                        
            }
            while (!_textReader.EndOfStream)
            {
                currentLine = _textReader.ReadLine();
                currentPosition = _textReader.BaseStream.Position;
                if (_textReader.EndOfStream)
                {
                    return false;
                }
                else
                {
                    string[] items = currentLine.Split('\t');
                    //if key or name is null move to next record.
                    if (string.IsNullOrEmpty(items[_keyColumn]) || string.IsNullOrEmpty(items[_nameColumn]))
                    {
                    }
                    else
                    {
                        _currentLookUpRecord = new LookUp() { LookUpKey = items[_keyColumn], Name = items[_nameColumn], RecordType = _category };
                        _recordCount++;
                        return true;
                    }
                }
            }
            return false;

        }
    
        public void Close()
        {
            _textReader.Close();
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
            _textReader.Dispose();
        }

        #endregion

        #region IDataRecord Members
        
        public int FieldCount
        {
            get { return 3; }
        }

        public object GetValue(int i)
        {
            switch (i)
            {
                case 0:
                    return DBNullIfEmptyString(_currentLookUpRecord.RecordType);
                    
                case 1:
                    return DBNullIfEmptyString(_currentLookUpRecord.LookUpKey);
                    
                case 2:
                    return DBNullIfEmptyString(_currentLookUpRecord.Name);
                
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
                case "LookUpType":
                    return 0;

                case "LookUpKey":
                    return 1;

                case "Name":
                    return 2;
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