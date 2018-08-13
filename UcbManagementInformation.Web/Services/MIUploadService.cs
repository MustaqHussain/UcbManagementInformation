
namespace UcbManagementInformation.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using UcbManagementInformation.Server.DataAccess;
    using UcbManagementInformation.Web.MIFileUpload.JobQueue;
    using UcbManagementInformation.Web.MIFileUpload.JobQueue.Jobs;
    using UcbManagementInformation.Server.DataAccess.BusinessObjects;


    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class MIUploadService : DomainService
    {
        private IUcbManagementInformationRepository<ProviderOrganisation> providerOrganisationRepository;
        private IUcbManagementInformationRepository<InputFileHistory> inputFileHistoryRepository;
        private IUcbManagementInformationRepository<Alert> alertRepository;
        private IUcbManagementInformationRepository<AlertUser> alertUserRepository;
        private IUcbManagementInformationRepository<MCUser> userRepository;
        private IUcbManagementInformationRepository<UploadJobQueue> uploadJobQueueRepository;

        private IUnitOfWork uow;

        public MIUploadService(IUnitOfWork uowItem, IUcbManagementInformationRepository<ProviderOrganisation> providerOrganisationRepositoryItem,
            IUcbManagementInformationRepository<InputFileHistory> inputFileHistoryRepositoryItem,IUcbManagementInformationRepository<Alert> alertRepositoryItem,
            IUcbManagementInformationRepository<AlertUser> alertUserRepositoryItem,IUcbManagementInformationRepository<MCUser> userRepositoryItem,
            IUcbManagementInformationRepository<UploadJobQueue> uploadJobQueueRepositoryItem)
        {
            uow = uowItem;
            providerOrganisationRepository = providerOrganisationRepositoryItem;
            inputFileHistoryRepository = inputFileHistoryRepositoryItem;
            alertRepository = alertRepositoryItem;
            alertUserRepository = alertUserRepositoryItem;
            userRepository = userRepositoryItem;
            uploadJobQueueRepository = uploadJobQueueRepositoryItem;
        }
        
        [Invoke]
        public Guid UploadFile(string ProviderKeyValue, string fileType, string FileName)
        {
            //_xmlFileLoad = new XmlSQLBulkUploadWrapper(KeyValue, _fileType, ConnectionString,
            //            FileToUploadTextBox.Text, Schema, MappingSchema, _providerOrganistaionCode);
            UploadJobQueue JobEntryToAdd = new UploadJobQueue()
            {
                Code = Guid.NewGuid(),
                JobDescription = "Bulk Load|" + ProviderKeyValue + @"|Participant|C:\MyUploads\" + FileName,
                Status = "Waiting",
                TimeAdded = DateTime.Now,
                UserId = this.ServiceContext.User.Identity.Name,
            };
            uploadJobQueueRepository.Add(JobEntryToAdd);

            Alert alertToAdd = new Alert() 
            {
                Code = Guid.NewGuid(),
                CreatedBy = this.ServiceContext.User.Identity.Name,
                CreatedDate = DateTime.Now,
                Name = "Alert for" + JobEntryToAdd.JobDescription.Substring(0,JobEntryToAdd.JobDescription.LastIndexOf("|")),
                RelatedEntityCode = JobEntryToAdd.Code,
                Status="Waiting",
                Type="Job"
            };
            alertRepository.Add(alertToAdd);

            alertUserRepository.Add(new AlertUser()
            {
                Code = Guid.NewGuid(),
                AlertCode = alertToAdd.Code,
                UserMessageStatus = "NotRead",
                UserCode = userRepository.First(x => x.Name == this.ServiceContext.User.Identity.Name).Code
            });

            uow.Commit();
            return JobEntryToAdd.Code;
        }
        [Invoke]
        public Guid UploadPostCodeFiles(string FileNameWPC,string FileNameLA,string FileNameCty,string FileNameLEA,string FileNameNUTS1,string FileNameWD,
            string FileNamePC)
        {
            UploadJobQueue JobEntryToAdd = new UploadJobQueue()
            {
                Code = Guid.NewGuid(),
                JobData = FileNameWPC + "|" + FileNameLA + "|" + FileNameCty + "|" + FileNameLEA + "|" + FileNameNUTS1 + "|" + FileNameWD + "|" + FileNamePC,
                JobDescription = "PostCode update",
                Status = "Waiting",
                TimeAdded = DateTime.Now,
                UserId = this.ServiceContext.User.Identity.Name,
            };
            uploadJobQueueRepository.Add(JobEntryToAdd);

            Alert alertToAdd = new Alert()
            {
                Code = Guid.NewGuid(),
                CreatedBy = this.ServiceContext.User.Identity.Name,
                CreatedDate = DateTime.Now,
                Name = "Alert for" + JobEntryToAdd.JobDescription,
                RelatedEntityCode = JobEntryToAdd.Code,
                Status = "Waiting",
                Type = "Job"
            };
            alertRepository.Add(alertToAdd);

            alertUserRepository.Add(new AlertUser()
            {
                Code = Guid.NewGuid(),
                AlertCode = alertToAdd.Code,
                UserMessageStatus = "NotRead",
                UserCode = userRepository.First(x => x.Name == this.ServiceContext.User.Identity.Name).Code
            });

            uow.Commit();
            return JobEntryToAdd.Code;
        }
        [Invoke]
        public Guid LoadRecords(Guid InputFileHistory,string ProviderKeyValue, string decisionLevel)
        {
            UploadJobQueue JobEntryToAdd = new UploadJobQueue()
            {
                Code = Guid.NewGuid(),
                JobDescription = "Copy Valid|" + ProviderKeyValue + @"|Participant|" + InputFileHistory+"|"+decisionLevel,
                Status = "Waiting",
                TimeAdded = DateTime.Now,
                UserId = this.ServiceContext.User.Identity.Name,
            };
            uploadJobQueueRepository.Add(JobEntryToAdd);

            Alert alertToAdd = new Alert()
            {
                Code = Guid.NewGuid(),
                CreatedBy = this.ServiceContext.User.Identity.Name,
                CreatedDate = DateTime.Now,
                Name = "Alert for" + JobEntryToAdd.JobDescription.Substring(0, JobEntryToAdd.JobDescription.LastIndexOf("|")),
                RelatedEntityCode = JobEntryToAdd.Code,
                Status = "Waiting",
                Type = "Job"
            };
            alertRepository.Add(alertToAdd);

            alertUserRepository.Add(new AlertUser()
            {
                Code = Guid.NewGuid(),
                AlertCode = alertToAdd.Code,
                UserMessageStatus = "NotRead",
                UserCode = userRepository.First(x => x.Name == this.ServiceContext.User.Identity.Name).Code
            });

            uow.Commit();
            return JobEntryToAdd.Code;
        }
        [Invoke]
        public Guid CancelUpload(Guid InputFileHistory, string ProviderKeyValue)
        {
            UploadJobQueue JobEntryToAdd = new UploadJobQueue()
            {
                Code = Guid.NewGuid(),
                JobDescription = "Clear Area|" + ProviderKeyValue + @"|Participant|" + InputFileHistory,
                Status = "Waiting",
                TimeAdded = DateTime.Now,
                UserId = this.ServiceContext.User.Identity.Name,
            };
            uploadJobQueueRepository.Add(JobEntryToAdd);

            Alert alertToAdd = new Alert()
            {
                Code = Guid.NewGuid(),
                CreatedBy = this.ServiceContext.User.Identity.Name,
                CreatedDate = DateTime.Now,
                Name = "Alert for" + JobEntryToAdd.JobDescription.Substring(0, JobEntryToAdd.JobDescription.LastIndexOf("|")),
                RelatedEntityCode = JobEntryToAdd.Code,
                Status = "Waiting",
                Type = "Job"
            };
            alertRepository.Add(alertToAdd);

            alertUserRepository.Add(new AlertUser()
            {
                Code = Guid.NewGuid(),
                AlertCode = alertToAdd.Code,
                UserMessageStatus = "NotRead",
                UserCode = userRepository.First(x => x.Name == this.ServiceContext.User.Identity.Name).Code
            });

            uow.Commit();
            return JobEntryToAdd.Code;
        }

        [Invoke]
        public JobBase GetFileUploadJobByCode(Guid code)
        {
            var job = (FileUploadJob)JobQueue.Instance.FindJobByCode(code);
            if (job == null)
            {
                IUcbManagementInformationRepository<UploadJobQueue> uploadJobQueueRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<UploadJobQueue>>();
                var uploadjob = uploadJobQueueRepository.Find(x => x.Code == code, "JobSteps").FirstOrDefault();
                if (uploadjob != null)
                {
                    var descSplit = uploadjob.JobDescription.Split('|');
                    FileUploadJob jobToReturn = new FileUploadJob()
                    {
                        Code = uploadjob.Code,
                        ProviderKey = descSplit[1],
                        Status = (JobStatus)Enum.Parse(typeof(JobStatus), uploadjob.Status),
                        UniqueFileName = descSplit[3],
                        UserId = uploadjob.UserId,
                        JobSteps = new List<MIFileUpload.JobQueue.JobStep>()
                    };
                    foreach (UcbManagementInformation.Server.DataAccess.JobStep step in uploadjob.JobSteps.OrderBy(x=>x.StepOrder))
                    {
                        jobToReturn.JobSteps.Add(new MIFileUpload.JobQueue.JobStep(step.StartTime, step.EndTime, (JobStepStatus)Enum.Parse(typeof(JobStepStatus), step.Status))
                        {
                            Code = step.Code,
                            Category = step.Category,
                            Name = step.Name,
                            ParentJob = jobToReturn,
                            Order=step.StepOrder
                        });
                    }
                    return jobToReturn;
                }
                else
                {
                    return null;
                }
            }
            else
            { return job; }
           
        }

        JobBase Clone(JobBase job)
        {
            JobBase Clone = new JobBase();
            Clone.Code = job.Code;
            Clone.CurrentStep = job.CurrentStep;
            Clone.JobSteps = job.JobSteps;
            Clone.Status = job.Status;
            Clone.UserId = job.UserId;
            Clone.AddedTime = job.AddedTime;
            Clone.Description = job.Description;
            Clone.EndTime = job.EndTime;
            Clone.StartTime = job.StartTime;
            return Clone;
        }

        [Invoke]
        public IEnumerable<JobBase> GetActiveFileUploadJobs()
        {
            List<JobBase> jobList = new List<JobBase>();
            foreach (JobBase job in JobQueue.Instance.PendingJobQueue)
            {
                jobList.Add(Clone(job));
            }
            if (JobQueue.Instance.CurrentJob != null)
            {
                jobList.Insert(0, Clone(JobQueue.Instance.CurrentJob));
            }
            return jobList.ToArray().AsEnumerable();
        }
        [Invoke]
        public IEnumerable<JobBase> GetCompleteFileUploadJobs()
        {
            List<JobBase> listToReturn = new List<JobBase>();
            DateTime Yesterday = DateTime.Now.AddDays(-1);
            IUcbManagementInformationRepository<UploadJobQueue> uploadJobQueueRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<UploadJobQueue>>();
            var uploadjobList = uploadJobQueueRepository.Find(x => x.TimeAdded > Yesterday && (x.Status == "Succeeded" || x.Status == "Failed"), "JobSteps").OrderByDescending(x=>x.TimeStarted);
            foreach (var uploadjob in uploadjobList)
            {
                
                    var descSplit = uploadjob.JobDescription.Split('|');
                    JobBase jobToReturn = new JobBase()
                    {
                        Code = uploadjob.Code,
                        AddedTime= uploadjob.TimeAdded,
                        Description= uploadjob.JobDescription,
                        StartTime = uploadjob.TimeStarted,
                        EndTime=uploadjob.EndTime,
                        Status = (JobStatus)Enum.Parse(typeof(JobStatus), uploadjob.Status),
                        UserId = uploadjob.UserId,
                        JobSteps = new List<MIFileUpload.JobQueue.JobStep>()
                    };
                    foreach (UcbManagementInformation.Server.DataAccess.JobStep step in uploadjob.JobSteps.OrderBy(x=>x.StepOrder))
                    {
                        jobToReturn.JobSteps.Add(
                            new MIFileUpload.JobQueue.JobStep(step.StartTime, step.EndTime, (JobStepStatus)Enum.Parse(typeof(JobStepStatus), step.Status))
                            {
                                Code = step.Code,
                                Category = step.Category,
                                Name = step.Name,
                                ParentJob = jobToReturn,
                                Order = step.StepOrder
                            });
                    }
                    listToReturn.Add(jobToReturn);
                   
                
            }
            return listToReturn;
           // return listToReturn;
        }

        public IQueryable<ProviderOrganisation> GetProviderOrganisations()
        {
           return providerOrganisationRepository.GetAll("InputFileHistory").OrderBy(x=>x.KeyValue).AsQueryable();
        }

        public void InsertProviderOrganisation(ProviderOrganisation providerOrganisationItem)
        {
            providerOrganisationRepository.Add(providerOrganisationItem);
        }

        public void UpdateProviderOrganisation(ProviderOrganisation providerOrganisationItem)
        {
            providerOrganisationRepository.Update(providerOrganisationItem);
        }

        public void DeleteProviderOrganisation(ProviderOrganisation providerOrganisationItem)
        {
            providerOrganisationRepository.Delete(providerOrganisationItem);
        }

        public IQueryable<InputFileHistory> GetInputFileHistoryAndPreviousByProviderKey(string providerKey)
        {
            InputFileHistory ifh = inputFileHistoryRepository.Find(x => x.ProviderOrganisationKeyValue == providerKey && x.Status == "Business Validated").OrderByDescending(x => x.LoadedDate).First();
            InputFileHistory previousIfh = inputFileHistoryRepository.Find(x => x.ProviderOrganisationKeyValue == ifh.ProviderOrganisationKeyValue &&
                x.LoadedDate <= ifh.LoadedDate && x.Status.StartsWith("Validated")).OrderByDescending(x => x.LoadedDate).FirstOrDefault();
            List<InputFileHistory> HistoryList = new List<InputFileHistory>();
            HistoryList.Add(ifh);
            if (previousIfh != null)
            {
                HistoryList.Add(previousIfh);
            }
           return HistoryList.AsQueryable();
        }
        
        public IQueryable<InputFileErrorSummary> GetInputFileErrorsSummary(Guid uploadFileHistoryCode)
        {
            UcbManagementInformationStoredProcedures mcProc = new UcbManagementInformationStoredProcedures();
            InputFileErrorSummary summary = new InputFileErrorSummary();
            int FileLevelInfo = 0;
            int FileLevelWarning = 0;
            int FileLevelError = 0;
            int RecordLevelInfo = 0;
            int RecordLevelWarning = 0;
            int RecordLevelError = 0;

            mcProc.InputFileErrorsSummary(uploadFileHistoryCode, out FileLevelInfo, out FileLevelWarning, out FileLevelError,
                out RecordLevelInfo, out RecordLevelWarning, out RecordLevelError);
            summary.Code = uploadFileHistoryCode;
            summary.FileLevelError = FileLevelError;
            summary.FileLevelInfo = FileLevelInfo;
            summary.FileLevelWarning = FileLevelWarning;
            summary.RecordLevelError = RecordLevelError;
            summary.RecordLevelInfo = RecordLevelInfo;
            summary.RecordLevelWarning = RecordLevelWarning;
            List<InputFileErrorSummary> summaryList = new List<InputFileErrorSummary>();
            summaryList.Add(summary);
            return summaryList.AsQueryable();
        }
        public IQueryable<InputFileHistory> GetInputFileHistories()
        {
            return inputFileHistoryRepository.GetAll().AsQueryable();
        }
        public IQueryable<InputFileHistory> GetInputFileHistoriesByProviderKey(string providerKey)
        {
            return inputFileHistoryRepository.Find(x => x.ProviderOrganisationKeyValue == providerKey).OrderByDescending(x=>x.LoadedDate).AsQueryable();
        }

        public void InsertProviderOrganisation(InputFileHistory inputFileHistoryItem)
        {
            inputFileHistoryRepository.Add(inputFileHistoryItem);
        }

        public void UpdateProviderOrganisation(InputFileHistory inputFileHistoryItem)
        {
            inputFileHistoryRepository.Update(inputFileHistoryItem);
        }

        public void DeleteProviderOrganisation(InputFileHistory inputFileHistoryItem)
        {
            inputFileHistoryRepository.Delete(inputFileHistoryItem);
        }
        protected override bool PersistChangeSet()
        {
            bool isWorking = base.PersistChangeSet();
            uow.Commit();
            return isWorking;
        }
    }
}


