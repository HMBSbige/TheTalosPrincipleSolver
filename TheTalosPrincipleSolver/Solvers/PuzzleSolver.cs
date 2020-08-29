﻿using System;
using System.Runtime.CompilerServices;
using TheTalosPrincipleSolver.Enums;

namespace TheTalosPrincipleSolver.Solvers
{
	public class PuzzleSolver
	{
		/// <summary>
		/// 行 x 列，每个 board 代表属于哪个 block（0 代表不属于任何 block）
		/// </summary>
		public readonly int[][] Board;

		/// <summary>
		/// 放在板上的块的类型
		/// </summary>
		public Block[] Blocks { get; }

		private int _blocksPtr;

		public int Width => Board[0].Length;
		public int Height => Board.Length;

		/// <summary>
		/// 总块数
		/// </summary>
		public readonly int NumberOfPieces;

		/// <summary>
		/// 迭代总次数
		/// </summary>
		public long Iterations { get; private set; }

		/// <summary>
		/// 是否已解完
		/// </summary>
		public bool Solved { get; private set; }

		/// <summary>
		/// 是否有解
		/// </summary>
		public bool Solveable { get; private set; }

		public PuzzleSolver(int height, int width, int iBlocks, int oBlocks, int tBlocks, int jBlocks, int lBlocks, int sBlocks, int zBlocks)
		{
			if (width <= 0 || height <= 0)
			{
				throw new ArgumentException(@"宽度和高度必须大于 0");
			}
			if (iBlocks < 0 || oBlocks < 0 || tBlocks < 0 || jBlocks < 0 || lBlocks < 0 || sBlocks < 0 || zBlocks < 0)
			{
				throw new ArgumentException(@"块个数必须大于等于 0");
			}
			Board = new int[height][];
			for (var xb = 0; xb < height; ++xb)
			{
				Board[xb] = new int[width];
			}
			for (var y = 0; y < height; ++y)
			{
				for (var x = 0; x < width; ++x)
				{
					Board[y][x] = 0;
				}
			}
			NumberOfPieces = iBlocks + oBlocks + tBlocks + jBlocks + lBlocks + sBlocks + zBlocks;
			Blocks = new Block[NumberOfPieces];
			for (var i = 0; i < iBlocks; ++i)
			{
				Blocks[_blocksPtr++] = Block.I;
			}
			for (var i = 0; i < oBlocks; ++i)
			{
				Blocks[_blocksPtr++] = Block.O;
			}
			for (var i = 0; i < tBlocks; ++i)
			{
				Blocks[_blocksPtr++] = Block.T;
			}
			for (var i = 0; i < jBlocks; ++i)
			{
				Blocks[_blocksPtr++] = Block.J;
			}
			for (var i = 0; i < lBlocks; ++i)
			{
				Blocks[_blocksPtr++] = Block.L;
			}
			for (var i = 0; i < sBlocks; ++i)
			{
				Blocks[_blocksPtr++] = Block.S;
			}
			for (var i = 0; i < zBlocks; ++i)
			{
				Blocks[_blocksPtr++] = Block.Z;
			}
			_blocksPtr = 0;
		}

		/// <summary>
		/// 计算一组相邻的空块有多大
		/// </summary>
		/// <param name="y">行数</param>
		/// <param name="x">列数</param>
		/// <returns>相邻的空块个数</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		private int Group(int y, int x)
		{
			if (y < 0 || y >= Height || x < 0 || x >= Width || Board[y][x] != 0)
			{
				return 0;
			}

			Board[y][x] = -1;
			return 1 + Group(y, x + 1) + Group(y, x - 1) + Group(y + 1, x) + Group(y - 1, x);
		}

		/// <summary>
		/// 清除计算时临时做的记号
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		private void ClearGroups()
		{
			foreach (var t in Board)
			{
				for (var x = 0; x < Width; ++x)
				{
					if (t[x] == -1)
					{
						t[x] = 0;
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		private bool IsStupidConfig()
		{
			try
			{
				for (var y = 0; y < Height; ++y)
				{
					for (var x = 0; x < Width; ++x)
					{
						if (Board[y][x] == 0 && (Group(y, x) & 3) != 0) // %4
						{
							return true;
						}
					}
				}
				return false;
			}
			finally
			{
				ClearGroups();
			}
		}

		/// <summary>
		/// 核心算法。暴力、贪心、分治、递归。
		/// </summary>
		/// <param name="p">传入 1 开始解</param>
		/// <returns>是否可解</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		private bool SolveCore(int p)
		{
			++Iterations;
			if (_blocksPtr >= Blocks.Length)
			{
				return true;
			}
			var block = Blocks[_blocksPtr++];

			if (block == Block.I)
			{
				// I 形块自旋后有2种放置方式
				for (var y = 0; y <= Board.Length - 4; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 1; ++x)
					{
						if (Board[y][x] == 0 && Board[y + 1][x] == 0 && Board[y + 2][x] == 0 && Board[y + 3][x] == 0)
						{
							Board[y][x] = p;
							Board[y + 1][x] = p;
							Board[y + 2][x] = p;
							Board[y + 3][x] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y][x] = 0;
							Board[y + 1][x] = 0;
							Board[y + 2][x] = 0;
							Board[y + 3][x] = 0;
						}
					}
				}
				for (var y = 0; y <= Board.Length - 1; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 4; ++x)
					{
						if (Board[y][x] == 0 && Board[y][x + 1] == 0 && Board[y][x + 2] == 0 && Board[y][x + 3] == 0)
						{
							Board[y][x] = p;
							Board[y][x + 1] = p;
							Board[y][x + 2] = p;
							Board[y][x + 3] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y][x] = 0;
							Board[y][x + 1] = 0;
							Board[y][x + 2] = 0;
							Board[y][x + 3] = 0;
						}
					}
				}
				--_blocksPtr;
				return false;
			}

			if (block == Block.O)
			{
				// 2x2正方形方块只有1种放置方式
				for (var y = 0; y <= Board.Length - 2; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 2; ++x)
					{
						if (Board[y][x] == 0 && Board[y + 1][x] == 0 && Board[y][x + 1] == 0 && Board[y + 1][x + 1] == 0)
						{

							Board[y][x] = p;
							Board[y + 1][x] = p;
							Board[y][x + 1] = p;
							Board[y + 1][x + 1] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y][x] = 0;
							Board[y + 1][x] = 0;
							Board[y][x + 1] = 0;
							Board[y + 1][x + 1] = 0;
						}
					}
				}


				--_blocksPtr;
				return false;
			}

			if (block == Block.T)
			{
				// T 形块自旋后有4种放置方式
				for (var y = 0; y <= Board.Length - 2; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 3; ++x)
					{
						if (Board[y][x] == 0 && Board[y][x + 1] == 0 && Board[y + 1][x + 1] == 0 && Board[y][x + 2] == 0)
						{
							Board[y][x] = p;
							Board[y][x + 1] = p;
							Board[y + 1][x + 1] = p;
							Board[y][x + 2] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y][x] = 0;
							Board[y][x + 1] = 0;
							Board[y + 1][x + 1] = 0;
							Board[y][x + 2] = 0;
						}
					}
				}
				for (var y = 0; y <= Board.Length - 3; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 2; ++x)
					{
						if (Board[y][x] == 0 && Board[y + 1][x] == 0 && Board[y + 1][x + 1] == 0 && Board[y + 2][x] == 0)
						{

							Board[y][x] = p;
							Board[y + 1][x] = p;
							Board[y + 1][x + 1] = p;
							Board[y + 2][x] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y][x] = 0;
							Board[y + 1][x] = 0;
							Board[y + 1][x + 1] = 0;
							Board[y + 2][x] = 0;
						}
					}
				}
				for (var y = 0; y <= Board.Length - 3; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 2; ++x)
					{
						if (Board[y][x + 1] == 0 && Board[y + 1][x] == 0 && Board[y + 1][x + 1] == 0 && Board[y + 2][x + 1] == 0)
						{

							Board[y][x + 1] = p;
							Board[y + 1][x] = p;
							Board[y + 1][x + 1] = p;
							Board[y + 2][x + 1] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y][x + 1] = 0;
							Board[y + 1][x] = 0;
							Board[y + 1][x + 1] = 0;
							Board[y + 2][x + 1] = 0;
						}
					}
				}
				for (var y = 0; y <= Board.Length - 2; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 3; ++x)
					{
						if (Board[y + 1][x] == 0 && Board[y][x + 1] == 0 && Board[y + 1][x + 1] == 0 && Board[y + 1][x + 2] == 0)
						{

							Board[y + 1][x] = p;
							Board[y][x + 1] = p;
							Board[y + 1][x + 1] = p;
							Board[y + 1][x + 2] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y + 1][x] = 0;
							Board[y][x + 1] = 0;
							Board[y + 1][x + 1] = 0;
							Board[y + 1][x + 2] = 0;
						}
					}
				}
				--_blocksPtr;
				return false;
			}

			if (block == Block.J)
			{
				// J 形块自旋后有4种放置方式
				for (var y = 0; y <= Board.Length - 2; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 3; ++x)
					{
						if (Board[y][x] == 0 && Board[y][x + 1] == 0 && Board[y + 1][x + 2] == 0 && Board[y][x + 2] == 0)
						{

							Board[y][x] = p;
							Board[y][x + 1] = p;
							Board[y + 1][x + 2] = p;
							Board[y][x + 2] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y][x] = 0;
							Board[y][x + 1] = 0;
							Board[y + 1][x + 2] = 0;
							Board[y][x + 2] = 0;
						}
					}
				}
				for (var y = 0; y <= Board.Length - 2; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 3; ++x)
					{
						if (Board[y + 1][x] == 0 && Board[y][x] == 0 && Board[y + 1][x + 1] == 0 && Board[y + 1][x + 2] == 0)
						{

							Board[y + 1][x] = p;
							Board[y][x] = p;
							Board[y + 1][x + 1] = p;
							Board[y + 1][x + 2] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y + 1][x] = 0;
							Board[y][x] = 0;
							Board[y + 1][x + 1] = 0;
							Board[y + 1][x + 2] = 0;
						}
					}
				}
				for (var y = 0; y <= Board.Length - 3; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 2; ++x)
					{
						if (Board[y][x] == 0 && Board[y + 1][x] == 0 && Board[y][x + 1] == 0 && Board[y + 2][x] == 0)
						{

							Board[y][x] = p;
							Board[y + 1][x] = p;
							Board[y][x + 1] = p;
							Board[y + 2][x] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y][x] = 0;
							Board[y + 1][x] = 0;
							Board[y][x + 1] = 0;
							Board[y + 2][x] = 0;
						}
					}
				}
				for (var y = 0; y <= Board.Length - 3; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 2; ++x)
					{
						if (Board[y][x + 1] == 0 && Board[y + 2][x] == 0 && Board[y + 1][x + 1] == 0 && Board[y + 2][x + 1] == 0)
						{

							Board[y][x + 1] = p;
							Board[y + 2][x] = p;
							Board[y + 1][x + 1] = p;
							Board[y + 2][x + 1] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y][x + 1] = 0;
							Board[y + 2][x] = 0;
							Board[y + 1][x + 1] = 0;
							Board[y + 2][x + 1] = 0;
						}
					}
				}
				--_blocksPtr;
				return false;
			}

			if (block == Block.L)
			{
				// L 形块自旋后有4种放置方式
				for (var y = 0; y <= Board.Length - 2; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 3; ++x)
					{
						if (Board[y][x] == 0 && Board[y][x + 1] == 0 && Board[y + 1][x] == 0 && Board[y][x + 2] == 0)
						{

							Board[y][x] = p;
							Board[y][x + 1] = p;
							Board[y + 1][x] = p;
							Board[y][x + 2] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y][x] = 0;
							Board[y][x + 1] = 0;
							Board[y + 1][x] = 0;
							Board[y][x + 2] = 0;
						}
					}
				}
				for (var y = 0; y <= Board.Length - 3; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 2; ++x)
					{
						if (Board[y][x] == 0 && Board[y + 1][x] == 0 && Board[y + 2][x + 1] == 0 && Board[y + 2][x] == 0)
						{

							Board[y][x] = p;
							Board[y + 1][x] = p;
							Board[y + 2][x + 1] = p;
							Board[y + 2][x] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y][x] = 0;
							Board[y + 1][x] = 0;
							Board[y + 2][x + 1] = 0;
							Board[y + 2][x] = 0;
						}
					}
				}
				for (var y = 0; y <= Board.Length - 3; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 2; ++x)
					{
						if (Board[y][x + 1] == 0 && Board[y][x] == 0 && Board[y + 1][x + 1] == 0 && Board[y + 2][x + 1] == 0)
						{

							Board[y][x + 1] = p;
							Board[y][x] = p;
							Board[y + 1][x + 1] = p;
							Board[y + 2][x + 1] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y][x + 1] = 0;
							Board[y][x] = 0;
							Board[y + 1][x + 1] = 0;
							Board[y + 2][x + 1] = 0;
						}
					}
				}
				for (var y = 0; y <= Board.Length - 2; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 3; ++x)
					{
						if (Board[y + 1][x] == 0 && Board[y][x + 2] == 0 && Board[y + 1][x + 1] == 0 && Board[y + 1][x + 2] == 0)
						{

							Board[y + 1][x] = p;
							Board[y][x + 2] = p;
							Board[y + 1][x + 1] = p;
							Board[y + 1][x + 2] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y + 1][x] = 0;
							Board[y][x + 2] = 0;
							Board[y + 1][x + 1] = 0;
							Board[y + 1][x + 2] = 0;
						}
					}
				}
				--_blocksPtr;
				return false;
			}

			if (block == Block.S)
			{
				// S 形块自旋后有2种放置方式
				for (var y = 0; y <= Board.Length - 3; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 2; ++x)
					{
						if (Board[y][x] == 0 && Board[y + 1][x] == 0 && Board[y + 1][x + 1] == 0 && Board[y + 2][x + 1] == 0)
						{

							Board[y][x] = p;
							Board[y + 1][x] = p;
							Board[y + 1][x + 1] = p;
							Board[y + 2][x + 1] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y][x] = 0;
							Board[y + 1][x] = 0;
							Board[y + 1][x + 1] = 0;
							Board[y + 2][x + 1] = 0;
						}
					}
				}
				for (var y = 0; y <= Board.Length - 2; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 3; ++x)
					{
						if (Board[y][x + 1] == 0 && Board[y][x + 2] == 0 && Board[y + 1][x] == 0 && Board[y + 1][x + 1] == 0)
						{

							Board[y][x + 1] = p;
							Board[y][x + 2] = p;
							Board[y + 1][x] = p;
							Board[y + 1][x + 1] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y][x + 1] = 0;
							Board[y][x + 2] = 0;
							Board[y + 1][x] = 0;
							Board[y + 1][x + 1] = 0;
						}
					}
				}
				--_blocksPtr;
				return false;
			}

			if (block == Block.Z)
			{
				// Z 形块自旋后有2种放置方式
				for (var y = 0; y <= Board.Length - 2; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 3; ++x)
					{
						if (Board[y][x] == 0 && Board[y][x + 1] == 0 && Board[y + 1][x + 1] == 0 && Board[y + 1][x + 2] == 0)
						{

							Board[y][x] = p;
							Board[y][x + 1] = p;
							Board[y + 1][x + 1] = p;
							Board[y + 1][x + 2] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y][x] = 0;
							Board[y][x + 1] = 0;
							Board[y + 1][x + 1] = 0;
							Board[y + 1][x + 2] = 0;
						}
					}
				}
				for (var y = 0; y <= Board.Length - 3; ++y)
				{
					for (var x = 0; x <= Board[0].Length - 2; ++x)
					{
						if (Board[y][x + 1] == 0 && Board[y + 1][x] == 0 && Board[y + 1][x + 1] == 0 && Board[y + 2][x] == 0)
						{

							Board[y][x + 1] = p;
							Board[y + 1][x] = p;
							Board[y + 1][x + 1] = p;
							Board[y + 2][x] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							Board[y][x + 1] = 0;
							Board[y + 1][x] = 0;
							Board[y + 1][x + 1] = 0;
							Board[y + 2][x] = 0;
						}
					}
				}
				--_blocksPtr;
				return false;
			}

			throw new InvalidOperationException(@"算法出错！");
		}

		/// <summary>
		/// 调用这个函数开始解拼图
		/// </summary>
		/// <returns>是否有解</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool Solve()
		{
			if (Solved)
			{
				return Solveable;
			}
			Solveable = NumberOfPieces << 2 == Width * Height && SolveCore(1);
			Solved = true;
			return Solveable;
		}
	}
}