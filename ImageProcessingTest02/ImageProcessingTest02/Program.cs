using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace ImageProcessingTest02
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
                filePath = image.FilePath.Remove(image.FilePath.Length - ext.Length) + @"." + DateTime.Now.Ticks + ext;
            }
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            image.Bitmap.Save(filePath, image.Format);
        }

        private static unsafe void GrayScale(ref BenriImage image)
        {
            var bmp = image.Bitmap.LockBits(new Rectangle(Point.Empty, new Size(image.Width,image.Height)),
                                            ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var ptr = (byte *)(void *)bmp.Scan0;

            for (var y = 0; y < bmp.Height; ++y)
            {
                for (var x = 0; x < bmp.Width; ++x)
                {
                    var gray = (byte) (*(ptr + 2)*0.3 + *(ptr + 1)*0.59 + *(ptr)*0.11);
                    *(ptr + 2) = gray;
                    *(ptr + 1) = gray;
                    *(ptr) = gray;
                    ptr += 4;
                }
            }

            image.Bitmap.UnlockBits(bmp);
        }

        private static unsafe void GaussianBlur(ref BenriImage image)
        {
            var bitmap = (Bitmap)image.Bitmap.Clone();
            var rBmp = bitmap.LockBits(new Rectangle(Point.Empty, new Size(image.Width, image.Height)),
                                       ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var wBmp = image.Bitmap.LockBits(new Rectangle(Point.Empty, new Size(image.Width, image.Height)),
                                            ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            var rPtr = (byte*)(void*)rBmp.Scan0;
            var wPtr = (byte*)(void*)wBmp.Scan0;

            var width = rBmp.Width * 4;
            rPtr += width * 3 + 3 * 4;
            wPtr += width * 3 + 3 * 4;
            for (var y = 3; y < rBmp.Height - 3; ++y)
            {
                for (var x = 3; x < rBmp.Width - 3; ++x)
                {
                    // 汚い
                    var pxVal = *(rPtr - width * 2 - 8) * 1 + *(rPtr - width * 2 - 4) * 4  + *(rPtr - width * 2) * 6  + *(rPtr - width * 2 + 4) * 4  + *(rPtr - width * 2 + 8) * 1
                              + *(rPtr - width - 8)     * 4 + *(rPtr - width - 4)     * 16 + *(rPtr - width)     * 24 + *(rPtr - width + 4)     * 16 + *(rPtr - width + 8)     * 4
                              + *(rPtr - 8)             * 6 + *(rPtr - 4)             * 24 + *(rPtr)             * 36 + *(rPtr + 4)             * 24 + *(rPtr + 4)             * 6
                              + *(rPtr + width - 8)     * 4 + *(rPtr + width - 4)     * 16 + *(rPtr + width)     * 24 + *(rPtr + width + 4)     * 16 + *(rPtr + width + 8)     * 4
                              + *(rPtr + width * 2 - 8) * 1 + *(rPtr + width * 2 - 4) * 4  + *(rPtr + width * 2) * 6  + *(rPtr + width * 2 + 4) * 4  + *(rPtr + width * 2 + 8) * 1;
                    pxVal /= 256;
                    *(wPtr + 2) = (byte)pxVal;
                    *(wPtr + 1) = (byte)pxVal;
                    *(wPtr) = (byte)pxVal;
                    rPtr += 4;
                    wPtr += 4;
                }
                rPtr += 2 * 3 * 4;
                wPtr += 2 * 3 * 4;
            }

            image.Bitmap.UnlockBits(wBmp);
            bitmap.UnlockBits(rBmp);
        }

        private static unsafe void EdgeExtraction(ref BenriImage image)
        {
            var bitmap = (Bitmap)image.Bitmap.Clone();
            var rBmp = bitmap.LockBits(new Rectangle(Point.Empty, new Size(image.Width, image.Height)),
                                       ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var wBmp = image.Bitmap.LockBits(new Rectangle(Point.Empty, new Size(image.Width, image.Height)),
                                            ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            var rPtr = (byte*)(void*)rBmp.Scan0;
            var wPtr = (byte*)(void*)wBmp.Scan0;

            var width = rBmp.Width*4;
            rPtr += width + 4;
            wPtr += width + 4;
            for (var y = 1; y < rBmp.Height - 1; ++y)
            {
                for (var x = 1; x < rBmp.Width - 1; ++x)
                {
                    var pxVal = *(rPtr - width - 4) * -1 + *(rPtr - width) * -2 + *(rPtr - width + 4) * -1
                              + *(rPtr - 4)         * -2 + *(rPtr)         * 12 + *(rPtr + 4)         * -2
                              + *(rPtr + width - 4) * -1 + *(rPtr + width) * -2 + *(rPtr + width + 4) * -1;
                    if (pxVal < 0)
                    {
                        pxVal = -pxVal;
                    }
                    if (pxVal > 256)
                    {
                        pxVal = 255;
                    }
                    *(wPtr + 3) = 0xFF;
                    *(wPtr + 2) = (byte)pxVal;
                    *(wPtr + 1) = (byte)pxVal;
                    *(wPtr) = (byte)pxVal;
                    rPtr += 4;
                    wPtr += 4;
                }
                rPtr += 2 * 4;
                wPtr += 2 * 4;
            }

            image.Bitmap.UnlockBits(wBmp);
            bitmap.UnlockBits(rBmp);
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
