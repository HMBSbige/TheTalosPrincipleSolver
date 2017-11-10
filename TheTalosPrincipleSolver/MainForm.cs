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
            Stop = StopSolve;
            ChangeButtonText = Button1TextChange2;
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
                if (s == null || s.ThreadState == ThreadState.Stopped)
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
                button1.Text = @"Stop";
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
                //MessageBox.Show(@"无解！用时: " + timestring + @"秒");
                //this?.Invoke(DisposeForm);
            }
        }
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadTimer?.Dispose();
        }
    }
}