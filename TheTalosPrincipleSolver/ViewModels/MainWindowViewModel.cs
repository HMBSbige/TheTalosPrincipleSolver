using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using TheTalosPrincipleSolver.Models;

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

		public MainWindowViewModel()
		{
			Puzzle = new Puzzle();
			StartCommand = ReactiveCommand.CreateFromObservable(StartSolve);
		}

		private IObservable<Unit> StartSolve()
		{
			return Observable.Start(() =>
			{
				Debug.WriteLine(Puzzle.Row);
			});
		}
	}
}
