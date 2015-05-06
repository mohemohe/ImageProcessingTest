using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageProcessingTest01
{
    class Program
    {
        private static List<string> _filePathList = new List<string>();

        static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                if (File.Exists(arg))
                {
                    _filePathList.Add(arg);
                }
            }

            foreach (var filePath in _filePathList)
            {
                DoProcess(filePath);
                Console.WriteLine("processed : " + filePath);
            }

            Console.WriteLine("done!");
            
            Console.ReadKey(false);
        }

        private static void DoProcess(string filePath)
        {
            var image = ReadImage(filePath);
            GrayScale(ref image);
            SaveImage(ref image);
        }

        private static BenriImage ReadImage(string filePath)
        {
            BenriImage bi;
            using (var bitmap = new Bitmap(filePath))
            {
                bi = new BenriImage((Bitmap)bitmap.Clone(), filePath);
            }

            return bi;
        }

        private static void SaveImage(ref BenriImage image, string filePath = null)
        {
            if (filePath == null)
            {
                var ext = Path.GetExtension(image.FilePath);
                filePath = image.FilePath.Remove(image.FilePath.Length - ext.Length) + @".processed" + ext;
            }
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            image.Bitmap.Save(filePath, image.Format);
        }

        private static void GrayScale(ref BenriImage image)
        {
            for (var y = 0; y < image.Height; y++)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    var color = image.Bitmap.GetPixel(x, y);
                    var glay = (int)(color.R*0.3 + color.G*0.59 + color.B*0.11);
                    image.Bitmap.SetPixel(x, y, Color.FromArgb(color.A, glay, glay, glay));
                }
            }
        }
    }

    class BenriImage
    {
        public Bitmap Bitmap;
        public ImageFormat Format { get; private set; }
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
    }
}
