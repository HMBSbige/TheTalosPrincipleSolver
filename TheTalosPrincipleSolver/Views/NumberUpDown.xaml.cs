using System.Windows;
using System.Windows.Controls;

namespace TheTalosPrincipleSolver.Views
{
	public partial class NumberUpDown
	{
		private int maxNum = 65535;

		private int minNum;

		public int Value
		{
			get
			{
				if (GetValue(ValueProperty) is int value)
				{
					if (value > maxNum)
					{
						return maxNum;
					}
					if (value < minNum)
					{
						return minNum;
					}
					return value;
				}
				return minNum;
			}
			set
			{
				SetValue(ValueProperty, value);
				TxtNum.Text = value.ToString();
			}
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumberUpDown));

		public int MinNum
		{
			get => minNum;
			set => minNum = value > maxNum ? maxNum : value;
		}

		public int MaxNum
		{
			get => maxNum;
			set => maxNum = value < minNum ? minNum : value;
		}

		public NumberUpDown()
		{
			InitializeComponent();
		}

		private void Up_Click(object sender, RoutedEventArgs e)
		{
			if (Value < maxNum)
			{
				++Value;
			}
			else
			{
				Value = maxNum;
			}
		}

		private void Down_Click(object sender, RoutedEventArgs e)
		{
			if (Value > minNum)
			{
				--Value;
			}
			else
			{
				Value = minNum;
			}
		}

		private void TxtNum_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (string.IsNullOrEmpty(TxtNum.Text))
			{
				return;
			}

			if (int.TryParse(TxtNum.Text, out var num))
			{
				Value = num;
			}
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			TxtNum.Text = Value.ToString();
		}

		private void Grid_LostFocus(object sender, RoutedEventArgs e)
		{
			TxtNum.Text = Value.ToString();
		}
	}
}