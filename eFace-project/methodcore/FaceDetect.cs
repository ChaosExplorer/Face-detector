using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace methodcore
{
    public class FaceDetect
    {
        
        public static Bitmap SkinSimDetect(string imgFile)
        {
            Bitmap srcBitmap = (Bitmap)Bitmap.FromFile(imgFile,false);
            int wide = srcBitmap.Width;
            int height = srcBitmap.Height;
            Rectangle rect = new Rectangle(0,0,wide,height);

            BitmapData srcBmData = srcBitmap.LockBits(rect,ImageLockMode.ReadWrite,PixelFormat.Format24bppRgb);

            Bitmap dstBitmap = new Bitmap(wide,height,PixelFormat.Format8bppIndexed);
            SetGrayscalePalette(dstBitmap);
            BitmapData dstBmData = dstBitmap.LockBits(rect,ImageLockMode.ReadWrite,PixelFormat.Format8bppIndexed);

            System.IntPtr srcPtr = srcBmData.Scan0;
            System.IntPtr dstPtr = dstBmData.Scan0;

            int src_bytes_len = srcBmData.Stride * height;
            byte[] srcValues=new byte[src_bytes_len];
            int dst_bytes_len = dstBmData.Stride * height;
            byte[] dstValues=new byte[dst_bytes_len];
            //copy GRB
            System.Runtime.InteropServices.Marshal.Copy(srcPtr,srcValues,0,src_bytes_len);
            System.Runtime.InteropServices.Marshal.Copy(dstPtr, dstValues, 0, dst_bytes_len);
            //相似度矩阵
            double[,] pSimArray = new double[height,wide];
            //肤色高斯模型参数
            double Cb_Mean = 117.4361;
            double Cr_Mean = 156.5599;
            double Cov00=160.1301;
            double Cov01=12.1430;
            double Cov10=12.1430;
            double Cov11=299.4574;
            //变换YCgCr色彩空间，并计算像素点属于皮肤区域的概率
            int sd=srcBmData.Stride;
            for(int i=0;i<height;i++)
                for (int j = 0; j < wide; j++)
                {   int k=3*j;
                    int C_b = (int)srcValues[i * sd + k];
                    int C_g = (int)srcValues[i * sd + k+1]; ;
                    int C_r = (int)srcValues[i * sd + k+2]; ;
                    double Cb=(128-37.797*C_r/255-74.203*C_g/255+112*C_b/255);
                    double Cr=(128+112*C_r/255-93.786*C_g/255-18.214*C_b/255);
                    double tt=(Cb-Cb_Mean)*((Cb-Cb_Mean)*Cov11-(Cr-Cr_Mean)*Cov10)+
                        (Cr-Cr_Mean)*(-(Cb-Cb_Mean)*Cov01+(Cr-Cr_Mean)*Cov00);
                    tt=(-0.5*tt)/(Cov00*Cov11-Cov01*Cov10);
                    pSimArray[i, j] = System.Math.Exp(tt);

                }
            //中值滤波,抑制噪声,9x9矩阵
            int n = 9;
            int midn = (int)(n / 2);
            double[,] temp=new double[height+2*midn,wide+2*midn];
            

            for (int i = 0; i < height + 2*midn; i++)
                for (int j = 0; j < wide + 2*midn; j++)
                    temp[i, j] = 0.0;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < wide; j++)
                    temp[i + midn,j + midn] = pSimArray[i,j];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < wide; j++) {
                    pSimArray[i, j] = 0.0;
                    for (int r = 0; r < n; r++)
                        for (int c = 0; c < n; c++)
                            pSimArray[i, j] += temp[i + r, j + c];
                    pSimArray[i, j] /= n * n;
                }
            
            //统计该幅图像所有像素点的最大肤色相似度，矩阵进行归一化处理 
            double max = 0.0;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < wide; j++) {
                    if (pSimArray[i, j] > max)
                        max = pSimArray[i, j];
                }
            for (int i = 0; i < height; i++)
                for (int j = 0; j < wide; j++)
                    pSimArray[i, j] /= max;
                
            //变换到[0，255]，进行灰度图像显示
            for (int i = 0; i < height; i++)
                for (int j = 0; j < wide; j++)
                    dstValues[i * dstBmData.Stride + j] = (byte)((int)(pSimArray[i, j] * 255));
            //Unlock bitmap
            System.Runtime.InteropServices.Marshal.Copy(dstValues, 0, dstPtr, dst_bytes_len);
            srcBitmap.UnlockBits(srcBmData);
            dstBitmap.UnlockBits(dstBmData);
            return dstBitmap;
        }
        public static Bitmap SkinSimDetect(Bitmap srcBitmap)
        {
            int wide = srcBitmap.Width;
            int height = srcBitmap.Height;
            Rectangle rect = new Rectangle(0, 0, wide, height);

            BitmapData srcBmData = srcBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            Bitmap dstBitmap = new Bitmap(wide, height, PixelFormat.Format8bppIndexed);
            SetGrayscalePalette(dstBitmap);
            BitmapData dstBmData = dstBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            System.IntPtr srcPtr = srcBmData.Scan0;
            System.IntPtr dstPtr = dstBmData.Scan0;

            int src_bytes_len = srcBmData.Stride * height;
            byte[] srcValues = new byte[src_bytes_len];
            int dst_bytes_len = dstBmData.Stride * height;
            byte[] dstValues = new byte[dst_bytes_len];
            //copy GRB
            System.Runtime.InteropServices.Marshal.Copy(srcPtr, srcValues, 0, src_bytes_len);
            System.Runtime.InteropServices.Marshal.Copy(dstPtr, dstValues, 0, dst_bytes_len);
            //相似度矩阵
            double[,] pSimArray = new double[height, wide];
            //肤色高斯模型参数
            double Cb_Mean = 117.4361;
            double Cr_Mean = 156.5599;
            double Cov00 = 160.1301;
            double Cov01 = 12.1430;
            double Cov10 = 12.1430;
            double Cov11 = 299.4574;
            //变换YCgCr色彩空间，并计算像素点属于皮肤区域的概率
            int sd = srcBmData.Stride;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < wide; j++)
                {
                    int k = 3 * j;
                    int C_b = (int)srcValues[i * sd + k];
                    int C_g = (int)srcValues[i * sd + k + 1]; ;
                    int C_r = (int)srcValues[i * sd + k + 2]; ;
                    double Cb = (128 - 37.797 * C_r / 255 - 74.203 * C_g / 255 + 112 * C_b / 255);
                    double Cr = (128 + 112 * C_r / 255 - 93.786 * C_g / 255 - 18.214 * C_b / 255);
                    double tt = (Cb - Cb_Mean) * ((Cb - Cb_Mean) * Cov11 - (Cr - Cr_Mean) * Cov10) +
                        (Cr - Cr_Mean) * (-(Cb - Cb_Mean) * Cov01 + (Cr - Cr_Mean) * Cov00);
                    tt = (-0.5 * tt) / (Cov00 * Cov11 - Cov01 * Cov10);
                    pSimArray[i, j] = System.Math.Exp(tt);

                }
            //中值滤波,抑制噪声,9x9矩阵
            double[,] temp = new double[height + 8, wide + 8];

            for (int i = 0; i < height + 8; i++)
                for (int j = 0; j < wide + 8; j++)
                    temp[i, j] = 0.0;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < wide; j++)
                    temp[i + 4, j + 4] = pSimArray[i, j];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < wide; j++)
                {
                    pSimArray[i, j] = 0.0;
                    for (int r = 0; r < 9; r++)
                        for (int c = 0; c < 9; c++)
                            pSimArray[i, j] += temp[i + r, j + c];
                    pSimArray[i, j] /= 9 * 9;
                }

            //统计该幅图像所有像素点的最大肤色相似度，矩阵进行归一化处理 
            double max = 0.0;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < wide; j++)
                {
                    if (pSimArray[i, j] > max)
                        max = pSimArray[i, j];
                }
            for (int i = 0; i < height; i++)
                for (int j = 0; j < wide; j++)
                    pSimArray[i, j] /= max;

            //变换到[0，255]，进行灰度图像显示
            for (int i = 0; i < height; i++)
                for (int j = 0; j < wide; j++)
                    dstValues[i * dstBmData.Stride + j] = (byte)((int)(pSimArray[i, j] * 255));
            //Unlock bitmap
            System.Runtime.InteropServices.Marshal.Copy(dstValues, 0, dstPtr, dst_bytes_len);
            srcBitmap.UnlockBits(srcBmData);
            dstBitmap.UnlockBits(dstBmData);
            return dstBitmap;
        }
        public static void SetGrayscalePalette(Bitmap srcImg)
        {
            // check pixel format
            if (srcImg.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new ArgumentException();
            // get palette
            ColorPalette cp = srcImg.Palette;
            // init palette
            for (int i = 0; i < 256; i++)
            {
                cp.Entries[i] = Color.FromArgb(i, i, i);
            }
            srcImg.Palette = cp;
        }
        public static Bitmap emguHaarDetect(string imgFile) {
            return FaceCompare.emguHaarDetect(imgFile);
        }
        public static Bitmap emguHaarDetect(Bitmap bt)
        {
            return FaceCompare.emguHaarDetect(bt);
        }





        
    }
}
