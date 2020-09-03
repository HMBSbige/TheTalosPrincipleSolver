using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TheTalosPrincipleSolver.Enums;
using TheTalosPrincipleSolver.Models;
using TheTalosPrincipleSolver.Utils;

namespace TheTalosPrincipleSolver.Solvers
{
	public class PuzzleSolverMT : IDisposable
	{
		public Block[] Blocks { get; }

		public uint NumberOfPieces { get; }

		public long Width { get; }
		public long Height { get; }

		public bool Solvable { get; private set; }

		public bool Solved { get; private set; }

		private long iterations;
		public long Iterations => Interlocked.Read(ref iterations);

		private int waitUnits;
		public int WaitUnits => waitUnits;

		public int Threads { get; } = Math.Max(1, Environment.ProcessorCount);

		public readonly BlockingCollection<BoardState> Stack = new BlockingCollection<BoardState>(new ConcurrentStack<BoardState>());

		public BoardState CachedResult;

		private readonly CancellationTokenSource cts;

		/// <summary>
		/// 是否被手动取消
		/// </summary>
		public bool IsCanceled { get; private set; }

		private SolveUnit[] solveUnits;

		/// <summary>
		/// 行 x 列，每个 board 代表属于哪个 block（0 代表不属于任何 block）
		/// </summary>
		public int[][] Board
		{
			get
			{
				if (CachedResult != null)
				{
					return CachedResult.Board;
				}

				if (solveUnits == null)
				{
					return null;
				}

				for (var i = 0; i < Threads; ++i)
				{
					var board = solveUnits[i].Board;
					if (board != null)
					{
						return board;
					}
				}

				return null;
			}
		}

		public PuzzleSolverMT(Puzzle puzzle, int numberOfThreads) : this(puzzle)
		{
			Threads = numberOfThreads;
		}

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
				Solvable = false;
				CachedResult = null;
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
			Stack.Add(s);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool Solve()
		{
			if (IsCanceled)
			{
				return false;
			}
			if (Solved)
			{
				return Solvable;
			}

			solveUnits = new SolveUnit[Threads];
			var tasks = new Task<BoardState>[Threads];
			for (var i = 0; i < Threads; ++i)
			{
				solveUnits[i] = new SolveUnit(this, cts.Token);
				tasks[i] = solveUnits[i].Task;
				solveUnits[i].Start();
			}

			int index;
			try
			{
				// ReSharper disable once CoVariantArrayConversion
				index = Task.WaitAny(tasks, Timeout.Infinite, cts.Token);
			}
			catch (OperationCanceledException) when (!IsCanceled)
			{
				throw new Exception(@"无解");
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				throw;
			}

			if (index < 0)
			{
				throw new TaskCanceledException(@"Unknown");
			}

			CachedResult = tasks[index].Result;
			Solved = true;
			Solvable = true;
			return Solvable;
		}

		public void IncrementIterations()
		{
			Interlocked.Increment(ref iterations);
		}

		public void Waiting()
		{
			Interlocked.Increment(ref waitUnits);
			if (WaitUnits == Threads && Stack.Count == 0) // 无解
			{
				cts.Cancel();
			}
		}

		public void NotWaiting()
		{
			Interlocked.Decrement(ref waitUnits);
		}

		public void Abort()
		{
			IsCanceled = true;
			cts.Cancel();
		}

		public void Dispose()
		{
			Stack?.Dispose();
			cts?.Cancel();
		}
	}
}
