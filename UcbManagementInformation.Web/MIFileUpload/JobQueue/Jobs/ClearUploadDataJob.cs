using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using UcbManagementInformation.Server.IoC.ServiceLocation;
using UcbManagementInformation.Server.DataAccess;

namespace UcbManagementInformation.Web.MIFileUpload.JobQueue.Jobs
{
    public class ClearUploadDataJob : JobBase
    {
        public ClearUploadDataJob()
        { }

        public ClearUploadDataJob(Guid code, string providerKey, Guid inputFileHistoryCode, string userID, string webPath, DateTime? added, DateTime? started, DateTime? ended, string description)
        {
            Code = code;
            ProviderKey = providerKey;
            AddedTime = added;
            StartTime = started;
            EndTime = ended;
            Description = description;
            _inputFileHistoryCode = inputFileHistoryCode;
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
                new JobStep("DeleteParticipantData","Clear Participant Data",this,1),
                new JobStep("DeleteInputFileErrorsOver1YearOld","Clear Participant Data",this,1)
                
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
                int jobReturnCode = ClearUpload();

                InputFileHistory ifh = inputFileHistoryRepository.Find(x => x.Code == _inputFileHistoryCode).First();
                ifh.Status = "Cancelled";
                inputFileHistoryRepository.Update(ifh);

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
                spRunner2.PublishError("Clear Participant Data", 3, e.Message, _inputFileHistoryCode, null, null);
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

        #region Clear data         
        /// <summary>
        /// Copys the uploaded data to a validated area
        /// </summary>
        /// <param name="uploadDecision">The type of data to copy: 'ValidOnly','ValidAndInfo' or 'ValidInfoAndWarning'</param>
        /// <returns></returns>
        public int ClearUpload()
        {
             DeleteParticipantData();
            //UPDATE STATUS
            using (uow)
            {
                InputFileHistory ifh = inputFileHistoryRepository.Find(x => x.Code == _inputFileHistoryCode).First();
                ifh.Status = "Cancelled";
                ifh.UploadDecision = -1;
                inputFileHistoryRepository.Update(ifh);
            }

            DeleteInputFileErrorsOver1YearOld();
            return 0;
           
        }
        #endregion
    }
}