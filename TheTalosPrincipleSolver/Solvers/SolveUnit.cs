using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TheTalosPrincipleSolver.Enums;

namespace TheTalosPrincipleSolver.Solvers
{
	public class SolveUnit
	{
		private readonly PuzzleSolverMT parent;
		private readonly CancellationToken token;

		public int[][] Board { get; private set; }

		public Task<BoardState> Task { get; }

		public SolveUnit(PuzzleSolverMT solver, CancellationToken ct)
		{
			parent = solver;
			token = ct;
			Task = UnitTask();
			Task.ContinueWith(t =>
			{
				if (t.IsCompletedSuccessfully)
				{
					parent.Stack.CompleteAdding();
				}
			}, token);
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		public void Start()
		{
			try
			{
				Task.Start();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		private Task<BoardState> UnitTask()
		{
			return new Task<BoardState>(() =>
			{
				try
				{
					while (true)
					{
						BoardState workUnit;

						parent.Waiting();
						try
						{
							workUnit = parent.Stack.Take(token);
						}
						finally
						{
							parent.NotWaiting();
						}

						parent.IncrementIterations();

						var board = workUnit.Board;
						var p = workUnit.P;
						var block = parent.Blocks[p - 1];
						Board = board;

						switch (block)
						{
							case Block.I:
							{
								// I 形块自旋后有2种放置方式
								// I
								for (var y = 0; y <= parent.Height - 4; ++y)
								{
									for (var x = 0; x <= parent.Width - 1; ++x)
									{
										if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y + 2][x] == 0 &&
											board[y + 3][x] == 0)
										{
											board[y][x] = p;
											board[y + 1][x] = p;
											board[y + 2][x] = p;
											board[y + 3][x] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
											}

											board[y][x] = 0;
											board[y + 1][x] = 0;
											board[y + 2][x] = 0;
											board[y + 3][x] = 0;
										}
									}
								}

								//—
								for (var y = 0; y <= parent.Height - 1; ++y)
								{
									for (var x = 0; x <= parent.Width - 4; ++x)
									{
										if (board[y][x] == 0 && board[y][x + 1] == 0 && board[y][x + 2] == 0 &&
											board[y][x + 3] == 0)
										{
											board[y][x] = p;
											board[y][x + 1] = p;
											board[y][x + 2] = p;
											board[y][x + 3] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
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
								for (var y = 0; y <= parent.Height - 2; ++y)
								{
									for (var x = 0; x <= parent.Width - 2; ++x)
									{
										if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y][x + 1] == 0 &&
											board[y + 1][x + 1] == 0)
										{
											board[y][x] = p;
											board[y + 1][x] = p;
											board[y][x + 1] = p;
											board[y + 1][x + 1] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
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
								for (var y = 0; y <= parent.Height - 2; ++y)
								{
									for (var x = 0; x <= parent.Width - 3; ++x)
									{
										if (board[y][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x + 1] == 0 &&
											board[y][x + 2] == 0)
										{
											board[y][x] = p;
											board[y][x + 1] = p;
											board[y + 1][x + 1] = p;
											board[y][x + 2] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
											}

											board[y][x] = 0;
											board[y][x + 1] = 0;
											board[y + 1][x + 1] = 0;
											board[y][x + 2] = 0;
										}
									}
								}

								// ┣
								for (var y = 0; y <= parent.Height - 3; ++y)
								{
									for (var x = 0; x <= parent.Width - 2; ++x)
									{
										if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y + 1][x + 1] == 0 &&
											board[y + 2][x] == 0)
										{

											board[y][x] = p;
											board[y + 1][x] = p;
											board[y + 1][x + 1] = p;
											board[y + 2][x] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
											}

											board[y][x] = 0;
											board[y + 1][x] = 0;
											board[y + 1][x + 1] = 0;
											board[y + 2][x] = 0;
										}
									}
								}

								// ┫
								for (var y = 0; y <= parent.Height - 3; ++y)
								{
									for (var x = 0; x <= parent.Width - 2; ++x)
									{
										if (board[y][x + 1] == 0 && board[y + 1][x] == 0 && board[y + 1][x + 1] == 0 &&
											board[y + 2][x + 1] == 0)
										{

											board[y][x + 1] = p;
											board[y + 1][x] = p;
											board[y + 1][x + 1] = p;
											board[y + 2][x + 1] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
											}

											board[y][x + 1] = 0;
											board[y + 1][x] = 0;
											board[y + 1][x + 1] = 0;
											board[y + 2][x + 1] = 0;
										}
									}
								}

								// ┻
								for (var y = 0; y <= parent.Height - 2; ++y)
								{
									for (var x = 0; x <= parent.Width - 3; ++x)
									{
										if (board[y + 1][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x + 1] == 0 &&
											board[y + 1][x + 2] == 0)
										{

											board[y + 1][x] = p;
											board[y][x + 1] = p;
											board[y + 1][x + 1] = p;
											board[y + 1][x + 2] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
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
								for (var y = 0; y <= parent.Height - 2; ++y)
								{
									for (var x = 0; x <= parent.Width - 3; ++x)
									{
										if (board[y][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x + 2] == 0 &&
											board[y][x + 2] == 0)
										{

											board[y][x] = p;
											board[y][x + 1] = p;
											board[y + 1][x + 2] = p;
											board[y][x + 2] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
											}

											board[y][x] = 0;
											board[y][x + 1] = 0;
											board[y + 1][x + 2] = 0;
											board[y][x + 2] = 0;
										}
									}
								}

								// ┗
								for (var y = 0; y <= parent.Height - 2; ++y)
								{
									for (var x = 0; x <= parent.Width - 3; ++x)
									{
										if (board[y + 1][x] == 0 && board[y][x] == 0 && board[y + 1][x + 1] == 0 &&
											board[y + 1][x + 2] == 0)
										{

											board[y + 1][x] = p;
											board[y][x] = p;
											board[y + 1][x + 1] = p;
											board[y + 1][x + 2] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
											}

											board[y + 1][x] = 0;
											board[y][x] = 0;
											board[y + 1][x + 1] = 0;
											board[y + 1][x + 2] = 0;
										}
									}
								}

								// ┏
								for (var y = 0; y <= parent.Height - 3; ++y)
								{
									for (var x = 0; x <= parent.Width - 2; ++x)
									{
										if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y][x + 1] == 0 &&
											board[y + 2][x] == 0)
										{

											board[y][x] = p;
											board[y + 1][x] = p;
											board[y][x + 1] = p;
											board[y + 2][x] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
											}

											board[y][x] = 0;
											board[y + 1][x] = 0;
											board[y][x + 1] = 0;
											board[y + 2][x] = 0;
										}
									}
								}

								// ┛
								for (var y = 0; y <= parent.Height - 3; ++y)
								{
									for (var x = 0; x <= parent.Width - 2; ++x)
									{
										if (board[y][x + 1] == 0 && board[y + 2][x] == 0 && board[y + 1][x + 1] == 0 &&
											board[y + 2][x + 1] == 0)
										{

											board[y][x + 1] = p;
											board[y + 2][x] = p;
											board[y + 1][x + 1] = p;
											board[y + 2][x + 1] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
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
								for (var y = 0; y <= parent.Height - 2; ++y)
								{
									for (var x = 0; x <= parent.Width - 3; ++x)
									{
										if (board[y][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x] == 0 &&
											board[y][x + 2] == 0)
										{

											board[y][x] = p;
											board[y][x + 1] = p;
											board[y + 1][x] = p;
											board[y][x + 2] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
											}

											board[y][x] = 0;
											board[y][x + 1] = 0;
											board[y + 1][x] = 0;
											board[y][x + 2] = 0;
										}
									}
								}

								// ┗
								for (var y = 0; y <= parent.Height - 3; ++y)
								{
									for (var x = 0; x <= parent.Width - 2; ++x)
									{
										if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y + 2][x + 1] == 0 &&
											board[y + 2][x] == 0)
										{

											board[y][x] = p;
											board[y + 1][x] = p;
											board[y + 2][x + 1] = p;
											board[y + 2][x] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
											}

											board[y][x] = 0;
											board[y + 1][x] = 0;
											board[y + 2][x + 1] = 0;
											board[y + 2][x] = 0;
										}
									}
								}

								// ┓
								for (var y = 0; y <= parent.Height - 3; ++y)
								{
									for (var x = 0; x <= parent.Width - 2; ++x)
									{
										if (board[y][x + 1] == 0 && board[y][x] == 0 && board[y + 1][x + 1] == 0 &&
											board[y + 2][x + 1] == 0)
										{

											board[y][x + 1] = p;
											board[y][x] = p;
											board[y + 1][x + 1] = p;
											board[y + 2][x + 1] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
											}

											board[y][x + 1] = 0;
											board[y][x] = 0;
											board[y + 1][x + 1] = 0;
											board[y + 2][x + 1] = 0;
										}
									}
								}

								// ┛
								for (var y = 0; y <= parent.Height - 2; ++y)
								{
									for (var x = 0; x <= parent.Width - 3; ++x)
									{
										if (board[y + 1][x] == 0 && board[y][x + 2] == 0 && board[y + 1][x + 1] == 0 &&
											board[y + 1][x + 2] == 0)
										{

											board[y + 1][x] = p;
											board[y][x + 2] = p;
											board[y + 1][x + 1] = p;
											board[y + 1][x + 2] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
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
								for (var y = 0; y <= parent.Height - 3; ++y)
								{
									for (var x = 0; x <= parent.Width - 2; ++x)
									{
										if (board[y][x] == 0 && board[y + 1][x] == 0 && board[y + 1][x + 1] == 0 &&
											board[y + 2][x + 1] == 0)
										{
											board[y][x] = p;
											board[y + 1][x] = p;
											board[y + 1][x + 1] = p;
											board[y + 2][x + 1] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
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
								for (var y = 0; y <= parent.Height - 2; ++y)
								{
									for (var x = 0; x <= parent.Width - 3; ++x)
									{
										if (board[y][x + 1] == 0 && board[y][x + 2] == 0 && board[y + 1][x] == 0 &&
											board[y + 1][x + 1] == 0)
										{

											board[y][x + 1] = p;
											board[y][x + 2] = p;
											board[y + 1][x] = p;
											board[y + 1][x + 1] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
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
								for (var y = 0; y <= parent.Height - 2; ++y)
								{
									for (var x = 0; x <= parent.Width - 3; ++x)
									{
										if (board[y][x] == 0 && board[y][x + 1] == 0 && board[y + 1][x + 1] == 0 &&
											board[y + 1][x + 2] == 0)
										{

											board[y][x] = p;
											board[y][x + 1] = p;
											board[y + 1][x + 1] = p;
											board[y + 1][x + 2] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
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
								for (var y = 0; y <= parent.Height - 3; ++y)
								{
									for (var x = 0; x <= parent.Width - 2; ++x)
									{
										if (board[y][x + 1] == 0 && board[y + 1][x] == 0 && board[y + 1][x + 1] == 0 &&
											board[y + 2][x] == 0)
										{

											board[y][x + 1] = p;
											board[y + 1][x] = p;
											board[y + 1][x + 1] = p;
											board[y + 2][x] = p;

											if (p == parent.NumberOfPieces)
											{
												return new BoardState(workUnit);
											}

											if (!workUnit.IsStupidConfig())
											{
												parent.Stack.Add(new BoardState(workUnit), token);
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
						}
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
					return null;
				}
			}, TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness);
		}
	}
}
