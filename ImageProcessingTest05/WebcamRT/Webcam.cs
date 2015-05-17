using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Capture;

namespace WebcamRT
{
    public class Webcam
    {
        private MediaCapture _capture;

        public Webcam()
        {
            if (!CheckOSVersion())
            {
                throw new PlatformNotSupportedException();
            }

            Task.Factory.StartNew(() => Initialize().Result)
                        .ContinueWith(task =>
            {
                if (task.Result)
                {
                    throw new NotSupportedException();
                }
            });
        }

        private bool CheckOSVersion()
        {
            var os = Environment.OSVersion;

            if (os.Platform == PlatformID.Win32NT)
            {
                if (os.Version.Major >= 6 && os.Version.Minor >= 2)
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> Initialize()
        {
            _capture = new MediaCapture();
            var a = DeviceInformation.FindAllAsync(DeviceClass.VideoCapture).GetResults();
            _capture.InitializeAsync();
            return true;
        }
    }
}
