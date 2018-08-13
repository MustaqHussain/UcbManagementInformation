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
using System.Collections.Generic;
namespace UcbManagementInformation.Helpers
{
    public class SessionManager<TKey,TValue>
    {
        private Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
        public void Add(TKey key,TValue value)
        {
            Update(key, value);
        }
        public void Update(TKey key, TValue value)
        {
            if (dic.ContainsKey(key))
            {
                dic.Remove(key);
            }
            dic.Add(key, value);
            
        }
        public bool Remove(TKey key)
        {
            if (dic.ContainsKey(key))
            {
                return dic.Remove(key);
            }
            return true;
        }
        public TValue Get(TKey key)
        {
            if (!dic.ContainsKey(key))
            {return default(TValue);}
            return dic[key];
        }
        public TValue this[TKey key]
        {
            get { return Get(key); }
            set { Update(key, value); }
        }
    }
    public enum SessionKey 
    { 
        AgreementCode = 1,
        SearchCriteria = 2,
        CurrentReportCode =3,
        UploadFileType =4,
        UploadProviderKey=5,
        JobCode=6,
        FileUploadHistoryCode=7
    }
}
