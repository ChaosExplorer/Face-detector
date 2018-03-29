using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace methodcore
{
    public class FaceLocate
    {
        public static Bitmap ImageBinary(Bitmap bt)
        {
            int w = bt.Width;
            int h = bt.Height;
            int i, j;
            long pixel_scales = 0;//灰度总值
            int threshold;//二值化阀值
            BitmapData btBmData = bt.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            System.IntPtr ptr = btBmData.Scan0;
            int ws = btBmData.Stride;
            int bytes_len = ws * h;
            byte[] btValues = new byte[bytes_len];
            System.Runtime.InteropServices.Marshal.Copy(ptr, btValues, 0, bytes_len);
            
            for (i = 0; i < bytes_len; i++)
                pixel_scales += btValues[i];

            threshold = (int)pixel_scales / (w * h);

            for (i = 0; i < h; i++)
                for (j = 0; j < w;j++ )
                {
                    if (btValues[i*ws+j] < threshold)
                        btValues[i * ws + j] = 0;
                    else
                        btValues[i * ws + j] = 255;
                }
            System.Runtime.InteropServices.Marshal.Copy(btValues, 0, ptr, bytes_len);
            bt.UnlockBits(btBmData);
            return bt;

        }
        public static Bitmap faceLocate(Bitmap bt,string imgFile) {
            int x, y,count;
            int rWidth, rHeight;
            int w = bt.Width;
            int h = bt.Height;
            int left=0, right=0, top=0, bottom=0;
            int i, j;
            BitmapData btBmData = bt.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            System.IntPtr ptr = btBmData.Scan0;
            int ws = btBmData.Stride;
            int bytes_len =  ws* h;
            byte[] btValues = new byte[bytes_len];
            System.Runtime.InteropServices.Marshal.Copy(ptr, btValues, 0, bytes_len);
            
            double[] temph=new double[w];
            double[] tempv=new double[h];
            int max = 0;
            int pos = -1;
            for (i = 0; i < w; i++) {
                count = 0;
                for (j = 0; j < h; j++)
                    if (btValues[j * ws + i] == 255)
                        count++;
                temph[i] = count;
            }
            for (i = 0; i < w; i++)
                if (temph[i] > max) { 
                    max = (int)(temph[i]+0.1);
                    pos=i;
                }
            for (i = 0; i < w; i++)
                temph[i] /= max;
            for (i = pos; i >= 0;i--)
            {
                if (temph[i] < 0.2 || i == 0)
                {
                    left = i;
                    break;
                }
            }
            for (i = pos; i <= w;i++)
            {
                if (temph[i] < 0.2 || i == w-1) {
                    right = i;
                    break;
                }
            }


            max = 0;
            pos = -1;
            for (i = 0; i < h; i++)
            {
                count = 0;
                for (j = 0; j < w; j++)
                    if (btValues[i * ws + j] == 255)
                        count++;
                tempv[i] = count;
            }
            for (i = 0; i < h; i++)
                if (tempv[i] > max)
                {
                    max = (int)(tempv[i] + 0.1);
                    pos = i;
                }
            for (i = 0; i < h; i++)
                tempv[i] /= max;

            for (i = pos; i >= 0; i--)
            {
                if (tempv[i] < 0.2 || i == 0) {
                    top = i;
                    break;
                }
            }

            bottom = (int)((right - left) * 1.2 + top);
            rWidth = right - left;
            rHeight = bottom - top;
            
            bt.UnlockBits(btBmData);

            Graphics g ;
            Pen pe = new Pen(Color.Green, 2);

            Bitmap resultbt = new Bitmap(imgFile);
            g = Graphics.FromImage(resultbt);
            g.DrawRectangle(pe,new Rectangle(left,top,rWidth,rHeight));

            return resultbt;
        }
        public static Bitmap faceLocate(Bitmap bt,Bitmap resultbt)
        {
            int x, y, count;
            int rWidth, rHeight;
            int w = bt.Width;
            int h = bt.Height;
            int left = 0, right = 0, top = 0, bottom = 0;
            int i, j;
            BitmapData btBmData = bt.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            System.IntPtr ptr = btBmData.Scan0;
            int ws = btBmData.Stride;
            int bytes_len = ws * h;
            byte[] btValues = new byte[bytes_len];
            System.Runtime.InteropServices.Marshal.Copy(ptr, btValues, 0, bytes_len);

            double[] temph = new double[w];
            double[] tempv = new double[h];
            int max = 0;
            int pos = -1;
            for (i = 0; i < w; i++)
            {
                count = 0;
                for (j = 0; j < h; j++)
                    if (btValues[j * ws + i] == 255)
                        count++;
                temph[i] = count;
            }
            for (i = 0; i < w; i++)
                if (temph[i] > max)
                {
                    max = (int)(temph[i] + 0.1);
                    pos = i;
                }
            for (i = 0; i < w; i++)
                temph[i] /= max;
            for (i = pos; i >= 0; i--)
            {
                if (temph[i] < 0.2 || i == 0)
                {
                    left = i;
                    break;
                }
            }
            for (i = pos; i <= w; i++)
            {
                if (temph[i] < 0.2 || i == w-1)
                {
                    right = i;
                    break;
                }
            }


            max = 0;
            pos = -1;
            for (i = 0; i < h; i++)
            {
                count = 0;
                for (j = 0; j < w; j++)
                    if (btValues[i * ws + j] == 255)
                        count++;
                tempv[i] = count;
            }
            for (i = 0; i < h; i++)
                if (tempv[i] > max)
                {
                    max = (int)(tempv[i] + 0.1);
                    pos = i;
                }
            for (i = 0; i < h; i++)
                tempv[i] /= max;

            for (i = pos; i >= 0; i--)
            {
                if (tempv[i] < 0.2 || i == 0)
                {
                    top = i;
                    break;
                }
            }

            bottom = (int)((right - left) * 1.2 + top);
            rWidth = right - left;
            rHeight = bottom - top;

            bt.UnlockBits(btBmData);

            Graphics g;
            Pen pe = new Pen(Color.Green, 3);
            g = Graphics.FromImage(resultbt);
            g.DrawRectangle(pe, new Rectangle(left, top, rWidth, rHeight));

            return resultbt;
        }
    }
}
