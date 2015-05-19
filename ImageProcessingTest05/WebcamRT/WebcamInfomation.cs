using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace WebcamRT
{
    public class WebcamInformation
    {
        public string Id;
        public bool IsDefault;
        public bool IsEnabled;
        public string Name;

        internal WebcamInformation(DeviceInformation information)
        {
            Id = information.Id;
            IsDefault = information.IsDefault;
            IsEnabled = information.IsEnabled;
            Name = information.Name;
        }
    }
}
