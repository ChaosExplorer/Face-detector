using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge.Video;
using AForge.Video.DirectShow;
using common ;
using methodcore;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

namespace eFace
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            st = new Stopwatch();
        }
		//common
        private void loadImageFile(System.Windows.Controls.Image img, System.Windows.Controls.TextBox tb)
        {
            string filename = FileOp.SelectPicture();
            if (filename != null && filename.Length>0)
            {
                tb.Text = filename;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(filename, UriKind.Absolute);
                bi.EndInit();
                img.Source = bi;
            }
        }
        private string loadImageFile(System.Windows.Controls.Image img)
        {
            string filename = FileOp.SelectPicture();
            if (filename != null && filename.Length > 0)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(filename, UriKind.Absolute);
                bi.EndInit();
                img.Source = bi;
            }
            return filename;
        }
        //bitmap to bitmapImage
        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }  

		//menu1图像人脸检测


		//打开图片
        private void m1openfile(object sender, System.Windows.RoutedEventArgs e)
        {
            loadImageFile(img_m11, tb_filepath);
        }

        //相似度计算
		
        private void m1SimCaculate(object sender, System.Windows.RoutedEventArgs e)
        {
            if (img_m11.Source != null)
            {
                st.Start();
                tempbt = FaceDetect.SkinSimDetect(tb_filepath.Text.Trim());
                st.Stop();
                img_m12.Source = BitmapToBitmapImage(tempbt);
                lab_time1.Content = String.Format("{0:F02}毫秒", st.ElapsedMilliseconds);
                lastElsp = st.ElapsedMilliseconds;
            }
            else {
                MessageBox.Show("请先打开一张图片");
            }
            
        }
		
		//二值化
        private void m1binaryImg(object sender, System.Windows.RoutedEventArgs e)
        {
            if (tempbt != null)
            {
                st.Start();
                tempbt1 = FaceLocate.ImageBinary(tempbt);
                st.Stop();
                img_m12.Source = BitmapToBitmapImage(tempbt1);
                lab_time1.Content = String.Format("{0:F02}毫秒", st.ElapsedMilliseconds-lastElsp);
                lastElsp = st.ElapsedMilliseconds;
            }
            else
                MessageBox.Show("请先进行肤色相似度计算");

        }
		//脸部定位
        private void m1faceLocate(object sender, System.Windows.RoutedEventArgs e)
        {
            if (tempbt1 != null)
            {
                st.Start();
                tempbt2 = FaceLocate.faceLocate(tempbt1,tb_filepath.Text);
                st.Stop();
                if(tempbt2 != null)
                img_m12.Source = BitmapToBitmapImage(tempbt2);
                lab_time1.Content = String.Format("{0:F02}毫秒", st.ElapsedMilliseconds-lastElsp);
                lastElsp = st.ElapsedMilliseconds;
            }
            else
                MessageBox.Show("请先进行二值化处理");
        }


		//menu2摄像头拍照检测人脸
		
        /// <summary>
        /// 加载摄像头列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void getCameraDevice(object sender, System.EventArgs e)
        {
            cameralist = Camera.GetCameraDeviceId();
            if (cameralist != null && cameralist.Count > 0) {
                foreach (var item in cameralist)
                    comb_cameralist.Items.Add(item);
            }
            else {
                comb_cameralist.IsEnabled = false;
            }
        }
        /// <summary>
        /// 加载摄像头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadCamera(object sender, System.Windows.RoutedEventArgs e)
        {
            if (comb_cameralist.SelectedItem != null)
            {
                _videoCaptureDevice = new VideoCaptureDevice(comb_cameralist.SelectedItem.ToString());
                _videoCaptureDevice.NewFrame += HandNewFrame;
            }
            if (_videoCaptureDevice != null)
            {
                _videoCaptureDevice.Start();
            }
        }
        private void HandNewFrame(object sender, NewFrameEventArgs args)
        {
            
            try
            {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                {

                    if (args != null)
                    {
                        imgshoot = args.Frame.Clone() as Bitmap;

                        img_m21.Source = BitmapToBitmapImage(imgshoot);
                        
                    }
               }));
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.Message);//throw;
            }

        }

        /// <summary>
        /// 拍照并检测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cameraShoot(object sender, System.Windows.RoutedEventArgs e)
        {
            img_m22.Source = BitmapToBitmapImage(FaceLocate.faceLocate(FaceLocate.ImageBinary(FaceDetect.SkinSimDetect(imgshoot)), imgshoot));
        }
		
		private void cameraShootHaar(object sender, System.Windows.RoutedEventArgs e)
        {
            Bitmap bt = FaceDetect.emguHaarDetect(imgshoot);
            if (bt != null)
                img_m22.Source = BitmapToBitmapImage(bt);
            else
                MessageBox.Show("未检测到人脸信息");
        }
		/// <summary>
		/// 关闭摄像头
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void stopCamera(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_videoCaptureDevice != null)
            {
                _videoCaptureDevice.SignalToStop();
            }
        }


		//memu3人脸比对
		
        private void m3openFile1(object sender, System.Windows.RoutedEventArgs e)
        {
            imgFile1=loadImageFile(img_m31);
        }

        private void m3openFile2(object sender, System.Windows.RoutedEventArgs e)
        {
            imgFle2=loadImageFile(img_m32);
        }

        private void compareFace(object sender, System.Windows.RoutedEventArgs e)
        {
            if (imgFile1.Length>0 && imgFle2.Length>0)
                lab_result.Content = FaceCompare.FaceSimilarity(imgFile1, imgFle2);
            else
                MessageBox.Show("请打开两张图片");
        }

        //menu4
        private void m4openfile(object sender, System.Windows.RoutedEventArgs e)
        {
            loadImageFile(img_m41,tb_filepath4);
        }

        private void m4detectface(object sender, System.Windows.RoutedEventArgs e)
        {
            st.Start();
            Bitmap bt=FaceDetect.emguHaarDetect(tb_filepath4.Text.Trim());
            st.Stop();
            if (bt != null)
                img_m41.Source = BitmapToBitmapImage(bt);
            else
                MessageBox.Show("未检测到人脸信息");
            lab_time4.Content = String.Format("{0:F02}毫秒", st.ElapsedMilliseconds - lastElsp);
            lastElsp = st.ElapsedMilliseconds;
        }


        private  List<string> cameralist=new List<string>();

        private VideoCaptureDevice _videoCaptureDevice;

        private Bitmap tempbt,tempbt1,tempbt2;
        private Bitmap imgshoot;
        private BitmapImage bt;
        private string imgFile1, imgFle2;
        private Stopwatch st;
        private double lastElsp;
    }
}
