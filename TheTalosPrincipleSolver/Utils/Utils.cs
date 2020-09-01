using System;

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
	}
}
