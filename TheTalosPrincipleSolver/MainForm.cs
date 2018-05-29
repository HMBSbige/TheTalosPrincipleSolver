using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ThreadState = System.Threading.ThreadState;
using Timer = System.Threading.Timer;

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
			HotkeyEvent1 = ScanScreenQRCode;
		}

		#region puzzle

		private PuzzleForm resultForm;
		private Thread thread;
		private PuzzleSolver t;

		private Timer threadTimer;
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
			if (resultForm != null && !resultForm.IsDisposed)
			{
				resultForm.Show();
			}
		}

		private void NewForm()
		{
			resultForm = new PuzzleForm(t.GetBoard(), t.GetNumberOfPieces());
		}

		private void CloseForm()
		{
			if (resultForm != null && !resultForm.IsDisposed)
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
			if (thread != null)
			{
				thread.Abort();
				thread = null;
			}
			this?.Invoke(DisposeForm);
			resultForm = null;
			if (!t.IsSolved())
			{
				ShowTime(@"已停止计算");
			}
			button1.Text = @"Start";
		}

		private void Display(object obj)
		{
			lock (sync)
			{
				if (t.IsSolved())
				{
					if (t.IsSolveable())
					{
						if (resultForm != null && !resultForm.IsDisposed)
						{
							resultForm.RePaint(t.GetBoard(), t.GetNumberOfPieces());
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
					resultForm.RePaint(t.GetBoard(), t.GetNumberOfPieces());
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
				if (thread == null || thread.ThreadState == ThreadState.Stopped)
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
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, @"出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
					thread = new Thread(Solve)
					{
						IsBackground = true
					};
					thread.Start();
					if (resultForm == null || resultForm.IsDisposed)
					{
						this?.Invoke(newresultForm);
					}
					threadTimer = new Timer(Display, null, 0, 10);
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
			var timestring = (timeSpan.TotalMilliseconds / 1000.0).ToString(CultureInfo.InvariantCulture);

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

		private void button4_Click(object sender, EventArgs e)
		{
			rowNumBox.Value = columnNumBox.Value + (columnNumBox.Value = rowNumBox.Value) * 0;
		}

		#endregion

		#region HexString

		private static byte[] HexStringToBinary(string hexstring)
		{
			hexstring = hexstring.Replace(@" ", string.Empty);
			var sb1 = new StringBuilder();
			foreach (var c in hexstring)
			{
				if (char.IsDigit(c) || char.IsUpper(c) || char.IsLower(c))
				{
					sb1.Append(c);
				}
			}
			var sb2 = new StringBuilder();
			for (var i = 0; i < sb1.Length; ++i)
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
					Char_textBox.Text = string.Empty;
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
					Ascii_textBox.Text = string.Empty;
				}
			}
		}

		private void isUpper_CheckedChanged(object sender, EventArgs e)
		{
			Ascii_textBox.Text = !isUpper.Checked ? Ascii_textBox.Text.ToLower() : Ascii_textBox.Text.ToUpper();
		}

		#endregion

		#region QRCode

		private void button2_Click(object sender, EventArgs e)
		{
			var f = new QRCodeForm();
			f.Show();
		}

		private static bool ScanQRCode(Image fullImage, Rectangle cropRect, out string res, out Rectangle rect)
		{
			using (var target = new Bitmap(cropRect.Width, cropRect.Height))
			{
				using (var g = Graphics.FromImage(target))
				{
					g.DrawImage(fullImage, new Rectangle(0, 0, cropRect.Width, cropRect.Height),
									cropRect,
									GraphicsUnit.Pixel);
				}
				var source = new BitmapLuminanceSource(target);
				var bitmap = new BinaryBitmap(new HybridBinarizer(source));
				var reader = new QRCodeReader();
				var result = reader.decode(bitmap);
				if (result != null)
				{
					res = result.Text;
					double minX = int.MaxValue, minY = int.MaxValue, maxX = 0, maxY = 0;
					foreach (var point in result.ResultPoints)
					{
						minX = Math.Min(minX, point.X);
						minY = Math.Min(minY, point.Y);
						maxX = Math.Max(maxX, point.X);
						maxY = Math.Max(maxY, point.Y);
					}
					rect = new Rectangle(cropRect.Left + (int)minX, cropRect.Top + (int)minY, (int)(maxX - minX), (int)(maxY - minY));
					return true;
				}
			}
			res = string.Empty;
			rect = new Rectangle();
			return false;
		}

		private static bool ScanQRCodeStretch(Image fullImage, Rectangle cropRect, double mul, out string res, out Rectangle rect)
		{
			using (var target = new Bitmap((int)(cropRect.Width * mul), (int)(cropRect.Height * mul)))
			{
				using (var g = Graphics.FromImage(target))
				{
					g.DrawImage(fullImage, new Rectangle(0, 0, target.Width, target.Height),
									cropRect,
									GraphicsUnit.Pixel);
				}
				var source = new BitmapLuminanceSource(target);
				var bitmap = new BinaryBitmap(new HybridBinarizer(source));
				var reader = new QRCodeReader();
				var result = reader.decode(bitmap);
				if (result != null)
				{
					res = result.Text;
					double minX = int.MaxValue, minY = int.MaxValue, maxX = 0, maxY = 0;
					foreach (var point in result.ResultPoints)
					{
						minX = Math.Min(minX, point.X);
						minY = Math.Min(minY, point.Y);
						maxX = Math.Max(maxX, point.X);
						maxY = Math.Max(maxY, point.Y);
					}
					rect = new Rectangle(cropRect.Left + (int)(minX / mul), cropRect.Top + (int)(minY / mul), (int)((maxX - minX) / mul), (int)((maxY - minY) / mul));
					return true;
				}
			}
			res = string.Empty;
			rect = new Rectangle();
			return false;
		}

		private static Rectangle GetScanRect(int width, int height, int index, out double mul)
		{
			mul = 1;
			if (index < 5)
			{
				const int div = 5;
				int w = width * 3 / div;
				int h = height * 3 / div;
				Point[] pt = {
					new Point(1, 1),

					new Point(0, 0),
					new Point(0, 2),
					new Point(2, 0),
					new Point(2, 2)
				};
				return new Rectangle(pt[index].X * width / div, pt[index].Y * height / div, w, h);
			}
			{
				const int base_index = 5;
				if (index < base_index + 6)
				{
					double[] _s = {
						1,
						2,
						3,
						4,
						6,
						8
					};
					mul = 1 / _s[index - base_index];
					return new Rectangle(0, 0, width, height);
				}
			}
			{
				const int base_index = 11;
				if (index < base_index + 8)
				{
					const int hdiv = 7;
					const int vdiv = 5;
					int w = width * 3 / hdiv;
					int h = height * 3 / vdiv;
					Point[] pt = {
						new Point(1, 1),
						new Point(3, 1),

						new Point(0, 0),
						new Point(0, 2),

						new Point(2, 0),
						new Point(2, 2),

						new Point(4, 0),
						new Point(4, 2)
					};
					return new Rectangle(pt[index - base_index].X * width / hdiv, pt[index - base_index].Y * height / vdiv, w, h);
				}
			}
			return new Rectangle(0, 0, 0, 0);
		}

		private static string Code;
		private static void ScanScreenQRCode()
		{
			foreach (var screen in Screen.AllScreens)
			{
				using (var fullImage = new Bitmap(screen.Bounds.Width, screen.Bounds.Height))
				{
					using (var g = Graphics.FromImage(fullImage))
					{
						g.CopyFromScreen(screen.Bounds.X,
										 screen.Bounds.Y,
										 0, 0,
										 fullImage.Size,
										 CopyPixelOperation.SourceCopy);
					}
					const int maxTry = 100;
					for (var i = 0; i < maxTry; ++i)
					{
						var cropRect = GetScanRect(fullImage.Width, fullImage.Height, i, out var mul);
						if (cropRect.Width == 0)
						{
							break;
						}
						if (Math.Abs(mul - 1.0) < 1e-6
							? ScanQRCode(fullImage, cropRect, out var res, out var rect)
							: ScanQRCodeStretch(fullImage, cropRect, mul, out res, out rect))
						{
							Code = res;
							var splash = new QRCodeSplashForm
							{
								Location = new Point(screen.Bounds.X, screen.Bounds.Y),
								TargetRect = new Rectangle(
									rect.Left + screen.Bounds.X,
									rect.Top + screen.Bounds.Y,
									rect.Width,
									rect.Height),
								Size = new Size(fullImage.Width, fullImage.Height)
							};
							splash.FormClosed += ShowQRCode;
							splash.Show();
							return;
						}
					}
				}
			}
			MessageBox.Show(@"未发现二维码，尝试把它放大或移动到靠近屏幕中间的位置", @"未扫描到二维码", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			ScanScreenQRCode();
		}

		private static void ShowQRCode(object sender, FormClosedEventArgs e)
		{
			var f = new QRCodeForm(Code);
			f.Show();
		}

		#endregion

		#region HotkeyReg

		private delegate void HotkeyEvent();
		private readonly HotkeyEvent HotkeyEvent1;

		private const int HotKeyID = 1; //热键ID（自定义）

		protected override void WndProc(ref Message msg)
		{
			base.WndProc(ref msg);
			switch (msg.Msg)
			{
				case 0x312://窗口消息：热键
					var tmpWParam = msg.WParam.ToInt32();
					if (tmpWParam == HotKeyID)
					{
						BeginInvoke(HotkeyEvent1);
					}
					break;
				case 0x1://窗口消息：创建
					Hotkey.RegHotKey(Handle, HotKeyID, Hotkey.KeyModifiers.Ctrl | Hotkey.KeyModifiers.Shift, Keys.A);//注册热键
					break;
				case 0x2://窗口消息：销毁
					Hotkey.UnRegHotKey(Handle, HotKeyID);//销毁热键
					break;
			}
		}

		#endregion

	}
}