using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TheTalosPrincipleSolver.Enums;
using TheTalosPrincipleSolver.Models;
using TheTalosPrincipleSolver.Utils;

namespace TheTalosPrincipleSolver.Solvers
{
	public class PuzzleSolverMT : IPuzzleSolver
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

		private int Threads { get; } = Environment.ProcessorCount;

		private readonly BlockingCollection<BoardState> stack = new BlockingCollection<BoardState>(new ConcurrentStack<BoardState>());

		private BoardState cachedResult;

		private readonly CancellationTokenSource cts;

		public bool IsCanceled { get; private set; }

		private SolveUnit[] solveUnits;

		public int[][] Board
		{
			get
			{
				if (cachedResult != null)
				{
					return cachedResult.Board;
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
			Threads = Math.Max(1, numberOfThreads);
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

			try
			{
				// ReSharper disable once CoVariantArrayConversion
				Task.WaitAny(tasks, Timeout.Infinite, cts.Token);
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

			cachedResult = tasks.FirstOrDefault(t => t.IsCompletedSuccessfully && t.Result != null)?.Result;
			if (cachedResult == null)
			{
				throw new Exception(@"无解");
			}
			Solved = true;
			Solvable = true;
			return Solvable;
		}

		public void IncrementIterations()
		{
			Interlocked.Increment(ref iterations);
		}

		private void Waiting()
		{
			Interlocked.Increment(ref waitUnits);
			if (waitUnits == Threads && stack.Count == 0) // 无解
			{
				cts.Cancel();
			}
		}

		private void NotWaiting()
		{
			Interlocked.Decrement(ref waitUnits);
		}

		public void StopAdding()
		{
			stack.CompleteAdding();
		}

		public BoardState TakeTask()
		{
			Waiting();
			try
			{
				return stack.Take(cts.Token);
			}
			finally
			{
				NotWaiting();
			}
		}

		public void AddTask(List<BoardState> list)
		{
			list.ForEach(x => stack.Add(x, cts.Token));
		}

		public void Abort()
		{
			IsCanceled = true;
			cts.Cancel();
		}
	}
}
