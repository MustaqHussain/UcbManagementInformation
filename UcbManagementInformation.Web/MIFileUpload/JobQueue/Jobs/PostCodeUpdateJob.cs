using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using UcbManagementInformation.Server.DataAccess;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Schema;
using System.Reflection;

namespace UcbManagementInformation.Web.MIFileUpload.JobQueue.Jobs
{
    public class PostCodeUpdateJob : JobBase
    {
        public PostCodeUpdateJob()
        { }
        IEnhancedDataReader reader = null;
            
        public PostCodeUpdateJob(Guid code, string userID, string webPath, DateTime? added, DateTime? started, DateTime? ended, string data)
        {
            Code = code;
            AddedTime = added;
            StartTime = started;
            EndTime = ended;
            Data = data;
            _uniqueFilename = Data.Split('|');
            //Get Connection string from configuration    
            _connectionString = ConfigurationManager.ConnectionStrings["MIStagingConnectionString"].ConnectionString;

            //set private variables
            SystemParameterRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<MCSystemParameter>>();

            _userID = userID;
            UserId = userID;
            JobSteps = new List<JobStep>()
            {
                new JobStep("DeleteOldFilesFromLoadingArea","DeleteFromLoadingArea",this,1),
                //new JobStep("BulkUploadWPC","Bulk Load File",this,2),
                //new JobStep("BulkUploadLA","Bulk Load File",this,3),
                //new JobStep("BulkUploadCty","Bulk Load File",this,4),
                //new JobStep("BulkUploadLEA","Bulk Load File",this,5),
                //new JobStep("BulkUploadRegion","Bulk Load File",this,6),
                //new JobStep("BulkUploadWARD","Bulk Load File",this,7),
                new JobStep("BulkUploadPC","Bulk Load File",this,8),
                //new JobStep("UpdateWPC","Bulk Load File",this,9),
                //new JobStep("UpdateLA","Bulk Load File",this,10),
                //new JobStep("UpdateLEA","Bulk Load File",this,11),
                //new JobStep("UpdateRegion","Bulk Load File",this,12),
                //new JobStep("UpdateWARD","Bulk Load File",this,13),
                //new JobStep("UpdateCty","Bulk Load File",this,14),
                //new JobStep("UpdatePC","Bulk Load File",this,15),
                //new JobStep("UpdatePostCodes","Update PostCode",this,16),
                new JobStep("DeletePreviousFromMI","Copy post codes",this,17),
                new JobStep("CopyToMI","Copy post codes",this,18)
            };
        }
        string[] _uniqueFilename;
        string _connectionString;
        long _fileLength;
        int _recordCount;
        int _flatRecordCount;
        string _userID;
        int _errorCode;
        int _recordsSoFar;

        int _truncatePostCodeDataTimeout;
        int _truncateMIPostCodeDataTimeout;
        int _populatePostCodesTimeout;
        MIStagingStoredProcedures spRunner = new MIStagingStoredProcedures();
        UcbManagementInformationStoredProcedures spRunner2 = new UcbManagementInformationStoredProcedures();
        IUcbManagementInformationRepository<MCSystemParameter> SystemParameterRepository;
        public string UniqueFileName { get; set; }
        
       
        
        
        public override int  RunJob()
        {
 	
            try
            {
                _truncatePostCodeDataTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("TruncatePostCodeDataTimeout"));
                _truncateMIPostCodeDataTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("TruncateMIPostCodeDataTimeout"));
                _populatePostCodesTimeout = Convert.ToInt32(DataAccessUtilities.GetSystemParameterByName("PopulatePostCodesTimeout"));
                int jobReturnCode = UploadFiles();
                return jobReturnCode;
            }
            catch (Exception e)
            {
                if (CurrentStep != null)
                {
                    CurrentStep.Finish(false);
                }
                UnhandledException = e;
                //Log error to db for debug purposes.

                return 1;
            }
        }

        #region UploadFile : ************************** Main flow of the code ****************************
        /// <summary>
        /// Executes a validation and bulk upload run for the file
        /// </summary>
        /// <param name="historyFileGuid">The unique identifier for the upload attempt for the file</param>
        /// <returns>An integer indicating outcome of the upload. 0=Success</returns>
        public int UploadFiles()
        {
            string prefix=@"c:\MyUploads\";
            //Delete existing from Loading area
            DeleteFromLoadingArea();

            ////Bulk Load the PArliamentary Constituencies data
            //LoadSql("Constituency",prefix + _uniqueFilename[0], _connectionString);
            
            ////Bulk Load the Local Authority data
            //LoadSql("LA", prefix + _uniqueFilename[1], _connectionString);
            
            ////Bulk Load the County data
            //LoadSql("County", prefix + _uniqueFilename[2], _connectionString);
            
            ////Bulk Load the LEA data
            //LoadSql("LEA", prefix + _uniqueFilename[3], _connectionString);
            
            ////Bulk Load the Nuts1 data
            //LoadSql("Region", prefix + _uniqueFilename[4], _connectionString);
            
            ////Bulk Load the Ward data
            //LoadSql("Ward", prefix + _uniqueFilename[5], _connectionString);
            
            //Bulk Load the PostCode data
            LoadSql("PostCode", prefix + _uniqueFilename[6], _connectionString);

            //UpdatePostCodeFromLookUps();

            //Delete Previous from MI Tables
            TruncateMIPostCodeData();
            
            //Copy to MI tables
            PopulatePostCodes();
                    
            

            return 0;
            
        }
        #endregion
        #region Clear Loading area
        /// <summary>
        /// Removes old data for the current file from the loading area.
        /// </summary>
        private void DeleteFromLoadingArea()
        {
            CurrentStep = this["DeleteOldFilesFromLoadingArea"];
            CurrentStep.Timeout = _truncatePostCodeDataTimeout;
            CurrentStep.Start();
            CurrentStep.UpdateProgress(0,"Deleting previous post code files from loading area");
            MIStagingStoredProcedures spRunner = new MIStagingStoredProcedures();
            spRunner.TruncatePostCodeData(_truncatePostCodeDataTimeout);
            
            CurrentStep.Finish(true);

        }
        #endregion

        private void TruncateMIPostCodeData()
        {
            CurrentStep = this["DeletePreviousFromMI"];
            CurrentStep.Timeout = _truncateMIPostCodeDataTimeout;
            CurrentStep.Start();
            CurrentStep.UpdateProgress(0, "Deleting previous post code files from MI");
            MIStagingStoredProcedures spRunner = new MIStagingStoredProcedures();
            spRunner.TruncateMIPostCodeData(_truncateMIPostCodeDataTimeout);
            CurrentStep.Finish(true);
        }

        private void PopulatePostCodes()
        {
            CurrentStep = this["CopyToMI"];
            CurrentStep.Timeout = _populatePostCodesTimeout;
            CurrentStep.Start();
            CurrentStep.UpdateProgress(0, "copy post code data to MI");
            MIStagingStoredProcedures spRunner = new MIStagingStoredProcedures();
            spRunner.PopulatePostCodes(_populatePostCodesTimeout);
            CurrentStep.Finish(true);
        }

        #region Business Validation
        


        /// <summary>
        /// Validates participant type files for business rules
        /// </summary>
        private void UpdatePostCodeFromLookUps()
        {
            //CurrentStep = this["UpdateWPC"];
            //CurrentStep.Start();
            //CurrentStep.UpdateProgress(0, "Updating Constituencies");
            //spRunner.UpdateConstituency();
            //CurrentStep.Finish(true);

            //CurrentStep = this["UpdateLA"];
            //CurrentStep.Start();
            //CurrentStep.UpdateProgress(0, "Updating Local Authorities");
            //spRunner.UpdateLocalAuthority();
            //CurrentStep.Finish(true);

            //CurrentStep = this["UpdateRegion"];
            //CurrentStep.Start();
            //CurrentStep.UpdateProgress(0, "Updating Regions");
            //spRunner.UpdateRegion();
            //CurrentStep.Finish(true);

            //CurrentStep = this["UpdateWARD"];
            //CurrentStep.Start();
            //CurrentStep.UpdateProgress(0, "Updating Wards");
            //spRunner.UpdateWard();
            //CurrentStep.Finish(true);

            //CurrentStep = this["UpdateLEA"];
            //CurrentStep.Start();
            //CurrentStep.UpdateProgress(0, "Updating Local Education Authorities");
            //spRunner.UpdateLea();
            //CurrentStep.Finish(true);

            //CurrentStep = this["UpdateCty"];
            //CurrentStep.Start();
            //CurrentStep.UpdateProgress(0, "Updating Counties");
            //spRunner.UpdateCounty();
            //CurrentStep.Finish(true);

            //CurrentStep = this["UpdatePC"];
            //CurrentStep.Start();
            //CurrentStep.UpdateProgress(0, "Updating PostCodes");
            //spRunner.UpdateConstituency();
            //CurrentStep.Finish(true);
 
            
        }

       


        #endregion

      

        #region Bulk Upload
        private void LoadSql(string category,string uniqueFilename, string connectionString)
        {
            List<MCSystemParameter> spList = new List<MCSystemParameter>(SystemParameterRepository.GetAll());
            int ConsFileKeyColumn = Convert.ToInt32(spList.Find(x=>x.Name == "ConsFileKeyColumn").ParameterValue);
            int ConsFileNameColumn = Convert.ToInt32(spList.Find(x=>x.Name == "ConsFileNameColumn").ParameterValue);
            int WardFileKeyColumn = Convert.ToInt32(spList.Find(x=>x.Name == "WardFileKeyColumn").ParameterValue);
            int WardFileNameColumn = Convert.ToInt32(spList.Find(x=>x.Name == "WardFileNameColumn").ParameterValue);
            int LAFileKeyColumn = Convert.ToInt32(spList.Find(x=>x.Name == "LAFileKeyColumn").ParameterValue);
            int LAFileNameColumn = Convert.ToInt32(spList.Find(x=>x.Name == "LAFileNameColumn").ParameterValue);
            int LEAFileKeyColumn = Convert.ToInt32(spList.Find(x=>x.Name == "LEAFileKeyColumn").ParameterValue);
            int LEAFileNameColumn = Convert.ToInt32(spList.Find(x=>x.Name == "LEAFileNameColumn").ParameterValue);
            int RegionFileKeyColumn = Convert.ToInt32(spList.Find(x=>x.Name == "RegionFileKeyColumn").ParameterValue);
            int RegionFileNameColumn = Convert.ToInt32(spList.Find(x=>x.Name == "RegionFileNameColumn").ParameterValue);
            int CountyFileKeyColumn = Convert.ToInt32(spList.Find(x=>x.Name == "CountyFileKeyColumn").ParameterValue);
            int CountyFileNameColumn = Convert.ToInt32(spList.Find(x=>x.Name == "CountyFileNameColumn").ParameterValue);
            int PostCode7Column = Convert.ToInt32(spList.Find(x=>x.Name == "PostCode7Column").ParameterValue);
            int PostCode8Column = Convert.ToInt32(spList.Find(x=>x.Name == "PostCode8Column").ParameterValue);
            int PostCodeCountyColumn = Convert.ToInt32(spList.Find(x=>x.Name == "PostCodeCountyColumn").ParameterValue);
            int PostCodeLAColumn = Convert.ToInt32(spList.Find(x=>x.Name == "PostCodeLAColumn").ParameterValue);
            int PostCodeWardColumn = Convert.ToInt32(spList.Find(x=>x.Name == "PostCodeWardColumn").ParameterValue);
            int PostCodeRegionColumn = Convert.ToInt32(spList.Find(x=>x.Name == "PostCodeRegionColumn").ParameterValue);
            int PostCodeConsColumn = Convert.ToInt32(spList.Find(x=>x.Name == "PostCodeConsColumn").ParameterValue);
            int PostCodeLEAColumn = Convert.ToInt32(spList.Find(x=>x.Name == "PostCodeLEAColumn").ParameterValue);
            int PostCodeEastingColumn = Convert.ToInt32(spList.Find(x=>x.Name == "PostCodeEastingColumn").ParameterValue);
            int PostCodeNorthingColumn = Convert.ToInt32(spList.Find(x=>x.Name == "PostCodeNorthingColumn").ParameterValue);
            reader = null;
            SqlBulkCopy Copier = new SqlBulkCopy(connectionString);
            string prefix=@"c:\MyUploads\";
     
            switch (category)
            {
                //case "LEA":
                //    CurrentStep = this["BulkUploadLEA"];
                //    Copier.DestinationTableName = "dbo.ImportLookUp";
                //    reader = new LookUpDataReader(category,uniqueFilename,0,1);
                //    break;
                //case "LA":
                //    CurrentStep = this["BulkUploadLA"];
                //    Copier.DestinationTableName = "dbo.ImportLookUp";
                //    reader = new LookUpDataReader(category,uniqueFilename,0,2);
                //    break;
                //case "Ward":
                //    CurrentStep = this["BulkUploadWARD"];
                //    Copier.DestinationTableName = "dbo.ImportLookUp";
                //    reader = new LookUpDataReader(category,uniqueFilename,0,2);
                //    break;
                //case "Constituency":
                //    CurrentStep = this["BulkUploadWPC"];
                //    Copier.DestinationTableName = "dbo.ImportLookUp";
                //    reader = new LookUpDataReader(category,uniqueFilename,0,2);
                //    break;
                //case "Region":
                //    CurrentStep = this["BulkUploadRegion"];
                //    Copier.DestinationTableName = "dbo.ImportLookUp";
                //    reader = new LookUpDataReader(category,uniqueFilename,0,2);
                //    break;
                //case "County":
                //    CurrentStep = this["BulkUploadCty"];
                //    Copier.DestinationTableName = "dbo.ImportLookUp";
                //    reader = new LookUpDataReader(category,uniqueFilename,0,2);
                //    break;
                case "PostCode":
                    CurrentStep = this["BulkUploadPC"];
                    Copier.DestinationTableName = "dbo.ImportPostCode";
                    reader = new PostCodeDataReader(category,uniqueFilename,PostCodeLEAColumn,PostCodeWardColumn,PostCodeRegionColumn,PostCodeConsColumn,
                        PostCodeLAColumn,PostCodeCountyColumn,PostCode7Column,PostCode8Column,PostCodeEastingColumn,PostCodeNorthingColumn,
                        prefix+_uniqueFilename[1],LAFileKeyColumn,LAFileNameColumn,//LA
                        prefix+_uniqueFilename[3],LEAFileKeyColumn,LEAFileNameColumn,//LEA
                       prefix+_uniqueFilename[2],CountyFileKeyColumn,CountyFileNameColumn,//County
                       prefix+_uniqueFilename[5],WardFileKeyColumn,WardFileNameColumn,//Ward
                       prefix+_uniqueFilename[4],RegionFileKeyColumn,RegionFileNameColumn,//Region
                       prefix+_uniqueFilename[0],ConsFileKeyColumn,ConsFileNameColumn//Constituency
                       );
                    break;
            }

            
            CurrentStep.Start();
            CurrentStep.UpdateProgress(0, "Bulk uploading");

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
            decimal percentthrough = Convert.ToDecimal(reader.CurrentPosition) / Convert.ToDecimal(reader.FileLength) * 100;
            int percentInt = Convert.ToInt32(decimal.Truncate(percentthrough));
            CurrentStep.UpdateProgress(percentInt, "Bulk uploading");
        }

       

            

        

        #endregion
    }
}