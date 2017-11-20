using System;
using System.Drawing;
using System.Windows.Forms;
using TheTalosPrincipleSolver.Properties;
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
            if (str == string.Empty)
            {
                pictureBox1.Image = null;
                CreateQRcode1(str);
                return;
            }
            
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
            
            var bm = writer.Write(str);

            using (var g = Graphics.FromImage(bm))
            {
                var custom = Resources.huaji512;
                var w = (int)(Math.Min(options.Width, options.Height) * 0.2);
                g.DrawImage(custom, new Rectangle((options.Width - w)/ 2, (options.Height-w)/2, w,w));
            }

            pictureBox1.Image = bm;
        }
        
        private void CreateQRcode1(string str)
        {
            var width = pictureBox1.Width;
            var height = pictureBox1.Height;
            var size = Math.Min(pictureBox1.Width, pictureBox1.Height);
            try
            {
                EncodingOptions options = new QrCodeEncodingOptions
                {
                    DisableECI = true,
                    CharacterSet = @"UTF-8",
                    Margin = 0
                };// options.Hints
                var code = Encoder.encode(str, ErrorCorrectionLevel.H, options.Hints);
                var m = code.Matrix;
                var blockSize = Math.Max(size / m.Width,1);
                var bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                using (var g = Graphics.FromImage(bm))
                {
                    g.Clear(Color.White);
                    using (Brush b = new SolidBrush(Color.Black))
                    {
                        for (var row = 0; row < m.Width; row++)
                        {
                            for (var col = 0; col < m.Height; col++)
                            {
                                if (m[row, col] != 0)
                                {
                                    g.FillRectangle(b,
                                        (int)(blockSize * row + (width - size) / 2.0),
                                        (int)(blockSize * col + (height - size) / 2.0),
                                        blockSize, blockSize);
                                }
                            }
                        }
                        var custom = Resources.huaji512;
                        var w = (int)(Math.Min(width, height) * 0.2);
                        g.DrawImage(custom, new Rectangle((width - w) / 2, (height - w) / 2, w, w));
                    }
                }
                
                pictureBox1.Image = bm;
            }
            catch
            {
                //return;
            }
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