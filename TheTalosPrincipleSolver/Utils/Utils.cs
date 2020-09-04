using System;
using System.Runtime.CompilerServices;

namespace TheTalosPrincipleSolver.Utils
{
	public static class Utils
	{
		public static void Shuffle<T>(this T[] array)
		{
			var rng = new Random(DateTime.Now.Millisecond);
			var n = array.Length;
			while (n > 1)
			{
				var k = rng.Next(n--);
				var temp = array[n];
				array[n] = array[k];
				array[k] = temp;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		public static int[][] Convert(this int[,] a, int height, int width)
		{
			var b = new int[height][];
			for (var i = 0; i < height; ++i)
			{
				b[i] = new int[width];
				Buffer.BlockCopy(a, i * width * sizeof(int), b[i], 0, width * sizeof(int));
			}
			return b;
		}
	}
}
