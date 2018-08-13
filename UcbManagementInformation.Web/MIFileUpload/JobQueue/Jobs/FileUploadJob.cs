using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.DataClasses;
using System.Threading;
using UcbManagementInformation.Server.DataAccess;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using UcbManagementInformation.Web.MIFileUpload.JobQueue.Jobs;
using System.Runtime.Serialization;

namespace UcbManagementInformation.Web.MIFileUpload.JobQueue
{
    
    public class FileUploadJob : JobBase
    {
        public FileUploadJob()
        { }
  
        public FileUploadJob(Guid code,string fileName,string providerKey,string userID,string webPath,DateTime? added,DateTime? started, DateTime? ended,string description)
        {
            Code = code;
            ProviderKey = providerKey;
            UniqueFileName = fileName;
            AddedTime = added;
            StartTime = started;
            EndTime = ended;
            Description = description;
            //GET Schema file paths from configuration
            _schema = webPath + ConfigurationManager.AppSettings["ParticipantSchema"];
            _mappingSchema = webPath + ConfigurationManager.AppSettings["ParticipantMappingSchema"]; 

            //Get Connection string from configuration    
            _connectionString = ConfigurationManager.ConnectionStrings["MIStagingConnectionString"].ConnectionString;

            //set private variables
            IUcbManagementInformationRepository<ProviderOrganisation> providerOrganisationRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<ProviderOrganisation>>();
            ProviderOrganisation providerOrganisationItem =
                providerOrganisationRepository.First(x=>x.KeyValue==ProviderKey);
           
            _providingOrganistaionCode = providerOrganisationItem.Code;
            _fileType = "Participant";
            FileInfo DataFileInfoItem = new FileInfo(UniqueFileName);
            _fileLength = DataFileInfoItem.Length;

            _userID = userID;
            UserId = userID;
            JobSteps = new List<JobStep>()
            {
                new JobStep("SchemaValidation","Schema Validation",this,1),
                new JobStep("DeleteFromLoadingArea","DeleteFromLoadingArea",this,2),
                new JobStep("BulkUpload","Bulk Load",this,3),
                new JobStep("CopyBulkLoadedData","CopyBulkLoadedData",this,4),
                new JobStep("ValidateParticipantDuplicates","Validate Participant",this,5),
                new JobStep("ValidateAgreementRegion","Validate Participant",this,6),
                new JobStep("ValidateParticipantPostCode","Validate Participant",this,7),
                new JobStep("ValidateLeaver","Validate Participant",this,8),
                new JobStep("ValidateLeavingDate","Validate Participant",this,9),
                new JobStep("ValidateStartDate","Validate Participant",this,10),
                new JobStep("LoadParticipantMessages","Validate Participant",this,11),
                new JobStep("ValidatePreviousLoad","Validate Participant",this,12),
                new JobStep("UpdateParticipantCounts","Validate Participant",this,13)
                
            };
        }

        Guid _providingOrganistaionCode;
        string _connectionString;
        string _schema;
        string _mappingSchema;
        string _fileType = "Participant";
        long _fileLength;
        int _recordCount;
        int _flatRecordCount;
        string _userID;
        bool _isSchemaValid;
        bool _isBusinessValid;
        int _errorCode;
        int _noOfValidRecords;
        int _noOfErrorRecords;
        int _noOfWarningRecords;
        int _noOfInformationalRecords; 
        int _recordsSoFar;
        Guid _inputFileHistoryCode;

        int _deleteParticipantDataTimeout;
        int _validateParticipantDuplicatesTimeout;
        int _validateAgreementRegionTimeout;
        int _validateParticipantPostCodeTimeout;
        int _validateLeaverTimeout;
        int _validateLeavingDateTimeout;
        int _validateStartDateTimeout;
        int _participantSelectValidCountTimeout;
        int _participantSelectErrorCountTimeout;
        int _participantSelectWarningCountTimeout;
        int _participantSelectInformationalCountTimeout;
        int _loadParticipantMessagesTimeout;
        int _copyParticipantBulkLoadedFlatTimeout;
        MIStagingStoredProcedures spRunner = new MIStagingStoredProcedures();
        UcbManagementInformationStoredProcedures spRunner2 = new UcbManagementInformationStoredProcedures();

        IUcbManagementInformationRepository<ProviderOrganisation> providerRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<ProviderOrganisation>>();
        IUcbManagementInformationRepository<InputFileHistory> inputFileHistoryRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<InputFileHistory>>();
        IUcbManagementInformationRepository<InputFileError> inputFileErrorsRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<InputFileError>>();
        IUcbManagementInformationRepository<UploadMonitor> uploadMonitorRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<UploadMonitor>>();

        InputFileHistory _inputFileHistoryRow;

        public string ProviderKey {get;set;}

        public string UniqueFileName { get; set; }
        
       
        
        
        public override int  RunJob()
        {
 	
            try
            {
                FileLock(true);
                //Insert of the file upload history record
                InsertHistoryRecord();
                
                //set up timeouts for job
                _deleteParticipantDataTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("DeleteParticipantDataTimeout"));
                _validateParticipantDuplicatesTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("ValidateParticipantDuplicatesTimeout"));
                _validateAgreementRegionTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("ValidateAgreementRegionTimeout"));
                _validateParticipantPostCodeTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("ValidateParticipantPostCodeTimeout"));
                _validateLeaverTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("ValidateLeaverTimeout"));
                _validateLeavingDateTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("ValidateLeavingDateTimeout"));
                _validateStartDateTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("ValidateStartDateTimeout"));
                _participantSelectValidCountTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("ParticipantSelectValidCountTimeout"));
                _participantSelectErrorCountTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("ParticipantSelectErrorCountTimeout"));
                _participantSelectWarningCountTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("ParticipantSelectWarningCountTimeout"));
                _participantSelectInformationalCountTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("ParticipantSelectInformationalCountTimeout"));
                _loadParticipantMessagesTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("LoadParticipantMessagesTimeout"));
                _copyParticipantBulkLoadedFlatTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("CopyParticipantBulkLoadedFlatTimeout"));
                Guid historyFileGuid = _inputFileHistoryRow.Code;

                int jobReturnCode = UploadFile();

                //leave locked until a decision has been made
                //FileLock(false);

                return jobReturnCode;
            }
            catch (Exception e)
            {
                if (CurrentStep != null)
                {
                    CurrentStep.Finish(false);
                }
                UnhandledException = e;
                spRunner2.PublishError("File Upload", 3, e.Message, _inputFileHistoryCode, null, null);
                //Log error to db for debug purposes.

                //unlock the File provider for future uploads
                FileLock(false);
                return 1;
            }
        }

        #region UploadFile : ************************** Main flow of the code ****************************
        /// <summary>
        /// Executes a validation and bulk upload run for the file
        /// </summary>
        /// <param name="historyFileGuid">The unique identifier for the upload attempt for the file</param>
        /// <returns>An integer indicating outcome of the upload. 0=Success</returns>
        public int UploadFile()
        {
            //Validate the file against the Schema
            Validate(UniqueFileName, _schema);

            //Delete existing from Loading area
            DeleteFromLoadingArea();
                    
            //Bulk Load the data
            LoadSql(UniqueFileName, _connectionString);
                    
            //Copy to structured tables
            CopyBulkLoadedData();
                    
            //Validate the uploaded data for business rules
            BusinessValidate();

            return 0;
            
        }




        //  --------------Monitor operation record metrics on table---UploadMonitor------------------

        //private void AddToMonitor(string stepText)
        //{

        //    AddToMonitorBack dlgt = new AddToMonitorBack(AddToMonitorDetail);

        //    IAsyncResult Result = dlgt.BeginInvoke(stepText, null, null);

        //}



        //private void AddToMonitorDetail(string stepText)
        //{

            
        //    //Insert 'step description' into UploadMonitor

        //    Guid MonitorCode = Guid.NewGuid();

        //    uploadMonitorRepository.Add(new UploadMonitor()
        //    {
        //        Code = MonitorCode,
        //        Position = 0,
        //        //position,
        //        //inputfilecode,
        //        InputFileCode = _inputFileHistoryCode,
        //        TimeOccured = DateTime.Now,
        //        StepDescription = stepText
        //    });
        //    uploadMonitorRepository.SaveChanges();

        //    //  --- jobType + " " +ProviderKey + " " + _fileType,
        //    //  --- WindowsIdentity.GetCurrent().Name);


            
        //}
        
        /// <summary>
        /// Changes the status of the uploaded file
        /// </summary>
        /// <param name="newStatus">The status to go to</param>
        private void UpdateHistoryStatus(string newStatus)
        {
            _inputFileHistoryRow.Status = newStatus;
            inputFileHistoryRepository.Update(_inputFileHistoryRow);
            inputFileHistoryRepository.SaveChanges();
        }

        /// <summary>
        /// Creates a history record of the upload attempt
        /// </summary>
        private void InsertHistoryRecord()
        {
            string userID = _userID;
            Guid historyCode = Guid.NewGuid();
            InputFileHistory ifh = new InputFileHistory()
            {
                Code = historyCode,
                Filename = (UniqueFileName != null ? UniqueFileName.Substring(UniqueFileName.LastIndexOf('\\') + 1) : "Not Found"),
                FileType = _fileType,
                LoadedBy = userID,
                LoadedDate = DateTime.Now,
                NumberOfErrorRecords = 0,
                NumberOfInformationalRecords = 0,
                NumberOfRecords = 0,
                NumberOfValidRecords = 0,
                NumberOfWarningRecords = 0,
                ProviderOrganisationKeyValue = ProviderKey,
                RowIdentifier = new byte[0],
                Status = "Loading",
                TransferDate = DateTime.Parse("01/01/2000"),
                UploadDecision = -2,
                ValidatedLoadDate = null

            };
            inputFileHistoryRepository.Add(ifh);
            inputFileHistoryRepository.SaveChanges();

            //retrieve the History Record for further updates throughout this class
            _inputFileHistoryRow = inputFileHistoryRepository.First(x => x.Code == historyCode);
            _inputFileHistoryCode = _inputFileHistoryRow.Code;
            _errorCode = 0;
            _isSchemaValid = true;
            _isBusinessValid = true;


        }

        /// <summary>
        /// Locks or unlocks the file
        /// </summary>
        /// <param name="isLock"></param>
        private void FileLock(bool isLock)
        {
            providerRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<ProviderOrganisation>>();
            ProviderOrganisation providerEntry = providerRepository.First(x => x.KeyValue == ProviderKey && x.FileType == _fileType);
            providerEntry.Status = (isLock ? "Locked" : "Unlocked");
            providerRepository.Update(providerEntry);
            providerRepository.SaveChanges();
        }
        #endregion

        #region Clear Loading area
        /// <summary>
        /// Removes old data for the current file from the loading area.
        /// </summary>
        private void DeleteFromLoadingArea()
        {
            CurrentStep = this["DeleteFromLoadingArea"];
            CurrentStep.Timeout = _deleteParticipantDataTimeout;
            CurrentStep.Start();
            CurrentStep.UpdateProgress(0,"Deleting previous file from loading area");
            if (_fileType == "Participant")
            {
                MIStagingStoredProcedures spRunner = new MIStagingStoredProcedures();
                spRunner.DeleteParticipantData(ProviderKey, _deleteParticipantDataTimeout);
            }
            CurrentStep.Finish(true);

        }
        #endregion

        

        #region Business Validation
        /// <summary>
        /// Runs the validation for business rules against the uploaded file
        /// </summary>
        private void BusinessValidate()
        {

            if (_fileType == "Participant")
            {
                ParticipantBusinessValidate();

            }
            //Update status to 'Business Validated'
            
            UpdateHistoryStatus("Business Validated");

        }


        /// <summary>
        /// Validates participant type files for business rules
        /// </summary>
        private void ParticipantBusinessValidate()
        {
            
            try
            {
                
                //currentStep = this["DeleteParticipantData"];
                //currentStep.Start();
                //currentStep.UpdateProgress(0, "Deleting Participant Data");
                //spRunner.DeleteParticipantData(ProviderKey);
                //currentStep.Finish(true);

                CurrentStep = this["ValidateParticipantDuplicates"];
                CurrentStep.Timeout = _validateParticipantDuplicatesTimeout;
                CurrentStep.Start();
                CurrentStep.UpdateProgress(0, "Checking for duplicate participants");
                spRunner.ValidateParticipantDuplicates(ProviderKey, _validateParticipantDuplicatesTimeout);
                CurrentStep.Finish(true);

                CurrentStep = this["ValidateAgreementRegion"];
                CurrentStep.Timeout = _validateAgreementRegionTimeout;
                CurrentStep.Start();
                CurrentStep.UpdateProgress(0, "Checking agreement regions");
                spRunner.ValidateAgreementRegion(ProviderKey, _validateAgreementRegionTimeout);
                CurrentStep.Finish(true);

                CurrentStep = this["ValidateParticipantPostCode"];
                CurrentStep.Timeout = _validateParticipantPostCodeTimeout;
                CurrentStep.Start();
                CurrentStep.UpdateProgress(0, "Checking participant post codes");
                spRunner.ValidateParticipantPostCode(ProviderKey, _validateParticipantPostCodeTimeout);
                CurrentStep.Finish(true);

                // ---------------------------- Remove check may have to put back, client dependant ---------  
                //OnProgressEvent(new ProgressEventArgs("Business", 40, 25,"Checking participant projects"));
                //ParticipantBulkLoaded.ValidateParticipantProject(ProviderKey);
                //  -----------------------------------------------------------------------------------------

                CurrentStep = this["ValidateLeaver"];
                CurrentStep.Timeout = _validateLeaverTimeout;
                CurrentStep.Start();
                CurrentStep.UpdateProgress(0, "Checking participant leaver information");
                spRunner.ValidateLeaver(ProviderKey,_validateLeaverTimeout);
                CurrentStep.Finish(true);

                CurrentStep = this["ValidateLeavingDate"];
                CurrentStep.Timeout = _validateLeavingDateTimeout;
                CurrentStep.Start();
                CurrentStep.UpdateProgress(0, "Checking participant leaving dates are within range");
                spRunner.ValidateLeavingDate(ProviderKey,_validateLeavingDateTimeout);
                CurrentStep.Finish(true);

                CurrentStep = this["ValidateStartDate"];
                CurrentStep.Timeout = _validateStartDateTimeout;
                CurrentStep.Start();
                CurrentStep.UpdateProgress(0, "Checking participant start dates are in range");
                spRunner.ValidateStartDate(ProviderKey, _validateStartDateTimeout);
                CurrentStep.Finish(true);

                CurrentStep = this["LoadParticipantMessages"];
                CurrentStep.Timeout = _loadParticipantMessagesTimeout;
                CurrentStep.Start();
                CurrentStep.UpdateProgress(0, "Loading participant error messages");
                spRunner2.LoadParticipantMessages(_inputFileHistoryCode, _loadParticipantMessagesTimeout);//first validation rule completed
                CurrentStep.Finish(true);

                CurrentStep = this["ValidatePreviousLoad"];
                CurrentStep.Start();
                CurrentStep.UpdateProgress(0, "Comparing previous load");
                ValidatePreviousLoad(_inputFileHistoryCode, ProviderKey, _fileType, _recordCount, _inputFileHistoryRow.TransferDate);
                CurrentStep.Finish(true);


                CurrentStep = this["UpdateParticipantCounts"];
                CurrentStep.Start();
                
                CurrentStep.UpdateProgress(0, "Counting Valid Participants");
                _noOfValidRecords = spRunner.ParticipantSelectValidCount(ProviderKey,_participantSelectValidCountTimeout);
                CurrentStep.UpdateProgress(25, "Counting Error Participants");

                _noOfErrorRecords = spRunner.ParticipantSelectErrorCount(ProviderKey, _participantSelectErrorCountTimeout);
                CurrentStep.UpdateProgress(50, "Counting Warning Participants");

                _noOfWarningRecords = spRunner.ParticipantSelectWarningCount(ProviderKey, _participantSelectWarningCountTimeout);
                CurrentStep.UpdateProgress(75, "Counting Informational Participants");

                _noOfInformationalRecords = spRunner.ParticipantSelectInformationalCount(ProviderKey, _participantSelectInformationalCountTimeout);
                CurrentStep.UpdateProgress(100, "Business validation complete");
                
                //finished so raise update event

                _inputFileHistoryRow.NumberOfErrorRecords = _noOfErrorRecords;
                _inputFileHistoryRow.NumberOfInformationalRecords = _noOfInformationalRecords;
                _inputFileHistoryRow.NumberOfValidRecords = _noOfValidRecords;
                _inputFileHistoryRow.NumberOfWarningRecords = _noOfWarningRecords;
                _inputFileHistoryRow.NumberOfRecords = _recordCount;

                inputFileHistoryRepository.Update(_inputFileHistoryRow);
                inputFileHistoryRepository.SaveChanges();

                CurrentStep.Finish(true);
               

            }

            catch (Exception ex)
            {
                _errorCode = 6;
                throw new Exception(ex.Message, ex);
                //_isBusinessValid = false;
                //InputFileErrors.PublishError(ex.Message + Environment.NewLine + ex.StackTrace, 3, "System", _inputFileHistoryCode);
            }
        }

        private InputFileHistory GetPreviousLoadedFile(string KeyValue, string fileType)
        {
            return inputFileHistoryRepository.Find
                (x => x.ProviderOrganisationKeyValue == KeyValue &&
                    x.FileType == fileType && x.Status.StartsWith("Validated Loaded")).OrderByDescending(x => x.LoadedDate).FirstOrDefault();
        }
        public void ValidatePreviousLoad(Guid inputFileHistoryCode, string KeyValue, string fileType, int numberOfRecords, DateTime fileRunDate)
        {
            InputFileHistory PreviousRow = GetPreviousLoadedFile(KeyValue, fileType);
            if (PreviousRow != null)
            {
                if (PreviousRow.NumberOfRecords > numberOfRecords)
                {
                    spRunner2.PublishError("Business Validation", 2, "Fewer records exist than in previously loaded file", inputFileHistoryCode, null, null);
                }
                if (PreviousRow.TransferDate.Date >= fileRunDate.Date)
                {
                    spRunner2.PublishError("Business Validation", 2, "A File has already been loaded with this date or later.", inputFileHistoryCode, null, null);
                }
            }
        }


        #endregion

        #region Schema Validation

        private void Validate(string datafile, string schema)
        {
            CurrentStep = this["SchemaValidation"];
            CurrentStep.Start();

            //Ensure all required parameters are available
            if (_schema == string.Empty || UniqueFileName == string.Empty || _connectionString == string.Empty
                || _mappingSchema == string.Empty || _schema == null || UniqueFileName == null
                || _connectionString == null
                || _mappingSchema == null)
            {
                //PublishError("Bulk Upload", 3, "Missing Schema, Mapping Schema, Data File or Connection String.", _inputFileHistoryCode, null, null);
                _errorCode = 1;
                _isSchemaValid = false;
                throw new ApplicationException("Missing Schema, Mapping Schema, Data File or Connection String.");

            }
            else
            {
                //Consider making the XmlReaderSettings instance static if it uses the same settings
                //for all xml validation
                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.CheckCharacters = true;
                xmlReaderSettings.CloseInput = true;
                //if you have a finite number of schemas, consider caching each schema in
                //a static XmlSchemaSet instance 
                xmlReaderSettings.Schemas.Add(null, schema);
                xmlReaderSettings.ValidationType = ValidationType.Schema;
                xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                //...add any other ValidationFlags necessary here...

                xmlReaderSettings.ValidationEventHandler += new ValidationEventHandler(ShowCompileErrors);
                try
                {
                    //Wrap the creation of the XmlReader in a 'using' block since    
                    //it implements IDisposable

                    string SchemaVersion = "";
                    int MaxChunksToReadForSchemaVersionCountdown = 10; // Max number of xml chunks to read to try and get schema version
                    int PercentThroughOld = 0;
                    int PercentThroughInt = 0;
                    FileStream DataFileStream = new FileStream(datafile, FileMode.Open, FileAccess.Read);
                    using (XmlReader xmlReader = XmlReader.Create(DataFileStream, xmlReaderSettings))
                    {
                        if (xmlReader != null)
                        {
                            // Definitions for reading extra info surrounding DOB
                            string LastRegion = string.Empty;
                            string LastAgreement = string.Empty;
                            string LastParticipantReferenceID = string.Empty;
                            string LastStartDate = string.Empty;

                            while (xmlReader.Read())
                            {

                                if (xmlReader.LocalName == "ParticipantInformation" && xmlReader.IsStartElement())
                                {
                                    SchemaVersion = xmlReader.GetAttribute("SchemaVersion");
                                }

                                if (MaxChunksToReadForSchemaVersionCountdown < 1 && SchemaVersion != "1.0")
                                {
                                    // Set the error and exit immediately as this must be an invalid xml file, e.g.,
                                    // saved from excel!!
                                    _errorCode = 5;
                                    _isSchemaValid = false;
                                    spRunner2.PublishError("Schema Validation", 3, "This xml file is in an unrecognisable format.",
                                       _inputFileHistoryCode, null, null);
                                    break;
                                }

                                // decrement countdown used for trying to get the schema version
                                if (MaxChunksToReadForSchemaVersionCountdown >= 1)
                                {
                                    MaxChunksToReadForSchemaVersionCountdown--;
                                }

                                //Ensure the providing organisation is the one selected on the ui
                                if (xmlReader.LocalName == "ProvidingOrganisation" && xmlReader.IsStartElement())
                                {

                                    string ProvidingOrganisationString = xmlReader.ReadString();
                                    if (ProvidingOrganisationString != ProviderKey.Substring(0, ProviderKey.Length - 2))
                                    {
                                        _errorCode = 5;
                                        //InputFileErrors.PublishError("The file providing organisation: " + ProvidingOrganisationString +
                                        //    " does not match the selected value of " +
                                        //    ProviderKey.Substring(0, ProviderKey.Length - 2), 3, "Schema Validation", _inputFileHistoryCode);

                                        // set the error and exit immediatley as the file is not fit for purpose

                                        _isSchemaValid = false;
                                        spRunner2.PublishError("Schema Validation", 3, "The xml file Providing Organisation does not match selection", _inputFileHistoryCode
                                            , null, null);
                                        break;

                                    }
                                }
                                //Ensure the providing organisation is the one selected on the ui

                                if (xmlReader.LocalName == "FileRunDate" && xmlReader.IsStartElement())
                                {

                                    string FileRunDate = xmlReader.ReadString();
                                    _inputFileHistoryRow.TransferDate = DateTime.Parse(FileRunDate);
                                    inputFileHistoryRepository.Update(_inputFileHistoryRow);
                                    inputFileHistoryRepository.SaveChanges();
                                }

                                //Ensure the region is the one selected on the ui

                                if (ProviderKey.Substring(ProviderKey.Length - 2, 2) != "XX")
                                {
                                    if (xmlReader.LocalName == "RegionCode" && xmlReader.IsStartElement())
                                    {
                                        string RegionCodeString = xmlReader.ReadString();

                                        // Used to store for reporting schema errors, e.g., DOB errors
                                        LastRegion = RegionCodeString;

                                        if (RegionCodeString != ProviderKey.Substring(ProviderKey.Length - 2, 2))
                                        {

                                            _errorCode = 5;

                                            //InputFileErrors.PublishError("The file RegionCode: " + RegionCodeString +
                                            //    " does not match the selected value of " +
                                            //    ProviderKey.Substring(ProviderKey.Length - 2, 2), 3, "Schema Validation", _inputFileHistoryCode);

                                            // set the error and exit immediatley as the file is not fit for purpose
                                            _isSchemaValid = false;
                                            spRunner2.PublishError("Schema Validation", 3, "The xml file Region does not match selection", _inputFileHistoryCode,
                                                null, null);
                                            break;

                                        }
                                    }
                                }


                                if (this._fileType == "Participant")
                                {

                                    #region Store info for reporting schema errors, e.g., DOB errors

                                    if (ProviderKey.Substring(ProviderKey.Length - 2, 2) == "XX")
                                    {
                                        if (xmlReader.LocalName == "RegionCode" && xmlReader.IsStartElement())
                                        {
                                            // Set last region if All region, i.e., XX otherwise Last region is already set elsewhere in
                                            // this code
                                            LastRegion = xmlReader.ReadString();
                                        }
                                    }

                                    // Set AgreementID
                                    if (xmlReader.LocalName == "AgreementID" && xmlReader.IsStartElement())
                                    {
                                        LastAgreement = xmlReader.ReadString();
                                    }

                                    // Set Participant Ref ID
                                    if (xmlReader.LocalName == "ParticipantReferenceID" && xmlReader.IsStartElement())
                                    {
                                        LastParticipantReferenceID = xmlReader.ReadString();
                                    }

                                    // Set Start Date
                                    if (xmlReader.LocalName == "StartDate" && xmlReader.IsStartElement())
                                    {
                                        LastStartDate = xmlReader.ReadString();
                                    }

                                    #endregion

                                    #region DOB Error Check

                                    if (xmlReader.LocalName == "DOB" && xmlReader.IsStartElement())
                                    {
                                        try
                                        {
                                            string DOBDateTimeString = xmlReader.ReadString();
                                            DateTime SqlMinDateTime = new DateTime(1753, 1, 1);
                                            DateTime DOBDateTime = Convert.ToDateTime(DOBDateTimeString);


                                            if (DOBDateTime < SqlMinDateTime)
                                            {
                                                _errorCode = 5;

                                                // Don't process the file
                                                _isSchemaValid = false;
                                                throw new ApplicationException("The DOB is prior to 1 Jan 1753 on record: "
                                                    + "KeyValue: " + ProviderKey
                                                    + " Region: " + LastRegion + " Agreement: " + LastAgreement +
                                                    " Participant Ref ID: " + LastParticipantReferenceID
                                                    + " Start Date: " + LastStartDate + " DOB: " + DOBDateTime.ToString("dd/MM/yyyy")
                                                    );
                                            }
                                        }
                                        catch (FormatException ex)
                                        {
                                            _errorCode = 5;

                                            // Don't process the file
                                            _isSchemaValid = false;
                                            throw new ApplicationException("The DOB is incorrect on record: " + "KeyValue: " + ProviderKey
                                                + " Region: " + LastRegion + " Agreement: " + LastAgreement +
                                                " Participant Ref ID: " + LastParticipantReferenceID
                                                + " Start Date: " + LastStartDate + " DOB: Invalid Date or Non Date Format"
                                                , ex);

                                        }

                                    }

                                    #endregion

                                }

                                long CurrentPosition = DataFileStream.Position;
                                decimal PercentThrough = Convert.ToDecimal(CurrentPosition) / Convert.ToDecimal(_fileLength) * 100;
                                PercentThroughInt = Convert.ToInt32(decimal.Truncate(PercentThrough));
                                if (PercentThroughInt > PercentThroughOld)
                                {
                                    CurrentStep.UpdateProgress(PercentThroughInt);
                                    //OnProgressEvent(new ProgressEventArgs("Schema", PercentThroughInt, PercentThroughInt - PercentThroughOld, "Validating file content"));
                                    PercentThroughOld = PercentThroughInt;
                                }
                                if (this._fileType == "Participant")
                                {
                                    if (xmlReader.LocalName == "Participant" && xmlReader.IsStartElement())
                                    {
                                        _recordCount++;
                                        _flatRecordCount++;
                                    }
                                    if (xmlReader.LocalName == "ESFP1P4Qualification" && xmlReader.IsStartElement())
                                    {
                                        _flatRecordCount++;
                                    }
                                    if (xmlReader.LocalName == "ESFP2P5Qualification" && xmlReader.IsStartElement())
                                    {
                                        _flatRecordCount++;
                                    }
                                }
                            }
                            //explicitly call Close on the XmlReader to reduce strain on the GC
                            xmlReader.Close();
                        }
                    }

                    //Update status to 'Schema Validated'
                    UpdateHistoryStatus("Schema Validated");
                }

                catch (Exception ex)//..various types of Exceptions can be thrown by the XmlReader.Create() method)
                {
                    _errorCode = 5;
                    throw new ApplicationException(ex.Message, ex);
                }

            }
            if (!_isSchemaValid)
            {
                throw new ApplicationException("Schema Invalid");
            }
            CurrentStep.Finish(true);
        }
        private void StepProgress(string stepName,int PercentThroughInt, int percentChange, string stepCategory)
        {
            var stepItem = (from x in JobSteps
                        where x.Name == stepName
                        select x).FirstOrDefault();
             
        }
                                    
        private void PublishError(string category, int? errorLevel, string errorMessage, Guid inputFileHistoryCode, DateTime? recordDateTime, string recordKey)
        {
            inputFileErrorsRepository.Add(new InputFileError()
            {
                Code = Guid.NewGuid(),
                Category = category,
                ErrorLevel = errorLevel,
                ErrorMessage = errorMessage,
                InputFileHistoryCode = inputFileHistoryCode,
                RecordDateTime = recordDateTime,
                RecordKey = recordKey,
                RowIdentifier = new byte[0]
            });
            inputFileErrorsRepository.SaveChanges();
        }

        #endregion

        #region Bulk Upload
        private void LoadSql(string xMLFilename, string connectionString)
        {
            CurrentStep = this["BulkUpload"];
            CurrentStep.Start();
            CurrentStep.UpdateProgress(0, "Bulk uploading");
            IDataReader reader = null;
            //new ParticipantDataReader(xMLFilename);
            SqlBulkCopy Copier = new SqlBulkCopy(connectionString);

            if (_fileType == "Participant")
            {
                Copier.DestinationTableName = "dbo.ParticipantBulkLoadedFlat";
                reader = new ParticipantDataReader(xMLFilename, _inputFileHistoryCode, _providingOrganistaionCode);
            }

            Copier.BatchSize = 5000;
            Copier.NotifyAfter = 1000;
            Copier.BulkCopyTimeout = 60;
            Copier.SqlRowsCopied += new SqlRowsCopiedEventHandler(Copier_SqlRowsCopied);
            Copier.WriteToServer(reader);
            CurrentStep.Finish(true);
        }
        
        void Copier_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            _recordsSoFar += 1000;
            decimal percentthrough = Convert.ToDecimal(_recordsSoFar) / Convert.ToDecimal(_flatRecordCount) * 100;
            int percentInt = Convert.ToInt32(decimal.Truncate(percentthrough));
            CurrentStep.UpdateProgress(percentInt, "Bulk uploading");
            
        }

        #region Copy Bulk Loaded Data to structured Tables

        private void CopyBulkLoadedData()
        {
            CurrentStep = this["CopyBulkLoadedData"];
            CurrentStep.Timeout = _copyParticipantBulkLoadedFlatTimeout;
            CurrentStep.Start();
            CurrentStep.UpdateProgress(0, "Copying bulk data");
            if (_fileType == "Participant")
            {
                MIStagingStoredProcedures spRunner = new MIStagingStoredProcedures();
                spRunner.CopyParticipantBulkLoadedFlat(ProviderKey, _copyParticipantBulkLoadedFlatTimeout);
            }
            UpdateHistoryStatus("Loaded");
                    
            CurrentStep.Finish(true);

        }

        #endregion


        private void ShowCompileErrors(object sender, ValidationEventArgs args)
        {
            Type SenderType = sender.GetType();
            PropertyInfo LineNumberPropertyInfo = SenderType.GetProperty("LineNumber");
            int LineNumber = (int)LineNumberPropertyInfo.GetValue(sender, null);
            PropertyInfo LinePositionPropertyInfo = SenderType.GetProperty("LinePosition");
            int LinePosition = (int)LinePositionPropertyInfo.GetValue(sender, null);
            string LineInfo = "LineNumber " + LineNumber.ToString() +
                Environment.NewLine + "Position " + LinePosition.ToString();
            string errorMessage = string.Format
            (
                "Xml validation failed against schema: " + Environment.NewLine
                + " Message: {0}",
                args.Message
            );
            errorMessage += Environment.NewLine + LineInfo;
            spRunner2.PublishError("Schema Validation", 3, errorMessage, _inputFileHistoryCode, null, null);
            _errorCode = 4;
            _isSchemaValid = false;

        }

        #endregion

    }
}