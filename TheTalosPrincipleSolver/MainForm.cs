using System;
using System.Globalization;
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
            //UsedTime = ShowTime;
        }
        private PuzzleForm resultForm;
        private Thread s;
        private PuzzleSolver t;

        private System.Threading.Timer threadTimer;
        private static readonly object sync = new object();

        private delegate void ResultDisplay();
        private readonly ResultDisplay resultFormDisplay;
        private readonly ResultDisplay newresultForm;
        private readonly ResultDisplay DisposeForm;

        //private delegate void StatusDisplay(string str);
        //private readonly StatusDisplay UsedTime;

        private void ShowForm()
        {
            if (!resultForm.IsDisposed)
            {
                resultForm?.Show();
            }
        }
        private void NewForm()
        {
            resultForm = new PuzzleForm(t.getBoard(), t.getNumberOfPieces());
        }

        private void CloseForm()
        {
            threadTimer?.Dispose();
            if (!resultForm.IsDisposed)
            {
                resultForm?.Dispose();
            }  
        }

        private void ShowTime(string str)
        {
            StatusInfo.Text = str;
        }
        
        private void Display(object obj)
        {
            lock (sync)
            {
                if (t.isSolved())
                {
                    if (resultForm != null && !resultForm.IsDisposed)
                    {
                        resultForm.RePaint(t.getBoard(), t.getNumberOfPieces());
                        this?.Invoke(resultFormDisplay);
                    }
                    else
                    {
                        this?.Invoke(newresultForm);

                        this?.Invoke(resultFormDisplay);
                    }
                    threadTimer.Dispose();
                    return;
                }
                
                if (resultForm != null && !resultForm.IsDisposed)
                {
                    resultForm.RePaint(t.getBoard(), t.getNumberOfPieces());
                }
                else
                {
                    this?.Invoke(newresultForm);
                    this?.Invoke(resultFormDisplay);
                }
                //else if (resultForm != null && resultForm.IsDisposed)
                //{
                //    resultForm = null;
                //    s?.Abort();
                //    ShowTime(@"已停止计算");
                //    threadTimer.Dispose();
                //}
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (s == null || s.ThreadState == ThreadState.Stopped)
            {
                try
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
                threadTimer = new System.Threading.Timer(Display, null, 0, 1);
            }
            else
            {
                MessageBox.Show(@"正在运行", @"Running", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Solve()
        {
            var time = DateTime.Now.Ticks;
            var isSolveable = t.Solve();
            var timestring = ((DateTime.Now.Ticks - time) / 10000000.0).ToString(CultureInfo.InvariantCulture);
            if (isSolveable)
            {
                ShowTime(@"成功！用时: " + timestring + @"秒");
            }
            else
            {
                ShowTime(@"无解！用时: " + timestring + @"秒");
                MessageBox.Show(@"无解！用时: " + timestring + @"秒");
                this?.Invoke(DisposeForm);
            }
        }
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadTimer?.Dispose();
        }
    }
}
