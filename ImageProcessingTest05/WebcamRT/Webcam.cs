using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;

namespace WebcamRT
{
    public class Webcam
    {
        private bool _initialized = false;
        private readonly MediaCapture _capture;
        public List<WebcamInformation> Devices { get; set; }

        public Webcam()
        {
            if (!IsSupportedOS())
            {
                throw new PlatformNotSupportedException();
            }

            try
            {
                _capture = new MediaCapture();
            }
            catch
            {
                throw new NotSupportedException();
            }

            GetDeviceList();
            if (Devices.Count == 0)
            {
                throw new DeviceNotFoundException();
            }
        }

        private bool IsSupportedOS()
        {
            var os = Environment.OSVersion;

            if (os.Platform == PlatformID.Win32NT)
            {
                //TODO: ちゃんとOSバージョンが帰ってくるようにする
                if (os.Version.Major >= 6 && os.Version.Minor >= 3)
                {
                    return true;
                }
            }
            return false;
        }

        private void GetDeviceList()
        {
            var task = Task.Factory.StartNew(() =>
            {
                var asyncOperation = DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
                var devices = asyncOperation.GetAwaiter().GetResult();
                var deviceList = devices.Select(device => new WebcamInformation(device)).ToList();
                Devices = new List<WebcamInformation>(deviceList);
            });
            task.Wait();
        } 

        public bool Initialize(string videoDeviceId, string audioDeviceId = null)
        {
            var settings = new MediaCaptureInitializationSettings();
            settings.VideoDeviceId = videoDeviceId;
            if (audioDeviceId != null)
            {
                settings.AudioDeviceId = audioDeviceId;
            }

            try
            { 
                _capture.InitializeAsync(settings).AsTask().Wait();
                _initialized = true;

                return true;
            }
            catch
            {
                return true;
            }
        }

        public async Task<Bitmap> GetBitmap()
        {
            if (!_initialized)
            {
                throw new DeviceNotInitializedException();
            }

            MemoryStream stream = new MemoryStream();
            using (var ras = new InMemoryRandomAccessStream())
            {
                //MEMO: LowLagPhotoCapture なるものがあるらしい　https://msdn.microsoft.com/library/windows/apps/dn278811
                await _capture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreatePng(), ras);
                ras.Seek(0);
                ras.AsStream().CopyTo(stream);
            }
            var bitmap = new Bitmap(stream);
            
            return bitmap;
        }
    }
}
