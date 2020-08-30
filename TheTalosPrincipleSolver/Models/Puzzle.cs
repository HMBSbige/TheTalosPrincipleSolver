using ReactiveUI;

namespace TheTalosPrincipleSolver.Models
{
	public class Puzzle : ReactiveObject
	{
		private uint row = 8;
		private uint column = 8;
		private uint numberOfI = 5;
		private uint numberOfJ;
		private uint numberOfL = 1;
		private uint numberOfO = 5;
		private uint numberOfS = 2;
		private uint numberOfT = 2;
		private uint numberOfZ = 1;

		public uint Row { get => row; set => this.RaiseAndSetIfChanged(ref row, value); }
		public uint Column { get => column; set => this.RaiseAndSetIfChanged(ref column, value); }
		public uint NumberOfI { get => numberOfI; set => this.RaiseAndSetIfChanged(ref numberOfI, value); }
		public uint NumberOfJ { get => numberOfJ; set => this.RaiseAndSetIfChanged(ref numberOfJ, value); }
		public uint NumberOfL { get => numberOfL; set => this.RaiseAndSetIfChanged(ref numberOfL, value); }
		public uint NumberOfO { get => numberOfO; set => this.RaiseAndSetIfChanged(ref numberOfO, value); }
		public uint NumberOfS { get => numberOfS; set => this.RaiseAndSetIfChanged(ref numberOfS, value); }
		public uint NumberOfT { get => numberOfT; set => this.RaiseAndSetIfChanged(ref numberOfT, value); }
		public uint NumberOfZ { get => numberOfZ; set => this.RaiseAndSetIfChanged(ref numberOfZ, value); }
	}
}
