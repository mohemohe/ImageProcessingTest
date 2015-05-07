using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using System.Drawing;
using OpenCvSharp.Extensions;
using System.Drawing.Imaging;

namespace ImageProcessingTest03
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
            GaussianBlur(ref image);
            SaveImage(ref image);
            EdgeExtraction(ref image);
            SaveImage(ref image);
        }

        private static BenriImage ReadImage(string filePath)
        {
            BenriImage image;
            using (var bitmap = new Bitmap(filePath))
            {
                image = new BenriImage((Bitmap)bitmap.Clone(), filePath);
            }

            return image;
        }

        private static void SaveImage(ref BenriImage image, string filePath = null)
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
            image.Bitmap.Save(filePath, image.Format);
        }

        private static void GrayScale(ref BenriImage image)
        {
            var result = new Mat();
            Cv2.CvtColor(image.Mat, result, ColorConversion.RgbaToGray);
            image.Mat = result;
        }

        private static void GaussianBlur(ref BenriImage image)
        {
            var result = new Mat();
            Cv2.GaussianBlur(image.Mat, result, new OpenCvSharp.CPlusPlus.Size(5,5), 4);
            image.Mat = result;
        }

        private static void EdgeExtraction(ref BenriImage image)
        {
            var result = new Mat();
            Cv2.Laplacian(image.Mat, result, MatType.CV_8U);
            image.Mat = result;
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
