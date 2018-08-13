using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using System.Threading;
using System.Linq.Expressions;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using System.IO;

namespace UcbManagementInformation.Silverlight.UnitTest.Helpers
{

    /// <summary> 
    /// A client only domain client holding entites to be returned by query/load operation. 
    /// </summary> 
    /// <remarks> 
    /// Note that the entites provided by this client are cloned objects of the entities registered 
    /// via the <see cref="M:AddEntity"/> method. Because the cloning uses the <see cref="DataContractSerializer"/> 
    /// to serialize the objects only the properties marked with the <see cref="System.Runtime.Serialization.DataMemberAttribute"/> 
    /// are copied. For any user defined statefull property of the entities the queries will return the  
    /// default values. 
    /// </remarks> 

    public abstract class FakeDomainClient : DomainClient
    {
        private SynchronizationContext syncContext;

        protected FakeDomainClient()
        {
            this.syncContext = SynchronizationContext.Current;
        }

        public abstract IQueryable<Entity> Query(EntityQuery query);

        public abstract IEnumerable<ChangeSetEntry> Submit(EntityChangeSet changeSet);

        public abstract object Invoke(InvokeArgs invokeArgs);

        protected override sealed IAsyncResult BeginInvokeCore(InvokeArgs invokeArgs, AsyncCallback callback, object userState)
        {
            object result = Invoke(invokeArgs);
            FakeInvokeAsyncResult asyncResult = new FakeInvokeAsyncResult
            {
                Callback = callback,
                AsyncState =  userState,
                Result = result
            };

            this.syncContext.Post(cb => ((FakeAsyncResult)cb).Complete(), asyncResult);
            return asyncResult;
        }

        protected override sealed InvokeCompletedResult EndInvokeCore(IAsyncResult asyncResult)
        {
            FakeInvokeAsyncResult localAsyncResult = (FakeInvokeAsyncResult)asyncResult;
            InvokeCompletedResult result = new InvokeCompletedResult(localAsyncResult.Result);
            return result; 
        }

        protected override sealed IAsyncResult BeginQueryCore(EntityQuery query, AsyncCallback callback, object userState)
        {
            var results = this.Query(query);
            //Debug.WriteLine(query.Query.ToString());
            var asyncResult = new FakeAsyncResult
            {
                Callback = callback,
                AsyncState = userState,
                Entities = results.ToArray(),
                TotalCount = results.Count()
            };

            this.syncContext.Post(cb => ((FakeAsyncResult)cb).Complete(), asyncResult);

            return asyncResult;
        }

        protected override sealed QueryCompletedResult EndQueryCore(IAsyncResult asyncResult)
        {
            var localAsyncResult = (FakeAsyncResult)asyncResult;
            return new QueryCompletedResult(
                    localAsyncResult.Entities,
                    localAsyncResult.IncludedEntities,
                    localAsyncResult.TotalCount,
                    new ValidationResult[0]);
        }

        protected override sealed IAsyncResult BeginSubmitCore(EntityChangeSet changeSet, AsyncCallback callback, object userState)
        {
            FakeAsyncResult asyncResult = null;

            try
            {
                IEnumerable<ChangeSetEntry> operationResults = this.Submit(changeSet);
                asyncResult = new FakeAsyncResult
                {
                    Callback = callback,
                    AsyncState = userState,
                    ChangeSet = changeSet,
                    OperationResults = operationResults
                };
            }
            catch (Exception error)
            {
                asyncResult = new FakeAsyncResult { Callback = callback, AsyncState = userState, Error = error };
            }

            this.syncContext.Post(
                o =>
                {
                    ((FakeAsyncResult)o).Complete();
                },
                asyncResult);

            return asyncResult;
        }

        protected override sealed SubmitCompletedResult EndSubmitCore(IAsyncResult asyncResult)
        {
            FakeAsyncResult localAsyncResult = (FakeAsyncResult)asyncResult;

            if (localAsyncResult.Error == null)
            {
                return new SubmitCompletedResult(localAsyncResult.ChangeSet, localAsyncResult.OperationResults);
            }
            else
            {
                throw localAsyncResult.Error;
            }
        }

        
    }
}