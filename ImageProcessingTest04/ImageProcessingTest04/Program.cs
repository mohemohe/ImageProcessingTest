using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;

namespace ImageProcessingTest04
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var source = new VideoCapture(0))
                {
                    source.Open(0);
                    var mat = new Mat();
                    var count = 1;
                    while (true)
                    {
                        if (count > 30)
                        {
                            break;
                        }

                        source.Read(mat);
                        if (!mat.Empty())
                        {
                            var image = new BenriImage(mat);
                            SaveImage(ref image, ImageFormat.Jpeg, "capture." + count + ".jpg");
                            Console.WriteLine("captured: " + count );
                            count++;
                        }
                        else
                        {
                            Console.WriteLine("frame empty");
                        }
                    }
                    source.Release();
                }
            }
            catch
            {
                Console.WriteLine("webcam is missing");
            }
            Console.WriteLine("press any key to exit");
            Console.ReadKey(false);
        }

        private static void SaveImage(ref BenriImage image, ImageFormat format = null,  string filePath = null)
        {
            if (filePath == null)
            {
                var ext = Path.GetExtension(image.FilePath);
                filePath = image.FilePath.Remove(image.FilePath.Length - ext.Length) + @"." + DateTime.Now.Ticks + ext;
            }
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            if (format != null)
            {
                image.Format = format;
            }
            image.Bitmap.Save(filePath, image.Format);
        }
    }

    class BenriImage
    {
        public Bitmap Bitmap { get; set; }
        public Mat Mat
        {
            get
            {
                return BitmapConverter.ToMat(Bitmap);
            }
            set
            {
                Bitmap = BitmapConverter.ToBitmap(value);
            }
        }
        public ImageFormat Format { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public string FilePath { get; private set; }

        public BenriImage(Bitmap bitmap)
        {
            Bitmap = bitmap;
            Format = bitmap.RawFormat;
            Width = bitmap.Width;
            Height = bitmap.Height;
        }

        public BenriImage(Bitmap bitmap, string filePath)
        {
            Bitmap = bitmap;
            Format = bitmap.RawFormat;
            Width = bitmap.Width;
            Height = bitmap.Height;
            FilePath = filePath;
        }

        public BenriImage(Mat matrix)
        {
            Mat = matrix;
            Format = ImageFormat.Bmp;
            Width = Bitmap.Width;
            Height = Bitmap.Height;
        }
    }
}
