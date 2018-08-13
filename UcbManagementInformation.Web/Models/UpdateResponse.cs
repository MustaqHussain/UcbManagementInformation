using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UcbManagementInformation.Web.Models
{
    public class UpdateResponse
    {

        public bool IsSuccess {get;set;}
        public IEnumerable<ObjectConcurrencyHolder> ConcurrencyObjectList { get; set; }
    }
}