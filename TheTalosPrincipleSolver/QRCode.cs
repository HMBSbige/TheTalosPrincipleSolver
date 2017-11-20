using System;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace TheTalosPrincipleSolver
{
    public partial class QRCodeForm : Form
    {
        public QRCodeForm(string str = @"")
        {
            InitializeComponent();
            CreateQRcode(str);
            textBox1.Text = str;
        }
        
        private void CreateQRcode(string str)
        {
            EncodingOptions options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = @"UTF-8",
                Margin = 0,
                Width = pictureBox1.Width,
                Height = pictureBox1.Height,
                ErrorCorrection = ErrorCorrectionLevel.H
            };// options.Hints

            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = options
            };
            
            if (str == string.Empty)
            {
                pictureBox1.Image = null;
                return;
            }
            var bm = writer.Write(str);

            //Bitmap overlay = new Bitmap(@"");

            //int deltaHeigth = bm.Height - overlay.Height;
            //int deltaWidth = bm.Width - overlay.Width;

            //Graphics g = Graphics.FromImage(bm);
            //g.DrawImage(overlay, new Point(deltaWidth / 2, deltaHeigth / 2));

            pictureBox1.Image = bm;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CreateQRcode(textBox1.Text);
        }

        private void QCode_SizeChanged(object sender, EventArgs e)
        {
            CreateQRcode(textBox1.Text);
        }
    }
}
