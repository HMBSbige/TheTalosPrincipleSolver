﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TheTalosPrincipleSolver.Enums;
using TheTalosPrincipleSolver.Models;
using TheTalosPrincipleSolver.Solvers.BaseUnits;
using TheTalosPrincipleSolver.Utils;

namespace TheTalosPrincipleSolver.Solvers
{
	public class PuzzleSolverMa : IPuzzleSolver
	{
		public long Width { get; }

		public long Height { get; }

		public bool Solved { get; private set; }

		public bool Solvable { get; private set; }

		public bool IsCanceled { get; private set; }

		private long iterations;
		public long Iterations => Interlocked.Read(ref iterations);

		private int waitUnits;

		private int Threads { get; } = Environment.ProcessorCount;

		private readonly BlockingCollection<BoardStateMa> stack = new BlockingCollection<BoardStateMa>(new ConcurrentStack<BoardStateMa>());

		private BoardStateMa cachedResult;

		public uint NumberOfPieces { get; }

		public Block[] Blocks { get; }

		private SolveUnitMa[] solveUnits;

		private readonly CancellationTokenSource cts;

		public int[][] Board
		{
			get
			{
				var b = BoardMa;
				return b?.Convert((int)Height, (int)Width);
			}
		}

		public int[,] BoardMa
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

		public PuzzleSolverMa(Puzzle puzzle, int numberOfThreads) : this(puzzle)
		{
			Threads = Math.Max(1, numberOfThreads);
		}

		public PuzzleSolverMa(Puzzle puzzle)
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

			var s = new BoardStateMa(Width, Height);
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

			solveUnits = new SolveUnitMa[Threads];
			var tasks = new Task<BoardStateMa>[Threads];
			for (var i = 0; i < Threads; ++i)
			{
				solveUnits[i] = new SolveUnitMa(this, cts.Token);
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
				goto 无解;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				throw;
			}

			cachedResult = tasks.FirstOrDefault(t => t.IsCompletedSuccessfully && t.Result != null)?.Result;
			if (cachedResult == null)
			{
				goto 无解;
			}

			Solved = true;
			Solvable = true;
			return Solvable;
无解:
			Solved = true;
			Solvable = false;
			return Solvable;
		}

		public void IncrementIterations()
		{
			Interlocked.Increment(ref iterations);
		}

		private void Waiting()
		{
			Interlocked.Increment(ref waitUnits);
		}

		private void NotWaiting()
		{
			Interlocked.Decrement(ref waitUnits);
		}

		public void StopAdding()
		{
			stack.CompleteAdding();
		}

		public BoardStateMa TakeTask()
		{
			Waiting();
			try
			{
				BoardStateMa value;
				while (!stack.TryTake(out value, 300, cts.Token))
				{
					if (waitUnits == Threads && stack.Count == 0) // 无解
					{
						cts.Cancel();
					}
				}
				return value;
			}
			finally
			{
				NotWaiting();
			}
		}

		public void AddTask(BoardStateMa unit)
		{
			stack.Add(unit, cts.Token);
		}

		public void Abort()
		{
			IsCanceled = true;
			cts.Cancel();
		}
	}
}
