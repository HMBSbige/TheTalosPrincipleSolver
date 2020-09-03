using TheTalosPrincipleSolver.Enums;

namespace TheTalosPrincipleSolver.Solvers
{
	public interface IPuzzleSolver
	{
		/// <summary>
		/// 行 x 列，每个 board 代表属于哪个 block（0 代表不属于任何 block）
		/// </summary>
		int[][] Board { get; }

		long Width { get; }
		long Height { get; }

		/// <summary>
		/// 是否已解完
		/// </summary>
		bool Solved { get; }

		/// <summary>
		/// 是否有解
		/// </summary>
		bool Solvable { get; }

		/// <summary>
		/// 是否被手动取消
		/// </summary>
		bool IsCanceled { get; }

		/// <summary>
		/// 迭代总次数
		/// </summary>
		long Iterations { get; }

		/// <summary>
		/// 总块数
		/// </summary>
		uint NumberOfPieces { get; }

		/// <summary>
		/// 放在板上的块的类型
		/// </summary>
		Block[] Blocks { get; }

		/// <summary>
		/// 调用这个函数开始解拼图
		/// </summary>
		/// <returns>是否有解</returns>
		bool Solve();

		/// <summary>
		/// 手动中断
		/// </summary>
		void Abort();
	}
}
