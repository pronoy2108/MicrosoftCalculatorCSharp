using System;
using System.Collections.Generic;
using System.Text;
using Windows.System;
using Windows.UI.Xaml.Input;

namespace WindowsCalculator.Shared.Controls
{
	class AppBar : AppBar
	{
		protected override void OnKeyDown(KeyRoutedEventArgs e)
		{
			OnKeyDown(e);
			if (e.Key == VirtualKey.Escape)
			{
				KeyboardShortcutManager.IgnoreEscape(true);
			}
		}
	}
}
