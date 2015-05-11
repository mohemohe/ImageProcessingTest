﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using System.Drawing;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace ImageProcessingTest05.Models
{
    public class Model : NotificationObject, IDisposable
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        VideoCapture source;
        public Bitmap OriginalBitmap;
        public Bitmap GrayScaleBitmap;
        public Bitmap BlurBitmap;
        public Bitmap EdgeBitmap;

        public async void Initialize()
        {
            Ffmpeg.Start();
            try
            {
                source = new VideoCapture(0);
                source.Open(0);
                await Task.Run(() => StartProcessing());
            }
            catch { }
        }

        public void Dispose()
        {
            Ffmpeg.Close();
            if (source != null)
            {
                source.Release();
                source.Dispose();
            }
        }

        public void StartProcessing()
        {
            while (true)
            {
                BenriImage image;
                if (GetWebcamImage())
                {
                    image = new BenriImage(OriginalBitmap);
                    GrayScale(ref image);
                    GrayScaleBitmap = (Bitmap)image.Bitmap.Clone();

                    image = new BenriImage(OriginalBitmap);
                    GaussianBlur(ref image);
                    BlurBitmap = (Bitmap)image.Bitmap.Clone();

                    image = new BenriImage(OriginalBitmap);
                    EdgeExtraction(ref image);
                    EdgeBitmap = (Bitmap)image.Bitmap.Clone();

                    var movImage = CreateImage();
                    Ffmpeg.Write(ref movImage);


                    RaisePropertyChanged();
                }
                Thread.Sleep(1);
            }
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
            Cv2.GaussianBlur(image.Mat, result, new OpenCvSharp.CPlusPlus.Size(5, 5), 4);
            image.Mat = result;
        }

        private static void EdgeExtraction(ref BenriImage image)
        {
            var result = new Mat();
            Cv2.Laplacian(image.Mat, result, MatType.CV_8U);
            image.Mat = result;
        }

        private bool GetWebcamImage()
        {
            var mat = new Mat();

            source.Read(mat);
            var updated = !mat.Empty();
            if (updated)
            {
                var image = new BenriImage(mat);
                OriginalBitmap = (Bitmap)image.Bitmap.Clone();
            }

            return updated;
        }

        private Bitmap CreateImage()
        {
            var bitmap = new Bitmap(OriginalBitmap.Width * 2, OriginalBitmap.Height * 2);
            var tmpBitmap = Graphics.FromImage(bitmap);
            tmpBitmap.DrawImage(OriginalBitmap, new System.Drawing.Point(0, 0));
            tmpBitmap.DrawImage(GrayScaleBitmap, new System.Drawing.Point(OriginalBitmap.Width, 0));
            tmpBitmap.DrawImage(BlurBitmap, new System.Drawing.Point(0, OriginalBitmap.Height));
            tmpBitmap.DrawImage(EdgeBitmap, new System.Drawing.Point(OriginalBitmap.Width, OriginalBitmap.Height));

            return bitmap;
        }

        private static void SaveImage(ref BenriImage image, ImageFormat format = null, string filePath = null)
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
}