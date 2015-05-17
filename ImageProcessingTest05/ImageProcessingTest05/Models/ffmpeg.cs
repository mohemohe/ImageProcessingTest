using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingTest05.Models
{
    class Ffmpeg
    {

        static  ProcessStartInfo info;
        static  Process ffm;

        public static void Start()
        {
            info = new ProcessStartInfo();
            info.FileName = @"ffmpeg.exe";
            info.Arguments = @"-f image2pipe -i pipe:.bmp -pix_fmt yuv420p -vcodec libx264 -crf 32 -preset fast -framerate 30 -bufsize 30000k -y capture.mp4";
            info.CreateNoWindow = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            ffm = new Process();
            ffm.StartInfo = info;
            ffm.Start();
        }

        public static void Write(Bitmap image)
        {
            image.Save(ffm.StandardInput.BaseStream, ImageFormat.Bmp);
        }

        public static void Close()
        {
            ffm.StandardInput.Flush();
            ffm.StandardInput.Close();
            ffm.Close();
        }
    }
}
