using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingTest05.Models
{
    class BenriImage
    {
        public Bitmap Bitmap { get; set; }
        public Mat Mat
        {
            get
            {
                return BitmapConverter.ToMat(new Bitmap(Bitmap));
            }
            set
            {
                Bitmap = BitmapConverter.ToBitmap(value);
            }
        }
        public IplImage IplImage
        {
            get
            {
                return BitmapConverter.ToIplImage(new Bitmap(Bitmap));
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

        public BenriImage(IplImage iplImage)
        {
            IplImage = iplImage;
            Format = ImageFormat.Bmp;
            Width = Bitmap.Width;
            Height = Bitmap.Height;
        }
    }
}
