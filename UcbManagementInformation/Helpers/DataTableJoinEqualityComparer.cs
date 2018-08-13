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
using System.Collections;
using System.Collections.Generic;
using UcbManagementInformation.Server.DataAccess;

namespace UcbManagementInformation.Helpers
{
    public class DataTableJoinEqualityComparer : IEqualityComparer<DataTableJoin>
    {


        public bool Equals(DataTableJoin x, DataTableJoin y)
        {
            return x.Code == y.Code;
        }

        public int GetHashCode(DataTableJoin obj)
        {
            return obj.Code.ToString().ToLower().GetHashCode();

        }
    }
}
