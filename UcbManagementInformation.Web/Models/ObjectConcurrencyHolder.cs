using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace UcbManagementInformation.Web.Models
{
    public class ObjectConcurrencyHolder
    {
        [Key]
        public Guid Key { get; set; }
        public byte[] RowIdentifier { get; set; }
        public string Type { get; set; }
    }
}