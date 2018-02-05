using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Video;
using Accord.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using MetroFramework.Forms;
using Tulpep.NotificationWindow;

namespace FaceID_Protocol
{




    public partial class FaceID : MetroForm

    {


        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private CascadeClassifier haarcascade_frontalface_default;


        Image<Gray, Byte> gray_face = null;

        private int k = 0;
        


        List<Image<Gray, Byte>> trainingImages = new List<Image<Gray, Byte>>();
        List<int> labels = new List<int>();
        List<string> NamePersons = new List<string>();



        Image<Gray, Byte>[] face = new Image<Gray, Byte>[50];
        String[] faceName = new String[50];
        int[] faceNum = new int[50];



        FaceRecognizer fr = new FisherFaceRecognizer(10 ,double.PositiveInfinity);
        FaceRecognizer.PredictionResult result = new FaceRecognizer.PredictionResult();


        System.Media.SoundPlayer startSoundPlayer = new System.Media.SoundPlayer(@"C:\Users\USER\Music\alarm.wav");
       

        public FaceID()
        {

            
            InitializeComponent();
            new Thread(SampleFunction).Start();
            new Thread(SampleFunction2).Start();
            new Thread(SampleFunction3).Start();
            this.Text = "                                                                                        FaceID  Protocol";


            this.WindowState = FormWindowState.Maximized;




           












        }


        void SampleFunction()
        {


        }

        void SampleFunction2()
        {


        }
        void SampleFunction3()
        {


        }




        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }

            textBox2.Text = value;

        }

        public void AppendTextBox2(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox2), new object[] { value });
                return;
            }


            textBox3.BackColor = Color.Red;

            textBox3.Text = value;

        }

        public void AppendTextBox3(string value)
        {

            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox3), new object[] { value });
                return;
            }


            textBox3.BackColor = Color.Blue;

            textBox3.Text = "No Faces Detected";

        }






        private void button1_Click(object sender, EventArgs e)
        {


            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.Start();
            videoSource.NewFrame += new NewFrameEventHandler(Video_NewFrame);



        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {



            Image<Bgr, Byte> ColordImage = new Image<Bgr, Byte>(eventArgs.Frame);
            Image<Gray, Byte> grayFrame = ColordImage.Convert<Gray, Byte>();
            Image<Bgr, Byte> resultFrame = new Image<Bgr, Byte>(eventArgs.Frame);
            Image<Bgr, Byte> save_face_frame = new Image<Bgr, Byte>(eventArgs.Frame);

            haarcascade_frontalface_default = new CascadeClassifier(@"D:\OpenCV\opencv\build\etc\haarcascades\haarcascade_frontalface_default.xml");
            var detectedFaces_default = haarcascade_frontalface_default.DetectMultiScale(grayFrame);

            




            













                foreach (var detectedFace in detectedFaces_default)
                {


                    save_face_frame = resultFrame.GetSubRect(detectedFace);



                }


                gray_face = save_face_frame.Convert<Gray, Byte>().Resize(100, 100, Emgu.CV.CvEnum.Inter.Linear);
                gray_face._EqualizeHist();

                

                    result = fr.Predict(gray_face);

                
                
                AppendTextBox(result.Distance.ToString());

            if (result.Label != 0)
            {


                if (result.Distance > 150)
                {



                    //textBox3.BackColor = Color.Red;
                    AppendTextBox2(result.Label.ToString());







                    pictureBox2.Image = Properties.Resources.granted;



                    foreach (var detectedFace in detectedFaces_default)
                    {

                        resultFrame.Draw(detectedFace, new Bgr(0, 255, 0), 3);






                    }
                }

                else

                {


                    pictureBox2.Image = Properties.Resources.denied;





                    AppendTextBox2(result.Label.ToString());







                    foreach (var detectedFace in detectedFaces_default)
                    {

                        resultFrame.Draw(detectedFace, new Bgr(0, 0, 255), 3);







                    }




                }

                pictureBox1.Image = resultFrame.ToBitmap();









            }


                //  throw new NotImplementedException();
            }
           



        




        private void button2_Click(object sender, EventArgs e)
        {


            videoSource.SignalToStop();
            //fr.Write(Application.StartupPath + "trainingImages.yml");

        }





        private void button3_Click(object sender, EventArgs e)
        {

            k++;

            trainingImages.Add(gray_face);
            labels.Add(k);
            NamePersons.Add(textBox1.Text);



            face = trainingImages.ToArray();
            faceName = NamePersons.ToArray();
            faceNum = labels.ToArray();

            fr.Train(face, faceNum);



            foreach (var trainingImage in trainingImages)
            {


                trainingImage.ToBitmap().Save(@"C:\Users\USER\Desktop\Test Faces\check2\Images\" + k + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);


            }


            StreamWriter file1 =
           new StreamWriter(@"C:\Users\USER\Desktop\Test Faces\check2\NameList\NameList.txt");

            foreach (var NamePerson in NamePersons)
            {


                file1.WriteLine(NamePerson);





            }

            file1.Flush();
            file1.Close();
            file1.Dispose();





            StreamWriter file2 =
           new StreamWriter(@"C:\Users\USER\Desktop\Test Faces\check2\NameList\labels.txt");

            foreach (var k in labels)
            {

                file2.WriteLine(k);

            }
            file2.Flush();
            file2.Close();
            file2.Dispose();





        }




         

        private void FaceID_Load(object sender, EventArgs e)
        {



            string line;
            System.IO.StreamReader file1 =
                   new System.IO.StreamReader(@"C:\Users\USER\Desktop\Test Faces\check2\NameList\NameList.txt");

            while ((line = file1.ReadLine()) != null)
            {


                NamePersons.Add(line);



            }



            int l;
            System.IO.StreamReader file2 =
                   new System.IO.StreamReader(@"C:\Users\USER\Desktop\Test Faces\check2\NameList\labels.txt");


            while ((line = file2.ReadLine()) != null)
            {

                l = Int32.Parse(line);
                labels.Add(l);
                k++;




            }

            var path = @"C:\Users\USER\Desktop\Test Faces\check2\Images\";
            string[] filePaths = Directory.GetFiles(path, "*.bmp");



            foreach (var filePath in filePaths)
            {




                Bitmap bmp = (Bitmap)Image.FromFile(filePath);

                Image<Gray, Byte> myImage = new Image<Gray, byte>(bmp);

                trainingImages.Add(myImage);


            }



            file1.Close();
            file2.Close();




            face = trainingImages.ToArray();
            faceName = NamePersons.ToArray();
            faceNum = labels.ToArray();












        }





        private void button4_Click(object sender, EventArgs e)
        {

            fr.Train(face, faceNum);

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }















}
