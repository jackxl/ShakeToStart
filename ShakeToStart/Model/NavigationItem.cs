using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ShakeToStart.Model
{
    public class NavigationItem
    {
        public string name { get; set; }
        public Symbol symbol { get; set; }
        public Type pageFrame { get; set; }
    }
}
