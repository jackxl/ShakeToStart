using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Collections.ObjectModel;

namespace ShakeToStart.Model
{
    [DataContract]
    public class UriItem
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public Uri uri { get; set; }
        [DataMember]
        public Symbol symbol { get; set; }

        //TODO: this thing
        //DataContractJsonSerializer
    }
}
