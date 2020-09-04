using System;
using System.Runtime.CompilerServices;

namespace TheTalosPrincipleSolver.Solvers.BaseUnits
{
	public class BoardStateMa
	{
		public int[,] Board { get; }

		public int P { get; set; } = 1;

		private long Width { get; }
		private long Height { get; }

		public BoardStateMa(long width, long height)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(width), @"Width must be > 0");
			}

			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(height), @"Height must be > 0");
			}

			Width = width;
			Height = height;

			Board = new int[Height, Width];
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		public BoardStateMa(BoardStateMa origin)
		{
			Width = origin.Width;
			Height = origin.Height;

			Board = new int[Height, Width];
			Buffer.BlockCopy(origin.Board, 0, Board, 0, (int)Height * (int)Width * sizeof(int));

			P = origin.P + 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		private int Group(int y, int x)
		{
			if (y < 0 || y >= Height || x < 0 || x >= Width || Board[y, x] != 0)
			{
				return 0;
			}

			Board[y, x] = -1;
			return 1 + Group(y, x + 1) + Group(y, x - 1) + Group(y + 1, x) + Group(y - 1, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		private void ClearGroups()
		{
			for (var y = 0; y < Height; ++y)
			{
				for (var x = 0; x < Width; ++x)
				{
					if (Board[y, x] == -1)
					{
						Board[y, x] = 0;
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public bool IsStupidConfig()
		{
			try
			{
				for (var y = 0; y < Height; ++y)
				{
					for (var x = 0; x < Width; ++x)
					{
						if (Board[y, x] == 0 && (Group(y, x) & 3) != 0) // %4
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
	}
}
