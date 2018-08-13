using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows;
//using System.Windows.Forms;

namespace UcbManagementInformation.Helpers
{
    public class FileProgressChangedEventArgs : EventArgs
    {
        public FileProgressChangedEventArgs(double percentComplete)
        {
            _percentComplete = percentComplete;
        }
        private double _percentComplete;

        public double PercentComplete
        {
            get { return _percentComplete; }
            set { _percentComplete = value; }
        }
    }

    public class ParticipantFile
    {
        public event FileProgressChangedEventHandler FileProgressChanged;

        public delegate void FileProgressChangedEventHandler(object sender, FileProgressChangedEventArgs a);

        protected virtual void OnFileProgressChanged(FileProgressChangedEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            FileProgressChangedEventHandler handler = FileProgressChanged;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, e);
            }
        }
        private double _lastPercentThrough;

        private Participant _previousParticipantRecord;
        private Participant _currentParticipantRecord;
        private Stream _xmlFileStream;

        public Stream XmlFileStream
        {
            get { return _xmlFileStream; }
            set { _xmlFileStream = value; }
        }
        private Stream _csvFileStream;

        public Stream CsvFileStream
        {
            get { return _csvFileStream; }
            set { _csvFileStream = value; }
        }
        private TextReader _csvTextReader;
        private XmlWriter _xmlWriter;
        private TextWriter _csvTextWriter;
        private XmlReader _xmlReader;

        private bool _isHeaderRow;
        private string _delimiter;
        private bool _is1Written;
        private bool _isP1P4Open;
        private bool _isP2P5Open;
        private bool _isLeaverOpen;

        private long _inputFileLength;
        
        public ParticipantFile(Stream csvFileStream,Stream xmlFileStream, string delimiter, bool isHeaderRow)
        {
            _csvFileStream = csvFileStream;
            _xmlFileStream = xmlFileStream;
            _isHeaderRow = isHeaderRow;
            _delimiter = delimiter;

            
                
        }
        
        public ParticipantFile()
        {
        }

        public void WriteXml()
        {
            if (_csvFileStream != null && _xmlFileStream != null)
            {
                _inputFileLength = _csvFileStream.Length;
                _lastPercentThrough = 0;
                //FileStream CsvStream = File.Open(_csvFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                _csvTextReader = new StreamReader(_csvFileStream);
                //FileStream XmlStream = File.Open(_xmlFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                XmlWriterSettings Settings = new XmlWriterSettings();
                Settings.Indent = true;
                _xmlWriter = XmlWriter.Create(_xmlFileStream, Settings);// Settings);
                _xmlWriter.WriteStartDocument();
                _xmlWriter.WriteStartElement("ParticipantInformation");
                _xmlWriter.WriteAttributeString("SchemaVersion", "1.0");
                //while (CsvStream.Position < CsvStream.Length)
                //{
                string Record;
                while ((Record = _csvTextReader.ReadLine()) != null)
                {
                    double percentThrough = Convert.ToDouble(_csvFileStream.Position) / Convert.ToDouble(_inputFileLength) * 100;
                    if (percentThrough - _lastPercentThrough > 1)
                    {
                        OnFileProgressChanged(new FileProgressChangedEventArgs(percentThrough));
                        _lastPercentThrough = percentThrough;
                    }
                    ProcessRecord(Record);
                }
                if (_isP1P4Open)
                {
                    _xmlWriter.WriteEndElement();//P1P4Leaver
                    _isP1P4Open = false;
                }
                if (_isP2P5Open)
                {
                    _xmlWriter.WriteEndElement();//P2P5Leaver
                    _isP2P5Open = false;
                }
                if (_isLeaverOpen)
                {
                    _xmlWriter.WriteEndElement();//Leaver
                    _isLeaverOpen = false;
                }
                //Assess Key Differences
                int SameToLevel = 2;
                for (int i = SameToLevel; i < 6; i++)
                {
                    _xmlWriter.WriteEndElement(); //Key value where different
                }
           
                _xmlWriter.WriteEndElement(); //ParticipantInformation
                _xmlWriter.WriteEndDocument();
                //App.Current.RootVisual.Dispatcher.BeginInvoke(() =>
                // {
                     _xmlWriter.Flush();
                     //_csvFileStream.Close();
                     //_xmlFileStream.Close();
                 //}
                 //);
                OnFileProgressChanged(new FileProgressChangedEventArgs(100));
            }
        }
        public void WriteCsv()
        {
            _inputFileLength = _xmlFileStream.Length;
            _lastPercentThrough = 0;
            //FileStream CsvStream = File.Open(_csvFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            _csvTextWriter = new StreamWriter(_csvFileStream);

            //FileStream XmlStream = File.Open(_xmlFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlReaderSettings Settings = new XmlReaderSettings();
            _xmlReader = XmlReader.Create(_xmlFileStream, Settings);
            if (_isHeaderRow)
            {
                WriteCsvHeaderRecord();
            }
            _currentParticipantRecord = new Participant();

            while (_xmlReader.Read())
            {
                double percentThrough = Convert.ToDouble(_xmlFileStream.Position) / Convert.ToDouble(_inputFileLength) * 100;
                if (percentThrough - _lastPercentThrough > 1)
                {
                    OnFileProgressChanged(new FileProgressChangedEventArgs(percentThrough));
                    _lastPercentThrough = percentThrough;
                }    
                if (_xmlReader.IsStartElement())
                {
                    switch (_xmlReader.LocalName)
                    {
                        case "ProvidingOrganisation":
                            _currentParticipantRecord.ProvidingOrganisation = _xmlReader.ReadInnerXml();
                            break;
                        case "FileRunDate":
                            _currentParticipantRecord.FileRunDate = _xmlReader.ReadInnerXml();
                            break;
                        case "RegionCode":
                            _currentParticipantRecord.RegionCode = _xmlReader.ReadInnerXml();
                            break;
                        case "RegionName":
                            _currentParticipantRecord.RegionName = _xmlReader.ReadInnerXml();
                            break;
                        case "AgreementID":
                            _currentParticipantRecord.AgreementID = _xmlReader.ReadInnerXml();
                            break;
                        case "ProviderProjectID":
                            _currentParticipantRecord.ProviderProjectID = _xmlReader.ReadInnerXml();
                            break;
                        case "FundingType":
                            _currentParticipantRecord.FundingType = _xmlReader.ReadInnerXml();
                            break;
                        case "ParticipantReferenceID":
                            _currentParticipantRecord.ParticipantReferenceID = _xmlReader.ReadInnerXml();
                            break;
                        case "StartDate":
                            _currentParticipantRecord.StartDate = _xmlReader.ReadInnerXml();
                            break;
                        case "Gender":
                            _currentParticipantRecord.Gender = _xmlReader.ReadInnerXml();
                            break;
                        case "DOB":
                            _currentParticipantRecord.DOB = _xmlReader.ReadInnerXml();
                            break;
                        case "Postcode":
                            _currentParticipantRecord.Postcode = _xmlReader.ReadInnerXml();
                            break;
                        case "EmploymentStatus":
                            _currentParticipantRecord.EmploymentStatus = _xmlReader.ReadInnerXml();
                            break;
                        case "UnemployedMonths":
                            _currentParticipantRecord.UnemployedMonths = _xmlReader.ReadInnerXml();
                            break;
                        case "Ethnicity":
                            _currentParticipantRecord.Ethnicity = _xmlReader.ReadInnerXml();
                            break;
                        case "Disability":
                            _currentParticipantRecord.Disability = _xmlReader.ReadInnerXml();
                            break;
                        case "QualificationHeld":
                            _currentParticipantRecord.QualificationHeld = _xmlReader.ReadInnerXml();
                            break;
                        case "InPostGradResearch":
                            _currentParticipantRecord.InPostGradResearch = _xmlReader.ReadInnerXml();
                            break;
                        case "GraduatePlacedWithSME":
                            _currentParticipantRecord.GraduatePlacedWithSME = _xmlReader.ReadInnerXml();
                            break;
                        case "LeavingDate":
                            _currentParticipantRecord.LeavingDate = _xmlReader.ReadInnerXml();
                            break;
                        case "P1P4LeavingStatus":
                            _currentParticipantRecord.P1P4LeavingStatus = _xmlReader.ReadInnerXml();
                            break;
                        case "P1P4NoQualificationGained":
                            _currentParticipantRecord.P1P4NoQualificationGained = _xmlReader.ReadInnerXml();
                            break;
                        case "P1P4QualificationGained":
                            _currentParticipantRecord.P1P4QualificationGained = _xmlReader.ReadInnerXml();
                            break;
                        case "P2P5IntoEducationOrTraining":
                            _currentParticipantRecord.P2P5IntoEducationOrTraining = _xmlReader.ReadInnerXml();
                            break;
                        case "P5GraduatePlacedWithinSMEWhoGainedEmployment":
                            _currentParticipantRecord.P5GraduatePlacedWithinSMEWhoGainedEmployment = _xmlReader.ReadInnerXml();
                            break;
                        case "P2P5NoQualificationGained":
                            _currentParticipantRecord.P2P5NoQualificationGained = _xmlReader.ReadInnerXml();
                            break;
                        case "P2P5QualificationGained":
                            _currentParticipantRecord.P2P5QualificationGained = _xmlReader.ReadInnerXml();
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
                            WriteCsvRecord("1");
                            //reset flag for next participant
                            _is1Written = false;
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
                       /* case "Leaver":
                            _currentParticipantRecord.LeavingDate = "";
                            _currentParticipantRecord.P1P4LeavingStatus = "";
                            _currentParticipantRecord.P1P4NoQualificationGained = "";
                            _currentParticipantRecord.P1P4QualificationGained = "";
                            _currentParticipantRecord.P2P5IntoEducationOrTraining = "";
                            _currentParticipantRecord.P5GraduatePlacedWithinSMEWhoGainedEmployment = "";
                            _currentParticipantRecord.P2P5NoQualificationGained = "";
                            _currentParticipantRecord.P2P5QualificationGained = "";
                            break;
                        case "P1P4Leaver":
                            _currentParticipantRecord.P1P4LeavingStatus = "";
                            _currentParticipantRecord.P1P4NoQualificationGained = "";
                            _currentParticipantRecord.P1P4QualificationGained = "";
                            break;
                        case "P2P5Leaver":
                            _currentParticipantRecord.P2P5IntoEducationOrTraining = "";
                            _currentParticipantRecord.P5GraduatePlacedWithinSMEWhoGainedEmployment = "";
                            _currentParticipantRecord.P2P5NoQualificationGained = "";
                            _currentParticipantRecord.P2P5QualificationGained = "";
                            break;*/
                        case "ESFP1P4Qualification":
                            WriteCsvRecord("2");
                            _currentParticipantRecord.P1P4QualificationGained = "";
                            break;
                        case "ESFP2P5Qualification":
                            WriteCsvRecord("3");
                            _currentParticipantRecord.P2P5QualificationGained = "";
                            break;
                    }
                }
            }
            //      _xmlReader.Close();
            //      _xmlFileStream.Close();
                
            //App.Current.RootVisual.Dispatcher.BeginInvoke(()=>
            //    {
                   _csvTextWriter.Flush();
            //       _csvFileStream.Close();
            //    });

            OnFileProgressChanged(new FileProgressChangedEventArgs(100));
        }
        private void WriteCsvHeaderRecord()
        {
            StringBuilder CsvLine = new StringBuilder();
            CsvLine.Append("FileType" + _delimiter + "ProvidingOrganisation");
            CsvLine.Append(_delimiter + "FileRunDate");
            CsvLine.Append(_delimiter + "RegionCode");
            CsvLine.Append(_delimiter + "RegionName");
            CsvLine.Append(_delimiter + "AgreementID");
            CsvLine.Append(_delimiter + "ProviderProjectID");
            CsvLine.Append(_delimiter + "FundingType");
            CsvLine.Append(_delimiter + "ParticipantReferenceID");
            CsvLine.Append(_delimiter + "StartDate");
            CsvLine.Append(_delimiter + "Gender");
            CsvLine.Append(_delimiter + "DOB");
            CsvLine.Append(_delimiter + "Postcode");
            CsvLine.Append(_delimiter + "EmploymentStatus");
            CsvLine.Append(_delimiter + "UnemployedMonths");
            CsvLine.Append(_delimiter + "Ethnicity");
            CsvLine.Append(_delimiter + "Disability");
            CsvLine.Append(_delimiter + "QualificationHeld");
            CsvLine.Append(_delimiter + "InPostGradResearch");
            CsvLine.Append(_delimiter + "GraduatePlacedWithSME");
            CsvLine.Append(_delimiter + "LeavingDate");
            CsvLine.Append(_delimiter + "P1P4LeavingStatus");
            CsvLine.Append(_delimiter + "P1P4NoQualificationGained");
            CsvLine.Append(_delimiter + "P1P4QualificationGained");
            CsvLine.Append(_delimiter + "P2P5IntoEducationOrTraining");
            CsvLine.Append(_delimiter + "P5GraduatePlacedWithinSMEWhoGainedEmployment");
            CsvLine.Append(_delimiter + "P2P5NoQualificationGained");
            CsvLine.Append(_delimiter + "P2P5QualificationGained");
            _csvTextWriter.WriteLine(CsvLine.ToString());
        }
        private void WriteCsvRecord(string RecordType)
        {
            StringBuilder CsvLine = new StringBuilder();
                
            switch (RecordType)
            {
                case "1":
                    if (!_is1Written)
                    {
                        CsvLine.Append("1" + _delimiter + _currentParticipantRecord.ProvidingOrganisation);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.FileRunDate);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.RegionCode);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.RegionName);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.AgreementID);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.ProviderProjectID);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.FundingType);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.ParticipantReferenceID);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.StartDate);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.Gender);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.DOB);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.Postcode);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.EmploymentStatus);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.UnemployedMonths);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.Ethnicity);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.Disability);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.QualificationHeld);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.InPostGradResearch);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.GraduatePlacedWithSME);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.LeavingDate);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P1P4LeavingStatus);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P1P4NoQualificationGained);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P1P4QualificationGained);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P2P5IntoEducationOrTraining);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P5GraduatePlacedWithinSMEWhoGainedEmployment);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P2P5NoQualificationGained);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P2P5QualificationGained);
                        _csvTextWriter.WriteLine(CsvLine.ToString());
                        
                    }
                    break;
                case "2":
                    if (!_is1Written)
                    {
                        CsvLine.Append("1" + _delimiter + _currentParticipantRecord.ProvidingOrganisation);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.FileRunDate);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.RegionCode);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.RegionName);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.AgreementID);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.ProviderProjectID);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.FundingType);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.ParticipantReferenceID);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.StartDate);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.Gender);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.DOB);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.Postcode);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.EmploymentStatus);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.UnemployedMonths);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.Ethnicity);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.Disability);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.QualificationHeld);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.InPostGradResearch);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.GraduatePlacedWithSME);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.LeavingDate);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P1P4LeavingStatus);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P1P4NoQualificationGained);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P1P4QualificationGained);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P2P5IntoEducationOrTraining);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P5GraduatePlacedWithinSMEWhoGainedEmployment);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P2P5NoQualificationGained);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P2P5QualificationGained);
                        _csvTextWriter.WriteLine(CsvLine.ToString());
                        _is1Written = true;
                    }
                    else
                    {
                        CsvLine.Append("2" + _delimiter + _currentParticipantRecord.ProvidingOrganisation);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.FileRunDate);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.RegionCode);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.RegionName);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.AgreementID);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.ProviderProjectID);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.FundingType);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.ParticipantReferenceID);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.StartDate);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.Gender);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.DOB);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.Postcode);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.EmploymentStatus);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.UnemployedMonths);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.Ethnicity);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.Disability);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.QualificationHeld);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.InPostGradResearch);
                        CsvLine.Append(_delimiter);//+ _currentParticipantRecord.GraduatePlacedWithSME);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.LeavingDate);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.P1P4LeavingStatus);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.P1P4NoQualificationGained);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P1P4QualificationGained);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.P2P5IntoEducationOrTraining);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.P5GraduatePlacedWithinSMEWhoGainedEmployment);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.P2P5NoQualificationGained);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.P2P5QualificationGained);
                        _csvTextWriter.WriteLine(CsvLine.ToString());
                    }
                    break;
                case "3":
                    if (!_is1Written)
                    {
                        CsvLine.Append("1" + _delimiter + _currentParticipantRecord.ProvidingOrganisation);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.FileRunDate);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.RegionCode);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.RegionName);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.AgreementID);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.ProviderProjectID);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.FundingType);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.ParticipantReferenceID);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.StartDate);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.Gender);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.DOB);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.Postcode);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.EmploymentStatus);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.UnemployedMonths);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.Ethnicity);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.Disability);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.QualificationHeld);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.InPostGradResearch);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.GraduatePlacedWithSME);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.LeavingDate);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P1P4LeavingStatus);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P1P4NoQualificationGained);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P1P4QualificationGained);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P2P5IntoEducationOrTraining);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P5GraduatePlacedWithinSMEWhoGainedEmployment);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P2P5NoQualificationGained);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P2P5QualificationGained);
                        _csvTextWriter.WriteLine(CsvLine.ToString());
                        _is1Written = true;
                    }
                    else
                    {
                        CsvLine.Append("3" + _delimiter + _currentParticipantRecord.ProvidingOrganisation);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.FileRunDate);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.RegionCode);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.RegionName);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.AgreementID);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.ProviderProjectID);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.FundingType);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.ParticipantReferenceID);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.StartDate);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.Gender);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.DOB);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.Postcode);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.EmploymentStatus);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.UnemployedMonths);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.Ethnicity);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.Disability);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.QualificationHeld);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.InPostGradResearch);
                        CsvLine.Append(_delimiter);//+ _currentParticipantRecord.GraduatePlacedWithSME);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.LeavingDate);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.P1P4LeavingStatus);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.P1P4NoQualificationGained);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.P1P4NoQualificationGained);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.P2P5IntoEducationOrTraining);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.P5GraduatePlacedWithinSMEWhoGainedEmployment);
                        CsvLine.Append(_delimiter);// + _currentParticipantRecord.P2P5NoQualificationGained);
                        CsvLine.Append(_delimiter + _currentParticipantRecord.P2P5QualificationGained);
                        _csvTextWriter.WriteLine(CsvLine.ToString());
                    }
                    break;
                }
            
        }
        private void ProcessRecord(string record)
        {
            string[] RecordFields = record.Split(Convert.ToChar(_delimiter));
            
            switch (RecordFields[0])//Record Type 1)Participant 2)P1P4Qual 3)P2P5Qual
            {
                case "1":
                     _currentParticipantRecord = new Participant(RecordFields);
                    ProcessParticipantRecord();
                   _previousParticipantRecord = _currentParticipantRecord;
                    break;
                case "2":
                    ProcessParticipantP1P4QualificationRecord(RecordFields);
                    break;
                case "3":
                    ProcessParticipantP2P5QualificationRecord(RecordFields);
                    break;
                case "FileType":
                    break;
            }
        }
        private void ProcessParticipantP1P4QualificationRecord(string[] recordFields)
        {
            if (_isP1P4Open)
            {
                if (_isP2P5Open)
                {
                    MessageBox.Show("P2P5 leaver information is already on this record. P1P4 Info" +
                    " will not be written", "Priority Leaver Information Warning", MessageBoxButton.OK);
                }
                else
                {
                    Participant P1P4Record = new Participant(recordFields);
                    _xmlWriter.WriteStartElement("ESFP1P4Qualification");
                    WriteElementStringIfNotBlank("P1P4QualificationGained", P1P4Record.P1P4QualificationGained);
                    _xmlWriter.WriteEndElement(); //ESFP1P4Qualification
                }
            }
            else
            {
                MessageBox.Show("P1P4 leaver information was not on the associated type 1 record consequently P1P4 Info" +
                                  " will not be written", "Priority Leaver Information Warning", MessageBoxButton.OK);
            }
        }
        private void ProcessParticipantP2P5QualificationRecord(string[] recordFields)
        {
            if (_isP2P5Open)
            {
                if (_isP1P4Open)
                {
                    MessageBox.Show("P1P4 leaver information is already on this record. P2P5 Info" +
                    " will not be written", "Priority Leaver Information Warning", MessageBoxButton.OK);
                }
                else
                {
                    Participant P2P5Record = new Participant(recordFields);
                    _xmlWriter.WriteStartElement("ESFP2P5Qualification");
                    WriteElementStringIfNotBlank("P2P5QualificationGained", P2P5Record.P2P5QualificationGained);
                    _xmlWriter.WriteEndElement(); //ESFP2P5Qualification
                }
            }
            else
            {
                MessageBox.Show("P2P5 leaver information was not on the associated type 1 record consequently P2P5 Info" +
                                  " will not be written", "Priority Leaver Information Warning", MessageBoxButton.OK);
            }
        }
        private void ProcessParticipantRecord()
        {
            if (_isP1P4Open)
            {
                _xmlWriter.WriteEndElement();//P1P4Leaver
                _isP1P4Open = false;
            }
            if (_isP2P5Open)
            {
                _xmlWriter.WriteEndElement();//P2P5Leaver
                _isP2P5Open = false;
            }
            if (_isLeaverOpen)
            {
                _xmlWriter.WriteEndElement();//Leaver
                _isLeaverOpen = false;
            }
            //Assess Key Differences
            int SameToLevel = AssessKeyDifferences();
            if (SameToLevel != -1)
            {
                for (int i = SameToLevel; i < 6; i++)
                {
                    _xmlWriter.WriteEndElement(); //Key value where different
                }
            }
            else
            {
                WriteElementStringIfNotBlank("ProvidingOrganisation", _currentParticipantRecord.ProvidingOrganisation);
                WriteElementStringIfNotBlank("FileRunDate", _currentParticipantRecord.FileRunDate);
                SameToLevel = 2;
            }
            for (int j = SameToLevel; j < 6; j++)
            {
                switch (j)
                {
                    
                    case 2:
                        _xmlWriter.WriteStartElement("Region");
                        WriteElementStringIfNotBlank("RegionCode", _currentParticipantRecord.RegionCode);
                        WriteElementStringIfNotBlank("RegionName", _currentParticipantRecord.RegionName);
                        break;
                    case 3:
                        _xmlWriter.WriteStartElement("Agreement");
                        WriteElementStringIfNotBlank("AgreementID",_currentParticipantRecord.AgreementID);
                        break;
                    case 4:
                        _xmlWriter.WriteStartElement("ProviderProjectReference");
                        WriteElementStringIfNotBlank("ProviderProjectID", _currentParticipantRecord.ProviderProjectID);
                        WriteElementStringIfNotBlank("FundingType", _currentParticipantRecord.FundingType);
                        break;
                    case 5:
                        _xmlWriter.WriteStartElement("Participant");
                        WriteElementStringIfNotBlank("ParticipantReferenceID", _currentParticipantRecord.ParticipantReferenceID);
                        WriteElementStringIfNotBlank("StartDate",_currentParticipantRecord.StartDate);
                        WriteElementStringIfNotBlank("Gender",_currentParticipantRecord.Gender);
                        WriteElementStringIfNotBlank("DOB",_currentParticipantRecord.DOB);
                        WriteElementStringIfNotBlank("Postcode",_currentParticipantRecord.Postcode);
                        WriteElementStringIfNotBlank("EmploymentStatus",_currentParticipantRecord.EmploymentStatus);
                        WriteElementStringIfNotBlank("UnemployedMonths",_currentParticipantRecord.UnemployedMonths);
                        WriteElementStringIfNotBlank("Ethnicity",_currentParticipantRecord.Ethnicity);
                        WriteElementStringIfNotBlank("Disability",_currentParticipantRecord.Disability);
                        WriteElementStringIfNotBlank("QualificationHeld",_currentParticipantRecord.QualificationHeld);
                        WriteElementStringIfNotBlank("InPostGradResearch",_currentParticipantRecord.InPostGradResearch);
                        WriteElementStringIfNotBlank("GraduatePlacedWithSME",_currentParticipantRecord.GraduatePlacedWithSME);
                        if (_currentParticipantRecord.LeavingDate != "" ||
                            _currentParticipantRecord.P2P5IntoEducationOrTraining != "" ||
                            _currentParticipantRecord.P5GraduatePlacedWithinSMEWhoGainedEmployment != "" ||
                            _currentParticipantRecord.P2P5NoQualificationGained != "" ||
                            _currentParticipantRecord.P2P5QualificationGained != "" ||
                            _currentParticipantRecord.P1P4LeavingStatus != "" ||
                            _currentParticipantRecord.P1P4NoQualificationGained != "" ||
                            _currentParticipantRecord.P1P4QualificationGained != ""
                            )
                        {
                            _xmlWriter.WriteStartElement("Leaver");
                            _isLeaverOpen = true;
                            WriteElementStringIfNotBlank("LeavingDate", _currentParticipantRecord.LeavingDate);
                            if (_currentParticipantRecord.P2P5IntoEducationOrTraining != "" ||
                            _currentParticipantRecord.P5GraduatePlacedWithinSMEWhoGainedEmployment != "" ||
                            _currentParticipantRecord.P2P5NoQualificationGained != "" ||
                            _currentParticipantRecord.P2P5QualificationGained != ""
                            )
                            {
                                if (_isP1P4Open)
                                {
                                    MessageBox.Show("P1P4 leaver information is already on this record. P2P5 Info" +
                                    " will not be written", "Priority Leaver Information Warning", MessageBoxButton.OK);
                                }
                                else
                                {
                                    _isP2P5Open = true;
                                    _xmlWriter.WriteStartElement("P2P5Leaver");
                                    WriteElementStringIfNotBlank("P2P5IntoEducationOrTraining", _currentParticipantRecord.P2P5IntoEducationOrTraining);
                                    WriteElementStringIfNotBlank("P5GraduatePlacedWithinSMEWhoGainedEmployment", _currentParticipantRecord.P5GraduatePlacedWithinSMEWhoGainedEmployment);
                                    if (_currentParticipantRecord.P2P5NoQualificationGained != "")
                                    {
                                        WriteElementStringIfNotBlank("P2P5NoQualificationGained", _currentParticipantRecord.P2P5NoQualificationGained);
                                    }
                                    if (_currentParticipantRecord.P2P5QualificationGained != "")
                                    {
                                        _xmlWriter.WriteStartElement("ESFP2P5Qualification");
                                        WriteElementStringIfNotBlank("P2P5QualificationGained", _currentParticipantRecord.P2P5QualificationGained);
                                        _xmlWriter.WriteEndElement(); //ESFP2P5Qualification

                                    }
                                }
                            }
                            if (_currentParticipantRecord.P1P4LeavingStatus != "" ||
                            _currentParticipantRecord.P1P4NoQualificationGained != "" ||
                            _currentParticipantRecord.P1P4QualificationGained != "")
                            {
                                if (_isP2P5Open)
                                {
                                    MessageBox.Show("P2P5 leaver information is already on this record. P1P4 info" +
                                    " will not be written", "Priority Leaver Information Warning", MessageBoxButton.OK);
                                }
                                else
                                {
                                    _isP1P4Open = true;
                                    _xmlWriter.WriteStartElement("P1P4Leaver");
                                    WriteElementStringIfNotBlank("P1P4LeavingStatus", _currentParticipantRecord.P1P4LeavingStatus);
                                    if (_currentParticipantRecord.P1P4NoQualificationGained != "")
                                    {
                                        WriteElementStringIfNotBlank("P1P4NoQualificationGained", _currentParticipantRecord.P1P4NoQualificationGained);
                                    }
                                    if (_currentParticipantRecord.P1P4QualificationGained != ""
                                        )
                                    {
                                        _xmlWriter.WriteStartElement("ESFP1P4Qualification");
                                        WriteElementStringIfNotBlank("P1P4QualificationGained", _currentParticipantRecord.P1P4QualificationGained);
                                        _xmlWriter.WriteEndElement();//ESFP1P4Qualification

                                    }
                                }
                            }
                            

                        }
                        break;
                }
            }

        }

        private int AssessKeyDifferences()
        {
            if (_previousParticipantRecord != null)
            {
                for (int i = 2; i < 6; i++)
                {
                    if (_currentParticipantRecord.KeyValues[i] != _previousParticipantRecord.KeyValues[i])
                    {
                        return i;
                    }
                }
            }
            else
            {
                return -1;
            }
            return 5;
        }
        private void WriteElementStringIfNotBlank(string localName, string ns, string Value)
        {
            if (Value == "" || Value == null)
            { }
            else
            {
                if (ns == null)
                {
                    _xmlWriter.WriteElementString(localName, Value);
                }
                else
                {
                    _xmlWriter.WriteElementString(localName, ns, Value);
                }
            }
        }
        private void WriteElementStringIfNotBlank(string localName, string Value)
        {
            WriteElementStringIfNotBlank(localName, null, Value);
        }
    }
}
