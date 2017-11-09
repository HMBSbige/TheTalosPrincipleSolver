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

        private void ShowForm()
        {
            resultForm?.Show();
        }
        private void NewForm()
        {
            resultForm = new PuzzleForm(t.getBoard(), t.getNumberOfPieces());
        }

        private void CloseForm()
        {
            resultForm?.Dispose();
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
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            /*if (方块总数 * 4 != 行 x 列)
            {
                MessageBox.Show(@"无解", @"出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/
            if (s == null || s.ThreadState == ThreadState.Stopped)
            {
                try
                {
                    t = new PuzzleSolver(8, 6, 1, 5, 2, 0, 1, 2, 1);
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
                threadTimer = new System.Threading.Timer(Display, null, 0, 10);
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
                //MessageBox.Show(@"完成");
            }
            else
            {
                MessageBox.Show(@"无解", @"出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this?.Invoke(DisposeForm);
            }
            MessageBox.Show(timestring);
        }
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadTimer?.Dispose();
        }
    }
}
