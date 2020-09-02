using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TheTalosPrincipleSolver.Enums;
using TheTalosPrincipleSolver.Models;
using TheTalosPrincipleSolver.Utils;

namespace TheTalosPrincipleSolver.Solvers
{
	public class PuzzleSolverMT
	{
		public Block[] Blocks { get; }

		public uint NumberOfPieces { get; }

		public long Width { get; }
		public long Height { get; }

		public bool Solved { get; private set; }

		private long iterations;
		public long Iterations => iterations;

		public int Threads { get; set; } = Environment.ProcessorCount;

		private readonly BlockingCollection<BoardState> stack = new BlockingCollection<BoardState>(new ConcurrentStack<BoardState>());

		private BoardState cachedResult;

		private readonly CancellationTokenSource cts;

		public bool IsCanceled => cts?.IsCancellationRequested ?? false;

		public PuzzleSolverMT(Puzzle puzzle)
		{
			Width = puzzle.Column;
			Height = puzzle.Row;

			if (Width <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(Width), @"Width must be > 0");
			}

			if (Height <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(Height), @"Height must be > 0");
			}

			cts = new CancellationTokenSource();

			NumberOfPieces = puzzle.NumberOfI + puzzle.NumberOfO + puzzle.NumberOfT + puzzle.NumberOfJ + puzzle.NumberOfL + puzzle.NumberOfS + puzzle.NumberOfZ;

			if (NumberOfPieces << 2 != Width * Height)
			{
				Solved = true;
				cachedResult = null;
				return;
			}

			Blocks = new Block[NumberOfPieces];

			var p = 0;
			for (var i = 0; i < puzzle.NumberOfI; ++i)
			{
				Blocks[p++] = Block.I;
			}
			for (var i = 0; i < puzzle.NumberOfO; ++i)
			{
				Blocks[p++] = Block.O;
			}
			for (var i = 0; i < puzzle.NumberOfT; ++i)
			{
				Blocks[p++] = Block.T;
			}
			for (var i = 0; i < puzzle.NumberOfJ; ++i)
			{
				Blocks[p++] = Block.J;
			}
			for (var i = 0; i < puzzle.NumberOfL; ++i)
			{
				Blocks[p++] = Block.L;
			}
			for (var i = 0; i < puzzle.NumberOfS; ++i)
			{
				Blocks[p++] = Block.S;
			}
			for (var i = 0; i < puzzle.NumberOfZ; ++i)
			{
				Blocks[p++] = Block.Z;
			}

			Blocks.Shuffle();
			var s = new BoardState(Width, Height);
			stack.Add(s);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public BoardState Solve()
		{
			if (Solved)
			{
				return cachedResult;
			}
			//TODO
			return null;
		}

		public int[][] CurrentBoard { get; private set; }

		private async Task<BoardState> Start(CancellationToken token = default)
		{
			while (true)
			{
				var workUnit = stack.Take(token);
				Interlocked.Increment(ref iterations);

				var board = workUnit.Board;
				var p = workUnit.P;
				var block = Blocks[p - 1];

				switch (block)
				{
					case Block.I:
					{
						// I 形块自旋后有2种放置方式
						// I
						for (var y = 0; y <= Height - 4; ++y)
						{
							for (var x = 0; x <= Width - 1; ++x)
							{
								if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y + 2][x] == 0 && board[y + 3][x] == 0)
								{
									board[y][x] = p;
									board[y + 1][x] = p;
									board[y + 2][x] = p;
									board[y + 3][x] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y][x] = 0;
									board[y + 1][x] = 0;
									board[y + 2][x] = 0;
									board[y + 3][x] = 0;
								}
							}
						}

						//—
						for (var y = 0; y <= Height - 1; ++y)
						{
							for (var x = 0; x <= Width - 4; ++x)
							{
								if (board[y][x] == 0 && board[y][x + 1] == 0 && board[y][x + 2] == 0 && board[y][x + 3] == 0)
								{
									board[y][x] = p;
									board[y][x + 1] = p;
									board[y][x + 2] = p;
									board[y][x + 3] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y][x] = 0;
									board[y][x + 1] = 0;
									board[y][x + 2] = 0;
									board[y][x + 3] = 0;
								}
							}
						}

						break;
					}
					case Block.O:
					{
						// 2x2正方形方块只有1种放置方式
						// O
						for (var y = 0; y <= Height - 2; ++y)
						{
							for (var x = 0; x <= Width - 2; ++x)
							{
								if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x + 1] == 0)
								{
									board[y][x] = p;
									board[y + 1][x] = p;
									board[y][x + 1] = p;
									board[y + 1][x + 1] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y][x] = 0;
									board[y + 1][x] = 0;
									board[y][x + 1] = 0;
									board[y + 1][x + 1] = 0;
								}
							}
						}

						break;
					}
					case Block.T:
					{
						// T 形块自旋后有4种放置方式
						// ┳
						for (var y = 0; y <= Height - 2; ++y)
						{
							for (var x = 0; x <= Width - 3; ++x)
							{
								if (board[y][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x + 1] == 0 && board[y][x + 2] == 0)
								{
									board[y][x] = p;
									board[y][x + 1] = p;
									board[y + 1][x + 1] = p;
									board[y][x + 2] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y][x] = 0;
									board[y][x + 1] = 0;
									board[y + 1][x + 1] = 0;
									board[y][x + 2] = 0;
								}
							}
						}

						// ┣
						for (var y = 0; y <= Height - 3; ++y)
						{
							for (var x = 0; x <= Width - 2; ++x)
							{
								if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y + 1][x + 1] == 0 && board[y + 2][x] == 0)
								{

									board[y][x] = p;
									board[y + 1][x] = p;
									board[y + 1][x + 1] = p;
									board[y + 2][x] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y][x] = 0;
									board[y + 1][x] = 0;
									board[y + 1][x + 1] = 0;
									board[y + 2][x] = 0;
								}
							}
						}

						// ┫
						for (var y = 0; y <= Height - 3; ++y)
						{
							for (var x = 0; x <= Width - 2; ++x)
							{
								if (board[y][x + 1] == 0 && board[y + 1][x] == 0 && board[y + 1][x + 1] == 0 && board[y + 2][x + 1] == 0)
								{

									board[y][x + 1] = p;
									board[y + 1][x] = p;
									board[y + 1][x + 1] = p;
									board[y + 2][x + 1] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y][x + 1] = 0;
									board[y + 1][x] = 0;
									board[y + 1][x + 1] = 0;
									board[y + 2][x + 1] = 0;
								}
							}
						}

						// ┻
						for (var y = 0; y <= Height - 2; ++y)
						{
							for (var x = 0; x <= Width - 3; ++x)
							{
								if (board[y + 1][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x + 1] == 0 && board[y + 1][x + 2] == 0)
								{

									board[y + 1][x] = p;
									board[y][x + 1] = p;
									board[y + 1][x + 1] = p;
									board[y + 1][x + 2] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y + 1][x] = 0;
									board[y][x + 1] = 0;
									board[y + 1][x + 1] = 0;
									board[y + 1][x + 2] = 0;
								}
							}
						}

						break;
					}
					case Block.J:
					{
						// J 形块自旋后有4种放置方式
						// ┓
						for (var y = 0; y <= Height - 2; ++y)
						{
							for (var x = 0; x <= Width - 3; ++x)
							{
								if (board[y][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x + 2] == 0 && board[y][x + 2] == 0)
								{

									board[y][x] = p;
									board[y][x + 1] = p;
									board[y + 1][x + 2] = p;
									board[y][x + 2] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y][x] = 0;
									board[y][x + 1] = 0;
									board[y + 1][x + 2] = 0;
									board[y][x + 2] = 0;
								}
							}
						}

						// ┗
						for (var y = 0; y <= Height - 2; ++y)
						{
							for (var x = 0; x <= Width - 3; ++x)
							{
								if (board[y + 1][x] == 0 && board[y][x] == 0 && board[y + 1][x + 1] == 0 && board[y + 1][x + 2] == 0)
								{

									board[y + 1][x] = p;
									board[y][x] = p;
									board[y + 1][x + 1] = p;
									board[y + 1][x + 2] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y + 1][x] = 0;
									board[y][x] = 0;
									board[y + 1][x + 1] = 0;
									board[y + 1][x + 2] = 0;
								}
							}
						}

						// ┏
						for (var y = 0; y <= Height - 3; ++y)
						{
							for (var x = 0; x <= Width - 2; ++x)
							{
								if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y][x + 1] == 0 && board[y + 2][x] == 0)
								{

									board[y][x] = p;
									board[y + 1][x] = p;
									board[y][x + 1] = p;
									board[y + 2][x] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y][x] = 0;
									board[y + 1][x] = 0;
									board[y][x + 1] = 0;
									board[y + 2][x] = 0;
								}
							}
						}

						// ┛
						for (var y = 0; y <= Height - 3; ++y)
						{
							for (var x = 0; x <= Width - 2; ++x)
							{
								if (board[y][x + 1] == 0 && board[y + 2][x] == 0 && board[y + 1][x + 1] == 0 && board[y + 2][x + 1] == 0)
								{

									board[y][x + 1] = p;
									board[y + 2][x] = p;
									board[y + 1][x + 1] = p;
									board[y + 2][x + 1] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y][x + 1] = 0;
									board[y + 2][x] = 0;
									board[y + 1][x + 1] = 0;
									board[y + 2][x + 1] = 0;
								}
							}
						}

						break;
					}
					case Block.L:
					{
						// L 形块自旋后有4种放置方式
						// ┏
						for (var y = 0; y <= Height - 2; ++y)
						{
							for (var x = 0; x <= Width - 3; ++x)
							{
								if (board[y][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x] == 0 && board[y][x + 2] == 0)
								{

									board[y][x] = p;
									board[y][x + 1] = p;
									board[y + 1][x] = p;
									board[y][x + 2] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y][x] = 0;
									board[y][x + 1] = 0;
									board[y + 1][x] = 0;
									board[y][x + 2] = 0;
								}
							}
						}

						// ┗
						for (var y = 0; y <= Height - 3; ++y)
						{
							for (var x = 0; x <= Width - 2; ++x)
							{
								if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y + 2][x + 1] == 0 && board[y + 2][x] == 0)
								{

									board[y][x] = p;
									board[y + 1][x] = p;
									board[y + 2][x + 1] = p;
									board[y + 2][x] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y][x] = 0;
									board[y + 1][x] = 0;
									board[y + 2][x + 1] = 0;
									board[y + 2][x] = 0;
								}
							}
						}

						// ┓
						for (var y = 0; y <= Height - 3; ++y)
						{
							for (var x = 0; x <= Width - 2; ++x)
							{
								if (board[y][x + 1] == 0 && board[y][x] == 0 && board[y + 1][x + 1] == 0 && board[y + 2][x + 1] == 0)
								{

									board[y][x + 1] = p;
									board[y][x] = p;
									board[y + 1][x + 1] = p;
									board[y + 2][x + 1] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y][x + 1] = 0;
									board[y][x] = 0;
									board[y + 1][x + 1] = 0;
									board[y + 2][x + 1] = 0;
								}
							}
						}

						// ┛
						for (var y = 0; y <= Height - 2; ++y)
						{
							for (var x = 0; x <= Width - 3; ++x)
							{
								if (board[y + 1][x] == 0 && board[y][x + 2] == 0 && board[y + 1][x + 1] == 0 && board[y + 1][x + 2] == 0)
								{

									board[y + 1][x] = p;
									board[y][x + 2] = p;
									board[y + 1][x + 1] = p;
									board[y + 1][x + 2] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y + 1][x] = 0;
									board[y][x + 2] = 0;
									board[y + 1][x + 1] = 0;
									board[y + 1][x + 2] = 0;
								}
							}
						}

						break;
					}
					case Block.S:
					{
						// S 形块自旋后有2种放置方式
						// #
						// ##
						//  #
						for (var y = 0; y <= Height - 3; ++y)
						{
							for (var x = 0; x <= Width - 2; ++x)
							{
								if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y + 1][x + 1] == 0 && board[y + 2][x + 1] == 0)
								{
									board[y][x] = p;
									board[y + 1][x] = p;
									board[y + 1][x + 1] = p;
									board[y + 2][x + 1] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y][x] = 0;
									board[y + 1][x] = 0;
									board[y + 1][x + 1] = 0;
									board[y + 2][x + 1] = 0;
								}
							}
						}

						//  ##
						// ##
						for (var y = 0; y <= Height - 2; ++y)
						{
							for (var x = 0; x <= Width - 3; ++x)
							{
								if (board[y][x + 1] == 0 && board[y][x + 2] == 0 && board[y + 1][x] == 0 && board[y + 1][x + 1] == 0)
								{

									board[y][x + 1] = p;
									board[y][x + 2] = p;
									board[y + 1][x] = p;
									board[y + 1][x + 1] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y][x + 1] = 0;
									board[y][x + 2] = 0;
									board[y + 1][x] = 0;
									board[y + 1][x + 1] = 0;
								}
							}
						}

						break;
					}
					case Block.Z:
					{
						// Z 形块自旋后有2种放置方式

						// Z
						for (var y = 0; y <= Height - 2; ++y)
						{
							for (var x = 0; x <= Width - 3; ++x)
							{
								if (board[y][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x + 1] == 0 && board[y + 1][x + 2] == 0)
								{

									board[y][x] = p;
									board[y][x + 1] = p;
									board[y + 1][x + 1] = p;
									board[y + 1][x + 2] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y][x] = 0;
									board[y][x + 1] = 0;
									board[y + 1][x + 1] = 0;
									board[y + 1][x + 2] = 0;
								}
							}
						}

						//  ┃
						// ━━
						// ┃
						for (var y = 0; y <= Height - 3; ++y)
						{
							for (var x = 0; x <= Width - 2; ++x)
							{
								if (board[y][x + 1] == 0 && board[y + 1][x] == 0 && board[y + 1][x + 1] == 0 &&
									board[y + 2][x] == 0)
								{

									board[y][x + 1] = p;
									board[y + 1][x] = p;
									board[y + 1][x + 1] = p;
									board[y + 2][x] = p;

									if (p == NumberOfPieces)
									{
										return new BoardState(workUnit);
									}

									if (!workUnit.IsStupidConfig())
									{
										stack.Add(new BoardState(workUnit), token);
									}

									board[y][x + 1] = 0;
									board[y + 1][x] = 0;
									board[y + 1][x + 1] = 0;
									board[y + 2][x] = 0;
								}
							}
						}
						
						break;
					}
					default:
					{
						throw new InvalidOperationException(@"算法出错！");
					}
				}

			}
		}
	}
}
