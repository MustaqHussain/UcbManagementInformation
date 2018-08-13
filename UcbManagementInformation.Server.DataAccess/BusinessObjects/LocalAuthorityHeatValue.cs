using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace UcbManagementInformation.Server.DataAccess.BusinessObjects
{
    [Serializable()]
    public class LocalAuthorityHeatValue
    {
        [KeyAttribute]
        public string LocalAuthorityName { get; set; }

        public int HeatValue { get; set; }

        public LocalAuthorityHeatValue()
        {
        }
    }
}
