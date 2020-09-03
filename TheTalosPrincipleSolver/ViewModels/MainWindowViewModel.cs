using System;
using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;
using TheTalosPrincipleSolver.Enums;
using TheTalosPrincipleSolver.Models;
using TheTalosPrincipleSolver.Views;

namespace TheTalosPrincipleSolver.ViewModels
{
	public class MainWindowViewModel : ReactiveObject
	{
		private Puzzle puzzle;
		private Method selectedMethod;
		private int threads;
		private bool isMultiThreading;

		public Puzzle Puzzle
		{
			get => puzzle;
			set => this.RaiseAndSetIfChanged(ref puzzle, value);
		}

		public Method SelectedMethod
		{
			get => selectedMethod;
			set => this.RaiseAndSetIfChanged(ref selectedMethod, value);
		}

		public int Threads
		{
			get => Math.Max(2, threads);
			set => this.RaiseAndSetIfChanged(ref threads, value);
		}

		public bool IsMultiThreading
		{
			get => isMultiThreading;
			set => this.RaiseAndSetIfChanged(ref isMultiThreading, value);
		}

		public ReactiveCommand<Unit, Unit> StartCommand { get; }

		private readonly MainWindow window;

		public readonly ObservableCollection<Method> MethodList = new ObservableCollection<Method> { Method.单线程, Method.多线程v1 };

		public MainWindowViewModel(MainWindow window)
		{
			this.window = window;
			Puzzle = new Puzzle();
			SelectedMethod = Method.多线程v1;
			Threads = Environment.ProcessorCount;

			this.WhenAnyValue(x => x.SelectedMethod).Subscribe(method => { IsMultiThreading = method != Method.单线程; });

			StartCommand = ReactiveCommand.Create(StartSolve);
		}

		private void StartSolve()
		{
			var puzzleWindow = new PuzzleWindow(Puzzle, SelectedMethod, Threads) { Owner = window };
			puzzleWindow.ShowDialog();
		}
	}
}
