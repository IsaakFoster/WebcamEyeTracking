using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;

namespace WebcamApp
{
    public partial class CfrmMain : Form
    {
        public CfrmMain()
        {
            InitializeComponent();
        }

        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;

        private void CfrmMain_Load(object sender, EventArgs e)
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
            {
                cmbCam.Items.Add(filterInfo.Name);
            }
            cmbCam.SelectedIndex = 0;
            videoCaptureDevice = new VideoCaptureDevice();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[cmbCam.SelectedIndex].MonikerString);
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();
        }

        static readonly CascadeClassifier cascadeClassifierFrontFace = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        static readonly CascadeClassifier cascadeClassifierEye = new CascadeClassifier("haarcascade_eye.xml");
        static readonly CascadeClassifier cascadeClassifierEyeGlasses = new CascadeClassifier("haarcascade_eye_tree_eyeglasses.xml");
        static readonly CascadeClassifier cascadeClassifierSmile = new CascadeClassifier("haarcascade_smile.xml");
        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);

            Rectangle[] rectanglesFace = cascadeClassifierFrontFace.DetectMultiScale(grayImage,2.2, 1);
            foreach (Rectangle rect in rectanglesFace)
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Pen pen = new Pen(Color.Red, 1))
                    {
                        graphics.DrawRectangle(pen, rect);
                    }
                }
            }
         

            Rectangle[] rectanglesEyes = cascadeClassifierEye.DetectMultiScale(grayImage,3.6, 5);
            foreach (Rectangle rect in rectanglesEyes)
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Pen pen = new Pen(Color.Green, 2))
                    {
                        graphics.DrawRectangle(pen, rect);
                    }
                }
            }
            Rectangle[] rectanglesEyeGlasse = cascadeClassifierEyeGlasses.DetectMultiScale(grayImage,3.6, 4);
            foreach (Rectangle rect in rectanglesEyeGlasse)
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Pen pen = new Pen(Color.YellowGreen, 1))
                    {
                        graphics.DrawRectangle(pen, rect);
                    }
                }
            }
         

            pic.Image = bitmap;
        }

        private void CfrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCaptureDevice.IsRunning)
                videoCaptureDevice.Stop();

            videoCaptureDevice.NewFrame -= VideoCaptureDevice_NewFrame;

            pic.Image = null;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (videoCaptureDevice.IsRunning)
                videoCaptureDevice.Stop();

            videoCaptureDevice.NewFrame -= VideoCaptureDevice_NewFrame;

            pic.Image = null;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
        }
    }
}
