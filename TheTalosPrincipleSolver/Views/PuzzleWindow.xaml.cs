using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TheTalosPrincipleSolver.Enums;
using TheTalosPrincipleSolver.Models;
using TheTalosPrincipleSolver.Solvers;
using TheTalosPrincipleSolver.Utils;

namespace TheTalosPrincipleSolver.Views
{
	public partial class PuzzleWindow
	{
		public PuzzleWindow(Puzzle puzzle, Method selectedMethod, int thread)
		{
			InitializeComponent();

			solver = selectedMethod switch
			{
				Method.单线程 => new PuzzleSolver(puzzle),
				Method.多线程v1 => new PuzzleSolverMT(puzzle, thread),
				_ => new PuzzleSolverMT(puzzle),
			};

			colors = ColorUtils.GetBrushes(solver.NumberOfPieces);
		}

		private readonly IPuzzleSolver solver;

		private readonly List<SolidColorBrush> colors;

		public ushort Unit { get; set; } = 32;

		private IDisposable d1;
		private IDisposable d2;

		private void PuzzleWindow_OnClosed(object sender, EventArgs e)
		{
			d1?.Dispose();
			d2?.Dispose();
			solver.Abort();
		}

		private void PuzzleWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			var nonClientSize = ViewUtils.GetNonClientSize(this);
			Width = solver.Width * Unit + nonClientSize.Width;
			Height = solver.Height * Unit + nonClientSize.Height;

			Solve();
		}

		private async void Solve()
		{
			d1 = Observable.Interval(TimeSpan.FromMilliseconds(30))
				.ObserveOnDispatcher()
				.Subscribe(_ => { Render(); });

			var i = solver.Iterations;
			d2 = Observable.Interval(TimeSpan.FromSeconds(1))
					.ObserveOnDispatcher()
					.Subscribe(_ =>
					{
						var j = solver.Iterations;
						Title = $@"{j - i}";
						i = j;
					});

			var task = new Task<string>(() =>
			{
				try
				{
					var sw = new Stopwatch();

					sw.Start();
					solver.Solve();
					sw.Stop();

					d2?.Dispose();

					return !solver.Solvable ? @"无解" : $@"{Convert.ToInt64(solver.Iterations / sw.Elapsed.TotalSeconds)} /s";
				}
				catch (Exception ex)
				{
					return ex.Message;
				}
			}, TaskCreationOptions.LongRunning);

			task.Start();

			var result = await task;

			DispatcherScheduler.Current.Schedule(() =>
			{
				if (solver.IsCanceled)
				{
					return;
				}
				if (solver.Solvable)
				{
					Title = result;
				}
				else
				{
					MessageBox.Show(result, nameof(TheTalosPrincipleSolver), MessageBoxButton.OK, MessageBoxImage.Error);
					Close();
				}
			});
		}

		private void Render()
		{
			var board = solver.Board;
			if (board == null)
			{
				return;
			}
			MyCanvas.Children.Clear();
			for (var i = 0; i < solver.Height; ++i)
			{
				for (var j = 0; j < solver.Width; ++j)
				{
					var rect = new Rectangle
					{
						Height = Unit,
						Width = Unit,
						Fill = colors[board[i][j] + 1]
					};
					MyCanvas.Children.Add(rect);
					Canvas.SetTop(rect, Unit * i);
					Canvas.SetLeft(rect, Unit * j);
				}
			}
		}
	}
}
