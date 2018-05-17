using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GriauleFingerprintLibrary;
using GriauleFingerprintLibrary.Events;
using GriauleFingerprintLibrary.Exceptions;

namespace controlasistencia
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            fingerprint = new FingerprintCore();
            fingerprint.onStatus += new StatusEventHandler(fingerPrint_onStatus);
            //fingerprint.onFinger += new StatusEventHandler(fingerprint_onFinger);
            fingerprint.onImage += new ImageEventHandler(fingerprint_onImage);
        }

        private FingerprintCore fingerprint;
        private GriauleFingerprintLibrary.DataTypes.FingerprintRawImage rawImage;
        GriauleFingerprintLibrary.DataTypes.FingerprintTemplate _template;
        //private void label1_Click(object sender, EventArgs e)
        //{

        //}

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void fingerprint_onImage(object sorce, GriauleFingerprintLibrary.Events.ImageEventArgs ie)
        {

            rawImage = ie.RawImage;
            SetImage(ie.RawImage.Image);

            ExtracTemplate();
        }

        private void ExtracTemplate()
        {
            if (rawImage != null)
            {
                try
                {
                    _template = null;
                    fingerprint.Extract(rawImage, ref _template);

                    SetQualityBar(_template.Quality);

                    DisplayImage(_template, false);

                }
                catch (Exception)
                {

                    SetQualityBar(-1);
                }
            }
        }

        delegate void delsetQuality(int quality);
        private void SetQualityBar(int quality)
        {

            if (prgbQuality.InvokeRequired == true)
            {
                this.Invoke(new delsetQuality(SetQualityBar), new object[] { quality });
            }

            else
            {
                switch (quality)
                {
                    case 0:
                        {
                            prgbQuality.Value = prgbQuality.Maximum / 3;
                        }
                        break;
                    case 1:
                        {
                            prgbQuality.Value = (prgbQuality.Maximum / 3) * 2;
                        }
                        break;

                    case 2:
                        {
                            prgbQuality.Value = prgbQuality.Maximum;
                        }
                        break;

                    default:
                        {
                            prgbQuality.Value = 0;

                        }
                        break;
                }
            }
        }

        private void DisplayImage(GriauleFingerprintLibrary.DataTypes.FingerprintTemplate template, bool identify)
        {
            IntPtr hdc = FingerprintCore.GetDC();

            IntPtr image = new IntPtr();

            if (identify)
            {
                fingerprint.GetBiometricDisplay(template, rawImage, hdc, ref image, FingerprintConstants.GR_DEFAULT_CONTEXT);
                button4.Enabled = true;
            }
            else
            {
                fingerprint.GetBiometricDisplay(template, rawImage, hdc, ref image, FingerprintConstants.GR_NO_CONTEXT);
                button2.Enabled = true;
            }

            SetImage(Bitmap.FromHbitmap(image));

            FingerprintCore.ReleaseDC(hdc);

        }




        private delegate void delSetImage(Image img);
        void SetImage(Image img)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new delSetImage(SetImage), new object[] { img });
            }
            else
            {
                Bitmap bmp = new Bitmap(img, pictureBox1.Width, pictureBox1.Height);
                pictureBox1.Image = bmp;

            }
        }


        void fingerPrint_onStatus(object source, GriauleFingerprintLibrary.Events.StatusEventArgs se)
        {
            if (se.StatusEventType == GriauleFingerprintLibrary.Events.StatusEventType.SENSOR_PLUG)
            {
                fingerprint.StartCapture(source.ToString());
            }
            else
            {
                fingerprint.StopCapture(source);
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }


    }
}
