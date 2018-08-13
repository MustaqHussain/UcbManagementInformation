namespace UcbManagementInformation.Helpers.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows.Markup;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Collections;

    public class AsyncXamlConfigMerger
    {
        private readonly ConfigurationDictionary mergedConfiguration = new ConfigurationDictionary();
        private readonly List<string> pendingDownloads = new List<string>();
        private bool complete;

        public event EventHandler<EnterpriseLibraryConfigurationCompletedEventArgs> DownloadComplete;

        public void AddXamlFilesToDownload(IEnumerable<Uri> filesToDownload)
        {
            var files = filesToDownload.ToList();
            this.complete = false;
            lock (this.pendingDownloads)
            {
                this.pendingDownloads.AddRange(files.Select(x => x.ToString()));
                this.complete = false;
            }

            foreach (Uri file in files)
            {
                var webClient = new WebClient();
                webClient.DownloadStringCompleted += OnDownloadCompleted;
                webClient.DownloadStringAsync(file, file.ToString());
            }
        }

        private void AddCompletedFile(string fileName, string xamlFileContent)
        {
            Exception exception = null;
            lock (this.pendingDownloads)
            {
                this.pendingDownloads.Remove(fileName);
                MergeXamlFile(xamlFileContent);

                if (this.pendingDownloads.Count == 0)
                {
                    this.complete = true;
                    try
                    {
                        ConfigureEnterpriseLibrary();
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                }
            }

            if (this.complete)
            {
                var handler = this.DownloadComplete;
                if (handler != null)
                {
                    handler(this, new EnterpriseLibraryConfigurationCompletedEventArgs(exception, null));
                }
            }
        }

        private void MergeXamlFile(string xamlFileContent)
        {
            var content = (IDictionary)XamlReader.Load(xamlFileContent);
            foreach (var key in content.Keys.OfType<string>())
            {
                var section = content[key] as ConfigurationSection;
                if (section != null)
                {
                    this.mergedConfiguration.Add(key, section);
                }
            }
        }

        private void ConfigureEnterpriseLibrary()
        {
            var configSource = DictionaryConfigurationSource.FromDictionary(this.mergedConfiguration);
            EnterpriseLibraryContainer.Current = EnterpriseLibraryContainer.CreateDefaultContainer(configSource);
        }

        private void OnDownloadCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw e.Error;
            }

            AddCompletedFile(e.UserState as string, e.Result);
        }
    }
}