using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel.DomainServices.Client;
using System.Collections.Generic;
using System.Threading;

namespace UcbManagementInformation.Silverlight.UnitTest.Helpers
{
    public class FakeAsyncResult : IAsyncResult
    {
        private readonly object asyncState;

        public FakeAsyncResult(object asyncState)
        {
            this.asyncState = asyncState;
        }

        public FakeAsyncResult()
        {
            Entities = new Entity[0];
            IncludedEntities = new Entity[0];
            OperationResults = new ChangeSetEntry[0];
        }
        public EntityChangeSet ChangeSet { get; set; }

        public IEnumerable<ChangeSetEntry> OperationResults { get; set; }

        public IEnumerable<Entity> Entities { get; set; }

        public IEnumerable<Entity> IncludedEntities { get; set; }

        public int TotalCount { get; set; }

        public Exception Error { get; set; }

        public object AsyncState { get; set; }

        public WaitHandle AsyncWaitHandle { get; set; }

        public bool CompletedSynchronously { get; set; }

        public bool IsCompleted { get; set; }

        public AsyncCallback Callback { get; set; }

        public void Complete()
        {
            IsCompleted = true;
            if (Callback != null) Callback(this);
        }
    }

}
