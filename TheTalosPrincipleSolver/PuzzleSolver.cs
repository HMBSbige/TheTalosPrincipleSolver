using System;
using System.Linq;

namespace TheTalosPrincipleSolver
{
	public class PuzzleSolver
	{
		public enum Block
		{
			I,
			O,
			T,
			J,
			L,
			S,
			Z
		}

		/// <summary>
		/// 行 x 列，每个 board 代表属于哪个 block（0 代表不属于任何 block）
		/// </summary>
		private readonly int[][] board;
		
		/// <summary>
		/// 放在板上的块的类型
		/// </summary>
		private readonly Block[] blocks;

		private int blocksPtr = 0;

		private readonly int nPieces;

		/// <summary>
		/// 构造函数，初始化
		/// </summary>
		/// <param name="height">行数</param>
		/// <param name="width">列数</param>
		/// <param name="iBlocks">I 形块的数量</param>
		/// <param name="oBlocks">2x2正方形方块的数量</param>
		/// <param name="tBlocks">T 形块的数量</param>
		/// <param name="jBlocks">J 形块的数量</param>
		/// <param name="lBlocks">L 形块的数量</param>
		/// <param name="sBlocks">S 形块的数量</param>
		/// <param name="zBlocks">Z 形块的数量</param>
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
			board = new int[height][];
			for (var xb = 0; xb < height; ++xb)
			{
				board[xb] = new int[width];
			}
			for (var y = 0; y < height; ++y)
			{
				for (var x = 0; x < width; ++x)
				{
					board[y][x] = 0;
				}
			}
			nPieces = iBlocks + oBlocks + tBlocks + jBlocks + lBlocks + sBlocks + zBlocks;
			blocks = new Block[nPieces];
			for (var i = 0; i < iBlocks; ++i)
			{
				blocks[blocksPtr++] = Block.I;
			}
			for (var i = 0; i < oBlocks; ++i)
			{
				blocks[blocksPtr++] = Block.O;
			}
			for (var i = 0; i < tBlocks; ++i)
			{
				blocks[blocksPtr++] = Block.T;
			}
			for (var i = 0; i < jBlocks; ++i)
			{
				blocks[blocksPtr++] = Block.J;
			}
			for (var i = 0; i < lBlocks; ++i)
			{
				blocks[blocksPtr++] = Block.L;
			}
			for (var i = 0; i < sBlocks; ++i)
			{
				blocks[blocksPtr++] = Block.S;
			}
			for (var i = 0; i < zBlocks; ++i)
			{
				blocks[blocksPtr++] = Block.Z;
			}
			blocksPtr = 0;
		}

		/// <summary>
		/// 计算一组相邻的空块有多大
		/// </summary>
		/// <param name="y">行数</param>
		/// <param name="x">列数</param>
		/// <returns>相邻的空块个数</returns>
		private int Group(int y, int x)
		{
			if (y >= 0 && y < board.Length && x >= 0 && x < board[0].Length && board[y][x] == 0)
			{
				board[y][x] = -1;
				return 1 + Group(y, x + 1) + Group(y, x - 1) + Group(y + 1, x) + Group(y - 1, x);
			}
			return 0;
		}

		/// <summary>
		/// 清除计算时临时做的记号
		/// </summary>
		private void ClearGroups()
		{
			foreach (var t in board)
			{
				for (var x = 0; x < board[0].Length; ++x)
				{
					if (t[x] == -1)
					{
						t[x] = 0;
					}
				}
			}
		}

		private bool IsStupidConfig()
		{
			for (var y = 0; y < board.Length; ++y)
			{
				for (var x = 0; x < board[0].Length; ++x)
				{
					if (board[y][x] == 0)
					{
						if (Group(y, x) % 4 != 0)
						{
							ClearGroups();
							return true;
						}
					}
				}
			}
			ClearGroups();
			return false;
		}

		private long iterations = 0;
		/// <summary>
		/// 获得递归了几次
		/// </summary>
		/// <returns>递归次数</returns>
		public long GetIterations()
		{
			return iterations;
		}

		/// <summary>
		/// 核心算法。暴力、贪心、分治、递归。
		/// </summary>
		/// <param name="p">传入 1 开始解</param>
		/// <returns>是否可解</returns>
		private bool SolveCore(int p)
		{
			iterations++;
			if (blocksPtr >= blocks.Length)
			{
				return true;
			}
			var block = blocks[blocksPtr++];

			if (block == Block.I)
			{
				// I 形块自旋后有2种放置方式
				for (int y = 0; y <= board.Length - 4; ++y)
				{
					for (int x = 0; x <= board[0].Length - 1; ++x)
					{
						if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y + 2][x] == 0 && board[y + 3][x] == 0)
						{
							board[y][x] = p;
							board[y + 1][x] = p;
							board[y + 2][x] = p;
							board[y + 3][x] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y][x] = 0;
							board[y + 1][x] = 0;
							board[y + 2][x] = 0;
							board[y + 3][x] = 0;
						}
					}
				}
				for (int y = 0; y <= board.Length - 1; ++y)
				{
					for (int x = 0; x <= board[0].Length - 4; ++x)
					{
						if (board[y][x] == 0 && board[y][x + 1] == 0 && board[y][x + 2] == 0 && board[y][x + 3] == 0)
						{
							board[y][x] = p;
							board[y][x + 1] = p;
							board[y][x + 2] = p;
							board[y][x + 3] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y][x] = 0;
							board[y][x + 1] = 0;
							board[y][x + 2] = 0;
							board[y][x + 3] = 0;
						}
					}
				}
				--blocksPtr;
				return false;
			}

			if (block == Block.O)
			{
				// 2x2正方形方块只有1种放置方式
				for (int y = 0; y <= board.Length - 2; ++y)
				{
					for (int x = 0; x <= board[0].Length - 2; ++x)
					{
						if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x + 1] == 0)
						{

							board[y][x] = p;
							board[y + 1][x] = p;
							board[y][x + 1] = p;
							board[y + 1][x + 1] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y][x] = 0;
							board[y + 1][x] = 0;
							board[y][x + 1] = 0;
							board[y + 1][x + 1] = 0;
						}
					}
				}


				--blocksPtr;
				return false;
			}

			if (block == Block.T)
			{
				// T 形块自旋后有4种放置方式
				for (int y = 0; y <= board.Length - 2; ++y)
				{
					for (int x = 0; x <= board[0].Length - 3; ++x)
					{
						if (board[y][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x + 1] == 0 && board[y][x + 2] == 0)
						{
							board[y][x] = p;
							board[y][x + 1] = p;
							board[y + 1][x + 1] = p;
							board[y][x + 2] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y][x] = 0;
							board[y][x + 1] = 0;
							board[y + 1][x + 1] = 0;
							board[y][x + 2] = 0;
						}
					}
				}
				for (int y = 0; y <= board.Length - 3; ++y)
				{
					for (int x = 0; x <= board[0].Length - 2; ++x)
					{
						if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y + 1][x + 1] == 0 && board[y + 2][x] == 0)
						{

							board[y][x] = p;
							board[y + 1][x] = p;
							board[y + 1][x + 1] = p;
							board[y + 2][x] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y][x] = 0;
							board[y + 1][x] = 0;
							board[y + 1][x + 1] = 0;
							board[y + 2][x] = 0;
						}
					}
				}
				for (int y = 0; y <= board.Length - 3; ++y)
				{
					for (int x = 0; x <= board[0].Length - 2; ++x)
					{
						if (board[y][x + 1] == 0 && board[y + 1][x] == 0 && board[y + 1][x + 1] == 0 && board[y + 2][x + 1] == 0)
						{

							board[y][x + 1] = p;
							board[y + 1][x] = p;
							board[y + 1][x + 1] = p;
							board[y + 2][x + 1] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y][x + 1] = 0;
							board[y + 1][x] = 0;
							board[y + 1][x + 1] = 0;
							board[y + 2][x + 1] = 0;
						}
					}
				}
				for (int y = 0; y <= board.Length - 2; ++y)
				{
					for (int x = 0; x <= board[0].Length - 3; ++x)
					{
						if (board[y + 1][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x + 1] == 0 && board[y + 1][x + 2] == 0)
						{

							board[y + 1][x] = p;
							board[y][x + 1] = p;
							board[y + 1][x + 1] = p;
							board[y + 1][x + 2] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y + 1][x] = 0;
							board[y][x + 1] = 0;
							board[y + 1][x + 1] = 0;
							board[y + 1][x + 2] = 0;
						}
					}
				}
				--blocksPtr;
				return false;
			}

			if (block == Block.J)
			{
				// J 形块自旋后有4种放置方式
				for (int y = 0; y <= board.Length - 2; ++y)
				{
					for (int x = 0; x <= board[0].Length - 3; ++x)
					{
						if (board[y][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x + 2] == 0 && board[y][x + 2] == 0)
						{

							board[y][x] = p;
							board[y][x + 1] = p;
							board[y + 1][x + 2] = p;
							board[y][x + 2] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y][x] = 0;
							board[y][x + 1] = 0;
							board[y + 1][x + 2] = 0;
							board[y][x + 2] = 0;
						}
					}
				}
				for (int y = 0; y <= board.Length - 2; ++y)
				{
					for (int x = 0; x <= board[0].Length - 3; ++x)
					{
						if (board[y + 1][x] == 0 && board[y][x] == 0 && board[y + 1][x + 1] == 0 && board[y + 1][x + 2] == 0)
						{

							board[y + 1][x] = p;
							board[y][x] = p;
							board[y + 1][x + 1] = p;
							board[y + 1][x + 2] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y + 1][x] = 0;
							board[y][x] = 0;
							board[y + 1][x + 1] = 0;
							board[y + 1][x + 2] = 0;
						}
					}
				}
				for (int y = 0; y <= board.Length - 3; ++y)
				{
					for (int x = 0; x <= board[0].Length - 2; ++x)
					{
						if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y][x + 1] == 0 && board[y + 2][x] == 0)
						{

							board[y][x] = p;
							board[y + 1][x] = p;
							board[y][x + 1] = p;
							board[y + 2][x] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y][x] = 0;
							board[y + 1][x] = 0;
							board[y][x + 1] = 0;
							board[y + 2][x] = 0;
						}
					}
				}
				for (int y = 0; y <= board.Length - 3; ++y)
				{
					for (int x = 0; x <= board[0].Length - 2; ++x)
					{
						if (board[y][x + 1] == 0 && board[y + 2][x] == 0 && board[y + 1][x + 1] == 0 && board[y + 2][x + 1] == 0)
						{

							board[y][x + 1] = p;
							board[y + 2][x] = p;
							board[y + 1][x + 1] = p;
							board[y + 2][x + 1] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y][x + 1] = 0;
							board[y + 2][x] = 0;
							board[y + 1][x + 1] = 0;
							board[y + 2][x + 1] = 0;
						}
					}
				}
				--blocksPtr;
				return false;
			}

			if (block == Block.L)
			{
				// L 形块自旋后有4种放置方式
				for (int y = 0; y <= board.Length - 2; ++y)
				{
					for (int x = 0; x <= board[0].Length - 3; ++x)
					{
						if (board[y][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x] == 0 && board[y][x + 2] == 0)
						{

							board[y][x] = p;
							board[y][x + 1] = p;
							board[y + 1][x] = p;
							board[y][x + 2] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y][x] = 0;
							board[y][x + 1] = 0;
							board[y + 1][x] = 0;
							board[y][x + 2] = 0;
						}
					}
				}
				for (int y = 0; y <= board.Length - 3; ++y)
				{
					for (int x = 0; x <= board[0].Length - 2; ++x)
					{
						if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y + 2][x + 1] == 0 && board[y + 2][x] == 0)
						{

							board[y][x] = p;
							board[y + 1][x] = p;
							board[y + 2][x + 1] = p;
							board[y + 2][x] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y][x] = 0;
							board[y + 1][x] = 0;
							board[y + 2][x + 1] = 0;
							board[y + 2][x] = 0;
						}
					}
				}
				for (int y = 0; y <= board.Length - 3; ++y)
				{
					for (int x = 0; x <= board[0].Length - 2; ++x)
					{
						if (board[y][x + 1] == 0 && board[y][x] == 0 && board[y + 1][x + 1] == 0 && board[y + 2][x + 1] == 0)
						{

							board[y][x + 1] = p;
							board[y][x] = p;
							board[y + 1][x + 1] = p;
							board[y + 2][x + 1] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y][x + 1] = 0;
							board[y][x] = 0;
							board[y + 1][x + 1] = 0;
							board[y + 2][x + 1] = 0;
						}
					}
				}
				for (int y = 0; y <= board.Length - 2; ++y)
				{
					for (int x = 0; x <= board[0].Length - 3; ++x)
					{
						if (board[y + 1][x] == 0 && board[y][x + 2] == 0 && board[y + 1][x + 1] == 0 && board[y + 1][x + 2] == 0)
						{

							board[y + 1][x] = p;
							board[y][x + 2] = p;
							board[y + 1][x + 1] = p;
							board[y + 1][x + 2] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y + 1][x] = 0;
							board[y][x + 2] = 0;
							board[y + 1][x + 1] = 0;
							board[y + 1][x + 2] = 0;
						}
					}
				}
				--blocksPtr;
				return false;
			}

			if (block == Block.S)
			{
				// S 形块自旋后有2种放置方式
				for (int y = 0; y <= board.Length - 3; ++y)
				{
					for (int x = 0; x <= board[0].Length - 2; ++x)
					{
						if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y + 1][x + 1] == 0 && board[y + 2][x + 1] == 0)
						{

							board[y][x] = p;
							board[y + 1][x] = p;
							board[y + 1][x + 1] = p;
							board[y + 2][x + 1] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y][x] = 0;
							board[y + 1][x] = 0;
							board[y + 1][x + 1] = 0;
							board[y + 2][x + 1] = 0;
						}
					}
				}
				for (int y = 0; y <= board.Length - 2; ++y)
				{
					for (int x = 0; x <= board[0].Length - 3; ++x)
					{
						if (board[y][x + 1] == 0 && board[y][x + 2] == 0 && board[y + 1][x] == 0 && board[y + 1][x + 1] == 0)
						{

							board[y][x + 1] = p;
							board[y][x + 2] = p;
							board[y + 1][x] = p;
							board[y + 1][x + 1] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y][x + 1] = 0;
							board[y][x + 2] = 0;
							board[y + 1][x] = 0;
							board[y + 1][x + 1] = 0;
						}
					}
				}
				--blocksPtr;
				return false;
			}

			if (block == Block.Z)
			{
				// Z 形块自旋后有2种放置方式
				for (int y = 0; y <= board.Length - 2; ++y)
				{
					for (int x = 0; x <= board[0].Length - 3; ++x)
					{
						if (board[y][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x + 1] == 0 && board[y + 1][x + 2] == 0)
						{

							board[y][x] = p;
							board[y][x + 1] = p;
							board[y + 1][x + 1] = p;
							board[y + 1][x + 2] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y][x] = 0;
							board[y][x + 1] = 0;
							board[y + 1][x + 1] = 0;
							board[y + 1][x + 2] = 0;
						}
					}
				}
				for (int y = 0; y <= board.Length - 3; ++y)
				{
					for (int x = 0; x <= board[0].Length - 2; ++x)
					{
						if (board[y][x + 1] == 0 && board[y + 1][x] == 0 && board[y + 1][x + 1] == 0 && board[y + 2][x] == 0)
						{

							board[y][x + 1] = p;
							board[y + 1][x] = p;
							board[y + 1][x + 1] = p;
							board[y + 2][x] = p;
							if (!IsStupidConfig() && SolveCore(p + 1))
							{
								return true;
							}
							board[y][x + 1] = 0;
							board[y + 1][x] = 0;
							board[y + 1][x + 1] = 0;
							board[y + 2][x] = 0;
						}
					}
				}
				--blocksPtr;
				return false;
			}
			
			throw new InvalidOperationException(@"算法出错！");
		}

		private bool _solved = false, _solveable = false;

		private readonly object _sync = new object();

		/// <summary>
		/// 调用这个函数开始解拼图
		/// </summary>
		/// <returns>是否有解</returns>
		public bool Solve()
		{
			lock (_sync)
			{
				if (_solved)
				{
					return _solveable;
				}
				_solveable = nPieces * 4 == board.Length * board[0].Length && SolveCore(1);
				_solved = true;
				return _solveable;
			}
		}

		/// <summary>
		/// 获得当前摆放状态
		/// </summary>
		/// <returns></returns>
		public int[][] GetBoard()
		{
			return board;
			//return board.Clone() as int[][];
		}

		/// <summary>
		/// 获得总块数
		/// </summary>
		/// <returns>总块数</returns>
		public int GetNumberOfPieces()
		{
			return nPieces;
		}

		/// <summary>
		/// 获得图形的宽
		/// </summary>
		/// <returns>图形的宽</returns>
		public int GetWidth()
		{
			return board[0].Length;
		}

		/// <summary>
		/// 获得图形的高
		/// </summary>
		/// <returns>图形的高</returns>
		public int GetHeight()
		{
			return board.Length;
		}

		/// <summary>
		/// 是否已经完成
		/// </summary>
		/// <returns></returns>
		public bool IsSolved()
		{
			return _solved;
		}

		/// <summary>
		/// 是否可解
		/// </summary>
		/// <returns></returns>
		public bool IsSolveable()
		{
			return _solveable;
		}

		/// <summary>
		/// 获得 I 形块的数量
		/// </summary>
		/// <returns>I 形块的数量</returns>
		public int GetIBlocks()
		{
			return blocks.Count(c => c == Block.I);
		}

		/// <summary>
		/// 获得正方形块的数量
		/// </summary>
		/// <returns>正方形块的数量</returns>
		public int GetOBlocks()
		{
			return blocks.Count(c => c == Block.O);
		}

		/// <summary>
		/// 获得 T 形块的数量
		/// </summary>
		/// <returns>T 形块的数量</returns>
		public int GetTBlocks()
		{
			return blocks.Count(c => c == Block.T);
		}

		/// <summary>
		/// 获得 J 形块的数量
		/// </summary>
		/// <returns>J 形块的数量</returns>
		public int GetJBlocks()
		{
			return blocks.Count(c => c == Block.J);
		}

		/// <summary>
		/// 获得 L 形块的数量
		/// </summary>
		/// <returns>L 形块的数量</returns>
		public int GetLBlocks()
		{
			return blocks.Count(c => c == Block.L);
		}

		/// <summary>
		/// 获得 S 形块的数量
		/// </summary>
		/// <returns>S 形块的数量</returns>
		public int GetSBlocks()
		{
			return blocks.Count(c => c == Block.S);
		}

		/// <summary>
		/// 获得 Z 形块的数量
		/// </summary>
		/// <returns>Z 形块的数量</returns>
		public int GetZBlocks()
		{
			return blocks.Count(c => c == Block.Z);
		}
	}
}