using System;
using System.Collections.Generic;
using System.Text;

namespace UcbManagementInformation.Web.Models
{
    public class Participant
    {
        private string _recordType;

        public string RecordType
        {
            get { return _recordType; }
            set { _recordType = value; }
        }
        private int _participantCode;

        public int ParticipantCode
        {
            get { return _participantCode; }
            set { _participantCode = value; }
        }
        private string _providingOrganisation;

        public string ProvidingOrganisation
        {
            get { return _providingOrganisation; }
            set { _providingOrganisation = value; }
        }
        private string _fileRunDate;

        public string FileRunDate
        {
            get { return _fileRunDate; }
            set { _fileRunDate = value; }
        }
        private string _regionCode;

        public string RegionCode
        {
            get { return _regionCode; }
            set { _regionCode = value; }
        }
        private string _regionName;

        public string RegionName
        {
            get { return _regionName; }
            set { _regionName = value; }
        }
        private string _agreementID;

        public string AgreementID
        {
            get { return _agreementID; }
            set { _agreementID = value; }
        }
        private string _providerProjectID;

        public string ProviderProjectID
        {
            get { return _providerProjectID; }
            set { _providerProjectID = value; }
        }
        private string _fundingType;

        public string FundingType
        {
            get { return _fundingType; }
            set { _fundingType = value; }
        }
        private string _participantReferenceID;

        public string ParticipantReferenceID
        {
            get { return _participantReferenceID; }
            set { _participantReferenceID = value; }
        }
        private string _startDate;

        public string StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }
        private string _gender;

        public string Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }
        private string _dOB;

        public string DOB
        {
            get { return _dOB; }
            set { _dOB = value; }
        }
        private string _postcode;

        public string Postcode
        {
            get { return _postcode; }
            set { _postcode = value; }
        }
        private string _employmentStatus;

        public string EmploymentStatus
        {
            get { return _employmentStatus; }
            set { _employmentStatus = value; }
        }
        private string _unemployedMonths;

        public string UnemployedMonths
        {
            get { return _unemployedMonths; }
            set { _unemployedMonths = value; }
        }
        private string _ethnicity;

        public string Ethnicity
        {
            get { return _ethnicity; }
            set { _ethnicity = value; }
        }
        private string _disability;

        public string Disability
        {
            get { return _disability; }
            set { _disability = value; }
        }
        private string _qualificationHeld;

        public string QualificationHeld
        {
            get { return _qualificationHeld; }
            set { _qualificationHeld = value; }
        }
        private string _inPostGradResearch;

        public string InPostGradResearch
        {
            get { return _inPostGradResearch; }
            set { _inPostGradResearch = value; }
        }
        private string _graduatePlacedWithSME;

        public string GraduatePlacedWithSME
        {
            get { return _graduatePlacedWithSME; }
            set { _graduatePlacedWithSME = value; }
        }
        private string _leavingDate;

        public string LeavingDate
        {
            get { return _leavingDate; }
            set { _leavingDate = value; }
        }
        private string _p1P4LeavingStatus;

        public string P1P4LeavingStatus
        {
            get { return _p1P4LeavingStatus; }
            set { _p1P4LeavingStatus = value; }
        }
        private string _p1P4NoQualificationGained;

        public string P1P4NoQualificationGained
        {
            get { return _p1P4NoQualificationGained; }
            set { _p1P4NoQualificationGained = value; }
        }
        private string _p1P4QualificationGained;

        public string P1P4QualificationGained
        {
            get { return _p1P4QualificationGained; }
            set { _p1P4QualificationGained = value; }
        }
        private string _p2P5IntoEducationOrTraining;

        public string P2P5IntoEducationOrTraining
        {
            get { return _p2P5IntoEducationOrTraining; }
            set { _p2P5IntoEducationOrTraining = value; }
        }

        private string _p5GraduatePlacedWithinSMEWhoGainedEmployment;
        
        public string P5GraduatePlacedWithinSMEWhoGainedEmployment
        {
            get { return _p5GraduatePlacedWithinSMEWhoGainedEmployment; }
            set { _p5GraduatePlacedWithinSMEWhoGainedEmployment = value; }
        }

        private string _p2P5NoQualificationGained;
        
        public string P2P5NoQualificationGained
        {
            get { return _p2P5NoQualificationGained; }
            set { _p2P5NoQualificationGained = value; }
        }


        private string _p2P5QualificationGained;

        public string P2P5QualificationGained
        {
            get { return _p2P5QualificationGained; }
            set { _p2P5QualificationGained = value; }
        }
        public string[] KeyValues
        {
            get 
            {
                return new string[] 
                {
                    _recordType,
                    _providingOrganisation+_fileRunDate,
                    _regionCode,
                    _agreementID,
                    _providerProjectID,
                    _participantReferenceID+_startDate
                };
            }
        }
        public Participant()
        {
            _recordType = "";
            _providingOrganisation = "";
            _fileRunDate = "";
            _regionCode = "";
            _regionName = "";
            _agreementID = "";
            _providerProjectID = "";
            _fundingType = "";
            _participantReferenceID = "";
            _startDate = "";
            _gender = "";
            _dOB = "";
            _postcode = "";
            _employmentStatus = "";
            _unemployedMonths = "";
            _ethnicity = "";
            _disability = "";
            _qualificationHeld = "";
            _inPostGradResearch = "";
            _graduatePlacedWithSME = "";
            _leavingDate = "";
            _p1P4LeavingStatus = "";
            _p1P4NoQualificationGained = "";
            _p1P4QualificationGained = "";
            _p2P5IntoEducationOrTraining = "";
            _p5GraduatePlacedWithinSMEWhoGainedEmployment = "";
            _p2P5NoQualificationGained = "";
            _p2P5QualificationGained = "";
        }
        public Participant(Participant participantToClone)
        {
            _recordType = participantToClone.RecordType;
            _participantCode = participantToClone.ParticipantCode;
            _providingOrganisation = participantToClone.ProvidingOrganisation;
            _fileRunDate = participantToClone.FileRunDate;
            _regionCode = participantToClone.RegionCode;
            _regionName = participantToClone.RegionName;
            _agreementID = participantToClone.AgreementID;
            _providerProjectID = participantToClone.ProviderProjectID;
            _fundingType = participantToClone.FundingType;
            _participantReferenceID = participantToClone.ParticipantReferenceID;
            _startDate = participantToClone.StartDate;
            _gender = participantToClone.Gender;
            _dOB = participantToClone.DOB;
            _postcode = participantToClone.Postcode;
            _employmentStatus = participantToClone.EmploymentStatus;
            _unemployedMonths = participantToClone.UnemployedMonths;
            _ethnicity = participantToClone.Ethnicity;
            _disability = participantToClone.Disability;
            _qualificationHeld = participantToClone.QualificationHeld;
            _inPostGradResearch = participantToClone.InPostGradResearch;
            _graduatePlacedWithSME = participantToClone.GraduatePlacedWithSME;
            _leavingDate = participantToClone.LeavingDate;
            _p1P4LeavingStatus = participantToClone.P1P4LeavingStatus;
            _p1P4NoQualificationGained = participantToClone.P1P4NoQualificationGained;
            _p1P4QualificationGained = participantToClone.P1P4QualificationGained;
            _p2P5IntoEducationOrTraining = participantToClone.P2P5IntoEducationOrTraining;
            _p5GraduatePlacedWithinSMEWhoGainedEmployment = participantToClone.P5GraduatePlacedWithinSMEWhoGainedEmployment;
            _p2P5NoQualificationGained = participantToClone.P2P5NoQualificationGained;
            _p2P5QualificationGained = participantToClone.P2P5QualificationGained;
        }
        public Participant(string[] record)
        {
            _recordType = record[0];
            _providingOrganisation = record[1];
            _fileRunDate = (record[2]!=""?DateTime.Parse(record[2]).ToString("yyyy-MM-dd"):"");
            _regionCode = record[3];
            _regionName = record[4];
            _agreementID = record[5];
            _providerProjectID = record[6];
            _fundingType = record[7];
            _participantReferenceID = record[8];
            _startDate = (record[9] != "" ? DateTime.Parse(record[9]).ToString("yyyy-MM-dd") : "");
            _gender = record[10];
            _dOB = (record[11] != "" ? DateTime.Parse(record[11]).ToString("yyyy-MM-dd") : "");
            _postcode = record[12];
            _employmentStatus = record[13];
            _unemployedMonths = record[14];
            _ethnicity = record[15];
            _disability = record[16];
            _qualificationHeld = record[17];
            _inPostGradResearch = record[18];
            _graduatePlacedWithSME = record[19];
            _leavingDate = (record[20] != "" ? DateTime.Parse(record[20]).ToString("yyyy-MM-dd") : "");
            _p1P4LeavingStatus = record[21];
            _p1P4NoQualificationGained = record[22];
            _p1P4QualificationGained = record[23];
            _p2P5IntoEducationOrTraining = record[24];
            _p5GraduatePlacedWithinSMEWhoGainedEmployment = record[25];
            _p2P5NoQualificationGained = record[26];
            _p2P5QualificationGained = record[27];
            
        }


    }
}
