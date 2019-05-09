using System;
using System.Collections.Generic;
using System.Text;
using Uno.UI.Converters;
using Windows.UI.Xaml.Interop;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

namespace WindowsCalculator.Shared.Converters
{
	[WebHostHidden]
	public sealed class VisibilityNegationConverter : IValueConverter
	{
		protected override object Convert(object value, Type targetType, object parameter)
		{
			var boxedVisibility = (Visibility)value;
			Visibility visibility = Visibility.Collapsed;

			if (boxedVisibility == Visibility.Collapsed)
			{
				visibility = Visibility.Visible;
			}

			return visibility;
		}

		protected override object ConvertBack(Object value, TypeName targetType, Object parameter, String language)
		{
			return Convert(value, targetType, parameter, language);
		}
	}
}
