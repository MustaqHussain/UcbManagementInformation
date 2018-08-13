using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects.DataClasses;
using System.Configuration;
using UcbManagementInformation.Server.DataAccess;
using System.Data.SqlClient;
using UcbManagementInformation.Server.IoC.ServiceLocation;

namespace UcbManagementInformation.Web.MIFileUpload.JobQueue.Jobs
{
    public class CopyToValidJob : JobBase
    {
        public CopyToValidJob()
        { }

        public CopyToValidJob(Guid code, string providerKey,Guid inputFileHistoryCode, int decisionLevel ,string userID, string webPath, DateTime? added, DateTime? started, DateTime? ended, string description)
        {
            Code = code;
            ProviderKey = providerKey;
            AddedTime = added;
            StartTime = started;
            EndTime = ended;
            Description = description;
            _inputFileHistoryCode = inputFileHistoryCode;
            _decisionLevel = decisionLevel;
            //Get Connection string from configuration    
            _connectionString = ConfigurationManager.ConnectionStrings["MIStagingConnectionString"].ConnectionString;

            uow = SimpleServiceLocator.Instance.Get<IUnitOfWork>("UcbManagementInformation");

            providerRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<ProviderOrganisation>>(uow.ObjectContext);
            inputFileHistoryRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<InputFileHistory>>(uow.ObjectContext);
            inputFileErrorsRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<InputFileError>>(uow.ObjectContext);
            uploadMonitorRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<UploadMonitor>>(uow.ObjectContext);


            //set private variables
            ProviderOrganisation providerOrganisationItem =
                providerRepository.First(x => x.KeyValue == ProviderKey);
           
            _providingOrganistaionCode = providerOrganisationItem.Code;
            _fileType = "Participant";
            
            _userID = userID;
            UserId = userID;


            JobSteps = new List<JobStep>()
            {
                new JobStep("DeleteValidatedParticipantData","Copy Valid Participant",this,1),
                new JobStep("CopyValidatedParticipants","Copy Valid Participant",this,2),
                new JobStep("DeleteParticipantData","Copy Valid Participant",this,3),
                //new JobStep("DeleteParticipantDataOver4HoursOld","Copy Valid Participant",this,4),
                new JobStep("DeleteInputFileErrorsOver1YearOld","Copy Valid Participant",this,4)
            };
        }
        string _connectionString;
        Guid _providingOrganistaionCode;
        Guid _inputFileHistoryCode;
        string _fileType = "Participant";
        string _userID;
        int _decisionLevel = -2;
        int _deleteParticipantDataTimeout;
        int _deleteInputFileErrorsOver1YearOldTimeout;
        int _deleteValidatedParticipantDataTimeout;
        int _copyValidatedParticipantsTimeout;
        MIStagingStoredProcedures spRunner = new MIStagingStoredProcedures();
        UcbManagementInformationStoredProcedures spRunner2 = new UcbManagementInformationStoredProcedures();

        IUnitOfWork uow;

        IUcbManagementInformationRepository<ProviderOrganisation> providerRepository;
        IUcbManagementInformationRepository<InputFileHistory> inputFileHistoryRepository;
        IUcbManagementInformationRepository<InputFileError> inputFileErrorsRepository;
        IUcbManagementInformationRepository<UploadMonitor> uploadMonitorRepository;

        string ProviderKey { get; set; }

        public override int RunJob()
        {

            try
            {
                FileLock(true);

                _deleteParticipantDataTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("DeleteParticipantDataTimeout"));
                _deleteInputFileErrorsOver1YearOldTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("DeleteOldInputFileErrorsTimeout"));
                _deleteValidatedParticipantDataTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("DeleteValidatedParticipantDataTimeout"));
                _copyValidatedParticipantsTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("CopyValidatedParticipantsTimeout"));
                int jobReturnCode = ValidatedUpload(_decisionLevel);

                //unlock the File provider for future uploads
                FileLock(false);

                return jobReturnCode;
            }
            catch (Exception e)
            {
                if (CurrentStep != null)
                {
                    CurrentStep.Finish(false);
                }
                UnhandledException = e;
                spRunner2.PublishError("Copy Valid Records", 3, e.Message, _inputFileHistoryCode, null, null);
                //Log error to db for debug purposes.

                //unlock the File provider for future uploads
                FileLock(false);
                return 1;
            }
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

        #region Clear Loading area
        /// <summary>
        /// Removes old data for the current file from the loading area.
        /// </summary>
        private void DeleteParticipantData()
        {
            CurrentStep = this["DeleteParticipantData"];
            CurrentStep.Timeout = _deleteParticipantDataTimeout;
            CurrentStep.Start();
            CurrentStep.UpdateProgress(0, "Deleting previous file from loading area");
            spRunner.DeleteParticipantData(ProviderKey,_deleteParticipantDataTimeout);
            
            CurrentStep.Finish(true);

        }
        #endregion
        #region Clear Loading area of files over 4 hours old
        ///// <summary>
        ///// Removes old data for the current file from the loading area.
        ///// </summary>
        //private void DeleteParticipantDataOver4HoursOld()
        //{
        //    CurrentStep = this["DeleteParticipantDataOver4HoursOld"];
        //    CurrentStep.Start();
        //    CurrentStep.UpdateProgress(0, "Deleting data over 4 hours old from loading area");
        //    spRunner.DeleteParticipantDataOver4HoursOld();

        //    CurrentStep.Finish(true);

        //}
        #endregion

        #region Clear Loading area of files over 4 hours old
        /// <summary>
        /// Removes old data for the current file from the loading area.
        /// </summary>
        private void DeleteInputFileErrorsOver1YearOld()
        {
            CurrentStep = this["DeleteInputFileErrorsOver1YearOld"]; 
            CurrentStep.Timeout = _deleteInputFileErrorsOver1YearOldTimeout;
            CurrentStep.Start();
            CurrentStep.UpdateProgress(0, "Deleting data over 1 year old from error message table");
            spRunner.DeleteInputFileErrorsOver1YearOld(_deleteInputFileErrorsOver1YearOldTimeout);

            CurrentStep.Finish(true);

        }
        #endregion

        #region Clear validated area
        /// <summary>
        /// Removes old data for the current file from the validated area.
        /// </summary>
        private void DeleteValidatedParticipantData(SqlTransaction transact)
        {
            CurrentStep = this["DeleteValidatedParticipantData"];
            CurrentStep.Timeout = _deleteValidatedParticipantDataTimeout;
            CurrentStep.Start();
            CurrentStep.UpdateProgress(0,"Deleting previous file from validated area");
            spRunner.DeleteValidatedParticipantData(ProviderKey, transact, _deleteValidatedParticipantDataTimeout);
            
            CurrentStep.Finish(true);

        }
        #endregion

        #region Copt to validated area
        /// <summary>
        /// Removes old data for the current file from the validated area.
        /// </summary>
        private void CopyValidatedParticipants(SqlTransaction transact)
        {
            CurrentStep = this["CopyValidatedParticipants"];
            CurrentStep.Timeout = _copyValidatedParticipantsTimeout;
            CurrentStep.Start();
            CurrentStep.UpdateProgress(0, "Copying records to validated area");
            spRunner.CopyValidatedParticipants(ProviderKey, _decisionLevel, transact, _copyValidatedParticipantsTimeout);
            CurrentStep.Finish(true);

        }
        #endregion

        #region Copy data to validated area
        /// <summary>
        /// Copys the uploaded data to a validated area
        /// </summary>
        /// <param name="uploadDecision">The type of data to copy: 'ValidOnly','ValidAndInfo' or 'ValidInfoAndWarning'</param>
        /// <returns></returns>
        public int ValidatedUpload(int maximumErrorLevel)
        {
            using (spRunner.Cn)
            {
                spRunner.Cn.Open();
                spRunner.IsOpen = true;   
                using (SqlTransaction transact = spRunner.Cn.BeginTransaction())
                {
                     DeleteValidatedParticipantData(transact);
                    CopyValidatedParticipants(transact);
                    transact.Commit();
                   
                }
                spRunner.Cn.Close();
                spRunner.IsOpen = false;
            }
            //Stamp number of records uploaded and remove lock of database
            using (uow = SimpleServiceLocator.Instance.Get<IUnitOfWork>("UcbManagementInformation"))
            {
                inputFileHistoryRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<InputFileHistory>>(uow.ObjectContext);
                providerRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<ProviderOrganisation>>(uow.ObjectContext);
                InputFileHistory ifh = inputFileHistoryRepository.Find(x => x.Code == _inputFileHistoryCode).First();
                ifh.Status = "Validated Loaded " + maximumErrorLevel.ToString();
                ifh.UploadDecision = maximumErrorLevel;
                //_inputFileHistoryRow.ValidatedLoadDate = DateTime.Now;
                inputFileHistoryRepository.Update(ifh);
                    
                ProviderOrganisation po = providerRepository.First(x => x.KeyValue == ProviderKey && x.FileType == "Participant");
                po.CurrentValidatedFileCode = _inputFileHistoryCode;
                providerRepository.Update(po);
                    
                uow.Commit();
            }

            DeleteParticipantData();
            //DeleteParticipantDataOver4HoursOld();
            DeleteInputFileErrorsOver1YearOld();
            return 0;
           
        }
        #endregion
    }
}