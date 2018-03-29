using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Drawing;
using System.Diagnostics;

namespace methodcore
{
    public class FaceCompare
    {
        public static Bitmap emguHaarDetect(string imgFile)
        {
            Image<Bgr, byte> img = new Image<Bgr, byte>(imgFile);
            HaarCascade haar = new HaarCascade(haarXmlPath);
            if (haar == null || img == null) return null;
            MCvAvgComp[] faces = haar.Detect(img.Convert<Gray, byte>(), 1.4, 1, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));
            if (faces.Length > 0)
            {
                foreach (MCvAvgComp face in faces)
                {
                    img.Draw(face.rect, new Bgr(Color.Yellow), 2);
                }
                return img.ToBitmap();
            }
            else {
                return null;
            }
        }
        public static Bitmap emguHaarDetect(Bitmap bt)
        {
            Image<Bgr, byte> img = new Image<Bgr, byte>(bt);
            HaarCascade haar = new HaarCascade(haarXmlPath);
            if (haar == null || img == null) return null;
            MCvAvgComp[] faces = haar.Detect(img.Convert<Gray, byte>(), 1.4, 1, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));
            if (faces.Length > 0)
            {
                foreach (MCvAvgComp face in faces)
                {
                    img.Draw(face.rect, new Bgr(Color.Yellow), 2);
                }
                return img.ToBitmap();
            }
            else
            {
                return null;
            }
        }
        public static double compare(string imgFile1, string imgFile2)
        {
            return 0.0;
        }
        private static string haarXmlPath = @"haarcascade_frontalface_alt_tree.xml";
        public static string  FaceSimilarity(string imgFile1,string imgFile2){
            HaarCascade haar = new HaarCascade(haarXmlPath);
            int[] hist_size = new int[1] { 256 };//建一个数组来存放直方图数据
            //IntPtr img1 = CvInvoke.cvLoadImage("", Emgu.CV.CvEnum.LOAD_IMAGE_TYPE.CV_LOAD_IMAGE_ANYCOLOR); //根据路径导入图像

            //准备轮廓  
            Image<Bgr, Byte> image1 = new Image<Bgr, byte>(imgFile1);
            Image<Bgr, Byte> image2 = new Image<Bgr, byte>(imgFile2);
            MCvAvgComp[] faces = haar.Detect(image1.Convert<Gray, byte>(), 1.4, 1, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));
            MCvAvgComp[] faces2 = haar.Detect(image2.Convert<Gray, byte>(), 1.4, 1, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));
            int l1 = faces.Length;
            int l2 = faces2.Length;
            double compareResult=0.0;
            double time=0.0;
            if (l1 > 0 && l2 > 0)
            {
                image1 = image1.Copy(faces[0].rect);
                image2 = image2.Copy(faces2[0].rect);
                Image<Gray, Byte> imageGray1 = image1.Convert<Gray, Byte>();
                Image<Gray, Byte> imageGray2 = image2.Convert<Gray, Byte>();
                Image<Gray, Byte> imageThreshold1 = imageGray1.ThresholdBinaryInv(new Gray(128d), new Gray(255d));
                Image<Gray, Byte> imageThreshold2 = imageGray2.ThresholdBinaryInv(new Gray(128d), new Gray(255d));
                //Contour<Point> contour1 = imageThreshold1.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_EXTERNAL);
                Contour<Point> contour1 = imageThreshold1.FindContours();
                Contour<Point> contour2 = imageThreshold2.FindContours();
                IntPtr HistImg1 = CvInvoke.cvCreateHist(1, hist_size, Emgu.CV.CvEnum.HIST_TYPE.CV_HIST_ARRAY, null, 1); //创建一个空的直方图
                IntPtr HistImg2 = CvInvoke.cvCreateHist(1, hist_size, Emgu.CV.CvEnum.HIST_TYPE.CV_HIST_ARRAY, null, 1);
                //CvInvoke.cvHaarDetectObjects();
                IntPtr[] inPtr1 = new IntPtr[1] { imageThreshold1 };
                IntPtr[] inPtr2 = new IntPtr[1] { imageThreshold2 };
                CvInvoke.cvCalcHist(inPtr1, HistImg1, false, IntPtr.Zero); //计算inPtr1指向图像的数据，并传入HistImg1中
                CvInvoke.cvCalcHist(inPtr2, HistImg2, false, IntPtr.Zero);
                Stopwatch sw = new Stopwatch();
                sw.Start();

                Emgu.CV.CvEnum.HISTOGRAM_COMP_METHOD compareMethod = Emgu.CV.CvEnum.HISTOGRAM_COMP_METHOD.CV_COMP_BHATTACHARYYA;//直方图对比方式 
                CvInvoke.cvNormalizeHist(HistImg1, 1d); 
                CvInvoke.cvNormalizeHist(HistImg2, 1d);
                compareResult = CvInvoke.cvCompareHist(HistImg1, HistImg2, compareMethod);
                compareResult = 1 - compareResult*1.49;
                //compareResult = CvInvoke.cvMatchShapes(HistImg1, HistImg2, Emgu.CV.CvEnum.CONTOURS_MATCH_TYPE.CV_CONTOURS_MATCH_I3, 1d); 
                sw.Stop();
                time = sw.Elapsed.TotalMilliseconds;
            }
            else {
                if (l1 == 0) return "图1未检测到人脸信息";
                else return "图2未检测到人脸信息";
            }

            return string.Format("{0:F02}%，用时：{1:F04}毫秒\r\n", compareResult*100, time);

        }
    

}
}
