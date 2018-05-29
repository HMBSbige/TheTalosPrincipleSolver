using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTalosPrincipleSolver
{
	public partial class PuzzleForm : Form
	{
		public PuzzleForm(int[][] block, int nPiece)
		{
			this.block = block;
			this.nPiece = nPiece;
			InitializeComponent();
		}

		private int[][] block;
		private int nPiece;
		private const int sizeofblock = 50;
		public void RePaint(int[][] blocks, int nPieces)
		{
			block = blocks.Clone() as int[][];
			nPiece = nPieces;
			Invalidate();
		}

		private void PuzzleForm_Paint(object sender, PaintEventArgs e)
		{
			Random ran = new Random((int)(DateTime.Now.Ticks / 10000000));
			Color[] color = new Color[nPiece + 1];
			color[0] = Color.Black;
			for (var i = 1; i < color.Length; ++i)
			{
				color[i] = Color.FromArgb(ran.Next(0, 256), ran.Next(0, 256), i * 255 / color.Length);
			}
			Graphics g = e.Graphics;

			for (var i = 0; i < block.Length; ++i)
			{
				for (var j = 0; j < block[i].Length; ++j)
				{
					var t = block[i][j];
					g.FillRectangle(t < 0 ? new SolidBrush(color[0]) : new SolidBrush(color[t]), j * sizeofblock, i * sizeofblock, sizeofblock, sizeofblock);
				}
			}
			Width = block[0].Length * sizeofblock + Width - ClientRectangle.Width;
			Height = block.Length * sizeofblock + Height - ClientRectangle.Height;
		}
	}
}
