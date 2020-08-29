using System.Windows;

namespace TheTalosPrincipleSolver.Utils
{
	public static class ViewUtils
	{
		public static Size GetNonClientSize(Window window)
		{
			var clientHeight = window.Height - 39;
			var clientWidth = window.Width - 16;
			if (window.Content is FrameworkElement pnlClient)
			{
				clientHeight = pnlClient.ActualHeight;
				clientWidth = pnlClient.ActualWidth;
			}
			return new Size(window.Width - clientWidth, window.Height - clientHeight);
		}
	}
}
