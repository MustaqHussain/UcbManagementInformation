using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;

namespace UcbManagementInformation.Web.MIFileUpload
{
    public class PostCodeDataReader : IEnhancedDataReader
    {

        private PostCode _currentPostCodeRecord;
        private int _recordCount = 0;
        private string _filePath;
       
        private Dictionary<string, string> LALookUp = new Dictionary<string,string>();
        private Dictionary<string, string> LEALookUp = new Dictionary<string,string>();
        private Dictionary<string, string> WardLookUp = new Dictionary<string,string>();
        private Dictionary<string, string> RegionLookUp = new Dictionary<string,string>();
        private Dictionary<string, string> ConstituencyLookUp = new Dictionary<string,string>();
        private Dictionary<string, string> CountyLookUp = new Dictionary<string,string>();

        private string _category;
        private StreamReader _textReader;
        private StreamReader _lookUpReader;
        private int _leaKeyColumn;
        private int _wardKeyColumn;
        private int _nuts1KeyColumn;
        private int _constituencyKeyColumn;
        private int _lAKeyColumn;
        private int _countyKeyColumn;
        private int _postCode7Column;
        private int _postCode8Column;
        private int _eastingColumn;
        private int _northingColumn;
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
        public PostCodeDataReader(string category, string postCodeFilePath, int leaKeyColumn, int wardKeyColumn, int nuts1KeyColumn, int constituencyKeyColumn,
            int lAKeyColumn,int countyKeyColumn,int postCode7Column,int postCode8Column,int eastingColumn,int northingColumn,
            string filepathLA,int lookupLAKeyColumn,int lookupLANameColumn,
            string filepathLEA,int lookupLEAKeyColumn,int lookupLEANameColumn,
            string filepathCty,int lookupCtyKeyColumn,int lookupCtyNameColumn,
            string filepathWard,int lookupWardKeyColumn,int lookupWardNameColumn,
            string filepathRegion,int lookupRegionKeyColumn,int lookupRegionNameColumn,
            string filepathConstituency,int lookupConstituencyKeyColumn,int lookupConstituencyNameColumn)
        {
            _filePath = postCodeFilePath;
            
            ReadLookUp("Constituency", filepathConstituency, lookupConstituencyKeyColumn, lookupConstituencyNameColumn);
            ReadLookUp("Ward", filepathWard, lookupWardKeyColumn, lookupWardNameColumn);
            ReadLookUp("Region", filepathRegion, lookupRegionKeyColumn, lookupRegionNameColumn);
            ReadLookUp("County", filepathCty, lookupCtyKeyColumn, lookupCtyNameColumn);
            ReadLookUp("LA", filepathLA, lookupLAKeyColumn, lookupLANameColumn);
            ReadLookUp("LEA", filepathLEA, lookupLEAKeyColumn, lookupLEANameColumn);
            
            _textReader = File.OpenText(_filePath);
            _category = category;
            _currentPostCodeRecord = new PostCode();
            _leaKeyColumn = leaKeyColumn;
            _wardKeyColumn = wardKeyColumn;
            _nuts1KeyColumn = nuts1KeyColumn;
            _constituencyKeyColumn = constituencyKeyColumn;
            _lAKeyColumn = lAKeyColumn;
            _countyKeyColumn = countyKeyColumn;
            _postCode7Column = postCode7Column;
            _postCode8Column = postCode8Column;
            _eastingColumn = eastingColumn;
            _northingColumn = northingColumn;
            _fileLength = _textReader.BaseStream.Length;
            currentPosition = 0;
        }

        void ReadLookUp(string type,string fileName, int keyCol, int nameCol)
        {
            using (StreamReader sReader = File.OpenText(fileName))
            {
                string headings = sReader.ReadLine();
                while (!sReader.EndOfStream)
                {
                    string currentLookUpRecord = sReader.ReadLine();
                    string[] recordsItems = currentLookUpRecord.Split('\t');
                    if (!String.IsNullOrEmpty(recordsItems[keyCol]) && !String.IsNullOrEmpty(recordsItems[nameCol]))
                    {
                        switch (type)
                        {
                            case "Constituency":
                                ConstituencyLookUp.Add(recordsItems[keyCol], recordsItems[nameCol]);
                                break;
                            case "Ward":
                                WardLookUp.Add(recordsItems[keyCol], recordsItems[nameCol]);
                                break;
                            case "LA":
                                LALookUp.Add(recordsItems[keyCol], recordsItems[nameCol]);
                                break;
                            case "LEA":
                                LEALookUp.Add(recordsItems[keyCol], recordsItems[nameCol]);
                                break;
                            case "Region":
                                RegionLookUp.Add(recordsItems[keyCol], recordsItems[nameCol]);
                                break;
                            case "County":
                                CountyLookUp.Add(recordsItems[keyCol], recordsItems[nameCol]);
                                break;
                        }
                    }
                }
            }
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

            currentLine = _textReader.ReadLine();
            currentPosition = _textReader.BaseStream.Position;
            if (_textReader.EndOfStream)
            {
                return false;
            }
            else
            {
                string[] items = currentLine.Replace("\",\"","|").Replace("\"","").Split('|');
                _currentPostCodeRecord = new PostCode() 
                { 
                    ConstituencyID = items[_constituencyKeyColumn],
                    County = items[_countyKeyColumn],
                    Lad_Ua = items[_lAKeyColumn],
                    Lea = items[_leaKeyColumn],
                    Nuts1 = items[_nuts1KeyColumn],
                    Parl_Con = items[_constituencyKeyColumn],
                    Ward = items[_wardKeyColumn],
                    PostCode7 = items[_postCode7Column],
                    PostCode8 = items[_postCode8Column],
                    ConstituencyName = ConstituencyLookUp.ContainsKey(items[_constituencyKeyColumn]) ?ConstituencyLookUp[items[_constituencyKeyColumn]] :null,
                    CountyName = CountyLookUp.ContainsKey(items[_countyKeyColumn]) ? CountyLookUp[items[_countyKeyColumn]] : null,
                    LEAName = LEALookUp.ContainsKey(items[_leaKeyColumn]) ? LEALookUp[items[_leaKeyColumn]] : null,
                    LocalAuthorityName = LALookUp.ContainsKey(items[_lAKeyColumn]) ? LALookUp[items[_lAKeyColumn]] : null,
                    RegionName = RegionLookUp.ContainsKey(items[_nuts1KeyColumn]) ? RegionLookUp[items[_nuts1KeyColumn]] : null,
                    WardName= WardLookUp.ContainsKey(items[_wardKeyColumn]) ? WardLookUp[items[_wardKeyColumn]] : null,
                    PostCodeCompressed = items[_postCode7Column].Replace(" ",""),
                    Easting=items[_eastingColumn],
                    Northinging=items[_northingColumn]
                };
                if (_currentPostCodeRecord.CountyName == null)
                {
                    if (_currentPostCodeRecord.Lad_Ua != null && _currentPostCodeRecord.Lad_Ua.Length > 8)
                    {
                        int LAInt = Convert.ToInt32(_currentPostCodeRecord.Lad_Ua.Substring(2));
                        if (LAInt >= 9000001 && LAInt <= 9000033)
                        {
                            _currentPostCodeRecord.CountyName = "Greater London";
                        }
                        else if (LAInt >= 8000001 && LAInt <= 8000010)
                        {
                            _currentPostCodeRecord.CountyName = "Greater Manchester";
                        }
                        else if (LAInt >= 8000011 && LAInt <= 8000015)
                        {
                            _currentPostCodeRecord.CountyName = "Merseyside";
                        }
                        else if (LAInt >= 8000016 && LAInt <= 8000019)
                        {
                            _currentPostCodeRecord.CountyName = "South Yorkshire";
                        }
                        else if (LAInt >= 8000020 && LAInt <= 8000024)
                        {
                            _currentPostCodeRecord.CountyName = "Tyne and Wear";
                        }
                        else if (LAInt >= 8000025 && LAInt <= 8000031)
                        {
                            _currentPostCodeRecord.CountyName = "West Midlands";
                        }
                        else if (LAInt >= 8000032 && LAInt <= 8000036)
                        {
                            _currentPostCodeRecord.CountyName = "West Yorkshire";
                        }
                        else
                        {
                            switch (LAInt)
                            {
                                case 6000032:
                                case 6000055:
                                case 6000056:
                                    _currentPostCodeRecord.CountyName = "Bedfordshire";
                                    break;
                                case 6000036:
                                case 6000037:
                                case 6000038:
                                case 6000039:
                                case 6000040:
                                case 6000041:
                                    _currentPostCodeRecord.CountyName = "Berkshire";
                                    break;
                                case 6000023:
                                    _currentPostCodeRecord.CountyName = "Bristol, City of";
                                    break;
                                case 6000042:
                                case 7000004:
                                    _currentPostCodeRecord.CountyName = "Buckinghamshire";
                                    break;
                                case 6000031:
                                    _currentPostCodeRecord.CountyName = "Cambridgeshire";
                                    break;
                                case 6000006:
                                case 6000007:
                                case 6000049:
                                case 6000050:
                                    _currentPostCodeRecord.CountyName = "Cheshire";
                                    break;
                                case 6000052:
                                case 6000053:
                                    _currentPostCodeRecord.CountyName = "Cornwall";
                                    break;
                                case 6000015:
                                    _currentPostCodeRecord.CountyName = "Derbyshire";
                                    break;
                                case 6000026:
                                case 6000027:
                                    _currentPostCodeRecord.CountyName = "Devon";
                                    break;
                                case 6000028:
                                case 6000029:
                                    _currentPostCodeRecord.CountyName = "Dorset";
                                    break;
                                case 6000001:
                                case 6000004:
                                case 6000005:
                                case 6000047:
                                    _currentPostCodeRecord.CountyName = "Durham";
                                    break;
                                case 6000010:
                                case 6000011:
                                    _currentPostCodeRecord.CountyName = "East Riding of Yorkshire";
                                    break;
                                case 6000043:
                                    _currentPostCodeRecord.CountyName = "East Sussex";
                                    break;
                                case 6000033:
                                case 6000034:
                                    _currentPostCodeRecord.CountyName = "Essex";
                                    break;
                                case 6000025:
                                    _currentPostCodeRecord.CountyName = "Gloucestershire";
                                    break;
                                case 6000044:
                                case 6000045:
                                    _currentPostCodeRecord.CountyName = "Hampshire";
                                    break;
                                case 6000019:
                                    _currentPostCodeRecord.CountyName = "Herefordshire";
                                    break;
                                case 6000046:
                                    _currentPostCodeRecord.CountyName = "Isle of Wight";
                                    break;
                                case 6000035:
                                    _currentPostCodeRecord.CountyName = "Kent";
                                    break;
                                case 6000008:
                                case 6000009:
                                    _currentPostCodeRecord.CountyName = "Lancashire";
                                    break;
                                case 6000016:
                                    _currentPostCodeRecord.CountyName = "Leicestershire";
                                    break;
                                case 6000012:
                                case 6000013:
                                    _currentPostCodeRecord.CountyName = "Lincolnshire";
                                    break;
                                case 6000002:
                                case 6000003:
                                case 6000014:
                                    _currentPostCodeRecord.CountyName = "North Yorkshire";
                                    break;
                                case 6000048:
                                    _currentPostCodeRecord.CountyName = "Northumberland";
                                    break;
                                case 6000018:
                                    _currentPostCodeRecord.CountyName = "Nottinghamshire";
                                    break;
                                case 6000017:
                                    _currentPostCodeRecord.CountyName = "Rutland";
                                    break;
                                case 6000020:
                                case 6000051:
                                    _currentPostCodeRecord.CountyName = "Shropshire";
                                    break;
                                case 6000022:
                                case 6000024:
                                    _currentPostCodeRecord.CountyName = "Somerset";
                                    break;
                                case 6000021:
                                    _currentPostCodeRecord.CountyName = "Staffordshire";
                                    break;
                                case 6000030:
                                case 6000054:
                                    _currentPostCodeRecord.CountyName = "Wiltshire";
                                    break;
                            }
                            if (LAInt == 6000004)
                            {
                                //Stockton is split between north yorkshire and durham
                                //change the ny wards from durham
                                switch (_currentPostCodeRecord.Ward)
                                {
                                    case "E05001538":
                                    case "E05001539":
                                    case "E05001540":
                                    case "E05001548":
                                    case "E05001550":
                                    case "E05001552":
                                        _currentPostCodeRecord.CountyName = "North Yorkshire";
                                        break;
                                }
                            }
                        }
                    }
                }
                if (_currentPostCodeRecord.CountyName == null)
                {
                    if (_currentPostCodeRecord.PostCode7.Substring(0, 2) == "GY")
                    {
                        _currentPostCodeRecord.CountyName = "Guernsey";
                    }
                    else if (_currentPostCodeRecord.PostCode7.Substring(0, 2) == "IM")
                    {
                        _currentPostCodeRecord.CountyName = "Isle of Man";
                    }
                    else if (_currentPostCodeRecord.PostCode7.Substring(0, 2) == "JE")
                    {
                        _currentPostCodeRecord.CountyName = "Jersey";
                    }
                }

            }
            _recordCount++;
            return true;
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
            get { return 18; }
        }

        public object GetValue(int i)
        {
            switch (i)
            {
                case 0:
                    return DBNullIfEmptyString(_currentPostCodeRecord.PostCode7);

                case 1:
                    return DBNullIfEmptyString(_currentPostCodeRecord.PostCode8);

                case 2:
                    return DBNullIfEmptyString(_currentPostCodeRecord.County);
                case 3:
                    return DBNullIfEmptyString(_currentPostCodeRecord.Lad_Ua);
                case 4:
                    return DBNullIfEmptyString(_currentPostCodeRecord.Ward);
                case 5:
                    return DBNullIfEmptyString(_currentPostCodeRecord.Parl_Con);
                case 6:
                    return DBNullIfEmptyString(_currentPostCodeRecord.Lea);
                case 7:
                    return DBNullIfEmptyString(_currentPostCodeRecord.ConstituencyID);
                case 8:
                    return DBNullIfEmptyString(_currentPostCodeRecord.Nuts1);
                case 9:
                    return DBNullIfEmptyString(_currentPostCodeRecord.Easting);
                case 10:
                    return DBNullIfEmptyString(_currentPostCodeRecord.Northinging);
                case 11:
                    return DBNullIfEmptyString(_currentPostCodeRecord.PostCodeCompressed);
                case 12:
                    return DBNullIfEmptyString(_currentPostCodeRecord.CountyName);
                case 13:
                    return DBNullIfEmptyString(_currentPostCodeRecord.LEAName);
                case 14:
                    return DBNullIfEmptyString(_currentPostCodeRecord.WardName);
                case 15:
                    return DBNullIfEmptyString(_currentPostCodeRecord.ConstituencyName);
                case 16:
                    return DBNullIfEmptyString(_currentPostCodeRecord.LocalAuthorityName);
                case 17:
                    return DBNullIfEmptyString(_currentPostCodeRecord.RegionName);
                
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
                case "POSTCODE7":
                    return 0;

                case "POSTCODE8":
                    return 1;

                case "COUNTY":
                    return 2;
                case "LAD_UA":
                    return 3;
                case "WARD":
                    return 4;
                case "PARL_CON":
                    return 5;
                case "LEA":
                    return 6;
                case "CONSTITUENCYID":
                    return 7;
                case "NUTS1":
                    return 8;
                case "Easting":
                    return 9;
                case "Northinging":
                    return 10;
                case "PostCodeCompressed":
                    return 11;
                case "CountyName":
                    return 12;
                case "LEAName":
                    return 13;
                case "WardName":
                    return 14;
                case "ConstituencyName":
                    return 15;
                case "LocalAuthorityName":
                    return 16;
                case "RegionName":
                    return 17;
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