﻿using System.Reactive.Disposables;
using ReactiveUI;
using TheTalosPrincipleSolver.ViewModels;

namespace TheTalosPrincipleSolver
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			ViewModel = new MainWindowViewModel();

			this.WhenActivated(d =>
			{
				this.Bind(ViewModel, vm => vm.Puzzle.Row, v => v.RowNumberUpDown.Value).DisposeWith(d);
				this.Bind(ViewModel, vm => vm.Puzzle.Column, v => v.ColumnNumberUpDown.Value).DisposeWith(d);
				this.Bind(ViewModel, vm => vm.Puzzle.NumberOfI, v => v.INumberUpDown.Value).DisposeWith(d);
				this.Bind(ViewModel, vm => vm.Puzzle.NumberOfJ, v => v.JNumberUpDown.Value).DisposeWith(d);
				this.Bind(ViewModel, vm => vm.Puzzle.NumberOfL, v => v.LNumberUpDown.Value).DisposeWith(d);
				this.Bind(ViewModel, vm => vm.Puzzle.NumberOfO, v => v.ONumberUpDown.Value).DisposeWith(d);
				this.Bind(ViewModel, vm => vm.Puzzle.NumberOfS, v => v.SNumberUpDown.Value).DisposeWith(d);
				this.Bind(ViewModel, vm => vm.Puzzle.NumberOfT, v => v.TNumberUpDown.Value).DisposeWith(d);
				this.Bind(ViewModel, vm => vm.Puzzle.NumberOfZ, v => v.ZNumberUpDown.Value).DisposeWith(d);

				this.BindCommand(ViewModel, vm => vm.StartCommand, v => v.StartButton).DisposeWith(d);
			});
		}
	}
}
