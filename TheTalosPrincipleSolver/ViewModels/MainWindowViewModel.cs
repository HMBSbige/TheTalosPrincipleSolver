using System.Reactive;
using ReactiveUI;
using TheTalosPrincipleSolver.Models;
using TheTalosPrincipleSolver.Views;

namespace TheTalosPrincipleSolver.ViewModels
{
	public class MainWindowViewModel : ReactiveObject
	{
		private Puzzle puzzle;

		public Puzzle Puzzle
		{
			get => puzzle;
			set => this.RaiseAndSetIfChanged(ref puzzle, value);
		}

		public ReactiveCommand<Unit, Unit> StartCommand { get; }

		private readonly MainWindow window;

		public MainWindowViewModel(MainWindow window)
		{
			this.window = window;
			Puzzle = new Puzzle();
			StartCommand = ReactiveCommand.Create(StartSolve);
		}

		private void StartSolve()
		{
			var puzzleWindow = new PuzzleWindow(Puzzle) { Owner = window };
			puzzleWindow.ShowDialog();
		}
	}
}
