using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using TheTalosPrincipleSolver.Enums;
using TheTalosPrincipleSolver.Models;
using TheTalosPrincipleSolver.Utils;

namespace TheTalosPrincipleSolver.Solvers
{
	public class PuzzleSolverMT
	{
		public Block[] Blocks { get; }

		public long Width { get; }
		public long Height { get; }

		public bool Solved { get; private set; }

		public long Iterations { get; private set; }

		private readonly Queue<BoardState> queue = new Queue<BoardState>();

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

			var n = puzzle.NumberOfI + puzzle.NumberOfO + puzzle.NumberOfT + puzzle.NumberOfJ + puzzle.NumberOfL + puzzle.NumberOfS + puzzle.NumberOfZ;

			if (n << 2 != Width * Height)
			{
				Solved = true;
				cachedResult = null;
				return;
			}

			Blocks = new Block[n];

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
			queue.Enqueue(s);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public BoardState Solve()
		{
			if (Solved)
			{
				return cachedResult;
			}

			return null;
		}
	}
}
