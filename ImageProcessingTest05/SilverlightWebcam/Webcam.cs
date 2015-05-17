using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightWebcam
{
    public class Webcam
    {
        public Webcam()
        {
            if (!(CaptureDeviceConfiguration.AllowedDeviceAccess || CaptureDeviceConfiguration.RequestDeviceAccess()))
            {
                throw new NotImplementedException();
            }
        }

        public List<VideoCaptureDevice> GetWebcamList()
        {
            return CaptureDeviceConfiguration.GetAvailableVideoCaptureDevices().ToList();
        }
    }
}
