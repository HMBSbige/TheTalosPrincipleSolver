using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTalosPrincipleSolver
{
    public partial class PuzzleForm : Form
    {
        public PuzzleForm(int[,] block, int nPiece)
        {
            this.block = block;
            this.nPiece = nPiece;
            InitializeComponent();
        }

        private int[,] block;
        private int nPiece;
        private const int sizeofblock=32;
        public void RePaint(int[,] blocks, int nPieces)
        {
            block = blocks.Clone() as int[,];
            nPiece = nPieces;
            Invalidate();
        }

        private void PuzzleForm_Paint(object sender, PaintEventArgs e)
        {
            Random ran = new Random((int)(DateTime.Now.Ticks /10000000));
            Color[] color= new Color[nPiece+1];
            color[0] = Color.Black;
            for (var i = 1; i < color.Length; ++i)
            {
                color[i] = Color.FromArgb(255, i* (255/color.Length), ran.Next(0, 256), ran.Next(0, 256));
            }
            Graphics g = e.Graphics;

            for (var i = 0; i < block.GetLength(0); ++i)
            {
                for (var j = 0; j < block.GetLength(1); ++j)
                {
                    if (block[i, j] < 0)
                        block[i, j] = 0;
                    g.FillRectangle(new SolidBrush(color[block[i,j]]), j * sizeofblock, i * sizeofblock, sizeofblock, sizeofblock);
                }
            }
            Width = block.GetLength(1) * sizeofblock + Width - ClientRectangle.Width;
            Height = block.GetLength(0) * sizeofblock + Height - ClientRectangle.Height;
        }
    }
}
