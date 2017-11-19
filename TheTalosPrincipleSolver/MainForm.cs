using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TheTalosPrincipleSolver
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            resultFormDisplay = ShowForm;
            newresultForm = NewForm;
            DisposeForm = CloseForm;
            Stop = StopSolve;
            ChangeButtonText = Button1TextChange2;
        }

        #region puzzle

        private PuzzleForm resultForm;
        private Thread s;
        private PuzzleSolver t;

        private System.Threading.Timer threadTimer;
        private static readonly object sync = new object();

        private delegate void ResultDisplay();
        private readonly ResultDisplay resultFormDisplay;
        private readonly ResultDisplay newresultForm;
        private readonly ResultDisplay DisposeForm;
        private readonly ResultDisplay Stop;

        private delegate void StatusDisplay(string str);
        private readonly StatusDisplay ChangeButtonText;

        private void Button1TextChange2(string str)
        {
            button1.Text = str;
        }

        private void ShowForm()
        {
            if(resultForm != null && !resultForm.IsDisposed)
            {
                resultForm.Show();
            }
        }

        private void NewForm()
        {
            resultForm = new PuzzleForm(t.getBoard(), t.getNumberOfPieces());
        }

        private void CloseForm()
        {
            if (resultForm!=null && !resultForm.IsDisposed)
            {
                resultForm.Dispose();
            }
        }

        private void ShowTime(string str)
        {
            StatusInfo.Text = str;
        }

        private void StopSolve()
        {
            threadTimer.Dispose();
            if (s != null)
            {
                s.Abort();
                s = null;
            }
            this?.Invoke(DisposeForm);
            resultForm = null;
            if (!t.isSolved())
            {
                ShowTime(@"已停止计算");
            }
            button1.Text = @"Start";
        }

        private void Display(object obj)
        {
            lock (sync)
            {
                if (t.isSolved())
                {
                    if (t.isSolveable())
                    {
                        if (resultForm != null && !resultForm.IsDisposed)
                        {
                            resultForm.RePaint(t.getBoard(), t.getNumberOfPieces());
                            this?.Invoke(resultFormDisplay);
                            this?.Invoke(ChangeButtonText, @"Start");
                        }
                        else
                        {
                            this?.Invoke(Stop);
                        }
                    }
                    else
                    {
                        this?.Invoke(DisposeForm);
                        this?.Invoke(ChangeButtonText, @"Start");
                    }
                    threadTimer?.Dispose();
                    return;
                }

                if (resultForm != null && !resultForm.IsDisposed)
                {
                    resultForm.RePaint(t.getBoard(), t.getNumberOfPieces());
                    this?.Invoke(resultFormDisplay);
                }
                else
                {
                    this?.Invoke(Stop);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == @"Stop")
            {
                StopSolve();
            }
            else
            {
                if (s == null || s.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    try
                    {
                        if (radioButton1.Checked)
                        {
                            t = new PuzzleSolver(
                                Convert.ToInt32(rowNumBox.Text),
                                Convert.ToInt32(columnNumBox.Text),
                                Convert.ToInt32(INumBox.Text),
                                Convert.ToInt32(ONumBox.Text),
                                Convert.ToInt32(TNumBox.Text),
                                Convert.ToInt32(JNumBox.Text),
                                Convert.ToInt32(LNumBox.Text),
                                Convert.ToInt32(SNumBox.Text),
                                Convert.ToInt32(ZNumBox.Text));
                        }
                        else
                        {
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, @"出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    s = new Thread(Solve)
                    {
                        IsBackground = true
                    };
                    s.Start();
                    if (resultForm == null || resultForm.IsDisposed)
                    {
                        this?.Invoke(newresultForm);
                    }
                    threadTimer = new System.Threading.Timer(Display, null, 0, 10);
                }
                else
                {
                    MessageBox.Show(@"正在运行", @"Running", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                ShowTime(@"正在计算...");
                button1.Text = @"Stop";
            }
        }

        private void Solve()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            var isSolveable = t.Solve();

            timer.Stop();
            TimeSpan timeSpan = timer.Elapsed;
            var timestring = (timeSpan.TotalMilliseconds/1000.0).ToString(CultureInfo.InvariantCulture);

            if (isSolveable)
            {
                ShowTime(@"成功！用时: " + timestring + @"秒");
            }
            else
            {
                ShowTime(@"无解！用时: " + timestring + @"秒");
                //MessageBox.Show(@"无解！用时: " + timestring + @"秒");
                //this?.Invoke(DisposeForm);
            }
        }
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadTimer?.Dispose();
        }

        #endregion

        #region HexString
        
        private static byte[] HexStringToBinary(string hexstring)
        {
            hexstring = hexstring.Replace(@" ", string.Empty);
            var sb1=new StringBuilder();
            foreach (var c in hexstring)
            {
                if (char.IsDigit(c) || char.IsUpper(c) || char.IsLower(c))
                {
                    sb1.Append(c);
                }
            }
            var sb2 = new StringBuilder();
            for (var i=0;i<sb1.Length;++i)
            {
                sb2.Append(sb1[i]);
                if ((i & 1) == 1)
                {
                    sb2.Append(' ');
                }
            }
            var tmp = sb2.ToString().Trim().Split(' ');
            var buff = new byte[tmp.Length];
            for (int i = 0; i < buff.Length; i++)
            {
                buff[i] = Convert.ToByte(tmp[i], 16);
            }
            return buff;
        }
        
        private static string Ascii2Str(byte[] buf)
        {
            return Encoding.UTF8.GetString(buf);
        }
        

        private void Ascii_textBox_TextChanged(object sender, EventArgs e)
        {
            if (Ascii_textBox.Focused)
            {
                try
                {
                    Char_textBox.Text = Ascii2Str(HexStringToBinary(Ascii_textBox.Text));
                }
                catch
                {
                    Char_textBox.Text = @"";
                }
            }
        }

        private static byte[] Str2Ascii(string xmlStr)
        {
            return Encoding.UTF8.GetBytes(xmlStr);
        }

        private void Char_textBox_TextChanged(object sender, EventArgs e)
        {
            if (Char_textBox.Focused)
            {
                try
                {
                    var B = Str2Ascii(Char_textBox.Text);
                    var hex = BitConverter.ToString(B, 0).Replace(@"-", @" ");
                    if (!isUpper.Checked)
                    {
                        hex = hex.ToLower();
                    }
                    Ascii_textBox.Text = hex;
                }
                catch
                {
                    Ascii_textBox.Text = @"";
                }
            }
        }

        private void isUpper_CheckedChanged(object sender, EventArgs e)
        {
            Ascii_textBox.Text = !isUpper.Checked ? Ascii_textBox.Text.ToLower() : Ascii_textBox.Text.ToUpper();
        }

        #endregion
    }
}