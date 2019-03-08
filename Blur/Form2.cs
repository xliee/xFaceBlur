﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using System.Drawing.Imaging;
using Blur;
using Emgu.CV.Util;
using System.Runtime.InteropServices;
using Emgu.CV.Dnn;
using System.IO;
using System.Diagnostics;

namespace Blur
{
    public partial class Form2 : Form
    {
        private VideoCapture _capture;
        private CascadeClassifier _cascadeClassifier;


        public Form2()
        {
            InitializeComponent();
            _capture = new VideoCapture();

            //pictureBox1.Image = _capture.QueryFrame().ToImage<Bgr, Byte>().Bitmap;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _cascadeClassifier = new CascadeClassifier(@"W:\Documents\Web\c++\face_detect_n_track-master\haarcascade_frontalface_default.xml");
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                string path = openFileDialog1.FileName;

                Bitmap frame = (Bitmap)Image.FromFile(path);
                Image<Bgr, Byte> imageFrame = new Image<Bgr, Byte>(frame);
                if (imageFrame != null)
                {
                    List<Rectangle> faces = new List<Rectangle>();
                    var grayframe = imageFrame.Convert<Gray, Byte>();
                    //var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 6, Size.Empty); //the actual face detection happens here

                    using (UMat ugray = new UMat())
                    {
                        CvInvoke.CvtColor(imageFrame, ugray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

                        //normalizes brightness and increases contrast of the image
                        CvInvoke.EqualizeHist(ugray, ugray);

                        //Detect the faces  from the gray scale image and store the locations as rectangle                   
                        Rectangle[] facesDetected = _cascadeClassifier.DetectMultiScale(
                           ugray, 1.1, 3, new Size(20, 20));

                        faces.AddRange(facesDetected);
                    }

                    foreach (var face in faces)
                    {
                        blur blurer = new blur();
                        Image<Bgr, Byte> frame2 = new Image<Bgr, Byte>(frame);


                        //frame2.Draw(face, new Bgr(Color.BurlyWood), 2); //the detected face(s) is highlighted here using a box that is drawn around it/them

                        frame = blurer.FastBoxBlur(frame2.Bitmap, 20, new Rectangle() { X = face.X, Y = face.Y, Width = face.Width, Height = face.Height });
                        frame = (Bitmap)blurer.ClipToCircle(frame2.Bitmap, frame, new Point(face.X + face.Width / 2, face.Y + face.Height / 2), face.Width / 2, new Color());

                    }
                }
                pictureBox1.Image = frame;
                
            }
        }



       
        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog1.FileName;

                Bitmap frame = (Bitmap)Image.FromFile(path);
                Image<Bgr, Byte> imageFrame = new Image<Bgr, Byte>(frame);
                Detector detectLM = new Detector();
                pictureBox1.Image = detectLM.drawFaceMark(imageFrame.Mat).ToImage<Bgr, Byte>().Bitmap;

            }
        }
    }
}
